using com.paypal.sdk.util;
using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Controllers;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
    public class OrderController : BaseMemberController
    {
        public OrderController()
        {
        }

        public JsonResult CalcFreight(long addressId, string datas)
        {
            if (string.IsNullOrEmpty(datas))
            {
                return Json(new { success = false, msg = "Shipping Calculate Failed" });
            }
            List<object[]> objArrays = new List<object[]>();
            string[] strArrays = datas.Split(new char[] { '|' });
            for (int i = 0; i < strArrays.Length; i++)
            {
                List<CartItemModel> cartItemModels = JsonConvert.DeserializeObject<List<CartItemModel>>(strArrays[i]);
                List<string> strs = new List<string>();
                List<int> nums = new List<int>();
                decimal num = new decimal(0);
                foreach (CartItemModel cartItemModel in cartItemModels)
                {
                    string str = cartItemModel.skuId;
                    if (!strs.Contains(str))
                    {
                        strs.Add(str);
                        nums.Add(cartItemModel.count);
                    }
                    else
                    {
                        int num1 = strs.IndexOf(str);
                        int item = nums[num1];
                        nums[num1] = item + cartItemModel.count;
                    }
                    num = num + (cartItemModel.count * cartItemModel.price);
                }
                ShippingAddressInfo userShippingAddress = Instance<IShippingAddressService>.Create.GetUserShippingAddress(addressId);
                int cityId = 0;
                if (userShippingAddress != null)
                {
                    cityId = Instance<IRegionService>.Create.GetCityId(userShippingAddress.RegionIdPath);
                }
                decimal freight = Instance<IProductService>.Create.GetFreight(strs, nums, cityId);
                object[] objArray = new object[] { num, freight };
                objArrays.Add(objArray);
            }
            return Json(objArrays);
        }

        public ActionResult ChargePay(string orderIds)
        {
            string str;
            if (string.IsNullOrEmpty(orderIds))
            {
                return RedirectToAction("index", "userCenter", new { url = "/UserCapital", tar = "UserCapital" });
            }
            IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
            ChargeDetailInfo chargeDetail = memberCapitalService.GetChargeDetail(long.Parse(orderIds));
            if (chargeDetail == null || chargeDetail.MemId != base.CurrentUser.Id || chargeDetail.ChargeStatus == ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess)
            {
                Log.Error(string.Concat("调用ChargePay方法时未找到充值申请记录：", orderIds));
                return RedirectToAction("index", "userCenter", new { url = "/UserCapital", tar = "UserCapital" });
            }
            SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            ViewBag.Logo = siteSettings.Logo;
            ViewBag.Orders = chargeDetail;
            string scheme = base.Request.Url.Scheme;
            string host = base.HttpContext.Request.Url.Host;
            if (base.HttpContext.Request.Url.Port == 80)
            {
                str = "";
            }
            else
            {
                int port = base.HttpContext.Request.Url.Port;
                str = string.Concat(":", port.ToString());
            }
            string str1 = string.Concat(scheme, "://", host, str);
            string str2 = string.Concat(str1, "/Pay/CapitalChargeReturn/{0}");
            string str3 = string.Concat(str1, "/Pay/CapitalChargeNotify/{0}");
            IEnumerable<Plugin<IPaymentPlugin>> plugins =
                from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
                where item.Biz.SupportPlatforms.Contains<PlatformType>(PlatformType.PC)
                select item;
            IEnumerable<PaymentModel> paymentModels = plugins.Select<Plugin<IPaymentPlugin>, PaymentModel>((Plugin<IPaymentPlugin> item) =>
            {
                string empty = string.Empty;
                try
                {
                    empty = item.Biz.GetRequestUrl(string.Format(str2, EncodePaymentId(item.PluginInfo.PluginId)), string.Format(str3, EncodePaymentId(item.PluginInfo.PluginId)), orderIds, chargeDetail.ChargeAmount, "预付款充值", null);
                }
                catch (Exception exception)
                {
                    Log.Error("支付页面加载支付插件出错", exception);
                }
                return new PaymentModel()
                {
                    Logo = string.Concat("/Plugins/Payment/", item.PluginInfo.ClassFullName.Split(new char[] { ',' })[1], "/", item.Biz.Logo),
                    RequestUrl = empty,
                    UrlType = item.Biz.RequestUrlType,
                    Id = item.PluginInfo.PluginId
                };
            });
            paymentModels =
                from item in paymentModels
                where !string.IsNullOrEmpty(item.RequestUrl)
                select item;
            ViewBag.OrderIds = orderIds;
            ViewBag.TotalAmount = chargeDetail.ChargeAmount;
            ViewBag.Step = 1;
            ViewBag.UnpaidTimeout = siteSettings.UnpaidTimeout;
            return View(paymentModels);
        }

        [HttpPost]
        public JsonResult DeleteInvoiceTitle(long id)
        {
            ServiceHelper.Create<IOrderService>().DeleteInvoiceTitle(id);
            return Json(true);
        }

        private string EncodePaymentId(string paymentId)
        {
            return paymentId.Replace(".", "-");
        }

        private void GetOrderProductsInfo(string skuIds, string counts, string collIds = null)
        {
            ShippingAddressInfo defaultUserShippingAddressByUserId = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
            int cityId = 0;
            if (defaultUserShippingAddressByUserId != null)
            {
                cityId = Instance<IRegionService>.Create.GetCityId(defaultUserShippingAddressByUserId.RegionIdPath);
            }
            IEnumerable<long> nums = null;
            string[] strArrays = skuIds.Split(new char[] { ',' });
            string str = counts.TrimEnd(new char[] { ',' });
            char[] chrArray = new char[] { ',' };
            IEnumerable<int> nums1 =
                from t in str.Split(chrArray)
                select int.Parse(t);
            if (!string.IsNullOrEmpty(collIds))
            {
                string str1 = collIds.TrimEnd(new char[] { ',' });
                char[] chrArray1 = new char[] { ',' };
                nums =
                    from t in str1.Split(chrArray1)
                    select long.Parse(t);
            }
            IProductService productService = ServiceHelper.Create<IProductService>();
            int num2 = 0;
            int length = strArrays.Length;
            List<CartItemModel> list = strArrays.Select<string, CartItemModel>((string item) =>
            {
                SKUInfo sku = productService.GetSku(item);
                int num = nums1.ElementAt<int>(num2);
                long num1 = (nums != null ? nums.ElementAt<long>(num2) : 0);
                num2++;
                return new CartItemModel()
                {
                    skuId = item,
                    id = sku.ProductInfo.Id,
                    imgUrl = sku.ProductInfo.GetImage(ProductInfo.ImageSize.Size_50, 1),
                    name = sku.ProductInfo.ProductName,
                    shopId = sku.ProductInfo.ShopId,
                    price = GetSalePrice(sku.ProductInfo.Id, sku, new long?(num1), length),
                    count = num,
                    productCode = sku.ProductInfo.ProductCode
                };
            }).ToList();
            IShopService create = Instance<IShopService>.Create;
            IEnumerable<IGrouping<long, CartItemModel>> groupings =
                from a in list
                group a by a.shopId;
            List<ShopCartItemModel> shopCartItemModels = new List<ShopCartItemModel>();
            foreach (IGrouping<long, CartItemModel> nums2 in groupings)
            {
                IEnumerable<long> nums3 =
                    from r in nums2
                    select r.id;
                IEnumerable<int> nums4 =
                    from r in nums2
                    select r.count;
                ShopCartItemModel shopCartItemModel = new ShopCartItemModel()
                {
                    shopId = nums2.Key

                };
                shopCartItemModel.CartItemModels = (
                        from a in list
                        where a.shopId == shopCartItemModel.shopId
                        select a).ToList();
                shopCartItemModel.ShopName = create.GetShop(shopCartItemModel.shopId, false).ShopName;
                if (cityId != 0)
                {
                    shopCartItemModel.Freight = ServiceHelper.Create<IProductService>().GetFreight(nums3, nums4, cityId);
                }
                shopCartItemModel.UserCoupons = ServiceHelper.Create<ICouponService>().GetUserCoupon(shopCartItemModel.shopId, base.CurrentUser.Id, nums2.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
                shopCartItemModel.UserBonus = ServiceHelper.Create<IShopBonusService>().GetDetailToUse(shopCartItemModel.shopId, base.CurrentUser.Id, nums2.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
                shopCartItemModels.Add(shopCartItemModel);
            }
            ViewBag.CollIds = collIds;
            ViewBag.products = shopCartItemModels;
            base.ViewBag.totalAmount = list.Sum<CartItemModel>((CartItemModel item) => item.price * item.count);
            base.ViewBag.Freight = shopCartItemModels.Sum<ShopCartItemModel>((ShopCartItemModel a) => a.Freight);
            dynamic viewBag = base.ViewBag;
            dynamic obj = ViewBag.totalAmount;
            viewBag.orderAmount = obj + ViewBag.Freight;
        }
        [HttpPost]
        //发送请求支付请求
        public JsonResult SetExpressCheckout(decimal amount, string currency_code, string item_name, string return_false_url, string webtype, string fq = "")
        {
            PaypalController paypal = new PaypalController();
            string hots = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port + "/";
            NVPCodec encoder = new NVPCodec();
            encoder.Add("PAYMENTACTION", "Sale");
            //不允许客户改地址
            //encoder.Add("ADDROVERRIDE", "1");
            encoder.Add("CURRENCYCODE", currency_code);
            encoder.Add("L_NAME0", item_name);
            encoder.Add("L_NUMBER0", item_name);
            encoder.Add("L_DESC0", item_name);
            encoder.Add("L_AMT0", amount.ToString());
            encoder.Add("L_QTY0", "1");
            double ft = double.Parse(amount.ToString());
            encoder.Add("AMT", ft.ToString());
            if (!string.IsNullOrEmpty(webtype))
            {
                encoder.Add("RETURNURL", hots + "/Pay/Return?orderid=" + item_name + "&price=" + amount + "&type=webzf&fq=" + fq + "&paymodel=paypal");
            }
            else
            {
                encoder.Add("RETURNURL", hots + "/Pay/Return?orderid=" + item_name + "&price=" + amount + "&type=webcz&fq=" + fq + "&paymodel=paypal");
            }
            encoder.Add("CANCELURL", return_false_url);
            NVPCodec decoder = paypal.SetExpressCheckout(encoder);
            string ack = decoder["ACK"];
            if (!string.IsNullOrEmpty(ack) && (ack.Equals("Success", System.StringComparison.OrdinalIgnoreCase) || ack.Equals("SuccessWithWarning", System.StringComparison.OrdinalIgnoreCase)))
            {
                //Session["TOKEN"] = decoder["token"];
                return Json(ConfigurationManager.AppSettings["RedirectURL"] + decoder["token"]);
            }
            else
            {
                return Json(return_false_url);
            }
        }
        private void GetOrderProductsInfo(string cartItemIds, long? regionId)
        {
            ShippingAddressInfo shippingAddressInfo = new ShippingAddressInfo();
            shippingAddressInfo = (!regionId.HasValue ? ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id) : ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(regionId.Value));
            int cityId = 0;
            if (shippingAddressInfo != null)
            {
                cityId = Instance<IRegionService>.Create.GetCityId(shippingAddressInfo.RegionIdPath);
            }
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
            IProductService productService = ServiceHelper.Create<IProductService>();
            IShopService shopService = ServiceHelper.Create<IShopService>();
            List<CartItemModel> list = cartItems.Select<ShoppingCartItem, CartItemModel>((ShoppingCartItem item) =>
            {
                ProductInfo product = productService.GetProduct(item.ProductId);
                SKUInfo sku = productService.GetSku(item.SkuId);
                return new CartItemModel()
                {
                    skuId = item.SkuId,
                    id = product.Id,
                    imgUrl = product.GetImage(ProductInfo.ImageSize.Size_50, 1),
                    name = product.ProductName,
                    price = sku.SalePrice,
                    shopId = product.ShopId,
                    count = item.Quantity,
                    productCode = product.ProductCode
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
                    shopId = nums1.Key,

                    Freight = (cityId <= 0 ? new decimal(0) : ServiceHelper.Create<IProductService>().GetFreight(nums2, nums3, cityId)),
                };

                shopCartItemModel.CartItemModels = (
                        from a in list
                        where a.shopId == shopCartItemModel.shopId
                        select a).ToList();
                shopCartItemModel.ShopName = shopService.GetShop(shopCartItemModel.shopId, false).ShopName;
                shopCartItemModel.UserCoupons = ServiceHelper.Create<ICouponService>().GetUserCoupon(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));
                shopCartItemModel.UserBonus = ServiceHelper.Create<IShopBonusService>().GetDetailToUse(shopCartItemModel.shopId, base.CurrentUser.Id, nums1.Sum<CartItemModel>((CartItemModel a) => a.price * a.count));

                shopCartItemModels.Add(shopCartItemModel);
            }
            ViewBag.products = shopCartItemModels;
            base.ViewBag.totalAmount = list.Sum<CartItemModel>((CartItemModel item) => item.price * item.count);
            base.ViewBag.Freight = shopCartItemModels.Sum<ShopCartItemModel>((ShopCartItemModel a) => a.Freight);
            dynamic viewBag = base.ViewBag;
            dynamic obj = ViewBag.totalAmount;
            viewBag.orderAmount = obj + ViewBag.Freight;
        }

        public JsonResult GetPayPwd()
        {
            if (string.IsNullOrWhiteSpace(base.CurrentUser.PayPwd))
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        private string GetProductNameDescriptionFromOrders(IEnumerable<OrderInfo> orders)
        {
            string str;
            List<string> strs = new List<string>();
            foreach (OrderInfo list in orders.ToList())
            {
                strs.AddRange(
                    from t in list.OrderItemInfo
                    select t.ProductName);
            }
            if (strs.Count() > 1)
            {
                object[] objArray = new object[] { strs.ElementAt<string>(0), "", strs.Count(), " type products" };
                str = string.Concat(objArray);
            }
            else
            {
                str = strs.ElementAt<string>(0);
            }
            return str;
        }

        private decimal GetSalePrice(long productId, SKUInfo sku, long? collid, int Count)
        {
            decimal salePrice = sku.SalePrice;
            if (collid.HasValue && collid.Value != 0 && Count > 1)
            {
                CollocationSkuInfo colloSku = ServiceHelper.Create<ICollocationService>().GetColloSku(collid.Value, sku.Id);
                if (colloSku != null)
                {
                    salePrice = colloSku.Price;
                }
            }
            else if (Count == 1)
            {
                LimitTimeMarketInfo limitTimeMarketItemByProductId = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(productId);
                if (limitTimeMarketItemByProductId != null)
                {
                    salePrice = limitTimeMarketItemByProductId.Price;
                }
            }
            return salePrice;
        }

        private void GetShippingAddress(long? regionId)
        {
            if (regionId.HasValue)
            {
                ViewBag.address = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(regionId.Value);
                return;
            }
            ViewBag.address = ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id);
        }

        [HttpPost]
        public JsonResult GetUserShippingAddresses()
        {
            ShippingAddressInfo[] array = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddressByUserId(base.CurrentUser.Id).ToArray();
            var variable =
                from item in array
                select new { id = item.Id, fullRegionName = item.RegionFullName, address = item.Address, phone = item.Phone, shipTo = item.ShipTo, fullRegionIdPath = item.RegionIdPath };
            return Json(variable);
        }

        public ActionResult Pay(string orderIds)
        {
            string str;
            if (string.IsNullOrEmpty(orderIds))
            {
                return RedirectToAction("index", "userCenter", new { url = "/userOrder", tar = "userOrder" });
            }
            char[] chrArray = new char[] { ',' };
            IEnumerable<long> nums =
                from item in orderIds.Split(chrArray)
                select long.Parse(item);
            if (ServiceHelper.Create<IOrderService>().GetOrders(nums).Any((OrderInfo item) =>
            {
                if (item.OrderStatus != OrderInfo.OrderOperateStatus.WaitPay)
                {
                    return true;
                }
                return item.UserId != base.CurrentUser.Id;
            }))
            {
                return RedirectToAction("index", "userCenter", new { url = string.Concat("/userOrder?orderids=", orderIds), tar = "userOrder" });
            }
            SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            ViewBag.Logo = siteSettings.Logo;
            IOrderService orderService = ServiceHelper.Create<IOrderService>();
            List<OrderInfo> list = orderService.GetOrders(nums).Where((OrderInfo item) =>
            {
                if (item.OrderStatus != OrderInfo.OrderOperateStatus.WaitPay)
                {
                    return false;
                }
                return item.UserId == base.CurrentUser.Id;
            }).ToList();
            bool flag = false;
            List<OrderInfo> orderInfos = new List<OrderInfo>();
            foreach (OrderInfo orderInfo in list)
            {
                orderInfo.HaveDelProduct = orderService.IsHaveNoOnSaleProduct(orderInfo.Id);
                if (!orderInfo.HaveDelProduct)
                {
                    continue;
                }
                orderInfos.Add(orderInfo);
                flag = true;
                orderService.PlatformCloseOrder(orderInfo.Id, "automatic", "You have not sell the goods, please contact the merchant or Administration");
            }
            if (flag)
            {
                foreach (OrderInfo orderInfo1 in orderInfos)
                {
                    list.Remove(orderInfo1);
                }
                throw new HimallException("有订单商品处于非销售状态，请手动处理。");
            }
            ViewBag.HaveNoSalePro = flag;
            if (list == null || list.Count == 0)
            {
                return RedirectToAction("index", "userCenter", new { url = "/userOrder", tar = "userOrder" });
            }
            ViewBag.Orders = list;
            decimal num = list.Sum<OrderInfo>((OrderInfo a) => a.OrderTotalAmount);
            if (num == new decimal(0))
            {
                ViewBag.TotalAmount = num;
                return View("PayConfirm");
            }
            string productNameDescriptionFromOrders = GetProductNameDescriptionFromOrders(list);
            string scheme = base.Request.Url.Scheme;
            string host = base.HttpContext.Request.Url.Host;
            if (base.HttpContext.Request.Url.Port == 80)
            {
                str = "";
            }
            else
            {
                int port = base.HttpContext.Request.Url.Port;
                str = string.Concat(":", port.ToString());
            }
            string str1 = string.Concat(scheme, "://", host, str);
            string str2 = string.Concat(str1, "/Pay/Return/{0}");
            string str3 = string.Concat(str1, "/Pay/Notify/{0}");
            IEnumerable<Plugin<IPaymentPlugin>> plugins =
                from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
                where item.Biz.SupportPlatforms.Contains<PlatformType>(PlatformType.PC)
                select item;
            char[] chrArray1 = new char[] { ',' };
            IEnumerable<OrderPayInfo> orderPayInfos =
                from item in orderIds.Split(chrArray1)
                select new OrderPayInfo()
                {
                    PayId = 0,
                    OrderId = long.Parse(item)
                };
            long num1 = orderService.SaveOrderPayInfo(orderPayInfos, PlatformType.PC);
            string str4 = num1.ToString();
            IEnumerable<PaymentModel> paymentModels = plugins.Select<Plugin<IPaymentPlugin>, PaymentModel>((Plugin<IPaymentPlugin> item) =>
            {
                string empty = string.Empty;
                try
                {
                    empty = item.Biz.GetRequestUrl(string.Format(str2, EncodePaymentId(item.PluginInfo.PluginId)), string.Format(str3, EncodePaymentId(item.PluginInfo.PluginId)), str4, num, productNameDescriptionFromOrders, null);
                }
                catch (Exception exception)
                {
                    Log.Error("支付页面加载支付插件出错", exception);
                }
                return new PaymentModel()
                {
                    Logo = string.Concat("/Plugins/Payment/", item.PluginInfo.ClassFullName.Split(new char[] { ',' })[1], "/", item.Biz.Logo),
                    RequestUrl = empty,
                    UrlType = item.Biz.RequestUrlType,
                    Id = item.PluginInfo.PluginId
                };
            });
            paymentModels =
                from item in paymentModels
                where !string.IsNullOrEmpty(item.RequestUrl)
                select item;
            ViewBag.OrderIds = orderIds;
            ViewBag.TotalAmount = num;
            ViewBag.Step = 1;
            ViewBag.UnpaidTimeout = siteSettings.UnpaidTimeout;
            CapitalInfo capitalInfo = ServiceHelper.Create<IMemberCapitalService>().GetCapitalInfo(base.CurrentUser.Id);
            if (capitalInfo != null)
            {
                ViewBag.Capital = capitalInfo.Balance;
            }
            else
            {
                ViewBag.Capital = 0;
            }
            ViewBag.PayId = num1;
            return View(paymentModels);
        }

        public JsonResult PayByCapital(string orderIds, string pwd, string payid)
        {
            if (ServiceHelper.Create<IMemberCapitalService>().GetMemberInfoByPayPwd(base.CurrentUser.Id, pwd) == null)
            {
                throw new HimallException("Payment Password is incorrect");
            }
            char[] chrArray = new char[] { ',' };
            IEnumerable<long> nums =
                from e in orderIds.Split(chrArray)
                select long.Parse(e);
            ServiceHelper.Create<IOrderService>().PayCapital(nums, null, long.Parse(payid));
            Dictionary<long, ShopBonusInfo> nums1 = new Dictionary<long, ShopBonusInfo>();
            string str = string.Concat("http://", base.Request.Url.Host.ToString(), "/m-weixin/shopbonus/index/");
            IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
            foreach (OrderInfo order in ServiceHelper.Create<IOrderService>().GetOrders(nums))
            {
                Log.Info(string.Concat("ShopID = ", order.ShopId));
                ShopBonusInfo byShopId = shopBonusService.GetByShopId(order.ShopId);
                if (byShopId == null)
                {
                    continue;
                }
                Log.Info(string.Concat("商家活动价格：", byShopId.GrantPrice));
                Log.Info(string.Concat("买家支付价格：", order.OrderTotalAmount));
                if (byShopId.GrantPrice > order.OrderTotalAmount)
                {
                    continue;
                }
                long num = shopBonusService.GenerateBonusDetail(byShopId, base.CurrentUser.Id, order.Id, str);
                Log.Info(string.Concat("生成红包组，红包Grantid = ", num));
                nums1.Add(num, byShopId);
            }
            return Json(new { success = true, msg = "Payment Success" });
        }

        [HttpPost]
        public ActionResult PayConfirm(string orderIds)
        {
            if (string.IsNullOrEmpty(orderIds))
            {
                return RedirectToAction("index","userCenter", new { url = "/userOrder", tar = "userOrder" });
            }
            char[] chrArray = new char[] { ',' };
            IEnumerable<long> nums =
                from item in orderIds.Split(chrArray)
                select long.Parse(item);
            ServiceHelper.Create<IOrderService>().ConfirmZeroOrder(nums, base.CurrentUser.Id);
            return RedirectToAction("ReturnSuccess", "pay", new { id = orderIds });
        }

        [HttpPost]
        public JsonResult SaveInvoiceTitle(string name)
        {
            InvoiceTitleInfo invoiceTitleInfo = new InvoiceTitleInfo()
            {
                Name = name,
                UserId = base.CurrentUser.Id,
                IsDefault = 0
            };
            long num = ServiceHelper.Create<IOrderService>().SaveInvoiceTitle(invoiceTitleInfo);
            return Json(num);
        }

        public ActionResult Submit(string cartItemIds, long? regionId)
        {
            int num;
            int num1;
            dynamic obj;
            IOrderService orderService = ServiceHelper.Create<IOrderService>();
            IMemberIntegralService memberIntegralService = ServiceHelper.Create<IMemberIntegralService>();
            MemberIntegralExchangeRules integralChangeRule = memberIntegralService.GetIntegralChangeRule();
            MemberIntegral memberIntegral = memberIntegralService.GetMemberIntegral(base.CurrentUser.Id);
            int num2 = (integralChangeRule == null ? 0 : integralChangeRule.MoneyPerIntegral);
            dynamic viewBag = base.ViewBag;
            num = (integralChangeRule == null ? 0 : integralChangeRule.IntegralPerMoney);
            viewBag.IntegralPerMoney = num;
            dynamic viewBag1 = base.ViewBag;
            num1 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
            viewBag1.Integral = num1;
            ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
            ViewBag.Step = 2;
            ViewBag.Member = base.CurrentUser;
            ViewBag.cartItemIds = cartItemIds;
            GetOrderProductsInfo(cartItemIds, regionId);
            dynamic obj1 = base.ViewBag;
            obj = (num2 == 0 ? 0 : Math.Floor(ViewBag.totalAmount / num2));
            obj1.TotalIntegral = obj;
            ViewBag.MoneyPerIntegral = num2;
            GetShippingAddress(regionId);
            ViewBag.InvoiceTitle = orderService.GetInvoiceTitles(base.CurrentUser.Id);
            ViewBag.InvoiceContext = orderService.GetInvoiceContexts();
            return View();
        }

        public ActionResult SubmitByProductId(string skuIds, string counts, long? regionId, string collpids = null)
        {
            int num;
            int num1;
            int num2;
            dynamic obj;
            IOrderService orderService = ServiceHelper.Create<IOrderService>();
            ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
            ViewBag.Member = base.CurrentUser;
            GetOrderProductsInfo(skuIds, counts, collpids);
            GetShippingAddress(regionId);
            IMemberIntegralService memberIntegralService = ServiceHelper.Create<IMemberIntegralService>();
            MemberIntegralExchangeRules integralChangeRule = memberIntegralService.GetIntegralChangeRule();
            MemberIntegral memberIntegral = memberIntegralService.GetMemberIntegral(base.CurrentUser.Id);
            dynamic viewBag = base.ViewBag;
            num = (integralChangeRule == null ? 0 : integralChangeRule.IntegralPerMoney);
            viewBag.IntegralPerMoney = num;
            dynamic viewBag1 = base.ViewBag;
            num1 = (integralChangeRule == null ? 0 : integralChangeRule.MoneyPerIntegral);
            viewBag1.MoneyPerIntegral = num1;
            dynamic obj1 = base.ViewBag;
            num2 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
            obj1.Integral = num2;
            dynamic viewBag2 = base.ViewBag;
            obj = (ViewBag.MoneyPerIntegral == 0 ? 0 : Math.Floor(ViewBag.totalAmount / ViewBag.MoneyPerIntegral));
            viewBag2.TotalIntegral = obj;
            ViewBag.collIds = collpids;
            ViewBag.skuIds = skuIds;
            ViewBag.counts = counts;
            ViewBag.InvoiceTitle = orderService.GetInvoiceTitles(base.CurrentUser.Id);
            ViewBag.InvoiceContext = orderService.GetInvoiceContexts();
            return View("Submit");
        }

        [HttpPost]
        public JsonResult SubmitOrder(string cartItemIds, long recieveAddressId, string couponIds, int invoiceType, string invoiceTitle, string invoiceContext, int integral = 0)
        {
            IEnumerable<long> items;
            IEnumerable<long> nums;
            IEnumerable<string[]> strArrays;
            IOrderService orderService = ServiceHelper.Create<IOrderService>();
            if (!string.IsNullOrWhiteSpace(cartItemIds))
            {
                char[] chrArray = new char[] { ',' };
                items =
                    from item in cartItemIds.Split(chrArray)
                    select long.Parse(item);
            }
            else
            {
                ShoppingCartInfo cart = ServiceHelper.Create<ICartService>().GetCart(base.CurrentUser.Id);
                items =
                    from item in cart.Items
                    select item.Id;
            }
            IEnumerable<string> strs = null;
            if (!string.IsNullOrEmpty(couponIds))
            {
                strs = couponIds.Split(new char[] { ',' });
            }
            if (integral < 0)
            {
                throw new HimallException("Points number is incorrect");
            }
            OrderCreateModel orderCreateModel = new OrderCreateModel()
            {
                CartItemIds = items
            };
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
            orderCreateModel.CurrentUser = base.CurrentUser;
            orderCreateModel.Integral = integral;
            orderCreateModel.Invoice = (InvoiceType)invoiceType;
            orderCreateModel.InvoiceTitle = invoiceTitle;
            orderCreateModel.InvoiceContext = invoiceContext;
            orderCreateModel.ReceiveAddressId = recieveAddressId;
            List<OrderInfo> orderInfos = orderService.CreateOrder(orderCreateModel);
            bool flag = false;
            if (orderInfos.Sum<OrderInfo>((OrderInfo a) => a.OrderTotalAmount) == new decimal(0))
            {
                flag = true;
            }
            IEnumerable<long> array = (
                from item in orderInfos
                select item.Id).ToArray();
            return Json(new { success = true, orderIds = array, redirect = flag });
        }

        [HttpPost]
        public JsonResult SubmitOrderByProductId(string skuIds, string counts, long recieveAddressId, string couponIds, int invoiceType, string invoiceTitle, string invoiceContext, int integral = 0, string collIds = "")
        {
            IEnumerable<long> nums;
            IEnumerable<string[]> strArrays;
            string[] strArrays1 = skuIds.Split(new char[] { ',' });
            string str = counts.TrimEnd(new char[] { ',' });
            char[] chrArray = new char[] { ',' };
            IEnumerable<int> nums1 =
                from t in str.Split(chrArray)
                select int.Parse(t);
            IProductService productService = ServiceHelper.Create<IProductService>();
            IOrderService orderService = ServiceHelper.Create<IOrderService>();
            if (integral < 0)
            {
                throw new HimallException("Points number is incorrect");
            }
            IEnumerable<string> strs = null;
            if (!string.IsNullOrEmpty(couponIds))
            {
                strs = couponIds.Split(new char[] { ',' });
            }
            IEnumerable<long> nums2 = null;
            if (!string.IsNullOrEmpty(collIds))
            {
                char[] chrArray1 = new char[] { ',' };
                nums2 =
                    from item in collIds.Split(chrArray1)
                    select long.Parse(item);
            }
            if (string.IsNullOrWhiteSpace(skuIds) || string.IsNullOrWhiteSpace(counts))
            {
                throw new HimallException("SKU is null or qty=0");
            }
            OrderCreateModel orderCreateModel = new OrderCreateModel()
            {
                SkuIds = strArrays1,
                Counts = nums1,
                CurrentUser = base.CurrentUser,
                Integral = integral
            };
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
            orderCreateModel.Invoice = (InvoiceType)invoiceType;
            orderCreateModel.InvoiceTitle = invoiceTitle;
            orderCreateModel.InvoiceContext = invoiceContext;
            orderCreateModel.CollPids = nums2;
            if (strArrays1.Count() == 1)
            {
                string str1 = strArrays1.ElementAt<string>(0);
                if (!string.IsNullOrWhiteSpace(str1))
                {
                    SKUInfo sku = productService.GetSku(str1);
                    bool flag = ServiceHelper.Create<ILimitTimeBuyService>().IsLimitTimeMarketItem(sku.ProductId);
                    orderCreateModel.IslimitBuy = flag;
                }
            }
            orderCreateModel.ReceiveAddressId = recieveAddressId;
            IEnumerable<long> array = (
                from item in orderService.CreateOrder(orderCreateModel)
                select item.Id).ToArray();
            return Json(new { success = true, orderIds = array });
        }
    }
}