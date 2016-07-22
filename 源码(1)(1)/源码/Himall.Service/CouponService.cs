using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class CouponService : ServiceBase, ICouponService, IService, IDisposable
	{
		private WXCardLogInfo.CouponTypeEnum ThisCouponType = WXCardLogInfo.CouponTypeEnum.Coupon;

		private IWXCardService ser_wxcard;

		public CouponService()
		{
            ser_wxcard = Instance<IWXCardService>.Create;
		}

		public void AddCoupon(CouponInfo info)
		{
            CanAddOrEditCoupon(info);
			ActiveMarketServiceInfo activeMarketServiceInfo = context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo a) => (int)a.TypeId == 2 && a.ShopId == info.ShopId);
			if (activeMarketServiceInfo == null)
			{
				throw new HimallException("您没有订购此服务");
			}
			if (activeMarketServiceInfo.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime.Date) < info.EndTime)
			{
				throw new HimallException("结束日期不能大于订购的日期");
			}
			info.WXAuditStatus = 1;
			if (info.IsSyncWeiXin == 1)
			{
				info.WXAuditStatus = 0;
			}
            context.CouponInfo.Add(info);
            context.SaveChanges();
			if (info.IsSyncWeiXin == 1)
			{
				WXCardLogInfo wXCardLogInfo = new WXCardLogInfo()
				{
					CardColor = info.FormWXColor,
					CardTitle = info.FormWXCTit,
					CardSubTitle = info.FormWXCSubTit,
					CouponType = new WXCardLogInfo.CouponTypeEnum?(ThisCouponType),
					CouponId = new long?(info.Id),
					ShopId = info.ShopId,
					ShopName = info.ShopName,
					ReduceCost = (int)(info.Price * new decimal(100)),
					LeastCost = (int)(info.OrderAmount * new decimal(100)),
					Quantity = info.Num,
					GetLimit = info.PerMax
				};
				decimal price = info.Price;
				wXCardLogInfo.DefaultDetail = string.Concat(price.ToString("F2"), "元优惠券1张");
				wXCardLogInfo.BeginTime = info.StartTime.Date;
				DateTime dateTime = info.EndTime.AddDays(1);
				wXCardLogInfo.EndTime = dateTime.AddMinutes(-1);
				if (!ser_wxcard.Add(wXCardLogInfo))
				{
                    context.CouponInfo.Remove(info);
                    context.SaveChanges();
					throw new HimallException("同步微信卡券失败，请检查参数是否有错！");
				}
				info.CardLogId = new long?(wXCardLogInfo.Id);
                context.SaveChanges();
			}
            SaveCover(info);
		}

		public void AddCouponRecord(CouponRecordInfo info)
		{
			string shopName = context.ShopInfo.FindById<ShopInfo>(info.ShopId).ShopName;
			CouponInfo couponInfo = context.CouponInfo.FirstOrDefault((CouponInfo d) => d.Id == info.CouponId);
			if (couponInfo.IsSyncWeiXin == 1 && couponInfo.WXAuditStatus != 1)
			{
				throw new HimallException("优惠券状态错误，不可领取");
			}
			if (couponInfo.ReceiveType == CouponInfo.CouponReceiveType.IntegralExchange)
			{
				MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
				{
					UserName = info.UserName,
					MemberId = info.UserId,
					RecordDate = new DateTime?(DateTime.Now),
					TypeId = MemberIntegral.IntegralType.Exchange,
					Integral = couponInfo.NeedIntegral
				};
				decimal price = couponInfo.Price;
				memberIntegralRecord.ReMark = string.Concat("兑换优惠券:面值", price.ToString("f2"));
				IConversionMemberIntegralBase conversionMemberIntegralBase = Instance<IMemberIntegralConversionFactoryService>.Create.Create(MemberIntegral.IntegralType.Exchange, memberIntegralRecord.Integral);
				Instance<IMemberIntegralService>.Create.AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
			}
			info.CounponStatus = CouponRecordInfo.CounponStatuses.Unuse;
			CouponRecordInfo couponRecordInfo = info;
			Guid guid = Guid.NewGuid();
			couponRecordInfo.CounponSN = guid.ToString().Replace("-", "");
			info.UsedTime = null;
			info.CounponTime = DateTime.Now;
			info.ShopName = shopName;
			info.OrderId = null;
            context.CouponRecordInfo.Add(info);
            context.SaveChanges();
		}

		public bool CanAddIntegralCoupon(long shopid, long id = 0L)
		{
			DateTime date = DateTime.Now.Date;
			DateTime now = DateTime.Now;
			IQueryable<CouponInfo> couponInfo = 
				from d in context.CouponInfo
				where d.ShopId == shopid && (int)d.ReceiveType == 1 && (d.EndIntegralExchange >= now) && (d.EndTime >= date)
				select d;
			if (id > 0)
			{
				couponInfo = 
					from d in couponInfo
					where d.Id != id
					select d;
			}
			return couponInfo.Count() < 1;
		}

		private void CanAddOrEditCoupon(CouponInfo info)
		{
			List<long> list = (
				from a in context.CouponInfo
				where (a.EndTime > DateTime.Now) && a.ShopId == info.ShopId && (int)a.ReceiveType != 1 && (int)a.ReceiveType != 2
				select a.Id).ToList();
			List<CouponSettingInfo> couponSettingInfos = (
				from a in context.CouponSettingInfo
				where list.Contains(a.CouponID)
				select a).ToList();
			if (info.Himall_CouponSetting != null && info.Himall_CouponSetting.Count > 0)
			{
				if (couponSettingInfos.Count((CouponSettingInfo a) => a.PlatForm == PlatformType.Wap) >= 5)
				{
					if (!list.Any((long d) => d == info.Id))
					{
						throw new HimallException("最多添加5个移动端优惠券");
					}
				}
				if (couponSettingInfos.Count((CouponSettingInfo a) => a.PlatForm == PlatformType.PC) >= 5)
				{
					if (!list.Any((long d) => d == info.Id))
					{
						throw new HimallException("最多添加5个PC端个优惠券");
					}
				}
			}
		}

		public void CancelCoupon(long couponId, long shopId)
		{
			CouponInfo couponInfo = context.CouponInfo.FirstOrDefault((CouponInfo a) => a.ShopId == shopId && a.Id == couponId);
			if (couponInfo == null)
			{
				throw new HimallException("找不到相对应的优惠券！");
			}
			couponInfo.EndTime = DateTime.Now.Date.AddDays(-1);
            context.SaveChanges();
			if (couponInfo.IsSyncWeiXin == 1 && couponInfo.CardLogId.HasValue)
			{
                ser_wxcard.Delete(couponInfo.CardLogId.Value);
			}
		}

		public void ClearErrorWeiXinCardSync()
		{
			DateTime date = DateTime.Now.AddDays(-2).Date;
			int num = 0;
			List<CouponInfo> list = (
				from d in context.CouponInfo
				where (d.CreateTime < date) && d.IsSyncWeiXin == 1 && d.WXAuditStatus == num
				select d).ToList();
			if (list.Count > 0)
			{
				List<long?> nullables = (
					from d in list
					select d.CardLogId).ToList<long?>();
				List<WXCardLogInfo> wXCardLogInfos = (
					from d in context.WXCardLogInfo
					where nullables.Contains(d.Id)
					select d).ToList();
				if (wXCardLogInfos.Count > 0)
				{
                    context.WXCardLogInfo.RemoveRange(wXCardLogInfos);
				}
				foreach (CouponInfo couponInfo in list)
				{
					couponInfo.WXAuditStatus = -1;
					couponInfo.EndTime = DateTime.Now.AddDays(-1);
				}
                context.SaveChanges();
			}
		}

		public void EditCoupon(CouponInfo info)
		{
            CanAddOrEditCoupon(info);
			CouponInfo himallCouponSetting = context.CouponInfo.FirstOrDefault((CouponInfo a) => a.ShopId == info.ShopId && a.Id == info.Id);
			if (himallCouponSetting == null)
			{
				throw new HimallException("错误的优惠券信息");
			}
			int num = context.CouponRecordInfo.Count((CouponRecordInfo d) => d.CouponId == himallCouponSetting.Id);
			if (info.Num < num)
			{
				throw new HimallException("错误的发放总量，己领取数己超过发放总量");
			}
            context.CouponSettingInfo.RemoveRange(himallCouponSetting.Himall_CouponSetting);
			if (info.Himall_CouponSetting != null && info.Himall_CouponSetting.Count > 0)
			{
				himallCouponSetting.Himall_CouponSetting = info.Himall_CouponSetting;
			}
			himallCouponSetting.CouponName = info.CouponName;
			himallCouponSetting.PerMax = info.PerMax;
			himallCouponSetting.Num = info.Num;
			himallCouponSetting.ReceiveType = info.ReceiveType;
			himallCouponSetting.EndIntegralExchange = info.EndIntegralExchange;
			himallCouponSetting.NeedIntegral = info.NeedIntegral;
			himallCouponSetting.IntegralCover = info.IntegralCover;
			if (himallCouponSetting.IsSyncWeiXin == 1 && himallCouponSetting.CardLogId.HasValue)
			{
				WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == himallCouponSetting.CardLogId.Value);
				if (wXCardLogInfo != null)
				{
					int num1 = himallCouponSetting.Num - num;
                    ser_wxcard.EditGetLimit(new int?(himallCouponSetting.PerMax), wXCardLogInfo.CardId);
                    ser_wxcard.EditStock(num1, wXCardLogInfo.CardId);
				}
			}
            context.SaveChanges();
            SaveCover(himallCouponSetting);
		}

		public List<CouponRecordInfo> GetAllCoupon(long userid)
		{
			return (
				from p in context.CouponRecordInfo
				where p.UserId == userid
				select p).ToList();
		}

		public List<UserCouponInfo> GetAllUserCoupon(long userid)
		{
			DateTime now = DateTime.Now;
			IQueryable<CouponRecordInfo> couponRecordInfo = 
				from item in context.CouponRecordInfo
				where item.UserId == userid && item.CounponStatus == 0 && (item.Himall_Coupon.EndTime > now)
				select item;
			List<UserCouponInfo> list = (
				from b in couponRecordInfo
				select new UserCouponInfo()
				{
					UserId = b.UserId,
					ShopId = b.ShopId,
					CouponId = b.CouponId,
					Price = b.Himall_Coupon.Price,
					PerMax = b.Himall_Coupon.PerMax,
					OrderAmount = b.Himall_Coupon.OrderAmount,
					Num = b.Himall_Coupon.Num,
					StartTime = b.Himall_Coupon.StartTime,
					EndTime = b.Himall_Coupon.EndTime,
					ShopName = b.Himall_Coupon.ShopName,
					CreateTime = b.Himall_Coupon.CreateTime,
					CouponName = b.Himall_Coupon.CouponName,
					UseStatus = b.CounponStatus,
					UseTime = b.UsedTime
				}).ToList();
			return list;
		}

		public CouponInfo GetCouponInfo(long shopId, long couponId)
		{
			CouponInfo couponInfo = context.CouponInfo.FirstOrDefault((CouponInfo a) => a.ShopId == shopId && a.Id == couponId);
			if (couponInfo != null && couponInfo.IsSyncWeiXin == 1)
			{
				couponInfo.WXCardInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo a) => a.Id == couponInfo.CardLogId);
			}
			return couponInfo;
		}

		public CouponInfo GetCouponInfo(long couponId)
		{
			CouponInfo couponInfo = context.CouponInfo.FirstOrDefault((CouponInfo a) => a.Id == couponId);
			if (couponInfo != null && couponInfo.IsSyncWeiXin == 1)
			{
				couponInfo.WXCardInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo a) => a.Id == couponInfo.CardLogId);
			}
			return couponInfo;
		}

		public PageModel<CouponInfo> GetCouponList(CouponQuery query)
		{
			long? shopId = query.ShopId;
			if ((shopId.GetValueOrDefault() > 0 ? false : shopId.HasValue))
			{
				throw new HimallException("ShopId不能识别");
			}
			int num = 0;
			int num1 = 1;
			IQueryable<CouponInfo> wXAuditStatus = context.CouponInfo.FindBy((CouponInfo d) => d.ShopId == query.ShopId);
			bool? isShowAll = query.IsShowAll;
			if ((!isShowAll.GetValueOrDefault() ? true : !isShowAll.HasValue))
			{
				wXAuditStatus = 
					from d in wXAuditStatus
					where d.WXAuditStatus == num1
					select d;
			}
			if (!string.IsNullOrWhiteSpace(query.CouponName))
			{
				wXAuditStatus = 
					from d in wXAuditStatus
					where d.CouponName.Contains(query.CouponName)
					select d;
			}
			wXAuditStatus = wXAuditStatus.GetPage(out num, (IQueryable<CouponInfo> d) => 
				from o in d
				orderby o.EndTime descending
				select o, query.PageNo, query.PageSize);
			return new PageModel<CouponInfo>()
			{
				Models = wXAuditStatus,
				Total = num
			};
		}

		public IQueryable<CouponInfo> GetCouponList(long shopid)
		{
			int num = 1;
			return 
				from item in context.CouponInfo
				where item.ShopId == shopid && item.WXAuditStatus == num
				select item;
		}

		public CouponRecordInfo GetCouponRecordById(long id)
		{
			CouponRecordInfo couponRecordInfo = context.CouponRecordInfo.FirstOrDefault((CouponRecordInfo d) => d.Id == id);
			if (couponRecordInfo.WXCodeId.HasValue)
			{
				couponRecordInfo.WXCardCodeInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.Id == couponRecordInfo.WXCodeId.Value);
			}
			return couponRecordInfo;
		}

		public CouponRecordInfo GetCouponRecordInfo(long userId, long orderId)
		{
			CouponRecordInfo couponRecordInfo = context.CouponRecordInfo.FirstOrDefault((CouponRecordInfo a) => a.UserId == userId && a.OrderId == orderId);
			if (couponRecordInfo != null && couponRecordInfo.WXCodeId.HasValue)
			{
				couponRecordInfo.WXCardCodeInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.Id == couponRecordInfo.WXCodeId.Value);
			}
			return couponRecordInfo;
		}

		public PageModel<CouponRecordInfo> GetCouponRecordList(CouponRecordQuery query)
		{
			int num = 0;
			DateTime now = DateTime.Now;
			IQueryable<CouponRecordInfo> couponId = context.CouponRecordInfo.AsQueryable<CouponRecordInfo>();
			if (query.CouponId.HasValue)
			{
				couponId = 
					from d in couponId
					where d.CouponId == query.CouponId
                    select d;
			}
			if (query.UserId.HasValue)
			{
				couponId = 
					from d in couponId
					where d.UserId == query.UserId.Value
					select d;
			}
			if (query.ShopId.HasValue)
			{
				couponId = 
					from d in couponId
					where d.ShopId == query.ShopId.Value
					select d;
			}
			if (!string.IsNullOrWhiteSpace(query.UserName))
			{
				couponId = 
					from d in couponId
					where d.UserName.Contains(query.UserName)
					select d;
			}
			int? status = query.Status;
			int valueOrDefault = status.GetValueOrDefault();
			if (status.HasValue)
			{
				switch (valueOrDefault)
				{
					case 0:
					{
						couponId = 
							from item in couponId
							where item.CounponStatus == 0 && (item.Himall_Coupon.EndTime > now)
							select item;
						break;
					}
					case 1:
					{
						couponId = 
							from item in couponId
							where (int)item.CounponStatus == 1
							select item;
						break;
					}
					case 2:
					{
						couponId = 
							from item in couponId
							where item.CounponStatus == 0 && (item.Himall_Coupon.EndTime <= now)
							select item;
						break;
					}
				}
			}
			couponId = couponId.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<CouponRecordInfo>()
			{
				Models = couponId,
				Total = num
			};
		}

		public ActiveMarketServiceInfo GetCouponService(long shopId)
		{
			if (shopId <= 0)
			{
				throw new HimallException("ShopId不能识别");
			}
			return context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo m) => m.ShopId == shopId && (int)m.TypeId == 2);
		}

		public PageModel<CouponInfo> GetIntegralCoupons(int page, int pageSize)
		{
			PageModel<CouponInfo> pageModel = new PageModel<CouponInfo>();
			DateTime now = DateTime.Now;
			DateTime dateTime = DateTime.Now;
			int num = 1;
			IQueryable<CouponInfo> couponInfo = 
				from d in context.CouponInfo
				where (int)d.ReceiveType == 1 && (d.EndIntegralExchange >= dateTime) && (d.EndTime >= now) && (d.StartTime <= now) && d.WXAuditStatus == num
				select d;
			int num1 = 0;
			couponInfo = couponInfo.GetPage(out num1, page, pageSize, (IQueryable<CouponInfo> d) => 
				from o in d
				orderby o.CreateTime descending
				select o);
			pageModel.Models = couponInfo;
			pageModel.Total = num1;
			return pageModel;
		}

		public IEnumerable<CouponRecordInfo> GetOrderCoupons(long userId, IEnumerable<long> Ids)
		{
			DateTime now = DateTime.Now;
			return (
				from a in context.CouponRecordInfo
				where a.UserId == userId && Ids.Contains(a.Id) && a.CounponStatus == 0 && (a.Himall_Coupon.StartTime <= now) && (a.Himall_Coupon.EndTime > now)
				select a).DistinctBy<CouponRecordInfo, long>((CouponRecordInfo a) => a.ShopId);
		}

		public IEnumerable<CouponInfo> GetTopCoupon(long shopId, int top = 5, PlatformType type = 0)
		{
			DateTime now = DateTime.Now;
			int num = 1;
			return (
				from a in context.CouponInfo
				where a.ShopId == shopId && (a.EndTime >= now) && a.WXAuditStatus == num && a.Himall_CouponSetting.Any((CouponSettingInfo b) => b.PlatForm == 0)
				orderby a.Price descending
				select a).Take(top);
		}

		public List<CouponRecordInfo> GetUserCoupon(long shopId, long userId, decimal totalPrice)
		{
			DateTime now = DateTime.Now;
			return (
				from item in context.CouponRecordInfo
				where item.ShopId == shopId && item.UserId == userId && item.CounponStatus == 0 && (item.Himall_Coupon.StartTime <= now) && (item.Himall_Coupon.EndTime > now) && item.Himall_Coupon.OrderAmount <= totalPrice && item.Himall_Coupon.Price < totalPrice
				orderby item.Himall_Coupon.Price descending
				select item).ToList();
		}

		public IQueryable<UserCouponInfo> GetUserCouponList(long userid)
		{
			IQueryable<UserCouponInfo> couponInfo = 
				from a in context.CouponInfo
				join b in 
					from item in context.CouponRecordInfo
					where item.UserId == userid
					select item on a.Id equals b.CouponId
				select new UserCouponInfo()
				{
					UserId = b.UserId,
					ShopId = a.ShopId,
					CouponId = a.Id,
					Price = a.Price,
					PerMax = a.PerMax,
					OrderAmount = a.OrderAmount,
					Num = a.Num,
					StartTime = a.StartTime,
					EndTime = a.EndTime,
					CreateTime = a.CreateTime,
					CouponName = a.CouponName,
					UseStatus = b.CounponStatus,
					UseTime = b.UsedTime
				};
			return couponInfo;
		}

		public int GetUserReceiveCoupon(long couponId, long userId)
		{
			return context.CouponRecordInfo.Count((CouponRecordInfo a) => a.CouponId == couponId && a.UserId == userId);
		}

		public void SaveCover(CouponInfo model)
		{
			string integralCover = model.IntegralCover;
			if (!string.IsNullOrWhiteSpace(integralCover) && integralCover.IndexOf("temp", StringComparison.InvariantCultureIgnoreCase) > -1)
			{
				string mapPath = IOHelper.GetMapPath(model.IntegralCover);
				if (!File.Exists(mapPath))
				{
					model.IntegralCover = "";
				}
				else
				{
					FileInfo fileInfo = new FileInfo(mapPath);
					string str = string.Format("/Storage/Shop/{0}/Coupon/", model.ShopId);
					string mapPath1 = IOHelper.GetMapPath(str);
					long id = model.Id;
					string str1 = string.Concat(id.ToString(), fileInfo.Extension);
					Directory.CreateDirectory(mapPath1);
					IOHelper.CopyFile(mapPath, mapPath1, true, str1);
					str = Path.Combine(str, str1);
					model.IntegralCover = str;
				}
                context.SaveChanges();
			}
		}

		public void SyncWeixinCardAudit(long id, string cardid, WXCardLogInfo.AuditStatusEnum auditstatus)
		{
			CouponInfo couponInfo = context.CouponInfo.FirstOrDefault((CouponInfo d) => d.Id == id);
			if (couponInfo != null)
			{
				couponInfo.WXAuditStatus = (int)auditstatus;
                context.SaveChanges();
			}
		}

		public void UseCoupon(long userId, IEnumerable<long> Ids, IEnumerable<OrderInfo> orders)
		{
			DateTime date = DateTime.Now.Date;
			List<CouponRecordInfo> list = (
				from a in context.CouponRecordInfo.Include("Himall_Coupon")
				where a.UserId == userId && Ids.Contains(a.Id) && a.CounponStatus == 0 && (a.Himall_Coupon.EndTime > date)
				select a).DistinctBy<CouponRecordInfo, long>((CouponRecordInfo a) => a.ShopId).ToList();
			foreach (CouponRecordInfo nullable in list)
			{
				nullable.CounponStatus = CouponRecordInfo.CounponStatuses.Used;
				nullable.UsedTime = new DateTime?(DateTime.Now);
				nullable.OrderId = new long?(orders.FirstOrDefault((OrderInfo a) => {
					if (a.ShopId != nullable.ShopId)
					{
						return false;
					}
					return a.ProductTotalAmount >= nullable.Himall_Coupon.OrderAmount;
				}).Id);
                ser_wxcard.Consume(nullable.Id, ThisCouponType);
			}
            context.SaveChanges();
		}
	}
}