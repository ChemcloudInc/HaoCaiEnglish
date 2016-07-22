using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class MemberController : BaseMobileMemberController
	{
		public MemberController()
		{
		}

		public ActionResult AccountManagement()
		{
			return View(base.CurrentUser);
		}

		[HttpPost]
		public JsonResult AddShippingAddress(ShippingAddressInfo info)
		{
			info.UserId = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().AddShippingAddress(info);
			return Json(new { success = true, msg = "添加成功", id = info.Id });
		}

		public ActionResult Center()
		{
			decimal num;
			IQueryable<OrderInfo> topOrders = ServiceHelper.Create<IOrderService>().GetTopOrders(2147483647, base.CurrentUser.Id);
			OrderQuery orderQuery = new OrderQuery()
			{
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish),
				UserId = new long?(base.CurrentUser.Id),
				PageSize = 2147483647,
				PageNo = 1,
				Commented = new bool?(false)
			};
			OrderQuery orderQuery1 = orderQuery;
			ViewBag.WaitingForComments = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery1, null).Total;
			UserMemberInfo member = ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id);
			ViewBag.AllOrders = topOrders.Count();
			base.ViewBag.WaitingForRecieve = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 3);
			base.ViewBag.WaitingForPay = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 1);
			CapitalInfo capitalInfo = ServiceHelper.Create<IMemberCapitalService>().GetCapitalInfo(base.CurrentUser.Id);
			if (capitalInfo == null)
			{
				ViewBag.Capital = 0;
			}
			else
			{
				dynamic viewBag = base.ViewBag;
				num = (capitalInfo.Balance.HasValue ? capitalInfo.Balance.Value : new decimal(0));
				viewBag.Capital = num;
			}
			int num1 = ServiceHelper.Create<ICouponService>().GetAllUserCoupon(base.CurrentUser.Id).Count();
			num1 = num1 + ServiceHelper.Create<IShopBonusService>().GetCanUseDetailByUserId(base.CurrentUser.Id).Count();
			ViewBag.GradeName = base.CurrentUser.MemberGradeName;
			ViewBag.CouponsCount = num1;
			ViewBag.CollectionShop = ServiceHelper.Create<IVShopService>().GetUserConcernVShops(base.CurrentUser.Id, 1, 2147483647).Count();
			return View(member);
		}

		[HttpPost]
		public JsonResult ChangePassword(string oldpassword, string password)
		{
			if (string.IsNullOrWhiteSpace(oldpassword) || string.IsNullOrWhiteSpace(password))
			{
				Result result = new Result()
				{
					success = false,
					msg = "密码不能为空！"
				};
				return Json(result);
			}
			UserMemberInfo currentUser = base.CurrentUser;
			string str = SecureHelper.MD5(string.Concat(SecureHelper.MD5(oldpassword), currentUser.PasswordSalt));
			bool flag = false;
			if (str == currentUser.Password)
			{
				flag = true;
			}
			if (currentUser.PasswordSalt.StartsWith("o"))
			{
				flag = true;
			}
			if (!flag)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "旧密码错误"
				};
				return Json(result1);
			}
			ServiceHelper.Create<IMemberService>().ChangePassWord(currentUser.Id, password);
			Result result2 = new Result()
			{
				success = true,
				msg = "修改成功"
			};
			return Json(result2);
		}

		public ActionResult CollectionProduct()
		{
			return View();
		}

		public ActionResult CollectionShop()
		{
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			return View();
		}

		[HttpPost]
		public JsonResult DeleteShippingAddress(long id)
		{
			long num = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().DeleteShippingAddress(id, num);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult EditShippingAddress(ShippingAddressInfo info)
		{
			info.UserId = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().UpdateShippingAddress(info);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功"
			};
			return Json(result);
		}

		public JsonResult GetUserCollectionProduct(int pageNo, int pageSize = 16)
		{
			PageModel<FavoriteInfo> userConcernProducts = ServiceHelper.Create<IProductService>().GetUserConcernProducts(base.CurrentUser.Id, pageNo, pageSize);
			var array = 
				from item in userConcernProducts.Models.ToArray()
                select new { Id = item.ProductId, Image = item.ProductInfo.GetImage(ProductInfo.ImageSize.Size_220, 1), ProductName = item.ProductInfo.ProductName, SalePrice = item.ProductInfo.MinSalePrice.ToString("F2"), Evaluation = item.ProductInfo.Himall_ProductComments.Count() };
			return Json(array);
		}

		public JsonResult GetUserCollectionShop(int pageNo, int pageSize = 8)
		{
			IQueryable<VShopInfo> userConcernVShops = ServiceHelper.Create<IVShopService>().GetUserConcernVShops(base.CurrentUser.Id, pageNo, pageSize);
			var array = 
				from item in userConcernVShops.ToArray()
                select new { Id = item.Id, Logo = item.Logo, Name = item.Name };
			return Json(array);
		}

		public JsonResult GetUserOrders(int? orderStatus, int pageNo, int pageSize = 8)
		{
			OrderInfo.OrderOperateStatus? nullable;
			if (orderStatus.HasValue)
			{
				int? nullable1 = orderStatus;
				if ((nullable1.GetValueOrDefault() != 0 ? false : nullable1.HasValue))
				{
					orderStatus = null;
				}
			}
			OrderQuery orderQuery = new OrderQuery();
			OrderQuery orderQuery1 = orderQuery;
			int? nullable2 = orderStatus;
			if (nullable2.HasValue)
			{
				nullable = new OrderInfo.OrderOperateStatus?((OrderInfo.OrderOperateStatus)nullable2.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			orderQuery1.Status = nullable;
			orderQuery.UserId = new long?(base.CurrentUser.Id);
			orderQuery.PageSize = pageSize;
			orderQuery.PageNo = pageNo;
			OrderQuery orderQuery2 = orderQuery;
			if (orderStatus.GetValueOrDefault() == 5)
			{
				orderQuery2.Commented = new bool?(false);
			}
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery2, null);
			IProductService productService = ServiceHelper.Create<IProductService>();
			var array = 
				from item in orders.Models.ToArray()
				select new { id = item.Id, status = item.OrderStatus.ToDescription(), orderStatus = item.OrderStatus, shopname = item.ShopName, orderTotalAmount = item.OrderTotalAmount.ToString("F2"), productCount = item.OrderProductQuantity, commentCount = item.OrderCommentInfo.Count(), itemInfo = item.OrderItemInfo.Select((OrderItemInfo a) => {
					ProductInfo product = productService.GetProduct(a.ProductId);
					return new { productId = a.ProductId, productName = a.ProductName, image = a.ThumbnailsUrl, count = a.Quantity, price = a.SalePrice, Unit = (product == null ? "" : product.MeasureUnit) };
				}) };
			return Json(array);
		}

		public ActionResult Orders(int? orderStatus)
		{
			IQueryable<OrderInfo> topOrders = ServiceHelper.Create<IOrderService>().GetTopOrders(2147483647, base.CurrentUser.Id);
			OrderQuery orderQuery = new OrderQuery()
			{
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish),
				UserId = new long?(base.CurrentUser.Id),
				PageSize = 2147483647,
				PageNo = 1,
				Commented = new bool?(false)
			};
			OrderQuery orderQuery1 = orderQuery;
			ViewBag.WaitingForComments = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery1, null).Total;
			ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id);
			ViewBag.AllOrders = topOrders.Count();
			base.ViewBag.WaitingForRecieve = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 3);
			base.ViewBag.WaitingForPay = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 1);
			return View();
		}

		public ActionResult PaymentToOrders(string ids)
		{
			Log.Info(string.Concat("ids = ", ids));
			Dictionary<long, ShopBonusInfo> nums = new Dictionary<long, ShopBonusInfo>();
			string str = string.Concat("http://", base.Request.Url.Host.ToString(), "/m-weixin/shopbonus/index/");
			if (!string.IsNullOrEmpty(ids))
			{
				string[] strArrays = ids.Split(new char[] { ',' });
				List<long> nums1 = new List<long>();
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					nums1.Add(long.Parse(strArrays1[i]));
				}
				IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
				foreach (OrderInfo order in ServiceHelper.Create<IOrderService>().GetOrders(nums1.AsEnumerable<long>()))
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
					nums.Add(num, byShopId);
				}
			}
			ViewBag.Path = str;
			ViewBag.BonusGrantIds = nums;
			ViewBag.BaseAddress = string.Concat("http://", base.Request.Url.Host.ToString());
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
			string ticket = wXApiService.GetTicket(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret);
			JSSDKHelper jSSDKHelper = new JSSDKHelper();
			string timestamp = JSSDKHelper.GetTimestamp();
			string noncestr = JSSDKHelper.GetNoncestr();
			string signature = jSSDKHelper.GetSignature(ticket, noncestr, timestamp, base.Request.Url.AbsoluteUri);
			ViewBag.Timestamp = timestamp;
			ViewBag.NonceStr = noncestr;
			ViewBag.Signature = signature;
			ViewBag.AppId = siteSettings.WeixinAppId;
			IQueryable<OrderInfo> topOrders = ServiceHelper.Create<IOrderService>().GetTopOrders(2147483647, base.CurrentUser.Id);
			OrderQuery orderQuery = new OrderQuery()
			{
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish),
				UserId = new long?(base.CurrentUser.Id),
				PageSize = 2147483647,
				PageNo = 1,
				Commented = new bool?(false)
			};
			OrderQuery orderQuery1 = orderQuery;
			ViewBag.WaitingForComments = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery1, null).Total;
			ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id);
			ViewBag.AllOrders = topOrders.Count();
			base.ViewBag.WaitingForRecieve = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 3);
			base.ViewBag.WaitingForPay = topOrders.Count((OrderInfo item) => (int)item.OrderStatus == 1);
			return View("~/Areas/Mobile/Templates/Default/Views/Member/Orders.cshtml");
		}

		public ActionResult ShippingAddress()
		{
			return View();
		}
	}
}