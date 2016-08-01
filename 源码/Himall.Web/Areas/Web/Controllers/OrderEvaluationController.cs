using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class OrderEvaluationController : BaseMemberController
	{
		public OrderEvaluationController()
		{
		}

		public JsonResult AddOrderEvaluation(OrderCommentInfo info)
		{
			info.UserId = base.CurrentUser.Id;
			ServiceHelper.Create<ITradeCommentService>().AddOrderComment(info);
			Result result = new Result()
			{
				success = true,
				msg = "评价成功"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult AddOrderEvaluationAndComment(int PackMark, int DeliveryMark, int ServiceMark, long OrderId, string productCommentsJSON)
		{
			using (TransactionScope transactionScope = new TransactionScope())
			{
				if (PackMark != 0 || DeliveryMark != 0 || ServiceMark != 0)
				{
					OrderCommentInfo orderCommentInfo = new OrderCommentInfo()
					{
						UserId = base.CurrentUser.Id,
						PackMark = PackMark,
						DeliveryMark = DeliveryMark,
						ServiceMark = ServiceMark,
						OrderId = OrderId
					};
					List<ProductCommentsModel> productCommentsModels = JsonConvert.DeserializeObject<List<ProductCommentsModel>>(productCommentsJSON);
					ServiceHelper.Create<ITradeCommentService>().AddOrderComment(orderCommentInfo);
					foreach (ProductCommentsModel productCommentsModel in productCommentsModels)
					{
						ProductCommentInfo productCommentInfo = new ProductCommentInfo()
						{
							ReviewDate = DateTime.Now,
							ReviewContent = productCommentsModel.content,
							UserId = base.CurrentUser.Id,
							UserName = base.CurrentUser.UserName,
							Email = base.CurrentUser.Email,
							SubOrderId = new long?(productCommentsModel.subOrderId),
							ReviewMark = productCommentsModel.star
						};
						ServiceHelper.Create<ICommentService>().AddComment(productCommentInfo);
					}
				}
				else
				{
					foreach (ProductCommentsModel productCommentsModel1 in JsonConvert.DeserializeObject<List<ProductCommentsModel>>(productCommentsJSON))
					{
						ProductCommentInfo productCommentInfo1 = new ProductCommentInfo()
						{
							ReviewDate = DateTime.Now,
							ReviewContent = productCommentsModel1.content,
							UserId = base.CurrentUser.Id,
							UserName = base.CurrentUser.UserName,
							Email = base.CurrentUser.Email,
							SubOrderId = new long?(productCommentsModel1.subOrderId),
							ReviewMark = productCommentsModel1.star
						};
						ServiceHelper.Create<ICommentService>().AddComment(productCommentInfo1);
					}
				}
				transactionScope.Complete();
			}
			Result result = new Result()
			{
				success = true,
				msg = "评价成功"
			};
			return Json(result);
		}

		public ActionResult Details(long orderId)
		{
			OrderCommentInfo orderCommentInfo = ServiceHelper.Create<ITradeCommentService>().GetOrderCommentInfo(orderId, base.CurrentUser.Id);
			base.ViewBag.PackMark = (orderCommentInfo != null ? orderCommentInfo.PackMark - 1 : -1);
			base.ViewBag.DeliveryMark = (orderCommentInfo != null ? orderCommentInfo.DeliveryMark - 1 : -1);
			base.ViewBag.ServiceMark = (orderCommentInfo != null ? orderCommentInfo.ServiceMark - 1 : -1);
			IList<ProductEvaluation> productEvaluationByOrderIdNew = ServiceHelper.Create<ICommentService>().GetProductEvaluationByOrderIdNew(orderId, base.CurrentUser.Id);
			dynamic viewBag = base.ViewBag;
			long shopId = ServiceHelper.Create<IOrderService>().GetOrder(orderId).ShopId;
			viewBag.IsSellerAdminProdcut = shopId.Equals(1);
			return View(productEvaluationByOrderIdNew);
		}

		public ActionResult Index(long id)
		{
			IList<ProductEvaluation> productEvaluationByOrderId = ServiceHelper.Create<ICommentService>().GetProductEvaluationByOrderId(id, base.CurrentUser.Id);
			OrderCommentInfo orderCommentInfo = ServiceHelper.Create<ITradeCommentService>().GetOrderCommentInfo(id, base.CurrentUser.Id);
			if (orderCommentInfo == null)
			{
				ViewBag.Mark = 0;
			}
			else
			{
				ViewBag.Mark = Math.Round((double)(orderCommentInfo.PackMark + orderCommentInfo.ServiceMark + orderCommentInfo.DeliveryMark) / 3);
			}
			ViewBag.OrderId = id;
			dynamic viewBag = base.ViewBag;
			long shopId = ServiceHelper.Create<IOrderService>().GetOrder(id).ShopId;
			viewBag.IsSellerAdminProdcut = shopId.Equals(1);
			return View(productEvaluationByOrderId);
		}

		public ActionResult Satisfied(int pageSize = 15, int pageNo = 1)
		{
			OrderQuery orderQuery = new OrderQuery()
			{
				PageNo = pageNo,
				PageSize = pageSize,
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish),
				UserId = new long?(base.CurrentUser.Id)
			};
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<int>(orderQuery, (OrderInfo item) => item.OrderCommentInfo.Count);
            
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orders.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(orders.Models);
		}
	}
}