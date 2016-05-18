using Himall.Core;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IMobileHomeProductsService : IService, IDisposable
	{
		void AddProductsToHomePage(long shopId, PlatformType platformType, IEnumerable<long> productIds);

		void Delete(long shopId, long id);

		PageModel<MobileHomeProductsInfo> GetMobileHomePageProducts(long shopId, PlatformType platformType, ProductQuery productQuery);

		IQueryable<MobileHomeProductsInfo> GetMobileHomePageProducts(long shopId, PlatformType platformType);

		PageModel<MobileHomeProductsInfo> GetSellerMobileHomePageProducts(long shopId, PlatformType platformType, ProductQuery productQuery);

		void UpdateSequence(long shopId, long id, short sequenc);
	}
}