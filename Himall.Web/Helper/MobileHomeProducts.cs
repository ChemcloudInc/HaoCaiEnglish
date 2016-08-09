using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Web
{
	public class MobileHomeProducts
	{
		public MobileHomeProducts()
		{
		}

		public void AddHomeProducts(long shopId, string productIds, PlatformType platformType)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in productIds.Split(chrArray)
				select long.Parse(item);
			ServiceHelper.Create<IMobileHomeProductsService>().AddProductsToHomePage(shopId, platformType, nums);
		}

		public void Delete(long shopId, long id)
		{
			ServiceHelper.Create<IMobileHomeProductsService>().Delete(shopId, id);
		}

		public object GetAllHomeProductIds(long shopId, PlatformType platformType)
		{
			return 
				from item in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(shopId, platformType)
				select item.ProductId;
		}

		public object GetMobileHomeProducts(long shopId, PlatformType platformType, int page, int rows, string keyWords, long? categoryId = null)
		{
			ProductQuery productQuery = new ProductQuery()
			{
				CategoryId = categoryId,
				KeyWords = keyWords,
				PageSize = rows,
				PageNo = page
			};
			PageModel<MobileHomeProductsInfo> mobileHomePageProducts = ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(shopId, platformType, productQuery);
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			var collection = mobileHomePageProducts.Models.ToArray().Select((MobileHomeProductsInfo item) => {
				BrandInfo brand = brandService.GetBrand(item.Himall_Products.BrandId);
				return new { name = item.Himall_Products.ProductName, image = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_50, 1), price = item.Himall_Products.MinSalePrice.ToString("F2"), brand = (brand == null ? "" : brand.Name), sequence = item.Sequence, categoryName = categoryService.GetCategory(long.Parse(categoryService.GetCategory(item.Himall_Products.CategoryId).Path.Split(new char[] { '|' }).Last<string>())).Name, id = item.Id, productId = item.ProductId };
			});
			return new { rows = collection, total = mobileHomePageProducts.Total };
		}

		public object GetSellerMobileHomePageProducts(long shopId, PlatformType platformType, int page, int rows, string brandName, long? categoryId = null)
		{
			ProductQuery productQuery = new ProductQuery()
			{
				ShopCategoryId = categoryId,
				KeyWords = brandName,
				PageSize = rows,
				PageNo = page
			};
			PageModel<MobileHomeProductsInfo> sellerMobileHomePageProducts = ServiceHelper.Create<IMobileHomeProductsService>().GetSellerMobileHomePageProducts(shopId, platformType, productQuery);
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			ServiceHelper.Create<IShopCategoryService>();
			var collection = sellerMobileHomePageProducts.Models.ToArray().Select((MobileHomeProductsInfo item) => {
				BrandInfo brand = brandService.GetBrand(item.Himall_Products.BrandId);
				ProductShopCategoryInfo productShopCategoryInfo = item.Himall_Products.Himall_ProductShopCategories.FirstOrDefault();
				return new { name = item.Himall_Products.ProductName, image = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_50, 1), price = item.Himall_Products.MinSalePrice.ToString("F2"), brand = (brand == null ? "" : brand.Name), sequence = item.Sequence, id = item.Id, categoryName = (productShopCategoryInfo == null ? "" : productShopCategoryInfo.ShopCategoryInfo.Name), productId = item.ProductId };
			});
			return new { rows = collection, total = sellerMobileHomePageProducts.Total };
		}

		public void UpdateSequence(long shopId, long id, short sequence)
		{
			ServiceHelper.Create<IMobileHomeProductsService>().UpdateSequence(shopId, id, sequence);
		}
	}
}