using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ShopCategoryService : ServiceBase, IShopCategoryService, IService, IDisposable
	{
		public ShopCategoryService()
		{
		}

		public void AddCategory(ShopCategoryInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model", "添加一个商品分类时，Model为空");
			}
            context.ShopCategoryInfo.Add(model);
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void DeleteCategory(long id, long shopId)
		{
            ProcessingDeleteCategory(id, shopId);
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public IQueryable<CategoryInfo> GetBusinessCategory(long shopId)
		{
			if (context.ShopInfo.FindById<ShopInfo>(shopId).IsSelf)
			{
				return context.CategoryInfo.FindAll<CategoryInfo>();
			}
			IQueryable<BusinessCategoryInfo> businessCategoryInfo = 
				from bsc in context.BusinessCategoryInfo
				where bsc.ShopId == shopId
				select bsc;
			IQueryable<CategoryInfo> categoryInfo = 
				from c in context.CategoryInfo
				where businessCategoryInfo.Any((BusinessCategoryInfo cc) => cc.CategoryId == c.Id)
				select c;
			List<long> nums = new List<long>();
			foreach (CategoryInfo list in categoryInfo.ToList())
			{
				string[] strArrays = list.Path.Split(new char[] { '|' });
				for (int i = 0; i < strArrays.Length; i++)
				{
					long num = long.Parse(strArrays[i]);
					if (!nums.Contains(num))
					{
						nums.Add(num);
					}
				}
			}
			return 
				from item in context.CategoryInfo
				where nums.Contains(item.Id)
				select item;
		}

		private IEnumerable<ShopCategoryInfo> GetCategories()
		{
			IEnumerable<ShopCategoryInfo> array = context.ShopCategoryInfo.FindAll<ShopCategoryInfo>().ToArray();
			return array;
		}

		public ShopCategoryInfo GetCategory(long id)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("获取一个商品分类时，id={0}", id));
			}
			ShopCategoryInfo shopCategoryInfo = (
				from t in GetCategories()
				where t.Id == id
				select t).FirstOrDefault();
			return shopCategoryInfo;
		}

		public IEnumerable<ShopCategoryInfo> GetCategoryByParentId(long id)
		{
			if (id < 0)
			{
				throw new ArgumentNullException("id", string.Format("获取子级商品分类时，id={0}", id));
			}
			if (id == 0)
			{
				return 
					from c in GetCategories()
					where c.ParentCategoryId == 0
                    select c;
			}
			IEnumerable<ShopCategoryInfo> categories = 
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

		public IEnumerable<ShopCategoryInfo> GetCategoryByParentId(long id, long shopId)
		{
			if (id < 0)
			{
				throw new HimallException(string.Format("获取子级分类时，id={0}", id));
			}
			return GetCategories().Where((ShopCategoryInfo c) => {
				if (c.ShopId != shopId)
				{
					return false;
				}
				return c.ParentCategoryId == id;
			}).OrderByDescending<ShopCategoryInfo, long>((ShopCategoryInfo t) => t.DisplaySequence);
		}

		public ShopCategoryInfo GetCategoryByProductId(long id)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("获取一个商品分类时，id={0}", id));
			}
			ProductShopCategoryInfo productShopCategoryInfo = (
				from p in context.ProductShopCategoryInfo
				where p.ProductId == id
				select p).FirstOrDefault();
			if (productShopCategoryInfo != null)
			{
				return productShopCategoryInfo.ShopCategoryInfo;
			}
			return new ShopCategoryInfo()
			{
				Name = ""
			};
		}

		public IEnumerable<ShopCategoryInfo> GetMainCategory(long shopId)
		{
			return GetCategories().Where((ShopCategoryInfo t) => {
				if (t.ParentCategoryId != 0)
				{
					return false;
				}
				return t.ShopId == shopId;
			});
		}

		public long GetMaxCategoryId()
		{
			if (GetCategories().Count() == 0)
			{
				return 0;
			}
			return GetCategories().Max<ShopCategoryInfo>((ShopCategoryInfo c) => c.Id);
		}

		public IQueryable<ShopCategoryInfo> GetShopCategory(long shopId)
		{
			return context.ShopCategoryInfo.FindBy((ShopCategoryInfo s) => s.ShopId == shopId);
		}

		private void ProcessingDeleteCategory(long id, long shopId)
		{
			IQueryable<long> nums = 
				from c in context.ShopCategoryInfo.FindBy((ShopCategoryInfo c) => c.ParentCategoryId == id && c.ShopId == shopId)
				select c.Id;
			if (nums.Count() == 0)
			{
                context.ShopCategoryInfo.Remove(context.ShopCategoryInfo.FindById<ShopCategoryInfo>(id));
				return;
			}
			foreach (long list in nums.ToList())
			{
                ProcessingDeleteCategory(list, shopId);
			}
            context.ShopCategoryInfo.Remove(context.ShopCategoryInfo.FindById<ShopCategoryInfo>(id));
		}

		public void UpdateCategory(ShopCategoryInfo model)
		{
			ShopCategoryInfo name = context.ShopCategoryInfo.FindById<ShopCategoryInfo>(model.Id);
			name.Name = model.Name;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void UpdateCategoryDisplaySequence(long id, long displaySequence)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("更新一个商品分类的显示顺序时，id={0}", id));
			}
			if (0 >= displaySequence)
			{
				throw new ArgumentNullException("displaySequence", "更新一个商品分类的显示顺序时，displaySequence小于等于零");
			}
			ShopCategoryInfo shopCategoryInfo = context.ShopCategoryInfo.FindById<ShopCategoryInfo>(id);
			if (shopCategoryInfo == null || shopCategoryInfo.Id != id)
			{
				throw new Exception(string.Format("更新一个商品分类的显示顺序时，找不到id={0} 的商品分类", id));
			}
			shopCategoryInfo.DisplaySequence = displaySequence;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}

		public void UpdateCategoryName(long id, string name)
		{
			if (id <= 0)
			{
				throw new ArgumentNullException("id", string.Format("更新一个商品分类的名称时，id={0}", id));
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name", "更新一个商品分类的名称时，name为空");
			}
			ShopCategoryInfo shopCategoryInfo = context.ShopCategoryInfo.FindById<ShopCategoryInfo>(id);
			if (shopCategoryInfo == null || shopCategoryInfo.Id != id)
			{
				throw new Exception(string.Format("更新一个商品分类的名称时，找不到id={0} 的商品分类", id));
			}
			shopCategoryInfo.Name = name;
            context.SaveChanges();
			Cache.Remove("Cache-Categories");
		}
	}
}