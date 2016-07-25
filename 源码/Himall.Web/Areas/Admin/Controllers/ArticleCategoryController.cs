using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ArticleCategoryController : BaseAdminController
	{
		public ArticleCategoryController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult Add(long? id, string name, long parentId)
		{
			ArticleCategoryInfo articleCategoryInfo = new ArticleCategoryInfo()
			{
				Id = id.GetValueOrDefault(),
				Name = name,
				ParentCategoryId = parentId
			};
			ArticleCategoryInfo articleCategoryInfo1 = articleCategoryInfo;
			IArticleCategoryService articleCategoryService = ServiceHelper.Create<IArticleCategoryService>();
			if (articleCategoryService.CheckHaveRename(articleCategoryInfo1.Id, articleCategoryInfo1.Name))
			{
				return Json(new { success = false, msg = "不可添加、修改为同名栏目！" });
			}
			long? nullable = id;
			if ((nullable.GetValueOrDefault() <= 0 ? true : !nullable.HasValue))
			{
				articleCategoryService.AddArticleCategory(articleCategoryInfo1);
			}
			else
			{
				articleCategoryService.UpdateArticleCategory(articleCategoryInfo1);
			}
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchDelete(string ids)
		{
			char[] chrArray = new char[] { ',' };
			long[] array = (
				from item in ids.Split(chrArray)
				select long.Parse(item)).ToArray();
			ServiceHelper.Create<IArticleCategoryService>().DeleteArticleCategory(array);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<IArticleCategoryService>().DeleteArticleCategory(new long[] { id });
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetArticleCategories(long parentId)
		{
			IQueryable<ArticleCategoryInfo> articleCategoriesByParentId = ServiceHelper.Create<IArticleCategoryService>().GetArticleCategoriesByParentId(parentId, false);
			ArticleCategoryModel[] array = (
				from item in articleCategoriesByParentId
				select new ArticleCategoryModel()
				{
					Id = item.Id,
					Name = item.Name,
					DisplaySequence = item.DisplaySequence,
					HasChild = false,
					Depth = 2
				}).ToArray();
			IArticleCategoryService articleCategoryService = ServiceHelper.Create<IArticleCategoryService>();
			ArticleCategoryModel[] articleCategoryModelArray = array;
			for (int i = 0; i < articleCategoryModelArray.Length; i++)
			{
				ArticleCategoryModel articleCategoryModel = articleCategoryModelArray[i];
				articleCategoryModel.HasChild = articleCategoryService.GetArticleCategoriesByParentId(articleCategoryModel.Id, false).Count() > 0;
			}
			return Json(array);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetArticleCategory(long id)
		{
			ArticleCategoryInfo articleCategory = ServiceHelper.Create<IArticleCategoryService>().GetArticleCategory(id);
			var variable = new { id = id, name = articleCategory.Name, parentId = articleCategory.ParentCategoryId };
			return Json(variable);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetCategories(long? key = null, int? level = -1)
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
			ArticleCategoryInfo[] array = ServiceHelper.Create<IArticleCategoryService>().GetArticleCategoriesByParentId(key.Value, false).ToArray();
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in array
                select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}

		public ActionResult Management()
		{
			IArticleCategoryService articleCategoryService = ServiceHelper.Create<IArticleCategoryService>();
			IQueryable<ArticleCategoryInfo> articleCategoriesByParentId = articleCategoryService.GetArticleCategoriesByParentId(0, false);
			ArticleCategoryModel[] array = (
				from item in articleCategoriesByParentId.ToArray()
                select new ArticleCategoryModel()
				{
					ParentId = item.ParentCategoryId,
					Name = item.Name,
					DisplaySequence = item.DisplaySequence,
					Id = item.Id,
					IsDefault = item.IsDefault
				}).ToArray();
			ArticleCategoryModel[] articleCategoryModelArray = array;
			for (int i = 0; i < articleCategoryModelArray.Length; i++)
			{
				ArticleCategoryModel articleCategoryModel = articleCategoryModelArray[i];
				articleCategoryModel.HasChild = articleCategoryService.GetArticleCategoriesByParentId(articleCategoryModel.Id, false).Count() > 0;
			}
			return View(array);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateName(long id, string name)
		{
			if (ServiceHelper.Create<IArticleCategoryService>().CheckHaveRename(id, name))
			{
				return Json(new { success = false, msg = "不可添加、修改为同名栏目！" });
			}
			ServiceHelper.Create<IArticleCategoryService>().UpdateArticleCategoryName(id, name);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateOrder(long id, int displaySequence)
		{
			ServiceHelper.Create<IArticleCategoryService>().UpdateArticleCategoryDisplaySequence(id, displaySequence);
			return Json(new { success = true });
		}
	}
}