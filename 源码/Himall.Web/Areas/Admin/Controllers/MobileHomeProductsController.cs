using Himall.Core;
using Himall.Model;
using Himall.Web;
using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class MobileHomeProductsController : BaseAdminController
	{
		private MobileHomeProducts mobileHomeproduct = new MobileHomeProducts();

		public MobileHomeProductsController()
		{
		}

		[HttpPost]
		public JsonResult AddHomeProducts(string productIds, PlatformType platformType)
		{
            mobileHomeproduct.AddHomeProducts(base.CurrentManager.ShopId, productIds, platformType);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult Delete(long id)
		{
            mobileHomeproduct.Delete(base.CurrentManager.ShopId, id);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult GetAllHomeProductIds(PlatformType platformType)
		{
			object allHomeProductIds = mobileHomeproduct.GetAllHomeProductIds(base.CurrentManager.ShopId, platformType);
			return Json(allHomeProductIds);
		}

		[HttpPost]
		public JsonResult GetMobileHomeProducts(PlatformType platformType, int page, int rows, string keyWords, long? categoryId = null)
		{
			object mobileHomeProducts = mobileHomeproduct.GetMobileHomeProducts(base.CurrentManager.ShopId, platformType, page, rows, keyWords, categoryId);
			return Json(mobileHomeProducts);
		}

		[HttpPost]
		public JsonResult UpdateSequence(long id, short sequence)
		{
            mobileHomeproduct.UpdateSequence(base.CurrentManager.ShopId, id, sequence);
			return Json(new { success = true });
		}
	}
}