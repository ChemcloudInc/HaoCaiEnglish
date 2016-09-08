using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
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
	public class ShopConcernController : BaseMemberController
	{
		public ShopConcernController()
		{
		}

		public JsonResult CancelConcernShops(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IShopService>().CancelConcernShops(nums, base.CurrentUser.Id);
			Result result = new Result()
			{
				success = true,
				msg = "Cancel Success！"
			};
			return Json(result);
		}

		[ChildActionOnly]
		public ActionResult CustmerServices(long shopId)
		{
			IOrderedQueryable<CustomerServiceInfo> customerService = 
				from m in ServiceHelper.Create<ICustomerService>().GetCustomerService(shopId)
				orderby m.Tool
				select m;
			return base.PartialView(customerService);
		}

		public ActionResult Index(int pageSize = 10, int pageNo = 1)
		{
			PageModel<FavoriteShopInfo> userConcernShops = ServiceHelper.Create<IShopService>().GetUserConcernShops(base.CurrentUser.Id, pageNo, pageSize);
			List<ShopConcernModel> shopConcernModels = new List<ShopConcernModel>();
			foreach (FavoriteShopInfo list in userConcernShops.Models.ToList())
			{
				if (list.Himall_Shops == null)
				{
					continue;
				}
				ShopConcernModel shopConcernModel = new ShopConcernModel();
				shopConcernModel.FavoriteShopInfo.Id = list.Id;
				shopConcernModel.FavoriteShopInfo.Logo = list.Himall_Shops.Logo;
				shopConcernModel.FavoriteShopInfo.ConcernTime = list.Date;
				shopConcernModel.FavoriteShopInfo.ShopId = list.ShopId;
				shopConcernModel.FavoriteShopInfo.ShopName = list.Himall_Shops.ShopName;
				shopConcernModel.FavoriteShopInfo.ConcernCount = list.Himall_Shops.Himall_FavoriteShops.Count();
				IQueryable<ProductInfo> hotSaleProduct = ServiceHelper.Create<IProductService>().GetHotSaleProduct(list.ShopId, 10);
				if (hotSaleProduct != null)
				{
					foreach (ProductInfo productInfo in hotSaleProduct)
					{
						List<HotProductInfo> hotSaleProducts = shopConcernModel.HotSaleProducts;
						HotProductInfo hotProductInfo = new HotProductInfo()
						{
							ImgPath = productInfo.ImagePath,
							Name = productInfo.ProductName,
							Price = productInfo.MinSalePrice,
							Id = productInfo.Id,
							SaleCount = (int)productInfo.SaleCounts
						};
						hotSaleProducts.Add(hotProductInfo);
					}
				}
				IQueryable<ProductInfo> newSaleProduct = ServiceHelper.Create<IProductService>().GetNewSaleProduct(list.ShopId, 10);
				if (newSaleProduct != null)
				{
					foreach (ProductInfo productInfo1 in newSaleProduct)
					{
						List<HotProductInfo> newSaleProducts = shopConcernModel.NewSaleProducts;
						HotProductInfo hotProductInfo1 = new HotProductInfo()
						{
							ImgPath = productInfo1.ImagePath,
							Name = productInfo1.ProductName,
							Price = productInfo1.MinSalePrice,
							Id = productInfo1.Id,
							SaleCount = productInfo1.ConcernedCount
						};
						newSaleProducts.Add(hotProductInfo1);
					}
				}
				shopConcernModels.Add(shopConcernModel);
			}
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = userConcernShops.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(shopConcernModels);
		}
	}
}