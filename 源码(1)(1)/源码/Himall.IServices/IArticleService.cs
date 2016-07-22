using Himall.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Himall.IServices
{
	public interface IArticleService : IService, IDisposable
	{
		void AddArticle(ArticleInfo article);

		void DeleteArticle(params long[] ids);

		PageModel<ArticleInfo> Find(long? articleCategoryId, string titleKeyWords, int pageSize, int pageNumber, bool isShowAll = true);

		ArticleInfo FindById(long id);

		ArticleInfo GetArticle(long id);

		IQueryable<ArticleInfo> GetArticleByArticleCategoryId(long articleCategoryId);

		IQueryable<ArticleInfo> GetTopNArticle<T>(int count, long categoryId, Expression<Func<ArticleInfo, T>> sort = null, bool isAsc = false);

		void UpdateArticle(ArticleInfo article);

		void UpdateArticleDisplaySequence(long id, long displaySequence);
	}
}