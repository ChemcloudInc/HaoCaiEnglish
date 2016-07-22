using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class CategoryController : BaseWebController
	{
		public CategoryController()
		{
		}

		[UnAuthorize]
		public JsonResult GetAuthorizationCategory(long shopId, long? key = null, int? level = -1)
		{
			long? nullable = key;
			int? nullable1 = level;
			if ((nullable1.GetValueOrDefault() != -1 ? false : nullable1.HasValue))
			{
				nullable = new long?(0);
			}
			if (!nullable.HasValue)
			{
				return Json(new object[0]);
			}
			CategoryInfo[] array = (
				from r in ServiceHelper.Create<IShopCategoryService>().GetBusinessCategory(shopId)
				where r.ParentCategoryId == nullable.Value
				select r).ToArray();
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in array
                select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetCategory(long? key = null, int? level = -1)
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
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in categoryByParentId
				select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}
	}
}