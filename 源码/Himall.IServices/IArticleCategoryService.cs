using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IArticleCategoryService : IService, IDisposable
	{
		void AddArticleCategory(ArticleCategoryInfo articleCategory);

		bool CheckHaveRename(long id, string name);

		void DeleteArticleCategory(params long[] ids);

		IQueryable<ArticleCategoryInfo> GetArticleCategoriesByParentId(long parentId, bool recursive = false);

		ArticleCategoryInfo GetArticleCategory(long id);

		IQueryable<ArticleCategoryInfo> GetCategories();

		string GetFullPath(long id, string seperator = ",");

		ArticleCategoryInfo GetSpecialArticleCategory(SpecialCategory categoryType);

		void UpdateArticleCategory(ArticleCategoryInfo articleCategory);

		void UpdateArticleCategoryDisplaySequence(long id, long displaySequence);

		void UpdateArticleCategoryName(long id, string name);
	}
}