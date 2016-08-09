using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class OrderRefundController : BaseSellerController
	{
		public OrderRefundController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ConfirmRefundGood(long refundId)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IRefundService>().SellerConfirmRefundGood(refundId, base.CurrentSellerManager.UserName);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DealRefund(long refundId, int auditStatus, string sellerRemark)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IRefundService>().SellerDealRefund(refundId, (OrderRefundInfo.OrderRefundAuditStatus)auditStatus, sellerRemark, base.CurrentSellerManager.UserName);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		public static string HTMLEncode(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return string.Empty;
			}
			string str = txt.Replace(" ", "&nbsp;");
			str = str.Replace("<", "&lt;");
			str = str.Replace(">", "&gt;");
			str = str.Replace("\"", "&quot;");
			return str.Replace("'", "&#39;").Replace("\n", "<br>");
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(DateTime? startDate, DateTime? endDate, long? orderId, int? auditStatus, string userName, string ProductName, int page, int rows, int showtype = 0)
		{
			OrderRefundInfo.OrderRefundAuditStatus? nullable;
			RefundQuery refundQuery = new RefundQuery()
			{
				StartDate = startDate,
				EndDate = endDate,
				OrderId = orderId,
				ProductName = ProductName
			};
			RefundQuery refundQuery1 = refundQuery;
			int? nullable1 = auditStatus;
			if (nullable1.HasValue)
			{
				nullable = new OrderRefundInfo.OrderRefundAuditStatus?((OrderRefundInfo.OrderRefundAuditStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			refundQuery1.AuditStatus = nullable;
			refundQuery.ShopId = new long?(base.CurrentSellerManager.ShopId);
			refundQuery.UserName = userName;
			refundQuery.PageSize = rows;
			refundQuery.PageNo = page;
			refundQuery.ShowRefundType = new int?(showtype);
			PageModel<OrderRefundInfo> orderRefunds = ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery);
			IEnumerable<OrderRefundModel> orderRefundModels = ((IEnumerable<OrderRefundInfo>)orderRefunds.Models.ToArray()).Select<OrderRefundInfo, OrderRefundModel>((OrderRefundInfo item) => {
				string str = string.Concat((string.IsNullOrWhiteSpace(item.OrderItemInfo.Color) ? "" : string.Concat(item.OrderItemInfo.Color, "，")), (string.IsNullOrWhiteSpace(item.OrderItemInfo.Size) ? "" : string.Concat(item.OrderItemInfo.Size, "，")), (string.IsNullOrWhiteSpace(item.OrderItemInfo.Version) ? "" : string.Concat(item.OrderItemInfo.Version, "，"))).TrimEnd(new char[] { '，' });
				if (!string.IsNullOrWhiteSpace(str))
				{
					str = string.Concat("  【", str, " 】");
				}
				return new OrderRefundModel()
				{
					RefundId = item.Id,
					OrderId = item.OrderId,
					AuditStatus = item.SellerAuditStatus.ToDescription(),
					ConfirmStatus = item.ManagerConfirmStatus.ToDescription(),
					ApplyDate = item.ApplyDate.ToShortDateString(),
					ShopId = item.ShopId,
					ShopName = item.ShopName.Replace("'", "‘").Replace("\"", "”"),
					UserId = item.UserId,
					UserName = item.Applicant,
					ContactPerson = OrderRefundController.HTMLEncode(item.ContactPerson),
					ContactCellPhone = OrderRefundController.HTMLEncode(item.ContactCellPhone),
					RefundAccount = (string.IsNullOrEmpty(item.RefundAccount) ? string.Empty : OrderRefundController.HTMLEncode(item.RefundAccount.Replace("'", "‘").Replace("\"", "”"))),
					Amount = item.Amount.ToString("F2"),
					ReturnQuantity = (item.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund ? 0 : item.OrderItemInfo.ReturnQuantity),
					Quantity = item.OrderItemInfo.Quantity,
					SalePrice = item.EnabledRefundAmount.ToString("F2"),
					ProductName = (item.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund ? "订单所有商品" : string.Concat(item.OrderItemInfo.ProductName, str)),
					Reason = (string.IsNullOrEmpty(item.Reason) ? string.Empty : OrderRefundController.HTMLEncode(item.Reason.Replace("'", "‘").Replace("\"", "”"))),
					ExpressCompanyName = OrderRefundController.HTMLEncode(item.ExpressCompanyName),
					ShipOrderNumber = item.ShipOrderNumber,
					Payee = (string.IsNullOrEmpty(item.Payee) ? string.Empty : OrderRefundController.HTMLEncode(item.Payee)),
					PayeeAccount = (string.IsNullOrEmpty(item.PayeeAccount) ? string.Empty : OrderRefundController.HTMLEncode(item.PayeeAccount.Replace("'", "‘").Replace("\"", "”"))),
					RefundMode = (int)item.RefundMode,
					SellerRemark = (string.IsNullOrEmpty(item.SellerRemark) ? string.Empty : OrderRefundController.HTMLEncode(item.SellerRemark.Replace("'", "‘").Replace("\"", "”"))),
					ManagerRemark = (string.IsNullOrEmpty(item.ManagerRemark) ? string.Empty : OrderRefundController.HTMLEncode(item.ManagerRemark.Replace("'", "‘").Replace("\"", "”"))),
					RefundStatus = (item.SellerAuditStatus == OrderRefundInfo.OrderRefundAuditStatus.Audited ? item.ManagerConfirmStatus.ToDescription() : item.SellerAuditStatus.ToDescription()),
					RefundPayType = (!item.RefundPayType.HasValue ? "线下处理" : ((Enum)(object)item.RefundPayType).ToDescription())
				};
			});
			DataGridModel<OrderRefundModel> dataGridModel = new DataGridModel<OrderRefundModel>()
			{
				rows = orderRefundModels,
				total = orderRefunds.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management(int showtype = 0)
		{
			ViewBag.ShowType = showtype;
			return View();
		}
	}
}