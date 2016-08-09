using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ShopSearchController : BaseWebController
	{
		public ShopSearchController()
		{
		}

		public ActionResult Index(string keywords = "", long categoryId = 0L, long brandId = 0L, int orderBy = 0, int pageNo = 1, int pageSize = 40)
		{
			ShopQuery shopQuery = new ShopQuery()
			{
				ShopName = keywords,
				CategoryId = categoryId,
				BrandId = brandId,
				PageNo = pageNo,
				PageSize = pageSize,
				OrderBy = orderBy
			};
			ShopQuery shopQuery1 = shopQuery;
			IShopService shopService = ServiceHelper.Create<IShopService>();
			PageModel<ShopInfo> shops = shopService.GetShops(shopQuery1);
			List<ShopInfo> list = shops.Models.ToList();
			string str = "5";
			Dictionary<long, string> nums = new Dictionary<long, string>();
			Dictionary<long, string> nums1 = new Dictionary<long, string>();
			foreach (ShopInfo sales in list)
			{
				sales.Sales = shopService.GetSales(sales.Id);
				IQueryable<StatisticOrderCommentsInfo> shopStatisticOrderComments = ServiceHelper.Create<IShopService>().GetShopStatisticOrderComments(sales.Id);
				StatisticOrderCommentsInfo statisticOrderCommentsInfo = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 1
					select c).FirstOrDefault();
				StatisticOrderCommentsInfo statisticOrderCommentsInfo1 = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 9
					select c).FirstOrDefault();
				StatisticOrderCommentsInfo statisticOrderCommentsInfo2 = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 9
					select c).FirstOrDefault();
				sales.ProductAndDescription = (statisticOrderCommentsInfo != null ? string.Format("{0:F}", statisticOrderCommentsInfo.CommentValue) : str);
				sales.SellerServiceAttitude = (statisticOrderCommentsInfo1 != null ? string.Format("{0:F}", statisticOrderCommentsInfo1.CommentValue) : str);
				sales.SellerDeliverySpeed = (statisticOrderCommentsInfo2 != null ? string.Format("{0:F}", statisticOrderCommentsInfo2.CommentValue) : str);
				foreach (ShopBrandsInfo himallShopBrand in sales.Himall_ShopBrands)
				{
					if (nums.ContainsKey(himallShopBrand.BrandId))
					{
						continue;
					}
					nums.Add(himallShopBrand.BrandId, himallShopBrand.Himall_Brands.Name);
				}
				foreach (ProductInfo himallProduct in sales.Himall_Products)
				{
					if (nums1.ContainsKey(himallProduct.CategoryId))
					{
						continue;
					}
					nums1.Add(himallProduct.CategoryId, himallProduct.Himall_Categories.Name);
				}
			}
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = shops.Total
			};
			ViewBag.pageInfo = pagingInfo;
			ViewBag.QueryModel = shopQuery1;
			ViewBag.Brands = nums;
			ViewBag.Categorys = nums1;
			return View(list);
		}
	}
}