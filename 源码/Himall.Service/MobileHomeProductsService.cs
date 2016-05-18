using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class MobileHomeProductsService : ServiceBase, IMobileHomeProductsService, IService, IDisposable
	{
		public MobileHomeProductsService()
		{
		}

		public void AddProductsToHomePage(long shopId, PlatformType platformType, IEnumerable<long> productIds)
		{
			IProductService create = Instance<IProductService>.Create;
			IQueryable<long> mobileHomeProductsInfo = 
				from item in context.MobileHomeProductsInfo
				where item.ShopId == shopId && (int)item.PlatFormType == (int)platformType
				select item.ProductId;
			IEnumerable<long> nums = 
				from item in productIds
				where !mobileHomeProductsInfo.Contains(item)
				select item;
			foreach (long num in nums)
			{
				ProductInfo product = create.GetProduct(num);
				if (shopId != 0 && product.ShopId != shopId)
				{
					throw new HimallException("待添加至首页的商品不得包含非本店铺商品");
				}
				MobileHomeProductsInfo mobileHomeProductsInfo1 = new MobileHomeProductsInfo()
				{
					PlatFormType = platformType,
					Sequence = 1,
					ProductId = num,
					ShopId = shopId
				};
                context.MobileHomeProductsInfo.Add(mobileHomeProductsInfo1);
			}
			IQueryable<long> nums1 = 
				from e in mobileHomeProductsInfo
				where !productIds.Contains(e)
				select e;
			if (nums1.Count() > 0)
			{
				IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = 
					from item in context.MobileHomeProductsInfo
					where item.ShopId == shopId && (int)item.PlatFormType == (int)platformType && nums1.Contains(item.ProductId)
					select item;
                context.MobileHomeProductsInfo.RemoveRange(mobileHomeProductsInfos);
			}
            context.SaveChanges();
		}

		public void Delete(long shopId, long id)
		{
            context.MobileHomeProductsInfo.Remove(new object[] { id });
            context.SaveChanges();
		}

        public PageModel<MobileHomeProductsInfo> GetMobileHomePageProducts(long shopId, PlatformType platformType, ProductQuery productQuery)
        {
            IQueryable<MobileHomeProductsInfo> entities = base.context.MobileHomeProductsInfo.FindBy(item => (item.ShopId == shopId) && (((int)item.PlatFormType) == ((int)platformType)));
            if (!string.IsNullOrWhiteSpace(productQuery.KeyWords))
            {
                productQuery.KeyWords = productQuery.KeyWords.Trim();
                long[] brandIds = (from item in base.context.BrandInfo.FindBy(item => item.Name.Contains(productQuery.KeyWords)) select item.Id).ToArray();
                entities = entities.FindBy(item => item.Himall_Products.ProductName.Contains(productQuery.KeyWords) || brandIds.Contains(item.Himall_Products.BrandId));
            }
            if (productQuery.CategoryId.HasValue)
            {
                entities = entities.FindBy(item => ("|" + item.Himall_Products.CategoryPath + "|").Contains("|" + productQuery.CategoryId.Value + "|"));
            }
            int num = entities.Count();
            entities = (from item in entities
                        orderby item.Sequence, item.Id
                        select item).Skip(((productQuery.PageNo - 1) * productQuery.PageSize)).Take(productQuery.PageSize);
            return new PageModel<MobileHomeProductsInfo> { Models = entities, Total = num };
        }


        public IQueryable<MobileHomeProductsInfo> GetMobileHomePageProducts(long shopId, PlatformType platformType)
		{
			return 
				from mp in context.MobileHomeProductsInfo
				join p in context.ProductInfo on mp.ProductId equals p.Id
				where mp.ShopId == shopId && (int)mp.PlatFormType == (int)platformType && (int)p.SaleStatus == 1 && (int)p.AuditStatus == 2
				select mp;
		}

		public PageModel<MobileHomeProductsInfo> GetSellerMobileHomePageProducts(long shopId, PlatformType platformType, ProductQuery productQuery)
		{
			IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = context.MobileHomeProductsInfo.FindBy((MobileHomeProductsInfo item) => item.ShopId == shopId && (int)item.PlatFormType == (int)platformType);
			if (!string.IsNullOrWhiteSpace(productQuery.KeyWords))
			{
				productQuery.KeyWords = productQuery.KeyWords.Trim();
				long[] array = (
					from item in context.BrandInfo.FindBy((BrandInfo item) => item.Name.Contains(productQuery.BrandNameKeyword))
					select item.Id).ToArray();
				mobileHomeProductsInfos = mobileHomeProductsInfos.FindBy((MobileHomeProductsInfo item) => item.Himall_Products.ProductName.Contains(productQuery.KeyWords) || array.Contains(item.Himall_Products.BrandId));
			}
			IEnumerable<long> productShopCategoryInfo = new long[0];
			if (productQuery.ShopCategoryId.HasValue)
			{
				productShopCategoryInfo = 
					from item in context.ProductShopCategoryInfo
					where item.ShopCategoryInfo.Id == productQuery.ShopCategoryId || item.ShopCategoryInfo.ParentCategoryId == productQuery.ShopCategoryId
                    select item.ProductId;
				mobileHomeProductsInfos = mobileHomeProductsInfos.FindBy((MobileHomeProductsInfo item) => productShopCategoryInfo.Contains(item.ProductId));
			}
			int num = mobileHomeProductsInfos.Count();
			mobileHomeProductsInfos = (
				from item in mobileHomeProductsInfos
				orderby item.Sequence
				select item).Skip((productQuery.PageNo - 1) * productQuery.PageSize).Take(productQuery.PageSize);
			return new PageModel<MobileHomeProductsInfo>()
			{
				Models = mobileHomeProductsInfos,
				Total = num
			};
		}

		public void UpdateSequence(long shopId, long id, short sequence)
		{
			MobileHomeProductsInfo mobileHomeProductsInfo = context.MobileHomeProductsInfo.FirstOrDefault((MobileHomeProductsInfo item) => item.Id == id && item.ShopId == shopId);
			if (mobileHomeProductsInfo == null)
			{
				throw new HimallException(string.Format("不存在Id为{0}的首页商品设置", id));
			}
			mobileHomeProductsInfo.Sequence = sequence;
            context.SaveChanges();
		}
	}
}