using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class CommentService : ServiceBase, ICommentService, IService, IDisposable
	{
		public CommentService()
		{
		}

		public void AddComment(ProductCommentInfo model)
		{
			OrderItemInfo orderItemInfo = (
				from a in context.OrderItemInfo
				where a.Id == model.SubOrderId && a.OrderInfo.UserId == model.UserId
				select a).FirstOrDefault();
			if (orderItemInfo == null)
			{
				throw new HimallException("不能对此商品进行评价！");
			}
			model.ShopId = orderItemInfo.ShopId;
			model.ProductId = orderItemInfo.ProductId;
			model.ShopName = (
				from a in context.ShopInfo
				where a.Id == orderItemInfo.ShopId
				select a.ShopName).FirstOrDefault();
            context.ProductCommentInfo.Add(model);
            context.SaveChanges();
		}

		public void DeleteComment(long id)
		{
            context.ProductCommentInfo.Remove(new object[] { id });
            context.SaveChanges();
		}

		public ProductCommentInfo GetComment(long id)
		{
			return context.ProductCommentInfo.FindById<ProductCommentInfo>(id);
		}

		public PageModel<ProductCommentInfo> GetComments(CommentQuery query)
		{
			int num = 0;
			IQueryable<ProductCommentInfo> shopID = context.ProductCommentInfo.Include<ProductCommentInfo, ProductInfo>((ProductCommentInfo a) => a.ProductInfo).AsQueryable<ProductCommentInfo>();
			if (query.IsReply.HasValue)
			{
				shopID = 
					from item in shopID
					where (query.IsReply.Value ? item.ReplyDate.HasValue : !item.ReplyDate.HasValue)
					select item;
			}
			if (query.ShopID > 0)
			{
				shopID = 
					from item in shopID
					where query.ShopID == item.ShopId
					select item;
			}
			if (query.ProductID > 0)
			{
				shopID = 
					from item in shopID
					where query.ProductID == item.ProductId
					select item;
			}
			if (query.UserID > 0)
			{
				shopID = 
					from item in shopID
					where query.UserID == item.UserId
					select item;
			}
			if (!string.IsNullOrWhiteSpace(query.KeyWords))
			{
				shopID = 
					from item in shopID
					where item.ReviewContent.Contains(query.KeyWords)
					select item;
			}
			shopID = shopID.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<ProductCommentInfo>()
			{
				Models = shopID,
				Total = num
			};
		}

		public IQueryable<ProductCommentInfo> GetCommentsByProductId(long productId)
		{
			return context.ProductCommentInfo.FindBy((ProductCommentInfo c) => c.ProductId.Equals(productId));
		}

		public PageModel<ProductEvaluation> GetProductEvaluation(CommentQuery query)
		{
			int num = 0;
			IQueryable<OrderItemInfo> page = context.OrderItemInfo.FindBy((OrderItemInfo a) => a.OrderInfo.UserId == query.UserID && (int)a.OrderInfo.OrderStatus == 5);
			Func<IQueryable<OrderItemInfo>, IOrderedQueryable<OrderItemInfo>> orderBy = page.GetOrderBy<OrderItemInfo>((IQueryable<OrderItemInfo> d) => 
				from o in d
				orderby o.Id descending
				select o);
			if (!string.IsNullOrWhiteSpace(query.Sort) && query.Sort.Equals("PComment"))
			{
				orderBy = page.GetOrderBy<OrderItemInfo>((IQueryable<OrderItemInfo> d) => 
					from a in d
					orderby context.OrderCommentInfo.Any((OrderCommentInfo b) => b.OrderId == a.OrderId && b.UserId == query.UserID)
					select a);
			}
			page = page.GetPage(out num, query.PageNo, query.PageSize, orderBy);
			IQueryable<ProductEvaluation> productEvaluation = 
				from a in page
				select new ProductEvaluation()
				{
					ProductId = a.ProductId,
					ThumbnailsUrl = a.ThumbnailsUrl,
					ProductName = a.ProductName,
					BuyTime = (context.OrderCommentInfo.Any((OrderCommentInfo c) => c.OrderId == a.OrderId) ? context.OrderCommentInfo.Where((OrderCommentInfo c) => c.OrderId == a.OrderId).FirstOrDefault().CommentDate : DateTime.MinValue),
					EvaluationStatus = context.OrderCommentInfo.Any((OrderCommentInfo b) => b.OrderId == a.OrderId && b.UserId == query.UserID),
					Id = a.Id,
					OrderId = a.OrderId
				};
			return new PageModel<ProductEvaluation>()
			{
				Models = productEvaluation,
				Total = num
			};
		}

		public IList<ProductEvaluation> GetProductEvaluationByOrderId(long orderId, long userId)
		{
			IOrderedQueryable<OrderItemInfo> orderItemInfo = 
				from a in context.OrderItemInfo
				where a.OrderId == orderId && a.OrderInfo.UserId == userId && (int)a.OrderInfo.OrderStatus == 5
				orderby context.ProductCommentInfo.Any((ProductCommentInfo b) => b.ProductId == a.ProductId && b.UserId == userId && b.SubOrderId == a.Id)
				select a;
			IQueryable<ProductEvaluation> productEvaluation = 
				from a in orderItemInfo
				select new ProductEvaluation()
				{
					ProductId = a.ProductId,
					ThumbnailsUrl = a.ThumbnailsUrl,
					ProductName = a.ProductName,
					BuyTime = a.OrderInfo.OrderDate,
					EvaluationStatus = context.ProductCommentInfo.Any((ProductCommentInfo b) => b.ProductId == a.ProductId && b.UserId == userId && b.SubOrderId == a.Id),
					Id = a.Id,
					OrderId = a.OrderId,
					Color = a.Color,
					Size = a.Size,
					Version = a.Version
				};
			return productEvaluation.ToList();
		}

		public IList<ProductEvaluation> GetProductEvaluationByOrderIdNew(long orderId, long userId)
		{
			List<ProductEvaluation> list = (
				from p in context.ProductCommentInfo
				where p.Himall_OrderItems.OrderId == orderId && p.UserId == userId
				select new ProductEvaluation()
				{
					ProductId = p.ProductId,
					ThumbnailsUrl = p.Himall_OrderItems.ThumbnailsUrl,
					ProductName = p.ProductInfo.ProductName,
					BuyTime = p.Himall_OrderItems.OrderInfo.OrderDate,
					EvaluationStatus = true,
					Id = p.SubOrderId.Value,
					OrderId = p.Himall_OrderItems.OrderId,
					EvaluationRank = p.ReviewMark - 1,
					EvaluationContent = p.ReviewContent,
					Color = p.Himall_OrderItems.Color,
					Size = p.Himall_OrderItems.Size,
					Version = p.Himall_OrderItems.Version
				}).ToList();
			return list.ToList();
		}

		public PageModel<ProductEvaluation> GetProductEvaluationTwo(CommentQuery query)
		{
			int num = 0;
			IQueryable<OrderItemInfo> page = context.OrderItemInfo.FindBy((OrderItemInfo a) => a.OrderInfo.UserId == query.UserID && (int)a.OrderInfo.OrderStatus == 5);
			Func<IQueryable<OrderItemInfo>, IOrderedQueryable<OrderItemInfo>> orderBy = page.GetOrderBy((IQueryable<OrderItemInfo> d) => 
				from o in d
				orderby o.Id descending
				select o);
			if (!string.IsNullOrWhiteSpace(query.Sort) && query.Sort.Equals("PComment"))
			{
				orderBy = page.GetOrderBy((IQueryable<OrderItemInfo> d) => 
					from a in d
					orderby context.OrderCommentInfo.Any((OrderCommentInfo b) => b.OrderId == a.OrderId && b.UserId == query.UserID)
					select a);
			}
			page = page.GetPage(out num, query.PageNo, query.PageSize, orderBy);
			IQueryable<ProductEvaluation> productEvaluation = 
				from a in page
				select new ProductEvaluation()
				{
					ProductId = a.ProductId,
					ThumbnailsUrl = a.ThumbnailsUrl,
					ProductName = a.ProductName,
					BuyTime = (context.ProductCommentInfo.Where((ProductCommentInfo c) => c.ProductId == a.ProductId && c.SubOrderId == a.Id).FirstOrDefault() != null ? context.ProductCommentInfo.Where((ProductCommentInfo c) => c.ProductId == a.ProductId && c.SubOrderId == a.Id).FirstOrDefault().ReviewDate : DateTime.MinValue),
					EvaluationStatus = context.ProductCommentInfo.Any((ProductCommentInfo b) => b.ProductId == a.ProductId && b.UserId == query.UserID && b.SubOrderId == a.Id),
					Id = a.Id,
					OrderId = a.OrderId
				};
			return new PageModel<ProductEvaluation>()
			{
				Models = productEvaluation,
				Total = num
			};
		}

		public IQueryable<OrderItemInfo> GetUnEvaluatProducts(long userId)
		{
			return 
				from a in context.OrderItemInfo
				where a.OrderInfo.UserId == userId && (int)a.OrderInfo.OrderStatus == 5 && !context.ProductCommentInfo.Any((ProductCommentInfo b) => b.ProductId == a.ProductId && b.UserId == userId && b.SubOrderId == a.Id)
				orderby a.Id descending
				select a;
		}

		public void ReplyComment(long id, string replyContent, long shopId)
		{
			ProductCommentInfo nullable = context.ProductCommentInfo.FindBy((ProductCommentInfo item) => item.Id == id && item.ShopId == shopId).FirstOrDefault();
			if (shopId == 0 || nullable == null)
			{
				throw new HimallException("不存在该商品评论");
			}
			nullable.ReplyContent = replyContent;
			nullable.ReplyDate = new DateTime?(DateTime.Now);
            context.SaveChanges();
		}

		public void SetCommentEmpty(long id)
		{
			ProductCommentInfo productCommentInfo = context.ProductCommentInfo.First<ProductCommentInfo>((ProductCommentInfo p) => p.Id == id);
			productCommentInfo.ReviewContent = "";
			productCommentInfo.ReplyContent = "";
            context.SaveChanges();
		}
	}
}