using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class OrderComplaintController : BaseMemberController
	{
		public OrderComplaintController()
		{
		}

		[HttpPost]
		public JsonResult AddOrderComplaint(OrderComplaintInfo model)
		{
			model.UserId = base.CurrentUser.Id;
			model.UserName = base.CurrentUser.UserName;
			model.ComplaintDate = DateTime.Now;
			model.Status = OrderComplaintInfo.ComplaintStatus.WaitDeal;
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(model.ShopId, false);
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(model.OrderId, base.CurrentUser.Id);
			if (model.ComplaintReason.Length < 5)
			{
				throw new HimallException("投诉内容不能小于5个字符！");
			}
			if (string.IsNullOrWhiteSpace(model.UserPhone))
			{
				throw new HimallException("投诉电话不能为空！");
			}
			if (order == null || order.ShopId != model.ShopId)
			{
				throw new HimallException("该订单不属于当前用户！");
			}
			model.ShopName = (shop == null ? "" : shop.ShopName);
			model.ShopPhone = (shop == null ? "" : shop.CompanyPhone);
			ServiceHelper.Create<IComplaintService>().AddComplaint(model);
			return Json(new { success = true, msg = "Success" }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ApplyArbitration(long id)
		{
			ServiceHelper.Create<IComplaintService>().UserApplyArbitration(id, base.CurrentUser.Id);
			return Json(new { success = true, msg = "Success" });
		}

		[HttpPost]
		public JsonResult DealComplaint(long id)
		{
			ServiceHelper.Create<IComplaintService>().UserDealComplaint(id, base.CurrentUser.Id);
            return Json(new { success = true, msg = "Success" });
		}

		public ActionResult Index(int pageSize = 10, int pageNo = 1)
		{
			OrderQuery orderQuery = new OrderQuery()
			{
				PageNo = pageNo,
				PageSize = pageSize,
				UserId = new long?(base.CurrentUser.Id),
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish)
			};
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
			IQueryable<OrderInfo> models = 
				from o in orders.Models
				where o.OrderComplaintInfo.Count() == 0
				select o;
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orders.Total
			};
			ViewBag.pageInfo = pagingInfo;
			ViewBag.UserPhone = base.CurrentUser.CellPhone;
			ViewBag.UserId = base.CurrentUser.Id;
			return View(models.ToList());
		}

		public ActionResult Record(int pageSize = 10, int pageNo = 1)
		{
			ComplaintQuery complaintQuery = new ComplaintQuery()
			{
				UserId = new long?(base.CurrentUser.Id),
				PageNo = pageNo,
				PageSize = pageSize
			};
			PageModel<OrderComplaintInfo> orderComplaints = ServiceHelper.Create<IComplaintService>().GetOrderComplaints(complaintQuery);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orderComplaints.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(orderComplaints.Models.ToList());
		}
	}
}