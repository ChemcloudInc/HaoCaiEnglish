using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IShopService : IService, IDisposable
	{
		void AddFavoriteShop(long memberId, long shopId);

		long AddShop(ShopInfo shop);

		void AddShopGrade(ShopGradeInfo shopGrade);

		void CancelConcernShops(IEnumerable<long> ids, long userId);

		void CancelConcernShops(long shopI, long userId);

		ShopInfo CreateEmptyShop();

		void DeleteShop(long id);

		void DeleteShopGrade(long id, out string msg);

		bool ExistCompanyName(string companyName, long shopId = 0L);

		bool ExistShop(string shopName, long shopId = 0L);

		PageModel<ShopInfo> GetAuditingShops(ShopQuery shopQueryModel);

		IQueryable<BusinessCategoryInfo> GetBusinessCategory(long id);

		IQueryable<FavoriteShopInfo> GetFavoriteShopInfos(long memberId);

		PlatConsoleModel GetPlatConsoleMode();

		int GetSales(long id);

		SellerConsoleModel GetSellerConsoleModel(long shopId);

		PageModel<ShopInfo> GetSellers(SellerQuery sellerQueryModel);

		ShopInfo GetShop(long id, bool businessCategoryOn = false);

		ShopInfo GetShopBasicInfo(long id);

		long GetShopConcernedCount(long shopId);

		ShopGradeInfo GetShopGrade(long id);

		IQueryable<ShopGradeInfo> GetShopGrades();

		string GetShopName(long id);

		PageModel<ShopInfo> GetShops(ShopQuery shopQueryModel);

		long GetShopSpaceUsage(long shopId);

		IQueryable<StatisticOrderCommentsInfo> GetShopStatisticOrderComments(long shopId);

		ShopInfo.ShopVistis GetShopVistiInfo(DateTime startDate, DateTime endDate, long shopId);

		PageModel<FavoriteShopInfo> GetUserConcernShops(long userId, int pageNo, int pageSize);

		bool IsExpiredShop(long shopId);

		bool IsFavoriteShop(long memberId, long shopId);

		void LogShopVisti(long shopId);

		void SaveBusinessCategory(long shopId, Dictionary<long, decimal> bCategoryList);

		void SaveBusinessCategory(long id, decimal commisRate);

		void UpdateLogo(long shopId, string img);

		void UpdateShop(ShopInfo shop);

		void UpdateShop(ShopInfo shop, IEnumerable<long> categoryIds);

		void UpdateShopFreight(long shopId, decimal freight, decimal freeFreight);

		void UpdateShopGrade(ShopGradeInfo shopGrade);

		void UpdateShopSenderInfo(long shopId, int regionId, string address, string senderName, string senderPhone);

		void UpdateShopStatus(long shopId, ShopInfo.ShopAuditStatus status, string comments = "");
	}
}