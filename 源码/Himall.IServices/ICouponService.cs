using Himall.Core;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface ICouponService : IService, IDisposable
	{
		void AddCoupon(CouponInfo info);

		void AddCouponRecord(CouponRecordInfo info);

		bool CanAddIntegralCoupon(long shopid, long id = 0L);

		void CancelCoupon(long couponId, long shopId);

		void ClearErrorWeiXinCardSync();

		void EditCoupon(CouponInfo info);

		List<UserCouponInfo> GetAllUserCoupon(long userid);

		CouponInfo GetCouponInfo(long shopId, long couponId);

		CouponInfo GetCouponInfo(long couponId);

		PageModel<CouponInfo> GetCouponList(CouponQuery query);

		IQueryable<CouponInfo> GetCouponList(long shopid);

		CouponRecordInfo GetCouponRecordById(long id);

		CouponRecordInfo GetCouponRecordInfo(long userId, long orderId);

		PageModel<CouponRecordInfo> GetCouponRecordList(CouponRecordQuery query);

		ActiveMarketServiceInfo GetCouponService(long shopId);

		PageModel<CouponInfo> GetIntegralCoupons(int page, int pageSize);

		IEnumerable<CouponRecordInfo> GetOrderCoupons(long userId, IEnumerable<long> Ids);

		IEnumerable<CouponInfo> GetTopCoupon(long shopId, int top = 5, PlatformType type = 0);

		List<CouponRecordInfo> GetUserCoupon(long shopId, long userId, decimal totalPrice);

		IQueryable<UserCouponInfo> GetUserCouponList(long userid);

		int GetUserReceiveCoupon(long couponId, long userId);

		void SyncWeixinCardAudit(long id, string cardid, WXCardLogInfo.AuditStatusEnum auditstatus);

		void UseCoupon(long userId, IEnumerable<long> Ids, IEnumerable<OrderInfo> orders);
	}
}