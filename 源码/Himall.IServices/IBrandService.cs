using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IBrandService : IService, IDisposable
	{
		void AddBrand(BrandInfo model);

		void ApplyBrand(ShopBrandApplysInfo model);

		void AuditBrand(long id, ShopBrandApplysInfo.BrandAuditStatus status);

		bool BrandInUse(long id);

		void DeleteApply(int id);

		void DeleteBrand(long id);

		BrandInfo GetBrand(long id);

		ShopBrandApplysInfo GetBrandApply(long id);

		PageModel<BrandInfo> GetBrands(string keyWords, int pageNo, int pageSize);

		IQueryable<BrandInfo> GetBrands(string keyWords);

		IEnumerable<BrandInfo> GetBrandsByCategoryIds(params long[] categoryIds);

		IEnumerable<BrandInfo> GetBrandsByCategoryIds(long shopId, params long[] categoryIds);

		PageModel<ShopBrandApplysInfo> GetShopBrandApplys(long? shopId, int? auditStatus, int pageNo, int pageSize, string keyWords);

		IQueryable<ShopBrandApplysInfo> GetShopBrandApplys(long shopId);

		PageModel<BrandInfo> GetShopBrands(long shopId, int pageNo, int pageSize);

		IQueryable<BrandInfo> GetShopBrands(long shopId);

		bool IsExistApply(long shopId, string brandName);

		bool IsExistBrand(string brandName);

		void UpdateBrand(BrandInfo model);

		void UpdateSellerBrand(ShopBrandApplysInfo model);
	}
}