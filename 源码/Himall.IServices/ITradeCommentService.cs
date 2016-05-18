using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface ITradeCommentService : IService, IDisposable
	{
		void AddOrderComment(OrderCommentInfo info);

		void DeleteOrderComment(long id);

		OrderCommentInfo GetOrderCommentInfo(long orderId, long userId);

		PageModel<OrderCommentInfo> GetOrderComments(OrderCommentQuery query);

		IQueryable<OrderCommentInfo> GetOrderComments(long userId);
	}
}