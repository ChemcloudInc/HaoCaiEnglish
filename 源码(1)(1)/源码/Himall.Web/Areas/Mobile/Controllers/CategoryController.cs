using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Mobile;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class CategoryController : BaseMobileTemplatesController
	{
		public CategoryController()
		{
		}

		private IEnumerable<CategoryModel> GetSubCategories(IEnumerable<CategoryInfo> allCategoies, long categoryId, int depth)
		{
			IOrderedEnumerable<CategoryModel> categoryModels = (
				from item in allCategoies
				where item.ParentCategoryId == categoryId
				select item).Select<CategoryInfo, CategoryModel>((CategoryInfo item) => {
				string empty = string.Empty;
				if (depth == 2)
				{
					empty = item.Icon;
					if (string.IsNullOrWhiteSpace(empty))
					{
						empty = string.Empty;
					}
				}
				return new CategoryModel()
				{
					Id = item.Id,
					Name = item.Name,
					Image = empty,
					SubCategories = GetSubCategories(allCategoies, item.Id, depth + 1),
					Depth = 1,
					DisplaySequence = item.DisplaySequence
				};
			}).OrderBy<CategoryModel, long>((CategoryModel c) => c.DisplaySequence);
			return categoryModels;
		}

		public ActionResult Index()
		{
			CategoryInfo[] array = ServiceHelper.Create<ICategoryService>().GetCategories().ToArray();
			IOrderedEnumerable<CategoryModel> parentCategoryId = 
				from item in array
                where item.ParentCategoryId == 0
                select new CategoryModel()
				{
					Id = item.Id,
					Name = item.Name,
					SubCategories = GetSubCategories(array, item.Id, 1),
					Depth = 0,
					DisplaySequence = item.DisplaySequence
				} into c
				orderby c.DisplaySequence
				select c;
			return View(parentCategoryId);
		}
	}
}