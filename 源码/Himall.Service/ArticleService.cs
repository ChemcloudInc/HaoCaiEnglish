using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ArticleService : ServiceBase, IArticleService, IService, IDisposable
	{
		public ArticleService()
		{
		}

		public void AddArticle(ArticleInfo article)
		{
			article.AddDate = DateTime.Now;
			article.DisplaySequence = 1;
            context.ArticleInfo.Add(article);
            context.SaveChanges();
			string str = TransferImage(article.IconUrl, article.Id);
			article.IconUrl = str;
			string str1 = string.Concat("/Storage/Plat/Article/", article.Id);
			IOHelper.GetMapPath(str1);
			article.Content = HTMLProcess(article.Content, article.Id);
            context.SaveChanges();
		}

		public void DeleteArticle(params long[] ids)
		{
			IQueryable<ArticleInfo> articleInfos = context.ArticleInfo.FindBy((ArticleInfo item) => ids.Contains(item.Id));
            context.ArticleInfo.RemoveRange(articleInfos);
            context.SaveChanges();
			long[] numArray = ids;
			for (int i = 0; i < numArray.Length; i++)
			{
				long num = numArray[i];
				string str = string.Concat(IOHelper.GetMapPath("/Storage/Plat/Article"), "/", num);
				if (Directory.Exists(str))
				{
					Directory.Delete(str, true);
				}
			}
		}

		public PageModel<ArticleInfo> Find(long? articleCategoryId, string titleKeyWords, int pageSize, int pageNumber, bool isShowAll = true)
		{
			int num;
			IQueryable<ArticleInfo> articleInfo = 
				from p in context.ArticleInfo
				join o in context.ArticleCategoryInfo on p.CategoryId equals o.Id
				select p;
			if (!isShowAll)
			{
				articleInfo = 
					from d in articleInfo
					where d.IsRelease
					select d;
			}
			if (articleCategoryId.HasValue)
			{
				List<long> list = (
					from item in (new ArticleCategoryService()).GetArticleCategoriesByParentId(articleCategoryId.Value, true)
					select item.Id).ToList();
				list.Add(articleCategoryId.Value);
				articleInfo = 
					from item in articleInfo
					where list.Contains(item.CategoryId)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(titleKeyWords))
			{
				articleInfo = 
					from item in articleInfo
					where item.Title.Contains(titleKeyWords)
					select item;
			}
			PageModel<ArticleInfo> pageModel = new PageModel<ArticleInfo>()
			{
				Models = articleInfo.FindBy<ArticleInfo, long>((ArticleInfo item) => true, pageNumber, pageSize, out num, (ArticleInfo item) => item.Id, false),
				Total = num
			};
			return pageModel;
		}

		public ArticleInfo FindById(long id)
		{
			return context.ArticleInfo.FindById<ArticleInfo>(id);
		}

		public ArticleInfo GetArticle(long id)
		{
			return context.ArticleInfo.FindById<ArticleInfo>(id);
		}

		public IQueryable<ArticleInfo> GetArticleByArticleCategoryId(long articleCategoryId)
		{
			return 
				from item in context.ArticleInfo
				where item.CategoryId == articleCategoryId
				select item;
		}

		public IQueryable<ArticleInfo> GetTopNArticle<T>(int num, long categoryId, Expression<Func<ArticleInfo, T>> sort = null, bool isAsc = false)
		{
			long[] array = (
				from a in context.ArticleCategoryInfo
				where a.ParentCategoryId == categoryId
				select a.Id).ToArray();
			IQueryable<ArticleInfo> articleInfos = context.ArticleInfo.FindBy((ArticleInfo a) => (a.CategoryId == categoryId || array.Contains(a.CategoryId)) && a.ArticleCategoryInfo.IsDefault && a.IsRelease);
			if (sort != null)
			{
				articleInfos = (!isAsc ? articleInfos.OrderByDescending<ArticleInfo, T>(sort).Take(num) : articleInfos.OrderBy<ArticleInfo, T>(sort).Take(num));
			}
			else
			{
				articleInfos = (!isAsc ? (
					from item in articleInfos
					orderby item.Id descending
					select item).Take(num) : (
					from item in articleInfos
					orderby item.Id
					select item).Take(num));
			}
			return articleInfos;
		}

		private string HTMLProcess(string content, long id)
		{
			string str = string.Concat("/Storage/Plat/Article/", id);
			string mapPath = IOHelper.GetMapPath(str);
			content = HtmlContentHelper.TransferToLocalImage(content, IOHelper.GetMapPath("/"), mapPath, string.Concat(str, "/"));
			content = HtmlContentHelper.RemoveScriptsAndStyles(content);
			return content;
		}

		private string TransferImage(string tempUrl, long id)
		{
			if (string.IsNullOrWhiteSpace(tempUrl) || tempUrl.Contains("/Storage/"))
			{
				return tempUrl;
			}
			string mapPath = IOHelper.GetMapPath(tempUrl);
			string str = "/Storage/Plat/Article";
			string mapPath1 = IOHelper.GetMapPath(str);
			if (!Directory.Exists(mapPath1))
			{
				Directory.CreateDirectory(mapPath1);
			}
			str = string.Concat(str, "/", id);
			mapPath1 = string.Concat(mapPath1, "/", id);
			if (!Directory.Exists(mapPath1))
			{
				Directory.CreateDirectory(mapPath1);
			}
			string str1 = mapPath.Substring(mapPath.LastIndexOf('.'));
			File.Copy(mapPath, string.Concat(mapPath1, "/image", str1), true);
			return string.Concat(str, "/image", str1);
		}

		public void UpdateArticle(ArticleInfo article)
		{
			ArticleInfo categoryId = context.ArticleInfo.FindById<ArticleInfo>(article.Id);
			categoryId.CategoryId = article.CategoryId;
			categoryId.IconUrl = article.IconUrl;
			categoryId.IsRelease = article.IsRelease;
			categoryId.Meta_Description = article.Meta_Description;
			categoryId.Meta_Keywords = article.Meta_Keywords;
			categoryId.Meta_Title = article.Meta_Title;
			categoryId.Title = article.Title;
			categoryId.IconUrl = TransferImage(categoryId.IconUrl, article.Id);
			categoryId.Content = HTMLProcess(article.Content, article.Id);
            context.SaveChanges();
		}

		public void UpdateArticleDisplaySequence(long id, long displaySequence)
		{
			ArticleInfo articleInfo = context.ArticleInfo.FindById<ArticleInfo>(id);
			if (articleInfo == null)
			{
				throw new HimallException(string.Concat("未找到id为", id, "的文章"));
			}
			articleInfo.DisplaySequence = displaySequence;
            context.SaveChanges();
		}
	}
}