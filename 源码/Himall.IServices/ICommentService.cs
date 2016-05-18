using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface ICommentService : IService, IDisposable
	{
		void AddComment(ProductCommentInfo model);

		void DeleteComment(long id);

		ProductCommentInfo GetComment(long id);

		PageModel<ProductCommentInfo> GetComments(CommentQuery query);

		IQueryable<ProductCommentInfo> GetCommentsByProductId(long productId);

		PageModel<ProductEvaluation> GetProductEvaluation(CommentQuery query);

		IList<ProductEvaluation> GetProductEvaluationByOrderId(long orderId, long userId);

		IList<ProductEvaluation> GetProductEvaluationByOrderIdNew(long orderId, long userId);

		IQueryable<OrderItemInfo> GetUnEvaluatProducts(long userId);

		void ReplyComment(long id, string replyConent, long shopId);

		void SetCommentEmpty(long id);
	}
}