using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class OrderComplaintController : BaseAdminController
	{
		public OrderComplaintController()
		{
		}

		[HttpPost]
		[OperationLog(Message="处理交易投诉")]
		[UnAuthorize]
		public JsonResult DealComplaint(long id)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IComplaintService>().DealComplaint(id);
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
		public JsonResult List(DateTime? startDate, DateTime? endDate, long? orderId, int? complaintStatus, string shopName, string userName, int page, int rows)
		{
			OrderComplaintInfo.ComplaintStatus? nullable;
			ComplaintQuery complaintQuery = new ComplaintQuery()
			{
				StartDate = startDate,
				EndDate = endDate,
				OrderId = orderId
			};
			ComplaintQuery complaintQuery1 = complaintQuery;
			int? nullable1 = complaintStatus;
			if (nullable1.HasValue)
			{
				nullable = new OrderComplaintInfo.ComplaintStatus?((OrderComplaintInfo.ComplaintStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			complaintQuery1.Status = nullable;
			complaintQuery.ShopName = shopName;
			complaintQuery.UserName = userName;
			complaintQuery.PageSize = rows;
			complaintQuery.PageNo = page;
			PageModel<OrderComplaintInfo> orderComplaints = ServiceHelper.Create<IComplaintService>().GetOrderComplaints(complaintQuery);
			var array = 
				from item in orderComplaints.Models.ToArray()
				select new { Id = item.Id, OrderId = item.OrderId, ComplaintStatus = item.Status.ToDescription(), ShopName = item.ShopName, ShopPhone = item.ShopPhone, UserName = item.UserName, UserPhone = item.UserPhone, ComplaintDate = item.ComplaintDate.ToShortDateString(), ComplaintReason = Server.HtmlEncode(item.ComplaintReason), SellerReply = (string.IsNullOrEmpty(item.SellerReply) ? string.Empty : Server.HtmlEncode(item.SellerReply)) };
			return Json(new { rows = array, total = orderComplaints.Total });
		}

		public ActionResult Management()
		{
			return View();
		}
	}
}