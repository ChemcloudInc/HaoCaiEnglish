using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ArticleController : BaseWebController
	{
		public ArticleController()
		{
		}

		public ActionResult Category(string id = "1", int pageNo = 1)
		{
			string str;
			int num = 20;
			long num1 = 0;
			long.TryParse(id, out num1);
			IQueryable<ArticleInfo> articleByArticleCategoryId = ServiceHelper.Create<IArticleService>().GetArticleByArticleCategoryId(num1);
			IQueryable<ArticleInfo> isRelease = 
				from m in articleByArticleCategoryId
				where m.IsRelease
				select m;
			List<CategoryJsonModel> articleCate = GetArticleCate();
			CategoryJsonModel categoryJsonModel = articleCate.FirstOrDefault((CategoryJsonModel c) => c.Id == num1.ToString());
			ViewBag.Cate = articleCate;
			ViewBag.ArticleCateId = num1;
			base.ViewBag.Articles = (
				from c in isRelease
				orderby c.Id
				select c).Skip((pageNo - 1) * num).Take(num).ToList();
			dynamic viewBag = base.ViewBag;
			str = (categoryJsonModel != null ? categoryJsonModel.Name : GetCateNameBySecond(num1));
			viewBag.FirstPath = str;
			base.ViewBag.SecondPath = (categoryJsonModel == null ? ServiceHelper.Create<IArticleCategoryService>().GetArticleCategory(num1).Name : "");
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = num,
				TotalItems = isRelease.Count()
			};
			ViewBag.pageInfo = pagingInfo;
			return View();
		}

		public ActionResult Details(long id)
		{
			string str;
			ViewBag.Cate = GetArticleCate();
			ArticleInfo article = ServiceHelper.Create<IArticleService>().GetArticle(id);
			ViewBag.ArticleCateId = article.CategoryId;
			dynamic viewBag = base.ViewBag;
			str = (article.ArticleCategoryInfo.ParentCategoryId == 0 ? article.ArticleCategoryInfo.Name : ServiceHelper.Create<IArticleCategoryService>().GetArticleCategory(article.ArticleCategoryInfo.ParentCategoryId).Name);
			viewBag.FirstPath = str;
			base.ViewBag.SecondPath = (article.ArticleCategoryInfo.ParentCategoryId != 0 ? article.ArticleCategoryInfo.Name : "");
			return View(article);
		}

		private List<CategoryJsonModel> GetArticleCate()
		{
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			ArticleCategoryInfo[] array = ServiceHelper.Create<IArticleCategoryService>().GetCategories().ToArray();
			foreach (ArticleCategoryInfo articleCategoryInfo in 
				from s in array
                where s.ParentCategoryId == 0
                select s)
			{
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = articleCategoryInfo.Name,
					Id = articleCategoryInfo.Id.ToString(),
					SubCategory = new List<SecondLevelCategory>()
				};
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				foreach (ArticleCategoryInfo articleCategoryInfo1 in 
					from s in array
                    where s.ParentCategoryId == articleCategoryInfo.Id
					select s)
				{
					SecondLevelCategory secondLevelCategory = new SecondLevelCategory()
					{
						Name = articleCategoryInfo1.Name,
						Id = articleCategoryInfo1.Id.ToString()
					};
					categoryJsonModel1.SubCategory.Add(secondLevelCategory);
				}
				categoryJsonModels.Add(categoryJsonModel1);
			}
			return categoryJsonModels;
		}

		private string GetCateNameBySecond(long id)
		{
			long parentCategoryId = ServiceHelper.Create<IArticleCategoryService>().GetCategories().FirstOrDefault((ArticleCategoryInfo c) => c.Id.Equals(id)).ParentCategoryId;
			return ServiceHelper.Create<IArticleCategoryService>().GetArticleCategory(parentCategoryId).Name;
		}
	}
}