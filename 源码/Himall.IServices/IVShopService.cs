using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IVShopService : IService, IDisposable
	{
		void AddBuyNumber(long vshopId);

		void AddVisitNumber(long vshopId);

		void AuditRefused(long vshopId);

		void AuditThrough(long vshopId);

		void CloseShop(long vshopId);

		void CreateVshop(VShopInfo vshopInfo);

		void DeleteHotShop(long vshopId);

		void DeleteTopShop(long vshopId);

		IEnumerable<VShopInfo> GetHotShop(VshopQuery vshopQuery, DateTime? startTime, DateTime? endTime, out int total);

		IQueryable<VShopInfo> GetHotShops(int page, int pageSize, out int total);

		VShopInfo GetTopShop();

		IQueryable<VShopInfo> GetUserConcernVShops(long userId, int pageNo, int pageSize);

		VShopInfo GetVShop(long id);

		IEnumerable<VShopInfo> GetVShopByParamete(VshopQuery vshopQuery, out int total);

		VShopInfo GetVShopByShopId(long shopId);

		IQueryable<CouponSettingInfo> GetVShopCouponSetting(long shopid);

		IQueryable<VShopInfo> GetVShops();

		IQueryable<VShopInfo> GetVShops(int page, int pageSize, out int total);

		IQueryable<VShopInfo> GetVShops(int page, int pageSize, out int total, VShopInfo.VshopStates state);

		WXShopInfo GetVShopSetting(long shopId);

		int LogVisit(long id);

		void ReplaceHotShop(long oldHotVShopId, long newHotVshopId);

		void ReplaceTopShop(long oldTopVshopId, long newTopVshopId);

		void SaveVShopCouponSetting(IEnumerable<CouponSettingInfo> infolist);

		void SaveVShopSetting(WXShopInfo wxShop);

		void SetHotShop(long vshopId);

		void SetTopShop(long vshopId);

		void UpdateSequence(long vshopId, int? sequence);

		void UpdateVShop(VShopInfo vshopInfo);
	}
}