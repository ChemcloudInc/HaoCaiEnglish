using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ProductCommentController : BaseSellerController
	{
		public ProductCommentController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Detail(long id)
		{
			ProductCommentInfo comment = ServiceHelper.Create<ICommentService>().GetComment(id);
			return Json(new { ConsulationContent = comment.ReviewContent, ReplyContent = comment.ReplyContent });
		}

		[UnAuthorize]
		public JsonResult List(int page, int rows, string Keywords, bool? isReply = null)
		{
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			CommentQuery commentQuery = new CommentQuery()
			{
				PageNo = page,
				PageSize = rows,
				KeyWords = Keywords,
				ShopID = base.CurrentSellerManager.ShopId,
				IsReply = isReply
			};
			PageModel<ProductCommentInfo> comments = ServiceHelper.Create<ICommentService>().GetComments(commentQuery);
			IEnumerable<ProductCommentModel> list = (
				from item in comments.Models
				select new ProductCommentModel()
				{
					CommentContent = item.ReviewContent,
					CommentDate = item.ReviewDate,
					ReplyContent = item.ReplyContent,
					CommentMark = item.ReviewMark,
					ReplyDate = item.ReplyDate,
					Id = item.Id,
					ProductName = item.ProductInfo.ProductName,
					ProductId = item.ProductId,
					ImagePath = item.Himall_OrderItems.ThumbnailsUrl,
					UserName = item.UserName,
					OderItemId = item.SubOrderId,
					Color = "",
					Version = "",
					Size = ""
				}).ToList();
			foreach (ProductCommentModel color in list)
			{
				if (!color.OderItemId.HasValue)
				{
					continue;
				}
				OrderItemInfo orderItem = orderService.GetOrderItem(color.OderItemId.Value);
				if (orderItem == null)
				{
					continue;
				}
				color.Color = orderItem.Color;
				color.Size = orderItem.Size;
				color.Version = orderItem.Version;
			}
			DataGridModel<ProductCommentModel> dataGridModel = new DataGridModel<ProductCommentModel>()
			{
				rows = list,
				total = comments.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ReplyComment(long id, string replycontent)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<ICommentService>().ReplyComment(id, replycontent, shopId);
			Result result = new Result()
			{
				success = true,
				msg = "回复成功！"
			};
			return Json(result);
		}
	}
}