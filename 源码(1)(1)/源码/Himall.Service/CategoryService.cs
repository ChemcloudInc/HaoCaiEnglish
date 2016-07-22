using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himall.Service
{
	public class CategoryService : ServiceBase, ICategoryService, IService, IDisposable
	{
		private const char CATEGORY_PATH_SEPERATOR = '|';

		public CategoryService()
		{
		}

		public void AddCategory(CategoryInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model", "添加一个分类时，Model为空");
			}
			if (model.ParentCategoryId == 0)
			{
				CategoryCashDepositInfo categoryCashDepositInfo = new CategoryCashDepositInfo()
				{
					Id = 0,
					CategoryId = model.Id
				};
				model.Himall_CategoryCashDeposit = categoryCashDepositInfo;
			}
            context.CategoryInfo.Add(model);
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void DeleteCategory(long id)
		{
			if (context.CategoryInfo.FindById<CategoryInfo>(id).Depth != 3)
			{
				IEnumerable<CategoryInfo> secondAndThirdLevelCategories = GetSecondAndThirdLevelCategories(new long[] { id });
				if (secondAndThirdLevelCategories.Any((CategoryInfo c) => context.BusinessCategoryInfo.Any((BusinessCategoryInfo b) => b.CategoryId.Equals(c.Id))))
				{
					throw new HimallException("删除失败，因为有店铺在经营该分类下的子分类");
				}
				if (secondAndThirdLevelCategories.Any((CategoryInfo c) => context.ProductInfo.Any((ProductInfo p) => p.CategoryId == c.Id)))
				{
					throw new HimallException("删除失败，因为有商品与该分类或子级分类关联");
				}
			}
			else
			{
				if (context.BusinessCategoryInfo.Any((BusinessCategoryInfo b) => b.CategoryId.Equals(id)))
				{
					throw new HimallException("删除失败，因为有店铺在经营该分类");
				}
				if (context.ProductInfo.Any((ProductInfo p) => p.CategoryId == id))
				{
					throw new HimallException("删除失败，因为有商品与该分类关联");
				}
			}
            ProcessingDeleteCategory(id);
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		private IEnumerable<CategoryInfo> GetCategories()
		{
			IEnumerable<CategoryInfo> array = null;
			if (Cache.Get("Cache-Categories") == null)
			{
				array = context.CategoryInfo.FindAll<CategoryInfo>().ToArray();
				Cache.Insert("Cache-Categories", array);
			}
			else
			{
				array = (IEnumerable<CategoryInfo>)Cache.Get("Cache-Categories");
			}
			return array;
		}

		public CategoryInfo GetCategory(long id)
		{
			if (id <= 0)
			{
				return null;
			}
			CategoryInfo categoryInfo = (
				from t in GetCategories()
				where t.Id == id
				select t).FirstOrDefault();
			return categoryInfo;
		}

		public IEnumerable<CategoryInfo> GetCategoryByParentId(long id)
		{
			if (id < 0)
			{
				throw new ArgumentNullException("id", string.Format("获取子级分类时，id={0}", id));
			}
			if (id == 0)
			{
				return 
					from c in GetCategories()
					where c.ParentCategoryId == 0
                    select c;
			}
			IEnumerable<CategoryInfo> categories = 
				from c in GetCategories()
				where c.ParentCategoryId == id
				select c;
			if (categories == null)
			{
				return null;
			}
			return (
				from c in categories
				orderby c.DisplaySequence descending
				select c).ToList();
		}

		public string GetEffectCategoryName(long shopId, long typeId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IQueryable<CategoryInfo> categoryInfos = context.CategoryInfo.FindBy((CategoryInfo c) => c.TypeId == typeId);
			IQueryable<BusinessCategoryInfo> businessCategoryInfos = context.BusinessCategoryInfo.FindBy((BusinessCategoryInfo b) => b.ShopId == shopId);
			foreach (CategoryInfo list in categoryInfos.ToList())
			{
				if (!businessCategoryInfos.Any((BusinessCategoryInfo b) => b.CategoryId == list.Id))
				{
					continue;
				}
				stringBuilder.Append(list.Name);
				stringBuilder.Append(',');
			}
			return stringBuilder.ToString().TrimEnd(new char[] { ',' });
		}

		public IEnumerable<CategoryInfo> GetFirstAndSecondLevelCategories()
		{
			return GetCategories().Where((CategoryInfo c) => {
				if (GetCategories().Any((CategoryInfo cc) => cc.ParentCategoryId == c.Id))
				{
					return true;
				}
				return c.Depth < 3;
			}).ToList();
		}

		public IEnumerable<CategoryInfo> GetMainCategory()
		{
			return 
				from t in GetCategories()
				where t.ParentCategoryId == 0
                select t;
		}

		public long GetMaxCategoryId()
		{
			if (GetCategories().Count() == 0)
			{
				return 0;
			}
			return GetCategories().Max<CategoryInfo>((CategoryInfo c) => c.Id);
		}

		public IEnumerable<CategoryInfo> GetSecondAndThirdLevelCategories(params long[] ids)
		{
			IEnumerable<CategoryInfo> categories = 
				from item in GetCategories()
				where ids.Contains(item.ParentCategoryId)
				select item;
			List<CategoryInfo> categoryInfos = new List<CategoryInfo>(categories);
			foreach (long list in (
				from item in categories
				select item.Id).ToList())
			{
				IEnumerable<CategoryInfo> categories1 = 
					from item in GetCategories()
					where item.ParentCategoryId == list
					select item;
				categoryInfos.AddRange(categories1);
			}
			return categoryInfos;
		}

		public IEnumerable<CategoryInfo> GetTopLevelCategories(IEnumerable<long> categoryIds)
		{
			IEnumerable<CategoryInfo> categories = 
				from item in GetCategories()
				where categoryIds.Contains(item.Id)
				select item;
			List<long> nums = new List<long>();
			foreach (CategoryInfo list in categories.ToList())
			{
				if (list.Depth != 1)
				{
					string path = list.Path;
					char[] chrArray = new char[] { '|' };
					long num = long.Parse(path.Split(chrArray)[0]);
					nums.Add(num);
				}
				else
				{
					nums.Add(list.Id);
				}
			}
			return 
				from item in GetCategories()
				where nums.Contains(item.Id)
				select item;
		}

		public IEnumerable<CategoryInfo> GetValidBusinessCategoryByParentId(long id)
		{
			CategoryInfo[] array = GetCategories().ToArray();
			CategoryInfo[] categoryInfoArray = (
				from item in array
				where item.ParentCategoryId == id
				select item).ToArray();
			if (id != 0)
			{
				CategoryInfo categoryInfo = ((IEnumerable<CategoryInfo>)array).FirstOrDefault((CategoryInfo item) => item.Id == id);
				if (categoryInfo != null && categoryInfo.Depth == 1)
				{
					IEnumerable<long> length = 
						from item in array
                        where item.Path.Split(new char[] { '|' }).Length == 3
                        select long.Parse(item.Path.Split(new char[] { '|' })[1]);
					categoryInfoArray = (
						from item in categoryInfoArray
						where length.Contains(item.Id)
						select item).ToArray();
				}
			}
			else
			{
				IEnumerable<long> nums = 
					from item in array
                    where item.Path.Split(new char[] { '|' }).Length == 3
                    select long.Parse(item.Path.Split(new char[] { '|' })[0]);
				categoryInfoArray = (
					from item in categoryInfoArray
					where nums.Contains(item.Id)
					select item).ToArray();
			}
			return categoryInfoArray;
		}

		IQueryable<CategoryInfo> Himall.IServices.ICategoryService.GetCategories()
		{
			return context.CategoryInfo.FindAll<CategoryInfo>();
		}

		private void ProcessingDeleteCategory(long id)
		{
			IQueryable<long> nums = 
				from c in context.CategoryInfo.FindBy((CategoryInfo c) => c.ParentCategoryId == id)
				select c.Id;
			if (nums.Count() == 0)
			{
                context.CategoryInfo.Remove(context.CategoryInfo.FindById<CategoryInfo>(id));
				return;
			}
			foreach (long list in nums.ToList())
			{
                ProcessingDeleteCategory(list);
			}
			Instance<ICashDepositsService>.Create.DeleteCategoryCashDeposits(id);
            context.CategoryInfo.Remove(context.CategoryInfo.FindById<CategoryInfo>(id));
		}

		public void UpdateCategory(CategoryInfo model)
		{
			CategoryInfo icon = context.CategoryInfo.FindById<CategoryInfo>(model.Id);
			icon.Icon = model.Icon;
			icon.Meta_Description = model.Meta_Description;
			icon.Meta_Keywords = model.Meta_Keywords;
			icon.Meta_Title = model.Meta_Title;
			icon.Name = model.Name;
			icon.RewriteName = model.RewriteName;
			icon.TypeId = model.TypeId;
			icon.CommisRate = model.CommisRate;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void UpdateCategoryDisplaySequence(long id, long displaySequence)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("更新一个分类的显示顺序时，id={0}", id));
			}
			if (0 >= displaySequence)
			{
				throw new ArgumentNullException("displaySequence", "更新一个分类的显示顺序时，displaySequence小于等于零");
			}
			CategoryInfo categoryInfo = context.CategoryInfo.FindById<CategoryInfo>(id);
			if (categoryInfo == null || categoryInfo.Id != id)
			{
				throw new Exception(string.Format("更新一个分类的显示顺序时，找不到id={0} 的分类", id));
			}
			categoryInfo.DisplaySequence = displaySequence;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void UpdateCategoryName(long id, string name)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("更新一个分类的名称时，id={0}", id));
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name", "更新一个分类的名称时，name为空");
			}
			CategoryInfo categoryInfo = context.CategoryInfo.FindById<CategoryInfo>(id);
			if (categoryInfo == null || categoryInfo.Id != id)
			{
				throw new Exception(string.Format("更新一个分类的名称时，找不到id={0} 的分类", id));
			}
			categoryInfo.Name = name;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}
	}
}