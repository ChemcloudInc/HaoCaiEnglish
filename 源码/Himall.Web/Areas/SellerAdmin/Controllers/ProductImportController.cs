using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ProductImportController : BaseSellerController
	{
		private long _shopid;

		private long _userid;

		public ProductImportController()
		{
            _shopid = base.CurrentSellerManager.ShopId;
            _userid = base.CurrentSellerManager.Id;
		}

		public JsonResult GetImportCount()
		{
			long num = 0;
			long num1 = 0;
			int num2 = 0;
            GetImportCountFromCache(out num, out num1);
			if (num1 == num && num1 > 0)
			{
				num2 = 1;
			}
			return Json(new { Count = num, Total = num1, Success = num2 }, JsonRequestBehavior.AllowGet);
		}

		private void GetImportCountFromCache(out long count, out long total)
		{
			object obj = Cache.Get(CacheKeyCollection.UserImportProductCount(_userid));
			object obj1 = Cache.Get(CacheKeyCollection.UserImportProductTotal(_userid));
			count = (obj == null ? 0 : long.Parse(obj.ToString()));
			total = (obj1 == null ? 0 : long.Parse(obj1.ToString()));
			if (count == total && total > 0)
			{
				Cache.Remove(CacheKeyCollection.UserImportProductCount(_userid));
				Cache.Remove(CacheKeyCollection.UserImportProductTotal(_userid));
			}
		}

		public JsonResult GetImportOpCount()
		{
			long num = 0;
			object obj = Cache.Get("Cache-UserImportOpCount");
			if (obj != null)
			{
				num = (string.IsNullOrEmpty(obj.ToString()) ? 0 : long.Parse(obj.ToString()));
			}
			return Json(new { Count = num }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetPlatFormCategory(long? key = null, int? level = -1)
		{
			int? nullable = level;
			if ((nullable.GetValueOrDefault() != -1 ? false : nullable.HasValue))
			{
				key = new long?(0);
			}
			if (!key.HasValue)
			{
				return Json(new object[0]);
			}
			IEnumerable<CategoryInfo> categoryByParentId = ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(key.Value);
			long? nullable1 = key;
			if ((nullable1.GetValueOrDefault() != 0 ? false : nullable1.HasValue))
			{
				IShopService shopService = ServiceHelper.Create<IShopService>();
				if (!(shopService.GetShop(base.CurrentSellerManager.ShopId, false) ?? new ShopInfo()).IsSelf)
				{
					IQueryable<long> businessCategory = 
						from e in shopService.GetBusinessCategory(base.CurrentSellerManager.ShopId)
						select e.CategoryId;
					categoryByParentId = ServiceHelper.Create<ICategoryService>().GetTopLevelCategories(businessCategory);
				}
			}
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in categoryByParentId
				select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetShopBrand(long categoryId)
		{
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			long shopId = base.CurrentSellerManager.ShopId;
			long[] numArray = new long[] { categoryId };
			IEnumerable<KeyValuePair<long, string>> brandsByCategoryIds = 
				from item in brandService.GetBrandsByCategoryIds(shopId, numArray)
				select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(brandsByCategoryIds);
		}

		public ActionResult ImportManage()
		{
			long num = 0;
			long num1 = 0;
			int num2 = 0;
            GetImportCountFromCache(out num, out num1);
			if (num1 == num && num1 > 0)
			{
				num2 = 1;
			}
			ViewBag.Count = num;
			ViewBag.Total = num1;
			ViewBag.Success = num2;
			ViewBag.shopid = _shopid;
			ViewBag.userid = _userid;
			return View();
		}
	}
}