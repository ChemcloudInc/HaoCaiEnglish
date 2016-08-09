using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web;
using Himall.Web.Areas.Mobile;
using Himall.Web.Areas.Mobile.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class OrderController : BaseMobileMemberController
	{
		public OrderController()
		{
		}

		private void AddVshopBuyNumber(IEnumerable<long> orderIds)
		{
			IEnumerable<long> orders = 
				from item in ServiceHelper.Create<IOrderService>().GetOrders(orderIds)
				select item.ShopId;
			IVShopService vShopService = ServiceHelper.Create<IVShopService>();
			IEnumerable<long> nums = orders.Select<long, long>((long item) => {
				VShopInfo vShopByShopId = vShopService.GetVShopByShopId(item);
				if (vShopByShopId != null)
				{
					return vShopByShopId.Id;
				}
				return 0;
			}).Where((long item) => item > 0);
			foreach (long num in nums)
			{
				vShopService.AddBuyNumber(num);
			}
		}

		public ActionResult ChooseShippingAddress(string returnURL = "")
		{
			IQueryable<ShippingAddressInfo> userShippingAddressByUserId = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddressByUserId(base.CurrentUser.Id);
			List<ShippingAddressInfo> shippingAddressInfos = new List<ShippingAddressInfo>();
			foreach (ShippingAddressInfo shippingAddressInfo in userShippingAddressByUserId)
			{
				ShippingAddressInfo shippingAddressInfo1 = new ShippingAddressInfo()
				{
					Id = shippingAddressInfo.Id,
					ShipTo = shippingAddressInfo.ShipTo,
					Phone = shippingAddressInfo.Phone,
					RegionFullName = shippingAddressInfo.RegionFullName,
					Address = shippingAddressInfo.Address
				};
				shippingAddressInfos.Add(shippingAddressInfo1);
			}
			return View(shippingAddressInfos);
		}

		[HttpPost]
		public JsonResult CloseOrder(long orderId)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(orderId, base.CurrentUser.Id);
			if (order == null)
			{
				Result result = new Result()
				{
					success = false,
					msg = "取消失败，该订单已删除或者不属于当前用户！"
				};
				return Json(result);
			}
			ServiceHelper.Create<IOrderService>().MemberCloseOrder(orderId, base.CurrentUser.UserName, false);
			foreach (OrderItemInfo orderItemInfo in order.OrderItemInfo)
			{
				ServiceHelper.Create<IProductService>().UpdateStock(orderItemInfo.SkuId, orderItemInfo.Quantity);
			}
			Result result1 = new Result()
			{
				success = true,
				msg = "取消成功"
			};
			return Json(result1);
		}

		[HttpPost]
		public JsonResult ConfirmOrder(long orderId)
		{
			ServiceHelper.Create<IOrderService>().MembeConfirmOrder(orderId, base.CurrentUser.UserName);
			Result result = new Result()
			{
				success = true,
				msg = "操作成功！"
			};
			return Json(result);
		}

		public ActionResult Detail(long id)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(id, base.CurrentUser.Id);
			IShopService shopService = ServiceHelper.Create<IShopService>();
			IProductService productService = ServiceHelper.Create<IProductService>();
			OrderDetail orderDetail = new OrderDetail()
			{
				ShopName = shopService.GetShop(order.ShopId, false).ShopName,
				ShopId = order.ShopId,
				OrderItems = 
					from item in order.OrderItemInfo
					select new OrderItem()
					{
						ProductId = item.ProductId,
						ProductName = item.ProductName,
						Count = item.Quantity,
						Price = item.SalePrice,
						ProductImage = productService.GetProduct(item.ProductId).GetImage(ProductInfo.ImageSize.Size_100, 1)
					}
			};
			ViewBag.Detail = orderDetail;
			ViewBag.Bonus = (dynamic)null;
			if (base.PlatformType == Himall.Core.PlatformType.WeiXin)
			{
				IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
				ShopBonusGrantInfo grantByUserOrder = shopBonusService.GetGrantByUserOrder(id, base.CurrentUser.Id);
				if (grantByUserOrder != null)
				{
					ViewBag.Bonus = grantByUserOrder;
					dynamic viewBag = base.ViewBag;
					object[] host = new object[] { "http://", base.Request.Url.Host, "/m-weixin/shopbonus/index/", shopBonusService.GetGrantIdByOrderId(id) };
					viewBag.ShareHref = string.Concat(host);
				}
			}
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			return View(order);
		}

		public ActionResult EditShippingAddress(long addressId = 0L, string returnURL = "")
		{
			ShippingAddressInfo shippingAddressInfo = new ShippingAddressInfo();
			if (addressId != 0)
			{
				shippingAddressInfo = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(addressId);
			}
			ViewBag.addId = addressId;
			return View(shippingAddressInfo);
		}

		public ActionResult ExpressInfo(long orderId)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(orderId, base.CurrentUser.Id);
			if (order != null)
			{
				ViewBag.ExpressCompanyName = order.ExpressCompanyName;
				ViewBag.ShipOrderNumber = order.ShipOrderNumber;
			}
			return View();
		}

		private void GetOrderProductsInfo(IEnumerable<string> skuIds, IEnumerable<int> counts)
		{
			IProductService productService = ServiceHelper.Create<IProductService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			int num3 = 0;
			int cityId = 0;
			ShippingAddressInfo defaultUserShippingAddressByUserId = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
			if (defaultUserShippingAddressByUserId != null)
			{
				cityId = Instance<IRegionService>.Create.GetCityId(defaultUserShippingAddressByUserId.RegionIdPath);
			}
			List<CartItemModel> list = skuIds.Select<string, CartItemModel>((string item) => {
				SKUInfo sku = productService.GetSku(item);
				IEnumerable<int> nums = counts;
				int num = num3;
				int num1 = num;
				num3 = num + 1;
				int num2 = nums.ElementAt<int>(num1);
				LimitTimeMarketInfo limitTimeMarketItemByProductId = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(sku.ProductInfo.Id);
				if (limitTimeMarketItemByProductId != null && num2 > limitTimeMarketItemByProductId.MaxSaleCount)
				{
					throw new HimallException(string.Concat("超过最大限购数量：", limitTimeMarketItemByProductId.MaxSaleCount.ToString()));
				}
				return new CartItemModel()
				{
					skuId = item,
					id = sku.ProductInfo.Id,
					imgUrl = sku.ProductInfo.GetImage(ProductInfo.ImageSize.Size_100, 1),
					name = sku.ProductInfo.ProductName,
					shopId = sku.ProductInfo.ShopId,
					price = (limitTimeMarketItemByProductId == null ? sku.SalePrice : limitTimeMarketItemByProductId.Price),
					count = num2,
					productCode = sku.ProductInfo.ProductCode
				};
			}).ToList();
			IEnumerable<IGrouping<long, CartItemModel>> groupings = 
				from a in list
				group a by a.shopId;
			List<ShopCartItemModel> shopCartItemModels = new List<ShopCartItemModel>();
			foreach (IGrouping<long, CartItemModel> nums1 in groupings)
			{
				IEnumerable<long> nums2 = 
					from r in nums1
					select r.id;
				IEnumerable<int> nums3 = 
					from r in nums1
					select r.count;
				ShopCartItemModel shopCartItemModel = new ShopCartItemModel()
				{
					shopId = nums1.Key
				};
                shopCartItemModel.CartItemModels = (
                        from a in list
                        where a.shopId == shopCartItemModel.shopId
                        select a).ToList();
                shopCartItemModel.ShopName = shopService.GetShop(shopCartItemModel.shopId, false).ShopName;
                if (cityId != 0)
				{
					shopCartItemModel.Freight = ServiceHelper.Create<IProductService>().GetFreight(nums2, nums3, cityId);
				}
				List<ShopBonusReceiveInfo> detailToUse = ServiceHelper.Create<IShopBonusService>().GetDetailToUse(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
				List<CouponRecordInfo> userCoupon = ServiceHelper.Create<ICouponService>().GetUserCoupon(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
				if (detailToUse.Count() > 0 && userCoupon.Count() > 0)
				{
					ShopBonusReceiveInfo shopBonusReceiveInfo = detailToUse.FirstOrDefault();
					CouponRecordInfo couponRecordInfo = userCoupon.FirstOrDefault();
					decimal? price = shopBonusReceiveInfo.Price;
					decimal price1 = couponRecordInfo.Himall_Coupon.Price;
					if ((price.GetValueOrDefault() <= price1 ? true : !price.HasValue))
					{
						shopCartItemModel.Coupon = new CouponModel()
						{
							CouponName = couponRecordInfo.Himall_Coupon.CouponName,
							CouponId = couponRecordInfo.Id,
							CouponPrice = couponRecordInfo.Himall_Coupon.Price,
							Type = 0
						};
					}
					else
					{
						shopCartItemModel.Coupon = new CouponModel()
						{
							CouponName = shopBonusReceiveInfo.Himall_ShopBonusGrant.Himall_ShopBonus.Name,
							CouponId = shopBonusReceiveInfo.Id,
							CouponPrice = shopBonusReceiveInfo.Price.Value,
							Type = 1
						};
					}
				}
				else if (detailToUse.Count() <= 0 && userCoupon.Count() <= 0)
				{
					shopCartItemModel.Coupon = null;
				}
				else if (detailToUse.Count() <= 0 && userCoupon.Count() > 0)
				{
					CouponRecordInfo couponRecordInfo1 = userCoupon.FirstOrDefault();
					shopCartItemModel.Coupon = new CouponModel()
					{
						CouponName = couponRecordInfo1.Himall_Coupon.CouponName,
						CouponId = couponRecordInfo1.Id,
						CouponPrice = couponRecordInfo1.Himall_Coupon.Price,
						Type = 0
					};
				}
				else if (detailToUse.Count() > 0 && userCoupon.Count() <= 0)
				{
					ShopBonusReceiveInfo shopBonusReceiveInfo1 = detailToUse.FirstOrDefault();
					shopCartItemModel.Coupon = new CouponModel()
					{
						CouponName = shopBonusReceiveInfo1.Himall_ShopBonusGrant.Himall_ShopBonus.Name,
						CouponId = shopBonusReceiveInfo1.Id,
						CouponPrice = shopBonusReceiveInfo1.Price.Value,
						Type = 1
					};
				}
				decimal num4 = shopCartItemModel.CartItemModels.Sum<CartItemModel>((CartItemModel d) => d.price * d.count);
				decimal num5 = num4 - (shopCartItemModel.Coupon == null ? new decimal(0) : shopCartItemModel.Coupon.CouponPrice);
				decimal freeFreight = shopService.GetShop(shopCartItemModel.shopId, false).FreeFreight;
				shopCartItemModel.isFreeFreight = false;
				if (freeFreight > new decimal(0))
				{
					shopCartItemModel.shopFreeFreight = freeFreight;
					if (num5 >= freeFreight)
					{
						shopCartItemModel.Freight = new decimal(0);
						shopCartItemModel.isFreeFreight = true;
					}
				}
				shopCartItemModels.Add(shopCartItemModel);
			}
			decimal num6 = (
				from a in shopCartItemModels
				where a.Coupon != null
				select a).Sum<ShopCartItemModel>((ShopCartItemModel b) => b.Coupon.CouponPrice);
			ViewBag.products = shopCartItemModels;
			base.ViewBag.totalAmount = list.Sum<CartItemModel>((CartItemModel item) => item.price * item.count);
			base.ViewBag.Freight = shopCartItemModels.Sum<ShopCartItemModel>((ShopCartItemModel a) => a.Freight);
			dynamic viewBag = base.ViewBag;
			dynamic obj = ViewBag.totalAmount;
			viewBag.orderAmount = obj + ViewBag.Freight - num6;
			IMemberIntegralService memberIntegralService = ServiceHelper.Create<IMemberIntegralService>();
			MemberIntegral memberIntegral = memberIntegralService.GetMemberIntegral(base.CurrentUser.Id);
			int num7 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
			ViewBag.userIntegrals = num7;
			ViewBag.integralPerMoney = 0;
			ViewBag.memberIntegralInfo = memberIntegral;
			MemberIntegralExchangeRules integralChangeRule = memberIntegralService.GetIntegralChangeRule();
			decimal viewBag1 = new decimal(0);
			decimal viewBag2 = new decimal(0);
			if (integralChangeRule == null || integralChangeRule.IntegralPerMoney <= 0)
			{
				viewBag1 = new decimal(0);
				viewBag2 = new decimal(0);
			}
			else
			{
				if (ViewBag.totalAmount - num6 - Math.Round((decimal)num7 / integralChangeRule.IntegralPerMoney, 2) <= 0)
				{
					viewBag1 = Math.Round(ViewBag.totalAmount - num6, 2);
					viewBag2 = Math.Round((ViewBag.totalAmount - num6) * integralChangeRule.IntegralPerMoney);
				}
				else
				{
					viewBag1 = Math.Round((decimal)num7 / integralChangeRule.IntegralPerMoney, 2);
					viewBag2 = num7;
				}
				if (viewBag1 <= new decimal(0))
				{
					viewBag1 = new decimal(0);
					viewBag2 = new decimal(0);
				}
			}
			ViewBag.integralPerMoney = viewBag1;
			ViewBag.userIntegrals = viewBag2;
		}

		private void GetOrderProductsInfo(string cartItemIds)
		{
			CartHelper cartHelper = new CartHelper();
			IEnumerable<ShoppingCartItem> cartItems = null;
			if (!string.IsNullOrWhiteSpace(cartItemIds))
			{
				char[] chrArray = new char[] { ',' };
				IEnumerable<long> nums = 
					from t in cartItemIds.Split(chrArray)
					select long.Parse(t);
				cartItems = ServiceHelper.Create<ICartService>().GetCartItems(nums);
			}
			else
			{
				cartItems = cartHelper.GetCart(base.CurrentUser.Id).Items;
			}
			int cityId = 0;
			ShippingAddressInfo defaultUserShippingAddressByUserId = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
			if (defaultUserShippingAddressByUserId != null)
			{
				cityId = Instance<IRegionService>.Create.GetCityId(defaultUserShippingAddressByUserId.RegionIdPath);
			}
			IProductService productService = ServiceHelper.Create<IProductService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			List<CartItemModel> list = cartItems.Select<ShoppingCartItem, CartItemModel>((ShoppingCartItem item) => {
				ProductInfo product = productService.GetProduct(item.ProductId);
				SKUInfo sku = productService.GetSku(item.SkuId);
				return new CartItemModel()
				{
					skuId = item.SkuId,
					id = product.Id,
					imgUrl = product.GetImage(ProductInfo.ImageSize.Size_100, 1),
					name = product.ProductName,
					price = sku.SalePrice,
					shopId = product.ShopId,
					count = item.Quantity
				};
			}).ToList();
			IEnumerable<IGrouping<long, CartItemModel>> groupings = 
				from a in list
				group a by a.shopId;
			List<ShopCartItemModel> shopCartItemModels = new List<ShopCartItemModel>();
			foreach (IGrouping<long, CartItemModel> nums1 in groupings)
			{
				IEnumerable<long> nums2 = 
					from r in nums1
					select r.id;
				IEnumerable<int> nums3 = 
					from r in nums1
					select r.count;
				ShopCartItemModel shopCartItemModel = new ShopCartItemModel()
				{
					shopId = nums1.Key
				};
				if (ServiceHelper.Create<IVShopService>().GetVShopByShopId(shopCartItemModel.shopId) != null)
				{
					shopCartItemModel.vshopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(shopCartItemModel.shopId).Id;
				}
				else
				{
					shopCartItemModel.vshopId = 0;
				}
				shopCartItemModel.CartItemModels = (
					from a in list
					where a.shopId == shopCartItemModel.shopId
					select a).ToList();
				shopCartItemModel.ShopName = shopService.GetShop(shopCartItemModel.shopId, false).ShopName;
				if (cityId > 0)
				{
					shopCartItemModel.Freight = ServiceHelper.Create<IProductService>().GetFreight(nums2, nums3, cityId);
				}
				List<ShopBonusReceiveInfo> detailToUse = ServiceHelper.Create<IShopBonusService>().GetDetailToUse(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
				List<CouponRecordInfo> userCoupon = ServiceHelper.Create<ICouponService>().GetUserCoupon(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
				if (detailToUse.Count() > 0 && userCoupon.Count() > 0)
				{
					ShopBonusReceiveInfo shopBonusReceiveInfo = detailToUse.FirstOrDefault();
					CouponRecordInfo couponRecordInfo = userCoupon.FirstOrDefault();
					decimal? price = shopBonusReceiveInfo.Price;
					decimal num = couponRecordInfo.Himall_Coupon.Price;
					if ((price.GetValueOrDefault() <= num ? true : !price.HasValue))
					{
						shopCartItemModel.Coupon = new CouponModel()
						{
							CouponName = couponRecordInfo.Himall_Coupon.CouponName,
							CouponId = couponRecordInfo.Id,
							CouponPrice = couponRecordInfo.Himall_Coupon.Price,
							Type = 0
						};
					}
					else
					{
						shopCartItemModel.Coupon = new CouponModel()
						{
							CouponName = shopBonusReceiveInfo.Himall_ShopBonusGrant.Himall_ShopBonus.Name,
							CouponId = shopBonusReceiveInfo.Id,
							CouponPrice = shopBonusReceiveInfo.Price.Value,
							Type = 1
						};
					}
				}
				else if (detailToUse.Count() <= 0 && userCoupon.Count() <= 0)
				{
					shopCartItemModel.Coupon = null;
				}
				else if (detailToUse.Count() <= 0 && userCoupon.Count() > 0)
				{
					CouponRecordInfo couponRecordInfo1 = userCoupon.FirstOrDefault();
					shopCartItemModel.Coupon = new CouponModel()
					{
						CouponName = couponRecordInfo1.Himall_Coupon.CouponName,
						CouponId = couponRecordInfo1.Id,
						CouponPrice = couponRecordInfo1.Himall_Coupon.Price,
						Type = 0
					};
				}
				else if (detailToUse.Count() > 0 && userCoupon.Count() <= 0)
				{
					ShopBonusReceiveInfo shopBonusReceiveInfo1 = detailToUse.FirstOrDefault();
					shopCartItemModel.Coupon = new CouponModel()
					{
						CouponName = shopBonusReceiveInfo1.Himall_ShopBonusGrant.Himall_ShopBonus.Name,
						CouponId = shopBonusReceiveInfo1.Id,
						CouponPrice = shopBonusReceiveInfo1.Price.Value,
						Type = 1
					};
				}
				decimal num1 = shopCartItemModel.CartItemModels.Sum<CartItemModel>((CartItemModel d) => d.price * d.count);
				decimal num2 = num1 - (shopCartItemModel.Coupon == null ? new decimal(0) : shopCartItemModel.Coupon.CouponPrice);
				decimal freeFreight = shopService.GetShop(shopCartItemModel.shopId, false).FreeFreight;
				shopCartItemModel.isFreeFreight = false;
				if (freeFreight > new decimal(0))
				{
					shopCartItemModel.shopFreeFreight = freeFreight;
					if (num2 >= freeFreight)
					{
						shopCartItemModel.Freight = new decimal(0);
						shopCartItemModel.isFreeFreight = true;
					}
				}
				shopCartItemModels.Add(shopCartItemModel);
			}
			decimal num3 = (
				from a in shopCartItemModels
				where a.Coupon != null
				select a).Sum<ShopCartItemModel>((ShopCartItemModel b) => b.Coupon.CouponPrice);
			ViewBag.products = shopCartItemModels;
			base.ViewBag.totalAmount = list.Sum<CartItemModel>((CartItemModel item) => item.price * item.count);
			base.ViewBag.Freight = shopCartItemModels.Sum<ShopCartItemModel>((ShopCartItemModel a) => a.Freight);
			dynamic viewBag = base.ViewBag;
			dynamic obj = ViewBag.totalAmount;
			viewBag.orderAmount = obj + ViewBag.Freight - num3;
			IMemberIntegralService memberIntegralService = ServiceHelper.Create<IMemberIntegralService>();
			MemberIntegral memberIntegral = memberIntegralService.GetMemberIntegral(base.CurrentUser.Id);
			int num4 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
			ViewBag.userIntegrals = num4;
			ViewBag.integralPerMoney = 0;
			ViewBag.memberIntegralInfo = memberIntegral;
			MemberIntegralExchangeRules integralChangeRule = memberIntegralService.GetIntegralChangeRule();
			decimal num5 = new decimal(0);
			decimal num6 = new decimal(0);
			decimal viewBag1 = (decimal)ViewBag.totalAmount;
			if (integralChangeRule == null || integralChangeRule.IntegralPerMoney <= 0)
			{
				num5 = new decimal(0);
				num6 = new decimal(0);
			}
			else
			{
				if (((viewBag1 - num3) - Math.Round((decimal)num4 / integralChangeRule.IntegralPerMoney, 2)) <= new decimal(0))
				{
					num5 = Math.Round(viewBag1 - num3, 2);
					num6 = Math.Round((viewBag1 - num3) * integralChangeRule.IntegralPerMoney);
				}
				else
				{
					num5 = Math.Round((decimal)num4 / integralChangeRule.IntegralPerMoney, 2);
					num6 = num4;
				}
				if (num5 <= new decimal(0))
				{
					num5 = new decimal(0);
					num6 = new decimal(0);
				}
			}
			ViewBag.integralPerMoney = num5;
			ViewBag.userIntegrals = num6;
		}

		private void GetShippingAddress()
		{
			ViewBag.address = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
		}

		[HttpPost]
		public JsonResult GetUserShippingAddresses(long addressId)
		{
			ShippingAddressInfo userShippingAddress = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(addressId);
			var variable = new { id = userShippingAddress.Id, fullRegionName = userShippingAddress.RegionFullName, address = userShippingAddress.Address, phone = userShippingAddress.Phone, shipTo = userShippingAddress.ShipTo, fullRegionIdPath = userShippingAddress.RegionIdPath };
			return Json(variable);
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult PayOrderByIntegral(string orderIds)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in orderIds.Split(chrArray)
				select long.Parse(item);
			ServiceHelper.Create<IOrderService>().ConfirmZeroOrder(nums, base.CurrentUser.Id);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult SetDefaultUserShippingAddress(long addId)
		{
			ServiceHelper.Create<IShippingAddressService>().SetDefaultShippingAddress(addId, base.CurrentUser.Id);
			return Json(new { success = true, addId = addId });
		}

		public ActionResult Submit(string skuIds, string counts)
		{
            GetShippingAddress();
			char[] chrArray = new char[] { ',' };
			IEnumerable<string> strs = 
				from item in skuIds.Split(chrArray)
				select item;
			char[] chrArray1 = new char[] { ',' };
			IEnumerable<int> nums = 
				from item in counts.Split(chrArray1)
				select int.Parse(item);
            GetOrderProductsInfo(strs, nums);
			ViewBag.sku = skuIds;
			ViewBag.count = counts;
			return View();
		}

		public ActionResult SubmiteByCart(string cartItemIds)
		{
            GetShippingAddress();
            GetOrderProductsInfo(cartItemIds);
			return View();
		}

		[HttpPost]
		public JsonResult SubmitOrder(string skuIds, string counts, long recieveAddressId, string couponIds, int integral = 0)
		{
			IEnumerable<long> nums;
			IEnumerable<string[]> strArrays;
			OrderCreateModel orderCreateModel = new OrderCreateModel();
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			IProductService productService = ServiceHelper.Create<IProductService>();
			char[] chrArray = new char[] { ',' };
			IEnumerable<string> strs = 
				from item in skuIds.Split(chrArray)
				select item.ToString();
			char[] chrArray1 = new char[] { ',' };
			IEnumerable<int> nums1 = 
				from item in counts.Split(chrArray1)
				select int.Parse(item);
			IEnumerable<string> strs1 = null;
			if (!string.IsNullOrEmpty(couponIds))
			{
				strs1 = couponIds.Split(new char[] { ',' });
			}
			OrderCreateModel orderCreateModel1 = orderCreateModel;
			if (strs1 == null)
			{
				nums = null;
			}
			else
			{
				nums = 
					from p in strs1
					select long.Parse(p.Split(new char[] { '\u005F' })[0]);
			}
			orderCreateModel1.CouponIds = nums;
			OrderCreateModel orderCreateModel2 = orderCreateModel;
			if (strs1 == null)
			{
				strArrays = null;
			}
			else
			{
				strArrays = 
					from p in strs1
					select p.Split(new char[] { '\u005F' });
			}
			orderCreateModel2.CouponIdsStr = strArrays;
			orderCreateModel.PlatformType = base.PlatformType;
			orderCreateModel.CurrentUser = base.CurrentUser;
			orderCreateModel.ReceiveAddressId = recieveAddressId;
			orderCreateModel.SkuIds = strs;
			orderCreateModel.Counts = nums1;
			orderCreateModel.Integral = integral;
			if (strs.Count() == 1)
			{
				string str = strs.ElementAt<string>(0);
				if (!string.IsNullOrWhiteSpace(str))
				{
					SKUInfo sku = productService.GetSku(str);
					bool flag = ServiceHelper.Create<ILimitTimeBuyService>().IsLimitTimeMarketItem(sku.ProductId);
					orderCreateModel.IslimitBuy = flag;
				}
			}
			List<OrderInfo> orderInfos = orderService.CreateOrder(orderCreateModel);
			IEnumerable<long> array = (
				from item in orderInfos
				select item.Id).ToArray();
			decimal num = orderInfos.Sum<OrderInfo>((OrderInfo item) => item.OrderTotalAmount);
            AddVshopBuyNumber(array);
			return Json(new { success = true, orderIds = array, realTotalIsZero = num == new decimal(0) });
		}

		[HttpPost]
		public JsonResult SubmitOrderByCart(string cartItemIds, long recieveAddressId, string couponIds, int integral = 0)
		{
			IEnumerable<long> nums;
			IEnumerable<string[]> strArrays;
			OrderCreateModel orderCreateModel = new OrderCreateModel();
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			orderCreateModel.PlatformType = base.PlatformType;
			orderCreateModel.CurrentUser = base.CurrentUser;
			orderCreateModel.ReceiveAddressId = recieveAddressId;
			orderCreateModel.Integral = integral;
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums1 = 
				from item in cartItemIds.Split(chrArray)
				select long.Parse(item);
			orderCreateModel.CartItemIds = nums1;
			IEnumerable<string> strs = null;
			if (!string.IsNullOrEmpty(couponIds))
			{
				strs = couponIds.Split(new char[] { ',' });
			}
			OrderCreateModel orderCreateModel1 = orderCreateModel;
			if (strs == null)
			{
				nums = null;
			}
			else
			{
				nums = 
					from p in strs
					select long.Parse(p.Split(new char[] { '\u005F' })[0]);
			}
			orderCreateModel1.CouponIds = nums;
			OrderCreateModel orderCreateModel2 = orderCreateModel;
			if (strs == null)
			{
				strArrays = null;
			}
			else
			{
				strArrays = 
					from p in strs
					select p.Split(new char[] { '\u005F' });
			}
			orderCreateModel2.CouponIdsStr = strArrays;
			List<OrderInfo> orderInfos = orderService.CreateOrder(orderCreateModel);
			IEnumerable<long> array = (
				from item in orderInfos
				select item.Id).ToArray();
			decimal num = orderInfos.Sum<OrderInfo>((OrderInfo item) => item.OrderTotalAmount);
			return Json(new { success = true, orderIds = array, realTotalIsZero = num == new decimal(0) });
		}
	}
}