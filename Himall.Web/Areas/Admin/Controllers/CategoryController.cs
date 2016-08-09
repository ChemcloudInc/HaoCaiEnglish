using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class CategoryController : BaseAdminController
	{
		public CategoryController()
		{
		}

		[UnAuthorize]
		public ActionResult Add()
		{
			if (base.TempData["Categories"] == null)
			{
				base.TempData["Categories"] = GetCatgegotyList();
			}
			if (base.TempData["Types"] == null)
			{
				base.TempData["Types"] = GetTypesList(-1);
			}
			if (base.TempData["Depth"] == null)
			{
				base.TempData["Depth"] = 1;
			}
			return View();
		}

		[HttpPost]
		[OperationLog(Message="添加平台分类")]
		[UnAuthorize]
		public ActionResult Add(CategoryModel category)
		{
			if (base.ModelState.IsValid)
			{
				ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
                ProcessingParentCategoryId(category);
                ProcessingDepth(category, categoryService);
                ProcessingPath(category, categoryService);
                ProcessingDisplaySequence(category, categoryService);
                ProcessingIcon(category, categoryService);
				categoryService.AddCategory(category);
              //  Response.Write(category.EnglishName);
				return RedirectToAction("Management");
			}
			ViewBag.Categories = ServiceHelper.Create<ICategoryService>().GetFirstAndSecondLevelCategories();
			ViewBag.Types = ServiceHelper.Create<ITypeService>().GetTypes();
			return View(category);
		}

		[UnAuthorize]
		public ActionResult AddByParent(long Id)
		{
			List<SelectListItem> catgegotyList = GetCatgegotyList();
			catgegotyList.FirstOrDefault((SelectListItem c) => c.Value.Equals(Id.ToString())).Selected = true;
			base.TempData["Categories"] = catgegotyList;
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(Id);
			string str = category.TypeId.ToString();
			List<SelectListItem> typesList = GetTypesList(-1);
			SelectListItem selectListItem = typesList.FirstOrDefault((SelectListItem c) => c.Value.Equals(str));
			if (selectListItem != null)
			{
				selectListItem.Selected = true;
			}
			else
			{
				typesList.FirstOrDefault().Selected = true;
			}
			base.TempData["Types"] = typesList;
			base.TempData["Depth"] = category.Depth;
			return RedirectToAction("Add");
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
					ServiceHelper.Create<ICategoryService>().DeleteCategory(num);
					IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
					LogInfo logInfo = new LogInfo()
					{
						Date = DateTime.Now,
						Description = string.Concat("删除平台分类，Id=", num),
						IPAddress = base.Request.UserHostAddress,
						PageUrl = string.Concat("/Category/BatchDeleteCategory/", num),
						UserName = base.CurrentManager.UserName,
						ShopId = 0
                    };
					operationLogService.AddPlatformOperationLog(logInfo);
				}
			}
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteCategoryById(long id)
		{
			ServiceHelper.Create<ICategoryService>().DeleteCategory(id);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("删除平台分类，Id=", id),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = string.Concat("/Category/DeleteCategoryById/", id),
				UserName = base.CurrentManager.UserName,
				ShopId = 0
            };
			operationLogService.AddPlatformOperationLog(logInfo);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Edit(long Id = 0L)
		{
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(Id);
			ViewBag.Depth = category.Depth;
			ViewBag.Types = GetTypesList(category.TypeId);
			category.CommisRate = Math.Round(category.CommisRate, 2);
			return View(new CategoryModel(category));
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult Edit(CategoryModel category)
		{
			if (base.ModelState.IsValid)
			{
				ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
                ProcessingIcon(category, categoryService);
				categoryService.UpdateCategory(category);
				IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
				LogInfo logInfo = new LogInfo()
				{
					Date = DateTime.Now,
					Description = string.Concat("修改平台分类，Id=", category.Id),
					IPAddress = base.Request.UserHostAddress,
					PageUrl = string.Concat("/Category/Edit/", category.Id),
					UserName = base.CurrentManager.UserName,
					ShopId = 0
                };
				operationLogService.AddPlatformOperationLog(logInfo);
				return RedirectToAction("Management");
			}
			ViewBag.Types = GetTypesList(category.TypeId);
			ViewBag.Depth = category.Depth;
			return View(category);
		}

		public JsonResult GetCateDepth(long id)
		{
			return Json(new { successful = true, depth = ServiceHelper.Create<ICategoryService>().GetCategory(id).Depth }, JsonRequestBehavior.AllowGet);
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

		public ActionResult GetCategoryById(long id)
		{
			CategoryModel categoryModel = new CategoryModel(ServiceHelper.Create<ICategoryService>().GetCategory(id));
			return Json(new { Successful = true, category = categoryModel }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public ActionResult GetCategoryByParentId(int id)
		{
			List<CategoryModel> categoryModels = new List<CategoryModel>();
			foreach (CategoryInfo categoryByParentId in ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(id))
			{
				categoryModels.Add(new CategoryModel(categoryByParentId));
			}
			return Json(new { Successfly = true, Category = categoryModels }, JsonRequestBehavior.AllowGet);
		}

		private List<SelectListItem> GetCatgegotyList()
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = false,
				Text = "请选择...",
				Value = "0"
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			foreach (CategoryInfo firstAndSecondLevelCategory in ServiceHelper.Create<ICategoryService>().GetFirstAndSecondLevelCategories())
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 1; i < firstAndSecondLevelCategory.Depth; i++)
				{
					stringBuilder.Append("&nbsp;&nbsp;&nbsp;");
				}
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Selected = false,
					Text = string.Concat(stringBuilder, firstAndSecondLevelCategory.Name),
					Value = firstAndSecondLevelCategory.Id.ToString()
				};
				selectListItems1.Add(selectListItem1);
			}
			return selectListItems1;
		}

		[UnAuthorize]
		public ActionResult GetNonLeafCategoryList()
		{
			IEnumerable<CategoryInfo> firstAndSecondLevelCategories = ServiceHelper.Create<ICategoryService>().GetFirstAndSecondLevelCategories();
			List<CategoryDropListModel> categoryDropListModels = new List<CategoryDropListModel>();
			foreach (CategoryInfo firstAndSecondLevelCategory in firstAndSecondLevelCategories)
			{
				CategoryDropListModel categoryDropListModel = new CategoryDropListModel()
				{
					Id = firstAndSecondLevelCategory.Id,
					ParentCategoryId = firstAndSecondLevelCategory.ParentCategoryId,
					Name = firstAndSecondLevelCategory.Name,
					Depth = firstAndSecondLevelCategory.Depth
				};
				categoryDropListModels.Add(categoryDropListModel);
			}
			return Json(new { Successful = true, list = (
				from d in categoryDropListModels
				orderby d.ParentCategoryId, d.Depth, d.Id
				select d).ToList() }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetSecondAndThirdCategoriesByTopId(long id)
		{
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			CategoryTreeModel[] array = (
				from item in categoryService.GetCategoryByParentId(id)
				select new CategoryTreeModel()
				{
					Id = item.Id,
					Name = item.Name,
					ParentCategoryId = item.ParentCategoryId,
					Depth = item.Depth
				}).ToArray();
			CategoryTreeModel[] categoryTreeModelArray = array;
			for (int i = 0; i < categoryTreeModelArray.Length; i++)
			{
				CategoryTreeModel categoryByParentId = categoryTreeModelArray[i];
				categoryByParentId.Children = 
					from item in categoryService.GetCategoryByParentId(categoryByParentId.Id)
					select new CategoryTreeModel()
					{
						Id = item.Id,
						Name = item.Name,
						ParentCategoryId = item.ParentCategoryId,
						Depth = item.Depth
					};
			}
			return Json(new { success = true, categoies = array }, JsonRequestBehavior.AllowGet);
		}

		private List<SelectListItem> GetTypesList(long selectId = -1L)
		{
			IQueryable<ProductTypeInfo> types = ServiceHelper.Create<ITypeService>().GetTypes();
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = false,
				Text = "请选择...",
				Value = "-1"
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			foreach (ProductTypeInfo type in types)
			{
				if (type.Id == selectId)
				{
					SelectListItem selectListItem1 = new SelectListItem()
					{
						Selected = true,
						Text = type.Name,
						Value = type.Id.ToString()
					};
					selectListItems1.Add(selectListItem1);
				}
				else
				{
					SelectListItem selectListItem2 = new SelectListItem()
					{
						Selected = false,
						Text = type.Name,
						Value = type.Id.ToString()
					};
					selectListItems1.Add(selectListItem2);
				}
			}
			return selectListItems1;
		}

		[HttpPost]
		public JsonResult GetValidCategories(long? key = null, int? level = -1)
		{
			IEnumerable<CategoryInfo> validBusinessCategoryByParentId = ServiceHelper.Create<ICategoryService>().GetValidBusinessCategoryByParentId(key.GetValueOrDefault());
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in validBusinessCategoryByParentId
				select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Management()
		{
			IOrderedEnumerable<CategoryInfo> mainCategory = 
				from c in ServiceHelper.Create<ICategoryService>().GetMainCategory()
				orderby c.DisplaySequence
				select c;
			List<CategoryModel> categoryModels = new List<CategoryModel>();
			foreach (CategoryInfo categoryInfo in mainCategory)
			{
				categoryModels.Add(new CategoryModel(categoryInfo));
			}
			return View(categoryModels);
		}

		private void ProcessingDepth(CategoryModel model, ICategoryService IProductCategory)
		{
			if (model.ParentCategoryId == 0)
			{
				model.Depth = 1;
				return;
			}
			CategoryInfo category = IProductCategory.GetCategory(model.ParentCategoryId);
			model.Depth = category.Depth + 1;
		}

		private void ProcessingDisplaySequence(CategoryModel model, ICategoryService IProductCategory)
		{
			int num = IProductCategory.GetCategoryByParentId(model.ParentCategoryId).Count() + 1;
			model.DisplaySequence = num;
		}

		private void ProcessingIcon(CategoryModel model, ICategoryService IProductCategory)
		{
			if (!string.IsNullOrWhiteSpace(model.Icon))
			{
				string str = Server.MapPath(model.Icon);
				string str1 = "/Storage/Plat/Category/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				model.Icon = Path.Combine(str1, Path.GetFileName(str));
			}
		}

		private void ProcessingParentCategoryId(CategoryModel model)
		{
		}

		private void ProcessingPath(CategoryModel model, ICategoryService IProductCategory)
		{
			long maxCategoryId = IProductCategory.GetMaxCategoryId() + 1;
			model.Id = maxCategoryId;
			if (model.ParentCategoryId == 0)
			{
				model.Path = maxCategoryId.ToString();
				return;
			}
			CategoryInfo category = IProductCategory.GetCategory(model.ParentCategoryId);
			model.Path = string.Format("{0}|{1}", category.Path, maxCategoryId);
		}

		[UnAuthorize]
		public JsonResult UpdateName(string name, long id)
		{
			ServiceHelper.Create<ICategoryService>().UpdateCategoryName(id, name);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult UpdateOrder(long order, long id)
		{
			ServiceHelper.Create<ICategoryService>().UpdateCategoryDisplaySequence(id, order);
			return Json(new { Successful = true }, JsonRequestBehavior.AllowGet);
		}
	}
}