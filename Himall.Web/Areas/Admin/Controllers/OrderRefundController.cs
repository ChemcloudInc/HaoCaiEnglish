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

namespace Himall.Web.Areas.Admin.Controllers
{
	public class OrderRefundController : BaseAdminController
	{
		public OrderRefundController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ConfirmRefund(long refundId, string managerRemark)
		{
			Result result = new Result();
			IRefundService refundService = ServiceHelper.Create<IRefundService>();
			refundService.ConfirmRefund(refundId, managerRemark, base.CurrentManager.UserName);
			result.success = true;
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
		[ValidateInput(false)]
		public JsonResult List(DateTime? startDate, DateTime? endDate, long? orderId, int? auditStatus, string shopName, string ProductName, string userName, int page, int rows, int showtype = 0)
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
			refundQuery.ShopName = shopName;
			refundQuery.UserName = userName;
			refundQuery.PageSize = rows;
			refundQuery.PageNo = page;
			refundQuery.ShowRefundType = new int?(showtype);
			RefundQuery nullable2 = refundQuery;
			if (auditStatus.HasValue && auditStatus.Value == 5)
			{
				nullable2.ConfirmStatus = new OrderRefundInfo.OrderRefundConfirmStatus?(OrderRefundInfo.OrderRefundConfirmStatus.UnConfirm);
			}
			PageModel<OrderRefundInfo> orderRefunds = ServiceHelper.Create<IRefundService>().GetOrderRefunds(nullable2);
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
					AuditStatus = (item.SellerAuditStatus == OrderRefundInfo.OrderRefundAuditStatus.Audited ? item.ManagerConfirmStatus.ToDescription() : item.SellerAuditStatus.ToDescription()),
					ProductId = item.OrderItemInfo.ProductId,
					ThumbnailsUrl = item.OrderItemInfo.ThumbnailsUrl,
					ConfirmStatus = item.ManagerConfirmStatus.ToDescription(),
					ApplyDate = item.ApplyDate.ToShortDateString(),
					ShopId = item.ShopId,
					ShopName = item.ShopName.Replace("'", "‘").Replace("\"", "”"),
					UserId = item.UserId,
					UserName = item.Applicant,
					Amount = item.Amount.ToString("F2"),
					ReturnQuantity = item.OrderItemInfo.ReturnQuantity,
					ProductName = string.Concat(item.OrderItemInfo.ProductName, str),
					Reason = (string.IsNullOrEmpty(item.Reason) ? string.Empty : OrderRefundController.HTMLEncode(item.Reason.Replace("'", "‘").Replace("\"", "”"))),
					RefundAccount = (string.IsNullOrEmpty(item.RefundAccount) ? string.Empty : OrderRefundController.HTMLEncode(item.RefundAccount.Replace("'", "‘").Replace("\"", "”"))),
					ContactPerson = (string.IsNullOrEmpty(item.ContactPerson) ? string.Empty : OrderRefundController.HTMLEncode(item.ContactPerson.Replace("'", "‘").Replace("\"", "”"))),
					ContactCellPhone = OrderRefundController.HTMLEncode(item.ContactCellPhone),
					PayeeAccount = (string.IsNullOrEmpty(item.PayeeAccount) ? string.Empty : OrderRefundController.HTMLEncode(item.PayeeAccount.Replace("'", "‘").Replace("\"", "”"))),
					Payee = (string.IsNullOrEmpty(item.Payee) ? string.Empty : OrderRefundController.HTMLEncode(item.Payee)),
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