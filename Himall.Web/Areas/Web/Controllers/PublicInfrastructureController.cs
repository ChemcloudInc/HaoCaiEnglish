using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class PublicInfrastructureController : BaseMemberController
	{
		public PublicInfrastructureController()
		{
		}

		public ActionResult Allbrand()
		{
			return View();
		}

		public ActionResult AllCategory()
		{
			return View(GetCategoryJson());
		}

		private List<CategoryJsonModel> GetCategoryJson()
		{
			IEnumerable<CategoryInfo> firstAndSecondLevelCategories = ServiceHelper.Create<ICategoryService>().GetFirstAndSecondLevelCategories();
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			foreach (CategoryInfo categoryInfo in 
				from s in firstAndSecondLevelCategories
				where s.ParentCategoryId == 0
                select s)
			{
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = categoryInfo.Name,
					Id = categoryInfo.Id.ToString(),
					SubCategory = new List<SecondLevelCategory>()
				};
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				foreach (CategoryInfo categoryInfo1 in 
					from s in firstAndSecondLevelCategories
					where s.ParentCategoryId == categoryInfo.Id
					select s)
				{
					SecondLevelCategory secondLevelCategory = new SecondLevelCategory()
					{
						Name = categoryInfo1.Name,
						Id = categoryInfo1.Id.ToString(),
						SubCategory = new List<ThirdLevelCategoty>()
					};
					SecondLevelCategory secondLevelCategory1 = secondLevelCategory;
					foreach (CategoryInfo categoryByParentId in ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(categoryInfo1.Id))
					{
						ThirdLevelCategoty thirdLevelCategoty = new ThirdLevelCategoty()
						{
							Name = categoryByParentId.Name,
							Id = categoryByParentId.Id.ToString()
						};
						secondLevelCategory1.SubCategory.Add(thirdLevelCategoty);
					}
					categoryJsonModel1.SubCategory.Add(secondLevelCategory1);
				}
				categoryJsonModels.Add(categoryJsonModel1);
			}
			return categoryJsonModels;
		}
	}
}