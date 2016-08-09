using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Helper;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ProductController : BaseWebController
	{
		public ProductController()
		{
		}

		[HttpPost]
		public JsonResult AddFavorite(long shopId)
		{
			JsonResult jsonResult;
			try
			{
				ServiceHelper.Create<IShopService>().AddFavoriteShop(base.CurrentUser.Id, shopId);
				return Json(new { success = true });
			}
			catch (HimallException himallException1)
			{
				HimallException himallException = himallException1;
				jsonResult = Json(new { success = false, msg = himallException.Message });
			}
			return jsonResult;
		}

		[HttpPost]
		public JsonResult AddFavoriteProduct(long pid)
		{
			int num = 0;
			ServiceHelper.Create<IProductService>().AddFavorite(pid, base.CurrentUser.Id, out num);
			if (num == 1)
			{
				return Json(new { successful = true, favorited = true, mess = "您已经关注过该商品了." }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { successful = true, favorited = false, mess = "成功关注该商品." }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Browse(long? shopId, long? categoryId, int? auditStatus, string ids, int page, int rows, string keyWords, int? saleStatus, bool? isShopCategory, bool isLimitTimeBuy = false)
		{
			IEnumerable<long> nums;
			ProductQuery productQuery = new ProductQuery()
			{
				PageSize = rows,
				PageNo = page,
				KeyWords = keyWords,
				CategoryId = categoryId
			};
			ProductQuery productQuery1 = productQuery;
			if (string.IsNullOrWhiteSpace(ids))
			{
				nums = null;
			}
			else
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
			}
			productQuery1.Ids = nums;
			productQuery.ShopId = shopId;
			productQuery.IsLimitTimeBuy = isLimitTimeBuy;
			ProductQuery value = productQuery;
			if (auditStatus.HasValue)
			{
				value.AuditStatus = new ProductInfo.ProductAuditStatus[] { (ProductInfo.ProductAuditStatus)auditStatus.Value };
			}
			if (saleStatus.HasValue)
			{
				value.SaleStatus = new ProductInfo.ProductSaleStatus?((ProductInfo.ProductSaleStatus)saleStatus.Value);
			}
			PageModel<ProductInfo> products = ServiceHelper.Create<IProductService>().GetProducts(value);
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			var array = 
				from item in products.Models.ToArray()
				select new { name = item.ProductName, brandName = (item.BrandId == 0 ? "" : brandService.GetBrand(item.BrandId).Name), categoryName = categoryService.GetCategory(item.CategoryId).Name, id = item.Id, imgUrl = item.GetImage(ProductInfo.ImageSize.Size_50, 1), price = item.MinSalePrice };
			return Json(new { rows = array, total = products.Total });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult CalceFreight(int cityId, long pId, int count)
		{
			List<long> nums = new List<long>();
			List<int> nums1 = new List<int>();
			nums.Add(pId);
			nums1.Add(count);
			decimal freight = Instance<IProductService>.Create.GetFreight(nums, nums1, cityId);
			string str = string.Concat("运费 ￥", freight);
			Result result = new Result()
			{
				success = true,
				msg = str
			};
			return Json(result);
		}

		[ChildActionOnly]
		public ActionResult CustmerServices(long shopId)
		{
			IOrderedQueryable<CustomerServiceInfo> customerService = 
				from c in ServiceHelper.Create<ICustomerService>().GetCustomerService(shopId)
				where (int)c.Type == 1
				select c into m
				orderby m.Tool
				select m;
			return base.PartialView(customerService);
		}

        public ActionResult Detail(string id = "")
        {
            BrandInfo info19;
            string str = "";
            ProductDetailModelForWeb web = new ProductDetailModelForWeb
            {
                HotAttentionProducts = new List<HotProductInfo>(),
                HotSaleProducts = new List<HotProductInfo>(),
                Product = new ProductInfo(),
                Shop = new ShopInfoModel(),
                ShopCategory = new List<CategoryJsonModel>(),
                Color = new CollectionSKU(),
                Size = new CollectionSKU(),
                Version = new CollectionSKU(),
                BoughtProducts = new List<HotProductInfo>()
            };
            ProductInfo product = null;
            new ShopInfoModel();
            long result = 0L;
            long.TryParse(id, out result);
            if (result == 0L)
            {
                return RedirectToAction("Error404", "Error", new { area = "Web" });
            }
            if (ServiceHelper.Create<ILimitTimeBuyService>().IsLimitTimeMarketItem(result))
            {
                long num2 = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(result).Id;
                return RedirectToAction("Detail", "LimitTimeBuy", new { area = "Web", id = num2 });
            }
            IProductService service = ServiceHelper.Create<IProductService>();
            IFreightTemplateService service2 = ServiceHelper.Create<IFreightTemplateService>();
            product = service.GetProduct(result);
            if (product == null)
            {
                return RedirectToAction("Error404", "Error", new { area = "Web" });
            }
            for (int i = 1; i < 6; i++)
            {
                if (System.IO.File.Exists(Server.MapPath(product.ImagePath + string.Format("/{0}.png", i))))
                {
                    if (!System.IO.File.Exists(Server.MapPath(product.ImagePath + string.Format("/{0}_{1}.png", i, base.CurrentSiteSetting.ProductDetailsPictureSize))))
                    {
                        string str8 = Server.MapPath(product.ImagePath + string.Format("/{0}_{1}.png", i, base.CurrentSiteSetting.ProductDetailsPictureSize));
                        ImageHelper.CreateThumbnail(Server.MapPath(product.ImagePath + string.Format("/{0}.png", i)), str8, int.Parse(base.CurrentSiteSetting.ProductDetailsPictureSize), int.Parse(base.CurrentSiteSetting.ProductDetailsPictureSize));
                    }
                }
            }
            IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(result);
            ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(product.ShopId, false);
            ShopServiceMarkModel shopComprehensiveMark = ShopServiceMark.GetShopComprehensiveMark(shop.Id);
            web.Shop.Name = shop.ShopName;
            web.Shop.CompanyName = shop.CompanyName;
            web.Shop.Id = shop.Id;
            web.Shop.PackMark = shopComprehensiveMark.PackMark;
            web.Shop.ServiceMark = shopComprehensiveMark.ServiceMark;
            web.Shop.ComprehensiveMark = shopComprehensiveMark.ComprehensiveMark;
            web.Shop.Phone = shop.CompanyPhone;
            web.Shop.FreeFreight = shop.FreeFreight;
            web.Shop.ProductMark = (commentsByProductId.Count() == 0) ? 5M : Convert.ToDecimal(commentsByProductId.Average<ProductCommentInfo>(c => c.ReviewMark));
            FreightTemplateInfo freightTemplate = service2.GetFreightTemplate(product.FreightTemplateId);
            decimal num3 = 0M;
            int cityId = 0;
            string str2 = "请选择";
            string str3 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            if (freightTemplate != null)
            {
                string[] strArray = ServiceHelper.Create<IRegionService>().GetRegionFullName(freightTemplate.SourceAddress.Value, " ").Split(new char[] { ' ' });
                if (strArray.Length >= 2)
                {
                    str5 = strArray[0] + " " + strArray[1];
                }
                else
                {
                    str5 = strArray[0];
                }
                ShippingAddressInfo defaultUserShippingAddressByUserId = null;
                if (base.CurrentUser != null)
                {
                    defaultUserShippingAddressByUserId = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
                }
                long regionId = 0L;
                if (defaultUserShippingAddressByUserId != null)
                {
                    regionId = defaultUserShippingAddressByUserId.RegionId;
                }
                else
                {
                    string iP = WebHelper.GetIP();
                    regionId = ServiceHelper.Create<IRegionService>().GetRegionByIPInTaobao(iP);
                }
                if (regionId > 0L)
                {
                    string[] strArray2 = ServiceHelper.Create<IRegionService>().GetRegionIdPath(regionId).Split(new char[] { ',' });
                    str3 = strArray2[0] + "," + strArray2[1];
                    string[] strArray3 = ServiceHelper.Create<IRegionService>().GetRegionFullName(regionId, " ").Split(new char[] { ' ' });
                    str2 = strArray3[0] + " " + strArray3[1];
                    cityId = int.Parse(strArray2[1]);
                }
                if (freightTemplate.IsFree == FreightTemplateInfo.FreightTemplateType.Free)
                {
                    str4 = "卖家承担运费";
                }
                else if ((base.CurrentUser != null) && (defaultUserShippingAddressByUserId != null))
                {
                //    List<long> productIds = new List<long>();
                    List<long> productIds = new List<long>() { product.Id};
                     
                    //productIds.Add(product.Id);
                    //List<int> counts = new List<int> { (int)result};
                    List<int> counts = new List<int> { 1};
                    num3 = Instance<IProductService>.Create.GetFreight(productIds, counts, cityId);
                    str4 = "运费 ￥" + num3;
                }
            }
            ViewBag.ProductAddress = str5;
            ViewBag.ShippingAddress = str2;
            ViewBag.ShippingValue = str3;
            ViewBag.Freight = str4;
            if ((product == null) || (product.Id == 0L))
            {
                return RedirectToAction("Error404", "Error", new { area = "Web" });
            }
            if ((product.SKUInfo != null) && (product.SKUInfo.Count() > 0))
            {
                long num6 = 0L;
                long num7 = 0L;
                long num8 = 0L;
                using (List<SKUInfo>.Enumerator enumerator = (from s in product.SKUInfo
                                                              orderby s.AutoId
                                                              select s).ToList().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<ProductSKU, bool> predicate = null;
                        Func<SKUInfo, bool> func2 = null;
                        Func<ProductSKU, bool> func3 = null;
                        Func<SKUInfo, bool> func4 = null;
                        Func<ProductSKU, bool> func5 = null;
                        Func<SKUInfo, bool> func6 = null;
                        SKUInfo sku = enumerator.Current;
                        string[] source = sku.Id.Split(new char[] { '_' });
                        if (source.Count() > 0)
                        {
                            long.TryParse(source[1], out num6);
                            if (num6 != 0L)
                            {
                                if (predicate == null)
                                {
                                    predicate = v => v.Value.Equals(sku.Color);
                                }
                                if (!web.Color.Any(predicate))
                                {
                                    if (func2 == null)
                                    {
                                        func2 = s => s.Color.Equals(sku.Color);
                                    }
                                    long num9 = product.SKUInfo.Where(func2).Sum<SKUInfo>(s => s.Stock);
                                    ProductSKU item = new ProductSKU
                                    {
                                        Name = "选择颜色",
                                        EnabledClass = (num9 != 0L) ? "enabled" : "disabled",
                                        SelectedClass = "",
                                        SKUId = num6,
                                        Value = sku.Color
                                    };
                                    web.Color.Add(item);
                                }
                            }
                        }
                        if (source.Count() > 1)
                        {
                            long.TryParse(source[2], out num7);
                            if (num7 != 0L)
                            {
                                if (func3 == null)
                                {
                                    func3 = v => v.Value.Equals(sku.Size);
                                }
                                if (!web.Size.Any(func3))
                                {
                                    if (func4 == null)
                                    {
                                        func4 = s => s.Size.Equals(sku.Size);
                                    }
                                    long num10 = product.SKUInfo.Where(func4).Sum<SKUInfo>(s1 => s1.Stock);
                                    ProductSKU tsku2 = new ProductSKU
                                    {
                                        Name = "选择尺码",
                                        EnabledClass = (num10 != 0L) ? "enabled" : "disabled",
                                        SelectedClass = "",
                                        SKUId = num7,
                                        Value = sku.Size
                                    };
                                    web.Size.Add(tsku2);
                                }
                            }
                        }
                        if (source.Count() > 2)
                        {
                            long.TryParse(source[3], out num8);
                            if (num8 != 0L)
                            {
                                if (func5 == null)
                                {
                                    func5 = v => v.Value.Equals(sku.Version);
                                }
                                if (!web.Version.Any(func5))
                                {
                                    if (func6 == null)
                                    {
                                        func6 = s => s.Version.Equals(sku.Version);
                                    }
                                    long num11 = product.SKUInfo.Where(func6).Sum<SKUInfo>(s => s.Stock);
                                    ProductSKU tsku3 = new ProductSKU
                                    {
                                        Name = "选择版本",
                                        EnabledClass = (num11 != 0L) ? "enabled" : "disabled",
                                        SelectedClass = "",
                                        SKUId = num8,
                                        Value = sku.Version
                                    };
                                    web.Version.Add(tsku3);
                                }
                            }
                        }
                    }
                }
                decimal num12 = 0M;
                decimal num13 = 0M;
                num12 = (from s in product.SKUInfo
                         where s.Stock >= 0L
                         select s).Min<SKUInfo>(s => s.SalePrice);
                num13 = (from s in product.SKUInfo
                         where s.Stock >= 0L
                         select s).Max<SKUInfo>(s => s.SalePrice);
                if ((num12 == 0M) && (num13 == 0M))
                {
                    str = product.MinSalePrice.ToString("f2");
                }
                else if (num13 > num12)
                {
                    str = string.Format("{0}-{1}", num12.ToString("f2"), num13.ToString("f2"));
                }
                else
                {
                    str = string.Format("{0}", num12.ToString("f2"));
                }
            }
            ViewBag.Price = string.IsNullOrWhiteSpace(str) ? product.MinSalePrice.ToString("f2") : str;
            ViewBag.DisableBuy = product.DisableBuy;
            web.Product = product;

            ViewBag.IsSellerAdminProdcut = product.Himall_Shops.IsSelf;
            ViewBag.CouponCount = ServiceHelper.Create<ICouponService>().GetTopCoupon(product.ShopId, 5, PlatformType.PC).Count();
            ViewBag.IsExpiredShop = ServiceHelper.Create<IShopService>().IsExpiredShop(product.ShopId);
            IQueryable<StatisticOrderCommentsInfo> shopStatisticOrderComments = ServiceHelper.Create<IShopService>().GetShopStatisticOrderComments(product.ShopId);
            StatisticOrderCommentsInfo info5 = (from c in shopStatisticOrderComments
                                                where ((int)c.CommentKey) == 1
                                                select c).FirstOrDefault();
            StatisticOrderCommentsInfo info6 = (from c in shopStatisticOrderComments
                                                where ((int)c.CommentKey) == 9
                                                select c).FirstOrDefault();
            StatisticOrderCommentsInfo info7 = (from c in shopStatisticOrderComments
                                                where ((int)c.CommentKey) == 5
                                                select c).FirstOrDefault();
            StatisticOrderCommentsInfo info8 = (from c in shopStatisticOrderComments
                                                where ((int)c.CommentKey) == 2
                                                select c).FirstOrDefault();
            StatisticOrderCommentsInfo info9 = (from c in shopStatisticOrderComments
                                                where ((int)c.CommentKey) == 10
                                                select c).FirstOrDefault();
            StatisticOrderCommentsInfo info10 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 6
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info11 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 3
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info12 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 4
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info13 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 11
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info14 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 12
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info15 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 7
                                                 select c).FirstOrDefault();
            StatisticOrderCommentsInfo info16 = (from c in shopStatisticOrderComments
                                                 where ((int)c.CommentKey) == 8
                                                 select c).FirstOrDefault();
            int num14 = 5;
            if ((info5 != null) && (info8 != null))
            {
                ViewBag.ProductAndDescription = info5.CommentValue;
                ViewBag.ProductAndDescriptionPeer = info8.CommentValue;
                ViewBag.ProductAndDescriptionMin = info12.CommentValue;
                ViewBag.ProductAndDescriptionMax = info11.CommentValue;
            }
            else
            {
                ViewBag.ProductAndDescription = num14;
                ViewBag.ProductAndDescriptionPeer = num14;
                ViewBag.ProductAndDescriptionMin = num14;
                ViewBag.ProductAndDescriptionMax = num14;
            }
            if ((info6 != null) && (info9 != null))
            {
                ViewBag.SellerServiceAttitude = info6.CommentValue;
                ViewBag.SellerServiceAttitudePeer = info9.CommentValue;
                ViewBag.SellerServiceAttitudeMax = info13.CommentValue;
                ViewBag.SellerServiceAttitudeMin = info14.CommentValue;
            }
            else
            {
                ViewBag.SellerServiceAttitude = num14;
                ViewBag.SellerServiceAttitudePeer = num14;
                ViewBag.SellerServiceAttitudeMax = num14;
                ViewBag.SellerServiceAttitudeMin = num14;
            }
            if ((info10 != null) && (info7 != null))
            {
                ViewBag.SellerDeliverySpeed = info7.CommentValue;
                ViewBag.SellerDeliverySpeedPeer = info10.CommentValue;
                ViewBag.SellerDeliverySpeedMax = (info15 != null) ? info15.CommentValue : 0M;
                ViewBag.sellerDeliverySpeedMin = (info16 != null) ? info16.CommentValue : 0M;
            }
            else
            {
                ViewBag.SellerDeliverySpeed = num14;
                ViewBag.SellerDeliverySpeedPeer = num14;
                ViewBag.SellerDeliverySpeedMax = num14;
                ViewBag.sellerDeliverySpeedMin = num14;
            }
            if (base.CurrentUser != null)
            {
                OrderQuery orderQuery = new OrderQuery
                {
                    UserName = base.CurrentUser.UserName,
                    UserId = new long?(base.CurrentUser.Id),
                    PageSize = 20,
                    PageNo = 1
                };
                List<long> ids = (from c in ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null).Models
                                  orderby c.OrderDate descending
                                  select c.Id).ToList();
                using (IEnumerator<KeyValuePair<long, OrderItemInfo>> enumerator2 = (from c in ServiceHelper.Create<IOrderService>().GetOrderItems(ids)
                                                                                     orderby c.Value.OrderInfo.OrderDate descending
                                                                                     select c).GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Func<HotProductInfo, bool> func7 = null;
                        KeyValuePair<long, OrderItemInfo> orderItem = enumerator2.Current;
                        if (func7 == null)
                        {
                            func7 = c => c.Id == orderItem.Value.ProductId;
                        }
                        if (web.BoughtProducts.Where(func7).Count() <= 0)
                        {
                            ProductInfo info17 = service.GetProduct(orderItem.Value.ProductId);
                            if ((info17 == null) || (info17.SaleStatus != ProductInfo.ProductSaleStatus.InDelete))
                            {
                                HotProductInfo info18 = new HotProductInfo
                                {
                                    Id = orderItem.Value.ProductId,
                                    Name = orderItem.Value.ProductName,
                                    Price = orderItem.Value.SalePrice,
                                    ImgPath = orderItem.Value.ThumbnailsUrl
                                };
                                web.BoughtProducts.Add(info18);
                            }
                        }
                    }
                }
                if (web.BoughtProducts.Count < 5)
                {
                    web.BoughtProducts.Clear();
                }
            }
            info19 = ServiceHelper.Create<IBrandService>().GetBrand(web.Product.BrandId);
            web.Product.BrandName = (info19 == null) ? "" : info19.Name;
            Bitmap bitmap = QRCodeHelper.Create(string.Concat(new object[] { "http://", base.HttpContext.Request.Url.Authority, "/m-wap/product/detail/", product.Id }));
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            string str7 = "data:image/gif;base64," + Convert.ToBase64String(stream.ToArray());
            stream.Dispose();
            ViewBag.Code = str7;
            ViewBag.CashDepositsObligation = Instance<ICashDepositsService>.Create.GetCashDepositsObligation(product.Id);
            return View(web);
        }


        [HttpGet]
		public JsonResult GetBrandOfficial(long gid)
		{
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(gid);
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(product.CategoryId);
			CategoryInfo categoryInfo = ServiceHelper.Create<ICategoryService>().GetCategory(category.ParentCategoryId);
			if (categoryInfo != null)
			{
				long[] array = (
					from C in ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(categoryInfo.Id)
					select C.Id).ToArray();
				foreach (BrandInfo brandsByCategoryId in ServiceHelper.Create<IBrandService>().GetBrandsByCategoryIds(array))
				{
					CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
					{
						Name = brandsByCategoryId.Name,
						Id = brandsByCategoryId.Id.ToString()
					};
					categoryJsonModels.Add(categoryJsonModel);
				}
			}
			return Json(categoryJsonModels, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCollocationProducts(string productIds, string colloPids)
		{
			char[] chrArray = new char[] { ',' };
			long[] array = (
				from a in productIds.Split(chrArray)
				select long.Parse(a)).ToArray();
			char[] chrArray1 = new char[] { ',' };
			long[] numArray = (
				from a in colloPids.Split(chrArray1)
				select long.Parse(a)).ToArray();
			Dictionary<long, long> nums = new Dictionary<long, long>();
			for (int i = 0; i < array.Length; i++)
			{
				nums.Add(array[i], numArray[i]);
			}
			List<ProductInfo> list = (
				from a in ServiceHelper.Create<IProductService>().GetProductByIds(array)
				where (int)a.SaleStatus == 1 && (int)a.AuditStatus == 2
				select a).ToList();
			IEnumerable<ProductInfo> productInfos = 
				from a in array
				select (
					from t in list
					where t.Id == a
					select t).FirstOrDefault();
			List<CollocationSkusModel> collocationSkusModels = new List<CollocationSkusModel>();
			foreach (ProductInfo productInfo in productInfos)
			{
				List<CollocationSkusModel> collocationSkusModels1 = collocationSkusModels;
				ProductInfo productInfo1 = productInfo;
				KeyValuePair<long, long> keyValuePair = (
					from a in nums
					where a.Key == productInfo.Id
					select a).FirstOrDefault<KeyValuePair<long, long>>();
				collocationSkusModels1.Add(GetCollocationSku(productInfo1, keyValuePair.Value));
			}
			return View(collocationSkusModels);
		}

		private CollocationSkusModel GetCollocationSku(ProductInfo product, long colloId)
		{
			CollocationSkusModel collocationSkusModel = new CollocationSkusModel()
			{
				ProductName = product.ProductName,
				ColloProductId = colloId,
				ImagePath = product.ImagePath,
				ProductId = product.Id,
				MeasureUnit = product.MeasureUnit,
				Stock = product.SKUInfo.Sum<SKUInfo>((SKUInfo a) => a.Stock)
			};
			IEnumerable<CollocationSkuInfo> himallCollocationSkus = 
				from a in product.Himall_CollocationSkus
				where a.ColloProductId == colloId
				select a;
			if (himallCollocationSkus == null)
			{
				collocationSkusModel.MinPrice = product.SKUInfo.Min<SKUInfo>((SKUInfo a) => a.SalePrice);
			}
			else
			{
				collocationSkusModel.MinPrice = himallCollocationSkus.Min<CollocationSkuInfo>((CollocationSkuInfo a) => a.Price);
			}
			if (product.SKUInfo != null && product.SKUInfo.Count() > 0)
			{
				long num = 0;
				long num1 = 0;
				long num2 = 0;
				foreach (SKUInfo list in (
					from s in product.SKUInfo
					orderby s.AutoId
					select s).ToList())
				{
					string[] strArrays = list.Id.Split(new char[] { '\u005F' });
					if (strArrays.Count() > 0)
					{
						long.TryParse(strArrays[1], out num);
						if (num != 0)
						{
							if (!collocationSkusModel.Color.Any((ProductSKU v) => v.Value.Equals(list.Color)))
							{
								long num3 = (
									from s in product.SKUInfo
									where s.Color.Equals(list.Color)
									select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
								CollectionSKU color = collocationSkusModel.Color;
								ProductSKU productSKU = new ProductSKU()
								{
									Name = "选择颜色",
									EnabledClass = (num3 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num,
									Value = list.Color
								};
                                color.Add(productSKU);
							}
						}
					}
					if (strArrays.Count() > 1)
					{
						long.TryParse(strArrays[2], out num1);
						if (num1 != 0)
						{
							if (!collocationSkusModel.Size.Any((ProductSKU v) => v.Value.Equals(list.Size)))
							{
								long num4 = (
									from s in product.SKUInfo
									where s.Size.Equals(list.Size)
									select s).Sum<SKUInfo>((SKUInfo s1) => s1.Stock);
								CollectionSKU size = collocationSkusModel.Size;
								ProductSKU productSKU1 = new ProductSKU()
								{
									Name = "选择尺码",
									EnabledClass = (num4 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num1,
									Value = list.Size
								};
                                size.Add(productSKU1);
							}
						}
					}
					if (strArrays.Count() <= 2)
					{
						continue;
					}
					long.TryParse(strArrays[3], out num2);
					if (num2 == 0)
					{
						continue;
					}
					if (collocationSkusModel.Version.Any((ProductSKU v) => v.Value.Equals(list.Version)))
					{
						continue;
					}
					long num5 = (
						from s in product.SKUInfo
						where s.Version.Equals(list.Version)
						select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
					CollectionSKU version = collocationSkusModel.Version;
					ProductSKU productSKU2 = new ProductSKU()
					{
						Name = "选择版本",
						EnabledClass = (num5 != 0 ? "enabled" : "disabled"),
						SelectedClass = "",
						SKUId = num2,
						Value = list.Version
					};
                    version.Add(productSKU2);
				}
			}
			return collocationSkusModel;
		}

		public JsonResult GetCommentByProduct(long pId, int pageNo, int commentType = 0, int pageSize = 3)
		{
			IEnumerable<ProductCommentInfo> array;
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(pId);
			if (commentsByProductId == null || commentsByProductId.Count() <= 0)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}
			switch (commentType)
			{
				case 1:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark >= 4
						select c;
					break;
				}
				case 2:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark == 3
						select c;
					break;
				}
				case 3:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark <= 2
						select c;
					break;
				}
				default:
				{
					array = (
						from c in commentsByProductId
						orderby c.ReviewMark descending
						select c).ToArray();
					break;
				}
			}
			ProductCommentInfo[] productCommentInfoArray = (
				from a in array
				orderby a.ReviewDate descending
				select a).Skip((pageNo - 1) * pageSize).Take(pageSize).ToArray();
			var variable = 
				from c in productCommentInfoArray
                select new { UserName = c.UserName, ReviewContent = c.ReviewContent, ReviewDate = c.ReviewDate.ToString("yyyy-MM-dd HH:mm:ss"), ReplyContent = (string.IsNullOrWhiteSpace(c.ReplyContent) ? "暂无回复" : c.ReplyContent), ReplyDate = (c.ReplyDate.HasValue ? c.ReplyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : " "), ReviewMark = c.ReviewMark, BuyDate = c.Himall_OrderItems.OrderInfo.OrderDate.ToString("yyyy-MM-dd HH:mm:ss") };
			return Json(new { successful = true, comments = variable, totalPage = (Math.Ceiling((decimal)array.Count() / pageSize)), pageSize = pageSize, currentPage = pageNo, goodComment = (
				from c in commentsByProductId.ToArray()
                where c.ReviewMark >= 4
				select c).Count(), badComment = (
				from c in commentsByProductId.ToArray()
                where c.ReviewMark <= 2
				select c).Count(), comment = (
				from c in commentsByProductId.ToArray()
                where c.ReviewMark == 3
				select c).Count(), commentType = commentType }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetCommentsNumber(long gid)
		{
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(gid);
			return Json(new { Comments = product.Himall_ProductComments.Count(), Consultations = product.ProductConsultationInfo.Count() }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetConsultationByProduct(long pId, int pageNo, int pageSize = 3)
		{
			IQueryable<ProductConsultationInfo> consultations = ServiceHelper.Create<IConsultationService>().GetConsultations(pId);
			if (consultations == null || consultations.Count() <= 0)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}
			ProductConsultationInfo[] array = (
				from a in consultations
				orderby a.ConsultationDate descending
				select a).Skip((pageNo - 1) * pageSize).Take(pageSize).ToArray();
			var variable = 
				from c in array
                select new { UserName = c.UserName, ConsultationContent = c.ConsultationContent, ConsultationDate = c.ConsultationDate.ToString("yyyy-MM-dd HH:mm:ss"), ReplyContent = (string.IsNullOrWhiteSpace(c.ReplyContent) ? "暂无回复" : c.ReplyContent), ReplyDate = (c.ReplyDate.HasValue ? c.ReplyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : " ") };
			return Json(new { successful = true, consults = variable, totalPage = (Math.Ceiling((decimal)consultations.Count() / pageSize)), currentPage = pageNo }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetEnableBuyInfo(long gid)
		{
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(gid);
			return Json(new { hasQuick = ((base.CurrentUser == null ? false : ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddressByUserId(base.CurrentUser.Id).Any((ShippingAddressInfo s) => s.IsQuick)) ? 1 : 0), Logined = (base.CurrentUser != null ? 1 : 0), hasSKU = product.SKUInfo.Any((SKUInfo s) => s.Stock > 0), IsOnSale = (product.AuditStatus != ProductInfo.ProductAuditStatus.Audited ? false : product.SaleStatus == ProductInfo.ProductSaleStatus.OnSale) }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetHotConcernedProduct(long sid)
		{
			List<HotProductInfo> hotProductInfos = new List<HotProductInfo>();
			IQueryable<ProductInfo> hotConcernedProduct = ServiceHelper.Create<IProductService>().GetHotConcernedProduct(sid, 5);
			if (hotConcernedProduct != null)
			{
				ProductInfo[] array = hotConcernedProduct.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					ProductInfo productInfo = array[i];
					HotProductInfo hotProductInfo = new HotProductInfo()
					{
						ImgPath = productInfo.ImagePath,
						Name = productInfo.ProductName,
						Price = productInfo.MinSalePrice,
						Id = productInfo.Id,
						SaleCount = productInfo.ConcernedCount
					};
					hotProductInfos.Add(hotProductInfo);
				}
			}
			return Json(hotProductInfos, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetHotSaleProduct(long sid)
		{
			List<HotProductInfo> hotProductInfos = new List<HotProductInfo>();
			IQueryable<ProductInfo> hotSaleProduct = ServiceHelper.Create<IProductService>().GetHotSaleProduct(sid, 5);
			if (hotSaleProduct != null)
			{
				ProductInfo[] array = hotSaleProduct.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					ProductInfo productInfo = array[i];
					HotProductInfo hotProductInfo = new HotProductInfo()
					{
						ImgPath = productInfo.ImagePath,
						Name = productInfo.ProductName,
						Price = productInfo.MinSalePrice,
						Id = productInfo.Id,
						SaleCount = (int)productInfo.SaleCounts
					};
					hotProductInfos.Add(hotProductInfo);
				}
			}
			return Json(hotProductInfos, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetPrice(string skuId)
		{
			decimal salePrice = ServiceHelper.Create<IProductService>().GetSku(skuId).SalePrice;
			string str = salePrice.ToString("f2");
			return Json(new { price = str }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetProductAttr(long pid)
		{
			List<TypeAttributesModel> typeAttributesModels = new List<TypeAttributesModel>();
			List<ProductAttributeInfo> list = ServiceHelper.Create<IProductService>().GetProductAttribute(pid).ToList();
			foreach (ProductAttributeInfo productAttributeInfo in list)
			{
				if (typeAttributesModels.Any((TypeAttributesModel p) => p.AttrId == productAttributeInfo.AttributeId))
				{
					TypeAttributesModel typeAttributesModel = typeAttributesModels.FirstOrDefault((TypeAttributesModel p) => p.AttrId == productAttributeInfo.AttributeId);
					if (typeAttributesModel.AttrValues.Any((TypeAttrValue p) => p.Id == productAttributeInfo.ValueId.ToString()))
					{
						continue;
					}
					if (productAttributeInfo.AttributesInfo.AttributeValueInfo.FirstOrDefault((AttributeValueInfo a) => a.Id == productAttributeInfo.ValueId) == null)
					{
						continue;
					}
					List<TypeAttrValue> attrValues = typeAttributesModel.AttrValues;
					TypeAttrValue typeAttrValue = new TypeAttrValue()
					{
						Id = productAttributeInfo.ValueId.ToString(),
						Name = productAttributeInfo.AttributesInfo.AttributeValueInfo.FirstOrDefault((AttributeValueInfo a) => a.Id == productAttributeInfo.ValueId).Value
					};
					attrValues.Add(typeAttrValue);
				}
				else
				{
					TypeAttributesModel typeAttributesModel1 = new TypeAttributesModel()
					{
						AttrId = productAttributeInfo.AttributeId,
						AttrValues = new List<TypeAttrValue>(),
						Name = productAttributeInfo.AttributesInfo.Name
					};
					TypeAttributesModel typeAttributesModel2 = typeAttributesModel1;
					foreach (AttributeValueInfo attributeValueInfo in productAttributeInfo.AttributesInfo.AttributeValueInfo.ToList())
					{
						if (!list.Any((ProductAttributeInfo p) => p.ValueId == attributeValueInfo.Id))
						{
							continue;
						}
						List<TypeAttrValue> typeAttrValues = typeAttributesModel2.AttrValues;
						TypeAttrValue typeAttrValue1 = new TypeAttrValue()
						{
							Id = attributeValueInfo.Id.ToString(),
							Name = attributeValueInfo.Value
						};
						typeAttrValues.Add(typeAttrValue1);
					}
					typeAttributesModels.Add(typeAttributesModel2);
				}
			}
			return Json(typeAttributesModels, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetProductDesc(long pid)
		{
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(pid);
			string description = product.ProductDescriptionInfo.Description;
			string str = "";
			string str1 = "";
			if (product.ProductDescriptionInfo.DescriptionPrefixId != 0)
			{
				ProductDescriptionTemplateInfo template = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplate(product.ProductDescriptionInfo.DescriptionPrefixId, product.ShopId);
				str = (template == null ? "" : template.Content);
			}
			if (product.ProductDescriptionInfo.DescriptiondSuffixId != 0)
			{
				ProductDescriptionTemplateInfo productDescriptionTemplateInfo = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplate(product.ProductDescriptionInfo.DescriptiondSuffixId, product.ShopId);
				str1 = (productDescriptionTemplateInfo == null ? "" : productDescriptionTemplateInfo.Content);
			}
			return Json(new { ProductDescription = description, DescriptionPrefix = str, DescriptiondSuffix = str1 }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult GetShopBonus(long shopid)
		{
			ShopBonusInfo byShopId = ServiceHelper.Create<IShopBonusService>().GetByShopId(shopid);
			if (byShopId == null)
			{
				return Json(null);
			}
			ProductBonusLableModel productBonusLableModel = new ProductBonusLableModel()
			{
				Count = byShopId.Count,
				GrantPrice = byShopId.GrantPrice,
				RandomAmountStart = byShopId.RandomAmountStart,
				RandomAmountEnd = byShopId.RandomAmountEnd
			};
			return Json(productBonusLableModel);
		}

		[HttpGet]
		public JsonResult GetShopCate(long gid)
		{
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(gid);
			List<ShopCategoryInfo> list = ServiceHelper.Create<IShopCategoryService>().GetShopCategory(product.ShopId).ToList();
			foreach (ShopCategoryInfo shopCategoryInfo in 
				from s in list
				where s.ParentCategoryId == 0
                select s)
			{
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = shopCategoryInfo.Name,
					Id = shopCategoryInfo.Id.ToString(),
					SubCategory = new List<SecondLevelCategory>()
				};
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				foreach (ShopCategoryInfo list1 in (
					from s in list
					where s.ParentCategoryId == shopCategoryInfo.Id
					select s).ToList())
				{
					SecondLevelCategory secondLevelCategory = new SecondLevelCategory()
					{
						Name = list1.Name,
						Id = list1.Id.ToString()
					};
					categoryJsonModel1.SubCategory.Add(secondLevelCategory);
				}
				categoryJsonModels.Add(categoryJsonModel1);
			}
			return Json(categoryJsonModels, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetShopCateOfficial(long gid)
		{
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(gid);
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(product.CategoryId);
			CategoryInfo categoryInfo = ServiceHelper.Create<ICategoryService>().GetCategory(category.ParentCategoryId);
			if (categoryInfo != null)
			{
				foreach (CategoryInfo categoryByParentId in ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(categoryInfo.Id))
				{
					CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
					{
						Name = categoryByParentId.Name,
						Id = categoryByParentId.Id.ToString()
					};
					categoryJsonModels.Add(categoryJsonModel);
				}
			}
			return Json(categoryJsonModels, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetShopInfo(long sid, long productId = 0L)
		{
			string empty = string.Empty;
			long num = 0;
			decimal currentBalance = new decimal(0);
			if (productId != 0)
			{
				ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(productId);
				if (product != null)
				{
					BrandInfo brand = ServiceHelper.Create<IBrandService>().GetBrand(product.BrandId);
					empty = (brand == null ? string.Empty : brand.Logo);
					num = (brand == null ? num : brand.Id);
				}
			}
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(sid, false);
			ShopServiceMarkModel shopComprehensiveMark = ShopServiceMark.GetShopComprehensiveMark(sid);
			CashDepositInfo cashDepositByShopId = Instance<ICashDepositsService>.Create.GetCashDepositByShopId(sid);
			if (cashDepositByShopId != null)
			{
				currentBalance = cashDepositByShopId.CurrentBalance;
			}
			CashDepositsObligation cashDepositsObligation = Instance<ICashDepositsService>.Create.GetCashDepositsObligation(productId);
			var variable = new { CompanyName = shop.CompanyName, Id = sid, PackMark = shopComprehensiveMark.PackMark, ServiceMark = shopComprehensiveMark.ServiceMark, ComprehensiveMark = shopComprehensiveMark.ComprehensiveMark, Phone = shop.CompanyPhone, Name = shop.ShopName, Address = ServiceHelper.Create<IRegionService>().GetRegionFullName(shop.CompanyRegionId, " "), ProductMark = 3, IsSelf = shop.IsSelf, BrandLogo = empty, BrandId = num, CashDeposits = currentBalance, IsSevenDayNoReasonReturn = cashDepositsObligation.IsSevenDayNoReasonReturn, IsCustomerSecurity = cashDepositsObligation.IsCustomerSecurity, TimelyDelivery = cashDepositsObligation.IsTimelyShip };
			return Json(variable, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetSKUInfo(long pId, long colloPid = 0L)
		{
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(pId);
			List<CollocationSkuInfo> productColloSKU = null;
			if (colloPid != 0)
			{
				productColloSKU = ServiceHelper.Create<ICollocationService>().GetProductColloSKU(pId, colloPid);
			}
			List<ProductSKUModel> productSKUModels = new List<ProductSKUModel>();
			foreach (SKUInfo sKUInfo in 
				from s in product.SKUInfo
				where s.Stock > 0
                select s)
			{
				decimal salePrice = sKUInfo.SalePrice;
				if (productColloSKU != null && productColloSKU.Count > 0)
				{
					CollocationSkuInfo collocationSkuInfo = productColloSKU.FirstOrDefault((CollocationSkuInfo a) => a.SkuID == sKUInfo.Id);
					if (collocationSkuInfo != null)
					{
						salePrice = collocationSkuInfo.Price;
					}
				}
				ProductSKUModel productSKUModel = new ProductSKUModel()
				{
					Price = salePrice,
					SKUId = sKUInfo.Id,
					Stock = (int)sKUInfo.Stock
				};
				productSKUModels.Add(productSKUModel);
			}
			foreach (ProductSKUModel productSKUModel1 in productSKUModels)
			{
				string[] strArrays = productSKUModel1.SKUId.Split(new char[] { '\u005F' });
				productSKUModel1.SKUId = string.Format("{0};{1};{2}", strArrays[1], strArrays[2], strArrays[3]);
			}
			return Json(new { skuArray = productSKUModels }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetStock(string skuId)
		{
			long stock = ServiceHelper.Create<IProductService>().GetSku(skuId).Stock;
			ProductInfo productInfo = ServiceHelper.Create<IProductService>().GetSku(skuId).ProductInfo;
			int num = 0;
			if (productInfo.AuditStatus == ProductInfo.ProductAuditStatus.Audited && productInfo.SaleStatus == ProductInfo.ProductSaleStatus.OnSale)
			{
				num = 1;
			}
			return Json(new { Stock = stock, Status = num }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(long? categoryId, string brandName, string productCode, int? auditStatus, string ids, int page, int rows, string keyWords, string shopName, int? saleStatus)
		{
			IEnumerable<long> nums;
			ProductQuery productQuery = new ProductQuery()
			{
				PageSize = rows,
				PageNo = page,
				BrandNameKeyword = brandName,
				KeyWords = keyWords,
				CategoryId = categoryId
			};
			ProductQuery productQuery1 = productQuery;
			if (string.IsNullOrWhiteSpace(ids))
			{
				nums = null;
			}
			else
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
			}
			productQuery1.Ids = nums;
			productQuery.ShopName = shopName;
			productQuery.ProductCode = productCode;
			ProductQuery value = productQuery;
			if (auditStatus.HasValue)
			{
				value.AuditStatus = new ProductInfo.ProductAuditStatus[] { (ProductInfo.ProductAuditStatus)auditStatus.Value };
				int? nullable = auditStatus;
				if ((nullable.GetValueOrDefault() != 1 ? false : nullable.HasValue))
				{
					value.SaleStatus = new ProductInfo.ProductSaleStatus?(ProductInfo.ProductSaleStatus.OnSale);
				}
			}
			if (saleStatus.HasValue)
			{
				value.SaleStatus = new ProductInfo.ProductSaleStatus?((ProductInfo.ProductSaleStatus)saleStatus.Value);
			}
			PageModel<ProductInfo> products = ServiceHelper.Create<IProductService>().GetProducts(value);
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			IEnumerable<Himall.Web.Areas.Admin.Models.Product.ProductModel> array = 
				from item in products.Models.ToArray()
				select new Himall.Web.Areas.Admin.Models.Product.ProductModel()
				{
					name = item.ProductName,
					brandName = (item.BrandId == 0 ? "" : (brandService.GetBrand(item.BrandId) == null ? "" : brandService.GetBrand(item.BrandId).Name)),
					categoryName = (categoryService.GetCategory(item.CategoryId) == null ? "" : categoryService.GetCategory(item.CategoryId).Name),
					id = item.Id,
					imgUrl = item.GetImage(ProductInfo.ImageSize.Size_50, 1),
					price = item.MinSalePrice,
					state = (item.AuditStatus == ProductInfo.ProductAuditStatus.WaitForAuditing ? (item.SaleStatus == ProductInfo.ProductSaleStatus.OnSale ? ProductInfo.ProductAuditStatus.WaitForAuditing.ToDescription() : ProductInfo.ProductSaleStatus.InStock.ToDescription()) : item.AuditStatus.ToDescription()),
					auditStatus = (int)item.AuditStatus,
					url = "",
					auditReason = (item.ProductDescriptionInfo != null ? item.ProductDescriptionInfo.AuditReason : ""),
					shopName = (shopService.GetShopBasicInfo(item.ShopId) == null ? "" : shopService.GetShopBasicInfo(item.ShopId).ShopName),
					saleStatus = (int)item.SaleStatus,
					productCode = item.ProductCode
				};
			DataGridModel<Himall.Web.Areas.Admin.Models.Product.ProductModel> dataGridModel = new DataGridModel<Himall.Web.Areas.Admin.Models.Product.ProductModel>()
			{
				rows = array,
				total = products.Total
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		public JsonResult LogProduct(long pid)
		{
			if (base.CurrentUser == null)
			{
				BrowseHistrory.AddBrowsingProduct(pid, 0);
			}
			else
			{
				BrowseHistrory.AddBrowsingProduct(pid, base.CurrentUser.Id);
			}
			ServiceHelper.Create<IProductService>().LogProductVisti(pid);
			return Json(null);
		}

		[ChildActionOnly]
		public ActionResult ProductColloCation(long productId)
		{
			CollocationInfo collocationByProductId = ServiceHelper.Create<ICollocationService>().GetCollocationByProductId(productId);
			List<CollocationProducts> list = null;
			if (collocationByProductId != null)
			{
				list = collocationByProductId.Himall_CollocationPoruducts.Where((CollocationPoruductInfo a) => {
					if (a.Himall_Products.SaleStatus != ProductInfo.ProductSaleStatus.OnSale)
					{
						return false;
					}
					return a.Himall_Products.AuditStatus == ProductInfo.ProductAuditStatus.Audited;
				}).Select<CollocationPoruductInfo, CollocationProducts>((CollocationPoruductInfo a) => new CollocationProducts()
				{
					DisplaySequence = a.DisplaySequence.Value,
					IsMain = a.IsMain,
					Stock = a.Himall_Products.SKUInfo.Sum<SKUInfo>((SKUInfo t) => t.Stock),
					MaxCollPrice = a.Himall_CollocationSkus.Max<CollocationSkuInfo>((CollocationSkuInfo x) => x.Price),
					MaxSalePrice = a.Himall_CollocationSkus.Max<CollocationSkuInfo>((CollocationSkuInfo x) => x.SkuPirce).GetValueOrDefault(),
					MinCollPrice = a.Himall_CollocationSkus.Min<CollocationSkuInfo>((CollocationSkuInfo x) => x.Price),
					MinSalePrice = a.Himall_CollocationSkus.Min<CollocationSkuInfo>((CollocationSkuInfo x) => x.SkuPirce).GetValueOrDefault(),
					ProductName = a.Himall_Products.ProductName,
					ProductId = a.ProductId,
					ColloPid = a.Id,
					Image = a.Himall_Products.ImagePath
				}).OrderBy<CollocationProducts, int>((CollocationProducts a) => a.DisplaySequence).ToList();
			}
			return base.PartialView(list);
		}
	}
}