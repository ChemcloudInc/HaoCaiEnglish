using Himall.Core;
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

namespace Himall.Service
{
	public class MemberIntegralService : ServiceBase, IMemberIntegralService, IService, IDisposable
	{
		public MemberIntegralService()
		{
		}

		public void AddMemberIntegral(MemberIntegralRecord model, IConversionMemberIntegralBase conversionMemberIntegralEntity = null)
		{
			if (model == null)
			{
				throw new NullReferenceException("添加会员积分记录时，会员积分Model为空.");
			}
			if (0 == model.MemberId)
			{
				throw new NullReferenceException("添加会员积分记录时，会员Id为空.");
			}
			if (!context.UserMemberInfo.Any((UserMemberInfo a) => a.Id == model.MemberId && (a.UserName == model.UserName)))
			{
				throw new HimallException("不存在此会员");
			}
			if (conversionMemberIntegralEntity != null)
			{
				model.Integral = conversionMemberIntegralEntity.ConversionIntegral();
			}
			if (model.Integral == 0)
			{
				return;
			}
			MemberIntegral memberIntegral = context.MemberIntegral.FirstOrDefault((MemberIntegral a) => a.MemberId == model.MemberId);
			if (memberIntegral != null)
			{
				if (model.Integral > 0)
				{
					MemberIntegral historyIntegrals = memberIntegral;
					historyIntegrals.HistoryIntegrals = historyIntegrals.HistoryIntegrals + model.Integral;
				}
				else if (memberIntegral.AvailableIntegrals < Math.Abs(model.Integral))
				{
					throw new HimallException("用户积分不足以扣减该积分！");
				}
				MemberIntegral availableIntegrals = memberIntegral;
				availableIntegrals.AvailableIntegrals = availableIntegrals.AvailableIntegrals + model.Integral;
			}
			else
			{
				memberIntegral = new MemberIntegral()
				{
					MemberId = new long?(model.MemberId),
					UserName = model.UserName
				};
				if (model.Integral <= 0)
				{
					throw new HimallException("用户积分不足以扣减该积分！");
				}
				MemberIntegral historyIntegrals1 = memberIntegral;
				historyIntegrals1.HistoryIntegrals = historyIntegrals1.HistoryIntegrals + model.Integral;
				MemberIntegral availableIntegrals1 = memberIntegral;
				availableIntegrals1.AvailableIntegrals = availableIntegrals1.AvailableIntegrals + model.Integral;
                context.MemberIntegral.Add(memberIntegral);
			}
            context.MemberIntegralRecord.Add(model);
            context.SaveChanges();
		}

		public MemberIntegralExchangeRules GetIntegralChangeRule()
		{
			return context.MemberIntegralExchangeRules.FirstOrDefault();
		}

		public PageModel<MemberIntegralRecord> GetIntegralRecordList(IntegralRecordQuery query)
		{
			int num = 0;
			IQueryable<MemberIntegralRecord> memberId = context.MemberIntegralRecord.AsQueryable<MemberIntegralRecord>();
			if (query.UserId.HasValue)
			{
				memberId = 
					from item in memberId
					where item.MemberId == query.UserId.Value
					select item;
			}
			if (query.StartDate.HasValue)
			{
				memberId = 
					from item in memberId
					where query.StartDate <= item.RecordDate
					select item;
			}
			if (query.IntegralType.HasValue)
			{
				memberId = 
					from item in memberId
					where (int)item.TypeId == (int)query.IntegralType.Value
					select item;
			}
			if (query.EndDate.HasValue)
			{
				memberId = 
					from item in memberId
					where query.EndDate >= item.RecordDate
					select item;
			}
			memberId = memberId.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<MemberIntegralRecord>()
			{
				Models = memberId,
				Total = num
			};
		}

		public IEnumerable<MemberIntegralRule> GetIntegralRule()
		{
			return context.MemberIntegralRule.ToList();
		}

		public MemberIntegral GetMemberIntegral(long userId)
		{
			MemberIntegral memberIntegral = context.MemberIntegral.FirstOrDefault((MemberIntegral a) => a.MemberId == userId) ?? new MemberIntegral();
			return memberIntegral;
		}

		public PageModel<MemberIntegral> GetMemberIntegralList(IntegralQuery query)
		{
			int num = 0;
			IQueryable<MemberIntegral> startDate = context.MemberIntegral.AsQueryable<MemberIntegral>();
			if (!string.IsNullOrWhiteSpace(query.UserName))
			{
				startDate = 
					from item in startDate
					where item.UserName.Equals(query.UserName)
					select item;
			}
			if (query.StartDate.HasValue)
			{
				startDate = 
					from item in startDate
					where query.StartDate <= item.Himall_Members.CreateDate
                    select item;
			}
			if (query.EndDate.HasValue)
			{
				startDate = 
					from item in startDate
					where query.EndDate >= item.Himall_Members.CreateDate
                    select item;
			}
			startDate = startDate.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<MemberIntegral> d) => 
				from item in d
				orderby item.HistoryIntegrals descending
				select item);
			return new PageModel<MemberIntegral>()
			{
				Models = startDate,
				Total = num
			};
		}

		public UserIntegralGroupModel GetUserHistroyIntegralGroup(long userId)
		{
			UserIntegralGroupModel userIntegralGroupModel = new UserIntegralGroupModel();
			int? nullable = (
				from a in context.MemberIntegralRecord
				where a.MemberId == userId && (int)a.TypeId == 6
				select a).Sum<MemberIntegralRecord>((MemberIntegralRecord a) => (int?)a.Integral);
			userIntegralGroupModel.BindWxIntegral = nullable.GetValueOrDefault();
			int? nullable1 = (
				from a in context.MemberIntegralRecord
				where a.MemberId == userId && (int)a.TypeId == 7
				select a).Sum<MemberIntegralRecord>((MemberIntegralRecord a) => (int?)a.Integral);
			userIntegralGroupModel.CommentIntegral = nullable1.GetValueOrDefault();
			int? nullable2 = (
				from a in context.MemberIntegralRecord
				where a.MemberId == userId && (int)a.TypeId == 1
				select a).Sum<MemberIntegralRecord>((MemberIntegralRecord a) => (int?)a.Integral);
			userIntegralGroupModel.ConsumptionIntegral = nullable2.GetValueOrDefault();
			int? nullable3 = (
				from a in context.MemberIntegralRecord
				where a.MemberId == userId && (int)a.TypeId == 5
				select a).Sum<MemberIntegralRecord>((MemberIntegralRecord a) => (int?)a.Integral);
			userIntegralGroupModel.LoginIntegral = nullable3.GetValueOrDefault();
			int? nullable4 = (
				from a in context.MemberIntegralRecord
				where a.MemberId == userId && (int)a.TypeId == 9
				select a).Sum<MemberIntegralRecord>((MemberIntegralRecord a) => (int?)a.Integral);
			userIntegralGroupModel.RegIntegral = nullable4.GetValueOrDefault();
			return userIntegralGroupModel;
		}

		public bool HasLoginIntegralRecord(long userId)
		{
			DateTime date = DateTime.Now.Date;
			DateTime dateTime = date.AddDays(1);
			return context.MemberIntegralRecord.Any((MemberIntegralRecord a) => a.MemberId == userId && (a.RecordDate.Value >= date) && (a.RecordDate.Value < dateTime) && (int)a.TypeId == 5);
		}

		public void SetIntegralChangeRule(MemberIntegralExchangeRules info)
		{
			MemberIntegralExchangeRules memberIntegralExchangeRule = context.MemberIntegralExchangeRules.FirstOrDefault();
			if (memberIntegralExchangeRule == null)
			{
				memberIntegralExchangeRule = new MemberIntegralExchangeRules()
				{
					IntegralPerMoney = info.IntegralPerMoney,
					MoneyPerIntegral = info.MoneyPerIntegral
				};
                context.MemberIntegralExchangeRules.Add(memberIntegralExchangeRule);
			}
			else
			{
				memberIntegralExchangeRule.IntegralPerMoney = info.IntegralPerMoney;
				memberIntegralExchangeRule.MoneyPerIntegral = info.MoneyPerIntegral;
			}
            context.SaveChanges();
		}

		public void SetIntegralRule(IEnumerable<MemberIntegralRule> info)
		{
			List<MemberIntegralRule> list = context.MemberIntegralRule.ToList();
			foreach (MemberIntegralRule memberIntegralRule in info)
			{
				MemberIntegralRule integral = list.FirstOrDefault((MemberIntegralRule a) => a.TypeId == memberIntegralRule.TypeId);
				if (integral == null)
				{
                    context.MemberIntegralRule.Add(memberIntegralRule);
				}
				else
				{
					integral.Integral = memberIntegralRule.Integral;
				}
			}
            context.SaveChanges();
		}
	}
}