using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class CategoryController : BaseSellerController
	{
		public CategoryController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchDeleteCategory(string Ids)
		{
			int num;
			string[] strArrays = Ids.Split(new char[] { '|' });
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrWhiteSpace(str) && int.TryParse(str, out num))
				{
					ServiceHelper.Create<IShopCategoryService>().DeleteCategory(num, base.CurrentSellerManager.ShopId);
				}
			}
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult CreateCategory(string name, long pId)
		{
			if (string.IsNullOrWhiteSpace(name) || name.Length > 5)
			{
				throw new Exception();
			}
			ShopCategoryInfo shopCategoryInfo = new ShopCategoryInfo()
			{
				Name = name,
				ParentCategoryId = pId,
				IsShow = true,
				DisplaySequence = ServiceHelper.Create<IShopCategoryService>().GetCategoryByParentId(pId).Count() + 1,
				ShopId = base.CurrentSellerManager.ShopId
			};
			ServiceHelper.Create<IShopCategoryService>().AddCategory(shopCategoryInfo);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("创建店铺分类，父Id=", pId),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/Category/CreateCategory",
				UserName = base.CurrentSellerManager.UserName,
				ShopId = base.CurrentSellerManager.ShopId
			};
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteCategoryById(long id)
		{
			ServiceHelper.Create<IShopCategoryService>().DeleteCategory(id, base.CurrentSellerManager.ShopId);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("删除店铺分类，Id=", id),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/Category/DeleteCategoryById",
				UserName = base.CurrentSellerManager.UserName,
				ShopId = base.CurrentSellerManager.ShopId
			};
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
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
			IEnumerable<ShopCategoryInfo> categoryByParentId = ServiceHelper.Create<IShopCategoryService>().GetCategoryByParentId(key.Value, base.CurrentSellerManager.ShopId);
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in categoryByParentId
				select new KeyValuePair<long, string>(item.Id, item.Name);
            
			return Json(keyValuePair);
		}

		[UnAuthorize]
		public ActionResult GetCategoryByParentId(int id)
		{
			List<ShopCategoryModel> shopCategoryModels = new List<ShopCategoryModel>();
			foreach (ShopCategoryInfo categoryByParentId in ServiceHelper.Create<IShopCategoryService>().GetCategoryByParentId(id))
			{
				shopCategoryModels.Add(new ShopCategoryModel(categoryByParentId));
			}
			return Json(new { Successful = true, Category = shopCategoryModels }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetCategoryDrop(long id = 0L)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = id == 0,
				Text = "请选择...",
				Value = "0"
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			foreach (ShopCategoryInfo mainCategory in ServiceHelper.Create<IShopCategoryService>().GetMainCategory(base.CurrentSellerManager.ShopId))
			{
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Selected = id == mainCategory.Id,
					Text = mainCategory.Name,
					Value = mainCategory.Id.ToString()
				};
				selectListItems1.Add(selectListItem1);
			}
			return Json(new { successful = true, category = selectListItems1 }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetEffectCategory(long categoryId)
		{
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(categoryId);
			string effectCategoryName = ServiceHelper.Create<ICategoryService>().GetEffectCategoryName(base.CurrentSellerManager.ShopId, category.TypeId);
			return Json(new { json = effectCategoryName }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Management()
		{
			IShopCategoryService shopCategoryService = ServiceHelper.Create<IShopCategoryService>();
			IEnumerable<ShopCategoryInfo> mainCategory = shopCategoryService.GetMainCategory(base.CurrentSellerManager.ShopId);
			List<ShopCategoryModel> shopCategoryModels = new List<ShopCategoryModel>();
			foreach (ShopCategoryInfo shopCategoryInfo in mainCategory)
			{
				shopCategoryModels.Add(new ShopCategoryModel(shopCategoryInfo));
			}
			return View(shopCategoryModels);
		}

		[UnAuthorize]
		public JsonResult UpdateName(string name, long id)
		{
			ServiceHelper.Create<IShopCategoryService>().UpdateCategoryName(id, name);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now
			};
			object[] objArray = new object[] { "修改店铺分类名称，Id=", id, ",名称=", name };
			logInfo.Description = string.Concat(objArray);
			logInfo.IPAddress = base.Request.UserHostAddress;
			logInfo.PageUrl = "/Category/UpdateName";
			logInfo.UserName = base.CurrentSellerManager.UserName;
			logInfo.ShopId = base.CurrentSellerManager.ShopId;
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult UpdateOrder(long order, long id)
		{
			ServiceHelper.Create<IShopCategoryService>().UpdateCategoryDisplaySequence(id, order);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}
	}
}