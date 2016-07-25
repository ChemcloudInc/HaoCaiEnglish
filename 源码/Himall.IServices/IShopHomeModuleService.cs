using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IShopHomeModuleService : IService, IDisposable
	{
		void AddShopProductModule(ShopHomeModuleInfo shopProductModuleInfo);

		void Delete(long shopId, long id);

		IQueryable<ShopHomeModuleInfo> GetAllShopHomeModuleInfos(long shopId);

		ShopHomeModuleInfo GetShopHomeModuleInfo(long shopId, long id);

		void UpdateShopProductModuleName(long shopId, long id, string name);

		void UpdateShopProductModuleProducts(long shopId, long id, IEnumerable<long> productIds);
	}
}