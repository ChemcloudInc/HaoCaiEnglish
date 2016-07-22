using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class PaymentController : BaseMobileTemplatesController
	{
		public PaymentController()
		{
		}

		private string DecodePaymentId(string paymentId)
		{
			return paymentId.Replace("-", ".");
		}

		public JsonResult Get(string orderIds)
		{
			string str;
			IEnumerable<Plugin<IPaymentPlugin>> plugins = 
				from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
				where item.Biz.SupportPlatforms.Contains<Himall.Core.PlatformType>(base.PlatformType)
				select item;
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
			object[] platformType = new object[] { str1, "/m-", base.PlatformType, "/Payment/" };
			string str2 = string.Concat(platformType);
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			IOrderService orderService1 = orderService;
			char[] chrArray = new char[] { ',' };
			IEnumerable<OrderInfo> list = orderService1.GetOrders(
				from t in orderIds.Split(chrArray)
				select long.Parse(t)).ToList();
			decimal num = list.Sum<OrderInfo>((OrderInfo t) => t.OrderTotalAmount);
			string productNameDescriptionFromOrders = GetProductNameDescriptionFromOrders(list);
			string cookie = WebHelper.GetCookie("Himall-User_OpenId");
			if (string.IsNullOrWhiteSpace(cookie))
			{
				MemberOpenIdInfo memberOpenIdInfo = ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id).MemberOpenIdInfo.FirstOrDefault((MemberOpenIdInfo item) => item.AppIdType == MemberOpenIdInfo.AppIdTypeEnum.Payment);
				if (memberOpenIdInfo != null)
				{
					cookie = memberOpenIdInfo.OpenId;
				}
			}
			else
			{
				cookie = SecureHelper.AESDecrypt(cookie, "Mobile");
			}
			string[] strArrays = orderIds.Split(new char[] { ',' });
			string str3 = string.Concat(str2, "Notify/");
			object[] objArray = new object[] { str1, "/m-", base.PlatformType, "/Member/PaymentToOrders?ids=", orderIds };
			string str4 = string.Concat(objArray);
			IEnumerable<OrderPayInfo> orderPayInfo = 
				from item in strArrays
				select new OrderPayInfo()
				{
					PayId = 0,
					OrderId = long.Parse(item)
				};
			string str5 = orderService.SaveOrderPayInfo(orderPayInfo, Himall.Core.PlatformType.PC).ToString();
			var collection = plugins.ToArray<Plugin<IPaymentPlugin>>().Select((Plugin<IPaymentPlugin> item) => {
				string empty = string.Empty;
				try
				{
					empty = item.Biz.GetRequestUrl(str4, string.Concat(str3, item.PluginInfo.PluginId.Replace(".", "-")), str5, num, productNameDescriptionFromOrders, cookie);
				}
				catch (Exception exception)
				{
					Log.Error("获取支付方式错误：", exception);
				}
				return new { id = item.PluginInfo.PluginId, name = item.PluginInfo.DisplayName, logo = item.Biz.Logo, url = empty };
			});
			collection = 
				from item in collection
				where !string.IsNullOrWhiteSpace(item.url)
				select item;
			return Json(collection);
		}

		private string GetProductNameDescriptionFromOrders(IEnumerable<OrderInfo> orders)
		{
			string str;
			List<string> strs = new List<string>();
			foreach (OrderInfo order in orders)
			{
				strs.AddRange(
					from t in order.OrderItemInfo
					select t.ProductName);
			}
			if (strs.Count() > 1)
			{
				object[] objArray = new object[] { strs.ElementAt<string>(0), " 等", strs.Count(), "种商品" };
				str = string.Concat(objArray);
			}
			else
			{
				str = strs.ElementAt<string>(0);
			}
			return str;
		}

		[ValidateInput(false)]
		public ContentResult Notify(string id)
		{
			id = DecodePaymentId(id);
			string empty = string.Empty;
			string str = string.Empty;
			try
			{
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				PaymentInfo paymentInfo = plugin.Biz.ProcessNotify(base.HttpContext.Request);
				if (paymentInfo != null)
				{
					long num = paymentInfo.OrderIds.FirstOrDefault();
					List<long> list = (
						from item in ServiceHelper.Create<IOrderService>().GetOrderPay(num)
						select item.OrderId).ToList();
					DateTime? tradeTime = paymentInfo.TradeTime;
					IOrderService orderService = ServiceHelper.Create<IOrderService>();
					DateTime? nullable = paymentInfo.TradeTime;
					orderService.PaySucceed(list, id, nullable.Value, paymentInfo.TradNo, num);
					string str1 = CacheKeyCollection.PaymentState(string.Join<long>(",", list));
					Cache.Insert(str1, true, 15);
					str = plugin.Biz.ConfirmPayResult();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string message = exception.Message;
				Log.Error(string.Concat("移动端支付异步通知返回出错，支持方式：", id), exception);
			}
			return base.Content(str);
		}

		public ActionResult Return(string id)
		{
			id = DecodePaymentId(id);
			string empty = string.Empty;
			try
			{
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				PaymentInfo paymentInfo = plugin.Biz.ProcessReturn(base.HttpContext.Request);
				if (paymentInfo != null)
				{
					DateTime? tradeTime = paymentInfo.TradeTime;
					long num = paymentInfo.OrderIds.FirstOrDefault();
					List<long> list = (
						from item in ServiceHelper.Create<IOrderService>().GetOrderPay(num)
						select item.OrderId).ToList();
					ViewBag.OrderIds = string.Join<long>(",", list);
					IOrderService orderService = ServiceHelper.Create<IOrderService>();
					DateTime? nullable = paymentInfo.TradeTime;
					orderService.PaySucceed(list, id, nullable.Value, paymentInfo.TradNo, num);
					string str = CacheKeyCollection.PaymentState(string.Join<long>(",", list));
					Cache.Insert(str, true, 15);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				empty = exception.Message;
				Log.Error(string.Concat("移动端同步返回出错，支持方式：", id), exception);
			}
			ViewBag.Error = empty;
			return View();
		}
	}
}