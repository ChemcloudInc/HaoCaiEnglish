using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Mobile;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class CommentController : BaseMobileMemberController
	{
		public CommentController()
		{
		}

		[HttpPost]
		public JsonResult AddComment(string comment)
		{
			bool flag = false;
			OrderCommentModel orderCommentModel = JsonConvert.DeserializeObject<OrderCommentModel>(comment);
			if (orderCommentModel != null)
			{
				using (TransactionScope transactionScope = new TransactionScope())
				{
                    AddOrderComment(orderCommentModel);
                    AddProductsComment(orderCommentModel.OrderId, orderCommentModel.ProductComments);
					transactionScope.Complete();
				}
				flag = true;
			}
			return Json(new { success = flag });
		}

		private void AddOrderComment(OrderCommentModel comment)
		{
			ITradeCommentService tradeCommentService = ServiceHelper.Create<ITradeCommentService>();
			OrderCommentInfo orderCommentInfo = new OrderCommentInfo()
			{
				OrderId = comment.OrderId,
				DeliveryMark = comment.DeliveryMark,
				ServiceMark = comment.ServiceMark,
				PackMark = comment.PackMark,
				UserId = base.CurrentUser.Id
			};
			tradeCommentService.AddOrderComment(orderCommentInfo);
		}

		private void AddProductsComment(long orderId, IEnumerable<ProductCommentModel> productComments)
		{
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			foreach (ProductCommentModel productComment in productComments)
			{
				ProductCommentInfo productCommentInfo = new ProductCommentInfo()
				{
					ProductId = productComment.ProductId,
					ReviewMark = productComment.Mark,
					ReviewContent = productComment.Content,
					UserId = base.CurrentUser.Id,
					SubOrderId = new long?(productComment.OrderItemId),
					ReviewDate = DateTime.Now,
					UserName = base.CurrentUser.UserName,
					Email = base.CurrentUser.Email
				};
				commentService.AddComment(productCommentInfo);
			}
		}

		public ActionResult Index(long orderId)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(orderId);
			if (order == null || order.OrderCommentInfo.Count != 0)
			{
				ViewBag.Valid = false;
			}
			else
			{
				ViewBag.Valid = true;
				IList<ProductEvaluation> productEvaluationByOrderId = ServiceHelper.Create<ICommentService>().GetProductEvaluationByOrderId(orderId, base.CurrentUser.Id);
				ServiceHelper.Create<ITradeCommentService>().GetOrderCommentInfo(orderId, base.CurrentUser.Id);
				ViewBag.Products = productEvaluationByOrderId;
				dynamic viewBag = base.ViewBag;
				ICollection<OrderItemInfo> orderItemInfo = order.OrderItemInfo;
				viewBag.OrderItemIds = 
					from item in orderItemInfo
					select item.Id;
			}
			return View();
		}
	}
}