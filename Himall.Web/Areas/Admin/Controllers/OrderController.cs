using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class OrderController : BaseAdminController
	{
		public OrderController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult CloseOrder(long orderId)
		{
			Result result = new Result();
			ServiceHelper.Create<IOrderService>().PlatformCloseOrder(orderId, base.CurrentManager.UserName, "");
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ConfirmPay(long orderId, string payRemark)
		{
			Result result = new Result();
			ServiceHelper.Create<IOrderService>().PlatformConfirmOrderPay(orderId, payRemark, base.CurrentManager.UserName);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		public ActionResult DeleteInvoiceContexts(long id)
		{
			ServiceHelper.Create<IOrderService>().DeleteInvoiceContext(id);
			return Json(true);
		}

		public ActionResult Detail(long id)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(id);
			ViewBag.Coupon = 0;
			return View(order);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetExpressData(string expressCompanyName, string shipOrderNumber)
		{
			string end = "暂时没有此快递单号的信息";
			if (string.IsNullOrEmpty(expressCompanyName) || string.IsNullOrEmpty(shipOrderNumber))
			{
				return Json(end);
			}
			string kuaidi100Code = ServiceHelper.Create<IExpressService>().GetExpress(expressCompanyName).Kuaidi100Code;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.kuaidi100.com/query?type={0}&postid={1}", kuaidi100Code, shipOrderNumber));
			httpWebRequest.Timeout = 8000;
			HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
				end = streamReader.ReadToEnd();
				end = end.Replace("&amp;", "");
				end = end.Replace("&nbsp;", "");
				end = end.Replace("&", "");
			}
			return Json(end);
		}

		private string GetIconSrc(PlatformType platform)
		{
			if (platform == PlatformType.IOS || platform == PlatformType.Android)
			{
				return "/images/app.png";
			}
			return string.Format("/images/{0}.png", platform.ToString());
		}

		[HttpPost]
		public JsonResult GetInvoiceContexts()
		{
			List<InvoiceContextInfo> list = ServiceHelper.Create<IOrderService>().GetInvoiceContexts().ToList();
			return Json(new { rows = list, total = list.Count() });
		}

		public ActionResult InvoiceContext()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(DateTime? startDate, DateTime? endDate, long? orderId, int? orderStatus, string shopName, string userName, string paymentTypeGateway, int page, int rows)
		{
			OrderInfo.OrderOperateStatus? nullable;
			OrderQuery orderQuery = new OrderQuery()
			{
				StartDate = startDate,
				EndDate = endDate,
				OrderId = orderId
			};
			OrderQuery orderQuery1 = orderQuery;
			int? nullable1 = orderStatus;
			if (nullable1.HasValue)
			{
				nullable = new OrderInfo.OrderOperateStatus?((OrderInfo.OrderOperateStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			orderQuery1.Status = nullable;
			orderQuery.ShopName = shopName;
			orderQuery.UserName = userName;
			orderQuery.PaymentTypeGateway = paymentTypeGateway;
			orderQuery.PageSize = rows;
			orderQuery.PageNo = page;
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
			IEnumerable<OrderModel> array = 
				from item in orders.Models.ToArray()
				select new OrderModel()
				{
					OrderId = item.Id,
					OrderStatus = item.OrderStatus.ToDescription(),
					OrderDate = item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
					ShopId = item.ShopId,
					ShopName = item.ShopName,
					UserId = item.UserId,
					UserName = item.UserName,
					TotalPrice = item.OrderTotalAmount,
					PaymentTypeName = item.PaymentTypeName,
					PlatForm = (int)item.Platform,
					IconSrc = GetIconSrc(item.Platform),
					PlatformText = item.Platform.ToDescription()
				};
			DataGridModel<OrderModel> dataGridModel = new DataGridModel<OrderModel>()
			{
				rows = array,
				total = orders.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			IEnumerable<PluginInfo> plugins = 
				from t in PluginsManagement.GetPlugins<IPaymentPlugin>()
				select t.PluginInfo;
			return View(plugins);
		}

		[HttpPost]
		public ActionResult SaveInvoiceContext(string name, long id = -1L)
		{
			InvoiceContextInfo invoiceContextInfo = new InvoiceContextInfo()
			{
				Id = id,
				Name = name
			};
			ServiceHelper.Create<IOrderService>().SaveInvoiceContext(invoiceContextInfo);
			return Json(true);
		}
	}
}