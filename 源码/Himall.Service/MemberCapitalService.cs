using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;

namespace Himall.Service
{
	public class MemberCapitalService : ServiceBase, IMemberCapitalService, IService, IDisposable
	{
		private static object obj;

		static MemberCapitalService()
		{
			MemberCapitalService.obj = new object();
		}

		public MemberCapitalService()
		{
		}

		public void AddCapital(CapitalDetailModel model)
		{
			decimal? nullable;
			decimal? nullable1;
			decimal? nullable2;
			CapitalInfo capitalInfo = context.CapitalInfo.FirstOrDefault((CapitalInfo e) => e.MemId == model.UserId);
			decimal num = new decimal(0);
			decimal num1 = new decimal(0);
			decimal amount = new decimal(0);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(model.Remark);
			stringBuilder.Append(" ");
			stringBuilder.Append(model.PayWay);
			switch (model.SourceType)
			{
				case CapitalDetailInfo.CapitalDetailType.ChargeAmount:
				{
					decimal amount1 = model.Amount;
					num1 = amount1;
					num = amount1;
					break;
				}
				case CapitalDetailInfo.CapitalDetailType.WithDraw:
				{
					amount = model.Amount;
					break;
				}
				default:
				{
					num = model.Amount;
					break;
				}
			}
			if (capitalInfo == null)
			{
				CapitalInfo capitalInfo1 = new CapitalInfo()
				{
					MemId = model.UserId,
					Balance = new decimal?(num),
					ChargeAmount = new decimal?(num1),
					FreezeAmount = new decimal?(amount)
				};
				List<CapitalDetailInfo> capitalDetailInfos = new List<CapitalDetailInfo>();
				CapitalDetailInfo capitalDetailInfo = new CapitalDetailInfo()
				{
					Id = CreateCode(model.SourceType),
					Amount = num,
					CreateTime = new DateTime?(DateTime.Parse(model.CreateTime)),
					SourceType = model.SourceType,
					SourceData = model.SourceData,
					Remark = stringBuilder.ToString()
				};
				capitalDetailInfos.Add(capitalDetailInfo);
				capitalInfo1.Himall_CapitalDetail = capitalDetailInfos;
				capitalInfo = capitalInfo1;
                context.CapitalInfo.Add(capitalInfo);
			}
			else if (context.CapitalDetailInfo.FirstOrDefault((CapitalDetailInfo e) => e.Id == model.Id && e.Id != 0) == null)
			{
				CapitalDetailInfo capitalDetailInfo1 = new CapitalDetailInfo()
				{
					Id = CreateCode(model.SourceType),
					Amount = model.Amount,
					CreateTime = new DateTime?(DateTime.Parse(model.CreateTime)),
					CapitalID = capitalInfo.Id,
					SourceType = model.SourceType,
					SourceData = model.SourceData,
					Remark = stringBuilder.ToString()
				};
                context.CapitalDetailInfo.Add(capitalDetailInfo1);
				CapitalInfo capitalInfo2 = capitalInfo;
				decimal? balance = capitalInfo2.Balance;
				decimal num2 = num;
				if (balance.HasValue)
				{
					nullable = new decimal?(balance.GetValueOrDefault() + num2);
				}
				else
				{
					nullable = null;
				}
				capitalInfo2.Balance = nullable;
				CapitalInfo capitalInfo3 = capitalInfo;
				decimal? chargeAmount = capitalInfo3.ChargeAmount;
				decimal num3 = num1;
				if (chargeAmount.HasValue)
				{
					nullable1 = new decimal?(chargeAmount.GetValueOrDefault() + num3);
				}
				else
				{
					nullable1 = null;
				}
				capitalInfo3.ChargeAmount = nullable1;
				CapitalInfo capitalInfo4 = capitalInfo;
				decimal? freezeAmount = capitalInfo4.FreezeAmount;
				decimal num4 = amount;
				if (freezeAmount.HasValue)
				{
					nullable2 = new decimal?(freezeAmount.GetValueOrDefault() + num4);
				}
				else
				{
					nullable2 = null;
				}
				capitalInfo4.FreezeAmount = nullable2;
			}
            context.SaveChanges();
		}

		public long AddChargeApply(ChargeDetailInfo model)
		{
			if (model.Id == 0)
			{
				model.Id = CreateCode(CapitalDetailInfo.CapitalDetailType.ChargeAmount);
			}
            context.ChargeDetailInfo.Add(model);
            context.SaveChanges();
			return model.Id;
		}

        public void AddWithDrawApply(ApplyWithDrawInfo model)
        {
            decimal? nullable;
            decimal value;
            context.ApplyWithDrawInfo.Add(model);
            CapitalInfo capitalInfo = context.CapitalInfo.FirstOrDefault((CapitalInfo e) => e.MemId == model.MemId);
            CapitalInfo capitalInfo1 = capitalInfo;
            decimal? balance = capitalInfo1.Balance;
            decimal applyAmount = model.ApplyAmount;
            if (balance.HasValue)
            {
                nullable = new decimal?(balance.GetValueOrDefault() - applyAmount);
            }
            else
            {
                nullable = null;
            }

            capitalInfo1.Balance = nullable;
            CapitalInfo nullable1 = capitalInfo;
            balance = capitalInfo.FreezeAmount;
            if (balance.HasValue)
            {
                balance = capitalInfo.FreezeAmount;
                value = balance.Value + model.ApplyAmount;
            }
            else
            {
                value = model.ApplyAmount;
            }
            nullable1.FreezeAmount = new decimal?(value);
            this.context.SaveChanges();

        }

		public void ConfirmApplyWithDraw(ApplyWithDrawInfo info)
		{
			ApplyWithDrawInfo applyStatus = context.ApplyWithDrawInfo.FirstOrDefault((ApplyWithDrawInfo e) => e.Id == info.Id);
			using (TransactionScope transactionScope = new TransactionScope())
			{
				applyStatus.ApplyStatus = info.ApplyStatus;
				applyStatus.OpUser = info.OpUser;
				applyStatus.Remark = info.Remark;
				applyStatus.ConfirmTime = new DateTime?((info.ConfirmTime.HasValue ? info.ConfirmTime.Value : DateTime.Now));
                context.SaveChanges();
				if (info.ApplyStatus == ApplyWithDrawInfo.ApplyWithDrawStatus.WithDrawSuccess)
				{
					applyStatus.PayNo = info.PayNo;
					applyStatus.PayTime = new DateTime?((info.PayTime.HasValue ? info.PayTime.Value : DateTime.Now));
					CapitalDetailModel capitalDetailModel = new CapitalDetailModel()
					{
						Amount = -info.ApplyAmount,
						UserId = applyStatus.MemId,
						PayWay = info.Remark,
						SourceType = CapitalDetailInfo.CapitalDetailType.WithDraw,
						SourceData = info.Id.ToString()
					};
                    AddCapital(capitalDetailModel);
				}
				transactionScope.Complete();
			}
		}

		public long CreateCode(CapitalDetailInfo.CapitalDetailType type)
		{
			long num;
			lock (MemberCapitalService.obj)
			{
				string empty = string.Empty;
				Guid guid = Guid.NewGuid();
				Random random = new Random(BitConverter.ToInt32(guid.ToByteArray(), 0));
				for (int i = 0; i < 4; i++)
				{
					int num1 = random.Next();
					char chr = (char)(48 + (ushort)(num1 % 10));
					empty = string.Concat(empty, chr.ToString());
				}
				DateTime now = DateTime.Now;
				num = long.Parse(string.Concat(now.ToString("yyMMddHHmmss"), (int)type, empty));
			}
			return num;
		}

		public PageModel<ApplyWithDrawInfo> GetApplyWithDraw(ApplyWithDrawQuery query)
		{
			IQueryable<ApplyWithDrawInfo> memId = context.ApplyWithDrawInfo.AsQueryable<ApplyWithDrawInfo>();
			if (query.memberId.HasValue)
			{
				memId = 
					from e in memId
					where e.MemId == query.memberId
                    select e;
			}
			if (query.withDrawNo.HasValue)
			{
				memId = 
					from e in memId
					where e.Id == query.withDrawNo
                    select e;
			}
			if (query.withDrawStatus.HasValue && query.withDrawStatus.Value != 0)
			{
				memId = 
					from e in memId
					where (int)e.ApplyStatus == (int)query.withDrawStatus.Value
					select e;
			}
			int num = 0;
			IQueryable<ApplyWithDrawInfo> applyWithDrawInfos = memId;
			applyWithDrawInfos = (!string.IsNullOrWhiteSpace(query.Sort) ? memId.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<ApplyWithDrawInfo> p) => 
				from e in p
				orderby e.ApplyTime descending
				select e) : memId.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<ApplyWithDrawInfo> p) => 
				from e in p
				orderby e.ApplyStatus, e.ApplyTime descending
				select e));
			return new PageModel<ApplyWithDrawInfo>()
			{
				Models = applyWithDrawInfos,
				Total = num
			};
		}

		public CapitalDetailInfo GetCapitalDetailInfo(long id)
		{
			return context.CapitalDetailInfo.FirstOrDefault((CapitalDetailInfo e) => e.Id == id);
		}

		public PageModel<CapitalDetailInfo> GetCapitalDetails(CapitalDetailQuery query)
		{
			IQueryable<CapitalDetailInfo> capitalDetailInfo = 
				from item in context.CapitalDetailInfo
				where item.Himall_Capital.MemId == query.memberId
				select item;
			if (query.capitalType.HasValue && query.capitalType.Value != 0)
			{
				capitalDetailInfo = 
					from e in capitalDetailInfo
					where (int)e.SourceType == (int)query.capitalType.Value
					select e;
			}
			if (query.startTime.HasValue)
			{
				capitalDetailInfo = 
					from e in capitalDetailInfo
					where e.CreateTime >= query.startTime
					select e;
			}
			if (query.endTime.HasValue)
			{
				capitalDetailInfo = 
					from e in capitalDetailInfo
					where e.CreateTime < query.endTime
					select e;
			}
			int num = 0;
			IQueryable<CapitalDetailInfo> page = capitalDetailInfo.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<CapitalDetailInfo> p) => 
				from e in p
				orderby e.CreateTime descending
				select e);
			return new PageModel<CapitalDetailInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public CapitalInfo GetCapitalInfo(long userid)
		{
			return context.CapitalInfo.FirstOrDefault((CapitalInfo e) => e.MemId == userid);
		}

		public PageModel<CapitalInfo> GetCapitals(CapitalQuery query)
		{
			IQueryable<CapitalInfo> capitalInfo = 
				from e in context.CapitalInfo
				where true
				select e;
			if (query.memberId.HasValue)
			{
				capitalInfo = 
					from e in context.CapitalInfo
					where e.MemId == query.memberId
                    select e;
			}
			int num = 0;
			IQueryable<CapitalInfo> page = capitalInfo.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<CapitalInfo> o) => 
				from e in o
				orderby e.Id descending
				select e);
			return new PageModel<CapitalInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public ChargeDetailInfo GetChargeDetail(long id)
		{
			return context.ChargeDetailInfo.FirstOrDefault((ChargeDetailInfo e) => e.Id == id);
		}

		public PageModel<ChargeDetailInfo> GetChargeLists(ChargeQuery query)
		{
			IQueryable<ChargeDetailInfo> chargeStatus = context.ChargeDetailInfo.AsQueryable<ChargeDetailInfo>();
			if (query.ChargeStatus.HasValue)
			{
				chargeStatus = 
					from e in chargeStatus
					where (int)e.ChargeStatus == (int)query.ChargeStatus.Value
					select e;
			}
			if (query.memberId.HasValue)
			{
				chargeStatus = 
					from e in chargeStatus
					where e.MemId == query.memberId.Value
					select e;
			}
			if (query.ChargeNo.HasValue)
			{
				chargeStatus = 
					from e in chargeStatus
					where e.Id == query.ChargeNo.Value
					select e;
			}
			int num = 0;
			IQueryable<ChargeDetailInfo> page = chargeStatus.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<ChargeDetailInfo> p) => 
				from o in p
				orderby o.CreateTime descending
				select o);
			return new PageModel<ChargeDetailInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public UserMemberInfo GetMemberInfoByPayPwd(long memid, string payPwd)
		{
			payPwd = payPwd.Trim();
			UserMemberInfo userMemberInfo = context.UserMemberInfo.FirstOrDefault((UserMemberInfo e) => e.Id == memid);
			if (userMemberInfo != null && SecureHelper.MD5(string.Concat(SecureHelper.MD5(payPwd), userMemberInfo.PayPwdSalt)) == userMemberInfo.PayPwd)
			{
				return userMemberInfo;
			}
			return null;
		}

		public void RefuseApplyWithDraw(long id, ApplyWithDrawInfo.ApplyWithDrawStatus status, string opuser, string remark)
		{
			decimal? nullable;
			ApplyWithDrawInfo applyWithDrawInfo = context.ApplyWithDrawInfo.FirstOrDefault((ApplyWithDrawInfo e) => e.Id == id);
            
			applyWithDrawInfo.ApplyStatus = status;
            applyWithDrawInfo.OpUser = applyWithDrawInfo.OpUser;
			applyWithDrawInfo.Remark = remark;
			applyWithDrawInfo.ConfirmTime = new DateTime?(DateTime.Now);
			CapitalInfo capitalInfo = context.CapitalInfo.FirstOrDefault((CapitalInfo e) => e.MemId == applyWithDrawInfo.MemId);
			decimal? balance = capitalInfo.Balance;
			capitalInfo.Balance = new decimal?(balance.Value + applyWithDrawInfo.ApplyAmount);
			CapitalInfo capitalInfo1 = capitalInfo;
			decimal? freezeAmount = capitalInfo.FreezeAmount;
			decimal applyAmount = applyWithDrawInfo.ApplyAmount;
			if (freezeAmount.HasValue)
			{
				nullable = new decimal?(freezeAmount.GetValueOrDefault() - applyAmount);
			}
			else
			{
				nullable = null;
			}
			capitalInfo1.FreezeAmount = nullable;
            context.SaveChanges();
		}

		public void SetPayPwd(long memid, string pwd)
		{
			pwd = pwd.Trim();
			string str = Guid.NewGuid().ToString("N");
			string str1 = SecureHelper.MD5(string.Concat(SecureHelper.MD5(pwd), str));
			UserMemberInfo userMemberInfo = context.UserMemberInfo.FirstOrDefault((UserMemberInfo e) => e.Id == memid);
			if (userMemberInfo != null)
			{
				userMemberInfo.PayPwd = str1;
				userMemberInfo.PayPwdSalt = str;
                context.SaveChanges();
			}
		}

		public void UpdateCapitalAmount(long memid, decimal amount, decimal freezeAmount, decimal chargeAmount)
		{
			throw new NotImplementedException();
		}

		public void UpdateChargeDetail(ChargeDetailInfo model)
		{
			ChargeDetailInfo chargeStatus = context.ChargeDetailInfo.FirstOrDefault((ChargeDetailInfo e) => e.Id == model.Id);
			using (TransactionScope transactionScope = new TransactionScope())
			{
				chargeStatus.ChargeStatus = model.ChargeStatus;
				chargeStatus.ChargeTime = new DateTime?(model.ChargeTime.Value);
				chargeStatus.ChargeWay = model.ChargeWay;
                context.SaveChanges();
				CapitalDetailModel capitalDetailModel = new CapitalDetailModel()
				{
					Amount = chargeStatus.ChargeAmount,
					UserId = chargeStatus.MemId,
					PayWay = model.ChargeWay,
					SourceType = CapitalDetailInfo.CapitalDetailType.ChargeAmount,
					SourceData = chargeStatus.Id.ToString()
				};
                AddCapital(capitalDetailModel);
				transactionScope.Complete();
			}
		}
	}
}