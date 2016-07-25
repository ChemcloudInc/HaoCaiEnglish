using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himall.Service
{
	public class ArticleCategoryService : ServiceBase, IArticleCategoryService, IService, IDisposable
	{
		public ArticleCategoryService()
		{
		}

		public void AddArticleCategory(ArticleCategoryInfo articleCategory)
		{
			ArticleCategoryInfo articleCategoryInfo = articleCategory;
			if (string.IsNullOrWhiteSpace(articleCategoryInfo.Name))
			{
				throw new HimallException("未指定文章分类名称");
			}
			if (articleCategoryInfo.ParentCategoryId != 0)
			{
				if (context.ArticleCategoryInfo.Count((ArticleCategoryInfo item) => item.Id == articleCategoryInfo.ParentCategoryId) == 0)
				{
					throw new HimallException(string.Concat("不存在父级为", articleCategoryInfo.ParentCategoryId, "的文章分类"));
				}
			}
			articleCategoryInfo.IsDefault = false;
			articleCategoryInfo.DisplaySequence = 1;
			articleCategoryInfo = context.ArticleCategoryInfo.Add(articleCategoryInfo);
            context.SaveChanges();
		}

		public bool CheckHaveRename(long id, string name)
		{
			bool flag = false;
			flag = context.ArticleCategoryInfo.Count((ArticleCategoryInfo d) => d.Id != id && (d.Name == name)) > 0;
			return flag;
		}

		public void DeleteArticleCategory(params long[] ids)
		{
			IQueryable<ArticleCategoryInfo> articleCategoryInfo = 
				from item in context.ArticleCategoryInfo
				where ids.Contains(item.Id)
				select item;
			long[] numArray = ids;
			for (int i = 0; i < numArray.Length; i++)
			{
				long num = numArray[i];
				articleCategoryInfo.Concat<ArticleCategoryInfo>(GetArticleCategoriesByParentId(num, true));
			}
			if (articleCategoryInfo.Count((ArticleCategoryInfo item) => item.IsDefault) > 0)
			{
				throw new HimallException("系统内置分类不能删除");
			}
            context.ArticleCategoryInfo.RemoveRange(articleCategoryInfo);
            context.SaveChanges();
		}

		public IQueryable<ArticleCategoryInfo> GetArticleCategoriesByParentId(long parentId, bool recursive = false)
		{
			IQueryable<ArticleCategoryInfo> articleCategoryInfo = 
				from item in context.ArticleCategoryInfo
				where item.ParentCategoryId == parentId
				select item;
			if (recursive)
			{
				long[] array = (
					from item in articleCategoryInfo
					select item.Id).ToArray();
				long[] numArray = array;
				for (int i = 0; i < numArray.Length; i++)
				{
					long num = numArray[i];
					articleCategoryInfo = articleCategoryInfo.Concat<ArticleCategoryInfo>(GetArticleCategoriesByParentId(num, true));
				}
			}
			return articleCategoryInfo;
		}

		public ArticleCategoryInfo GetArticleCategory(long id)
		{
			return context.ArticleCategoryInfo.FindById<ArticleCategoryInfo>(id);
		}

		public IQueryable<ArticleCategoryInfo> GetCategories()
		{
			return context.ArticleCategoryInfo.FindAll<ArticleCategoryInfo>();
		}

		public string GetFullPath(long id, string seperator = ",")
		{
			StringBuilder stringBuilder = new StringBuilder(id.ToString());
			long parentCategoryId = id;
			do
			{
				parentCategoryId = GetArticleCategory(parentCategoryId).ParentCategoryId;
				stringBuilder.Insert(0, string.Concat(parentCategoryId, seperator));
			}
			while (parentCategoryId != 0);
			return stringBuilder.ToString();
		}

		public ArticleCategoryInfo GetSpecialArticleCategory(SpecialCategory categoryType)
		{
			return GetArticleCategory((long)categoryType);
		}

		public void UpdateArticleCategory(ArticleCategoryInfo articleCategory)
		{
			if (articleCategory == null)
			{
				throw new HimallException("未指定ArticleCategoryInfo实例");
			}
			if (string.IsNullOrWhiteSpace(articleCategory.Name))
			{
				throw new HimallException("未指定文章分类名称");
			}
			if (articleCategory.ParentCategoryId != 0)
			{
				if (context.ArticleCategoryInfo.Count((ArticleCategoryInfo item) => item.Id == articleCategory.ParentCategoryId) == 0)
				{
					throw new HimallException(string.Concat("不存在父级为", articleCategory.ParentCategoryId, "的文章分类"));
				}
			}
			ArticleCategoryInfo name = context.ArticleCategoryInfo.FindById<ArticleCategoryInfo>(articleCategory.Id);
			if (name == null)
			{
				throw new HimallException(string.Concat("未找到id为", articleCategory.Id, "的对象"));
			}
			name.Name = articleCategory.Name;
			name.ParentCategoryId = articleCategory.ParentCategoryId;
            context.SaveChanges();
		}

		public void UpdateArticleCategoryDisplaySequence(long id, long displaySequence)
		{
			ArticleCategoryInfo articleCategoryInfo = context.ArticleCategoryInfo.FindById<ArticleCategoryInfo>(id);
			if (articleCategoryInfo == null)
			{
				throw new HimallException(string.Concat("未找到id为", id, "的对象"));
			}
			articleCategoryInfo.DisplaySequence = displaySequence;
            context.SaveChanges();
		}

		public void UpdateArticleCategoryName(long id, string name)
		{
			ArticleCategoryInfo articleCategoryInfo = context.ArticleCategoryInfo.FindById<ArticleCategoryInfo>(id);
			if (articleCategoryInfo == null)
			{
				throw new HimallException(string.Concat("未找到id为", id, "的对象"));
			}
			articleCategoryInfo.Name = name;
            context.SaveChanges();
		}
	}
}