using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IRefundService : IService, IDisposable
	{
		void AddOrderRefund(OrderRefundInfo info);

		bool CanApplyRefund(long orderId, long orderItemId, bool? isAllOrderRefund = null);

		void ConfirmRefund(long id, string managerRemark, string managerName);

		OrderRefundInfo GetOrderRefund(long id, long userId);

		PageModel<OrderRefundInfo> GetOrderRefunds(RefundQuery refundQuery);

		void SellerConfirmRefundGood(long id, string sellerName);

		void SellerDealRefund(long id, OrderRefundInfo.OrderRefundAuditStatus auditStatus, string sellerRemark, string sellerName);

		void UserConfirmRefundGood(long id, string sellerName, string expressCompanyName, string shipOrderNumber);
	}
}