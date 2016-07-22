using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IShopCategoryService : IService, IDisposable
	{
		void AddCategory(ShopCategoryInfo model);

		void DeleteCategory(long id, long shopId);

		IQueryable<CategoryInfo> GetBusinessCategory(long shopId);

		ShopCategoryInfo GetCategory(long id);

		IEnumerable<ShopCategoryInfo> GetCategoryByParentId(long id);

		IEnumerable<ShopCategoryInfo> GetCategoryByParentId(long id, long shopId);

		ShopCategoryInfo GetCategoryByProductId(long id);

		IEnumerable<ShopCategoryInfo> GetMainCategory(long shop);

		IQueryable<ShopCategoryInfo> GetShopCategory(long shopId);

		void UpdateCategory(ShopCategoryInfo model);

		void UpdateCategoryDisplaySequence(long id, long displaySequence);

		void UpdateCategoryName(long id, string name);
	}
}