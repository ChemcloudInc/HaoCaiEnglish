using Himall.Core;
using Himall.Model;
using Himall.Web;
using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class MobileHomeProductsController : BaseSellerController
	{
		private MobileHomeProducts mobileHomeproduct = new MobileHomeProducts();

		public MobileHomeProductsController()
		{
		}

		[HttpPost]
		public JsonResult AddHomeProducts(string productIds, PlatformType platformType)
		{
            mobileHomeproduct.AddHomeProducts(base.CurrentSellerManager.ShopId, productIds, platformType);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult Delete(long id)
		{
            mobileHomeproduct.Delete(base.CurrentSellerManager.ShopId, id);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult GetAllHomeProductIds(PlatformType platformType)
		{
			object allHomeProductIds = mobileHomeproduct.GetAllHomeProductIds(base.CurrentSellerManager.ShopId, platformType);
			return Json(allHomeProductIds);
		}

		[HttpPost]
		public JsonResult GetMobileHomeProducts(PlatformType platformType, int page, int rows, string brandName, string productName, long? categoryId = null)
		{
			object sellerMobileHomePageProducts = mobileHomeproduct.GetSellerMobileHomePageProducts(base.CurrentSellerManager.ShopId, platformType, page, rows, brandName, categoryId);
			return Json(sellerMobileHomePageProducts);
		}

		[HttpPost]
		public JsonResult UpdateSequence(long id, short sequence)
		{
            mobileHomeproduct.UpdateSequence(base.CurrentSellerManager.ShopId, id, sequence);
			return Json(new { success = true });
		}
	}
}