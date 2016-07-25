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
	public class ShopHomeModuleService : ServiceBase, IShopHomeModuleService, IService, IDisposable
	{
		public ShopHomeModuleService()
		{
		}

		public void AddShopProductModule(ShopHomeModuleInfo shopProductModuleInfo)
		{
			if (string.IsNullOrWhiteSpace(shopProductModuleInfo.Name))
			{
				throw new InvalidPropertyException("商品模块名称不能为空");
			}
            context.ShopHomeModuleInfo.Add(shopProductModuleInfo);
            context.SaveChanges();
		}

		public void Delete(long shopId, long id)
		{
            context.ShopHomeModuleInfo.OrderBy((ShopHomeModuleInfo item) => item.Id == id && item.ShopId == shopId);
            context.SaveChanges();
		}

		public IQueryable<ShopHomeModuleInfo> GetAllShopHomeModuleInfos(long shopId)
		{
			return context.ShopHomeModuleInfo.FindBy((ShopHomeModuleInfo item) => item.ShopId == shopId);
		}

		public ShopHomeModuleInfo GetShopHomeModuleInfo(long shopId, long id)
		{
			return context.ShopHomeModuleInfo.FirstOrDefault((ShopHomeModuleInfo item) => item.ShopId == shopId && item.Id == id);
		}

		public void UpdateShopProductModuleName(long shopId, long id, string name)
		{
			ShopHomeModuleInfo shopHomeModuleInfo = context.ShopHomeModuleInfo.FirstOrDefault((ShopHomeModuleInfo item) => item.Id == id && item.ShopId == shopId);
			if (shopHomeModuleInfo == null)
			{
				throw new HimallException("在本店铺中未找到指定商品模块");
			}
			shopHomeModuleInfo.Name = name;
            context.SaveChanges();
		}

		public void UpdateShopProductModuleProducts(long shopId, long id, IEnumerable<long> productIds)
		{
			ShopHomeModuleInfo shopHomeModuleInfo = context.ShopHomeModuleInfo.FirstOrDefault((ShopHomeModuleInfo item) => item.Id == id && item.ShopId == shopId);
			if (shopHomeModuleInfo == null)
			{
				throw new HimallException("在本店铺中未找到指定商品模块");
			}
			ShopHomeModuleProductInfo[] array = shopHomeModuleInfo.ShopHomeModuleProductInfo.ToArray();
			IEnumerable<long> nums = 
				from item in array
				where !productIds.Contains(item.ProductId)
				select item.Id;
			IEnumerable<long> productId = 
				from item in array
                select item.ProductId;
			IEnumerable<long> nums1 = 
				from item in productIds
				where !productId.Contains(item)
				select item;
            context.ShopHomeModuleProductInfo.OrderBy((ShopHomeModuleProductInfo item) => nums.Contains(item.Id));
			foreach (long list in nums1.ToList())
			{
				ShopHomeModuleProductInfo shopHomeModuleProductInfo = new ShopHomeModuleProductInfo()
				{
					ProductId = list,
					HomeModuleId = shopHomeModuleInfo.Id
				};
                context.ShopHomeModuleProductInfo.Add(shopHomeModuleProductInfo);
			}
            context.SaveChanges();
		}
	}
}