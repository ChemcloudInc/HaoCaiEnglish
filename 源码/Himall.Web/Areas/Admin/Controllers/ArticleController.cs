using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ArticleController : BaseAdminController
	{
		public ArticleController()
		{
		}

		public ActionResult Add(long? id)
		{
			ArticleModel articleModel;
			if (!id.HasValue)
			{
				articleModel = new ArticleModel()
				{
					IsRelease = true
				};
			}
			else
			{
				ArticleInfo articleInfo = ServiceHelper.Create<IArticleService>().FindById(id.Value);
				ArticleModel articleModel1 = new ArticleModel()
				{
					CategoryId = new long?(articleInfo.CategoryId),
					Content = articleInfo.Content,
					IconUrl = articleInfo.IconUrl,
					Id = articleInfo.Id,
					IsRelease = articleInfo.IsRelease,
					Meta_Description = articleInfo.Meta_Description,
					Meta_Keywords = articleInfo.Meta_Keywords,
					Meta_Title = articleInfo.Meta_Title,
					Title = articleInfo.Title,
					ArticleCategoryFullPath = ServiceHelper.Create<IArticleCategoryService>().GetFullPath(articleInfo.CategoryId, ",")
				};
				articleModel = articleModel1;
			}
			return View(articleModel);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Add(ArticleModel model)
		{
			if (!base.ModelState.IsValid)
			{
				return Json(new { success = false });
			}
			ArticleInfo articleInfo = new ArticleInfo()
			{
				Title = model.Title,
				Meta_Title = model.Meta_Title,
				Meta_Keywords = model.Meta_Keywords,
				Meta_Description = model.Meta_Description,
				IsRelease = model.IsRelease,
				CategoryId = model.CategoryId.GetValueOrDefault(),
				Content = model.Content,
				IconUrl = model.IconUrl,
				Id = model.Id
			};
			ArticleInfo articleInfo1 = articleInfo;
			if (articleInfo1.Id <= 0)
			{
				ServiceHelper.Create<IArticleService>().AddArticle(articleInfo1);
			}
			else
			{
				ServiceHelper.Create<IArticleService>().UpdateArticle(articleInfo1);
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
			ServiceHelper.Create<IArticleService>().DeleteArticle(array);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<IArticleService>().DeleteArticle(new long[] { id });
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(long? categoryId, string titleKeyWords, int rows, int page)
		{
			PageModel<ArticleInfo> pageModel = ServiceHelper.Create<IArticleService>().Find(categoryId, titleKeyWords, rows, page, true);
			var models = 
				from item in pageModel.Models
				select new { id = item.Id, categoryId = item.CategoryId, categoryName = item.ArticleCategoryInfo.Name, isShow = item.IsRelease, title = item.Title, displaySequence = item.DisplaySequence };
			return Json(new { rows = models, total = pageModel.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateDisplaySequence(long id, long displaySequence)
		{
			ServiceHelper.Create<IArticleService>().UpdateArticleDisplaySequence(id, displaySequence);
			return Json(new { success = true });
		}
	}
}