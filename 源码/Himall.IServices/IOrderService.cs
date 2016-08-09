using Himall.Core;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.IServices
{
	public interface IOrderService : IService, IDisposable
	{
		void AgreeToRefundBySeller(long orderId);

		void AutoCloseOrder();

		void AutoConfirmOrder();

		void CalculateOrderItemRefund(long orderId, bool isCompel = false);

		void ConfirmZeroOrder(IEnumerable<long> Ids, long userId);

		List<OrderInfo> CreateOrder(OrderCreateModel model);

		void DeleteInvoiceContext(long id);

		void DeleteInvoiceTitle(long id);

		List<InvoiceContextInfo> GetInvoiceContexts();

		List<InvoiceTitleInfo> GetInvoiceTitles(long userid);

		OrderInfo GetOrder(long orderId, long userId);

		OrderInfo GetOrder(long orderId);

        void UpdateOrderInfo(OrderInfo orderInfo);
		OrderItemInfo GetOrderItem(long orderItemId);

		Dictionary<long, OrderItemInfo> GetOrderItems(IEnumerable<long> ids);

		IQueryable<OrderPayInfo> GetOrderPay(long id);

		PageModel<OrderInfo> GetOrders<Tout>(OrderQuery orderQuery, Expression<Func<OrderInfo, Tout>> sort = null);

		IEnumerable<OrderInfo> GetOrders(IEnumerable<long> ids);

		decimal GetRecentMonthAveragePrice(long shopId, long productId);

		SKUInfo GetSkuByID(string skuid);

		int GetSuccessOrderCountByProductID(long productId = 0L, OrderInfo.OrderOperateStatus orserStatus = (OrderInfo.OrderOperateStatus)5);

		IQueryable<OrderInfo> GetTopOrders(int top, long userId);

		bool IsHaveNoOnSaleProduct(long id);

		void MembeConfirmOrder(long orderId, string memberName);

		void MemberCloseOrder(long orderId, string memberName, bool isBackStock = false);

		void PayCapital(IEnumerable<long> orderIds, string payNo = null, long payId = 0L);

		void PaySucceed(IEnumerable<long> orderIds, string paymentId, DateTime payTime, string payNo = null, long payId = 0L);

		void PlatformCloseOrder(long orderId, string managerName, string CloseReason = "");

		void PlatformConfirmOrderPay(long orderId, string payRemark, string managerName);

		void SaveInvoiceContext(InvoiceContextInfo info);

		long SaveInvoiceTitle(InvoiceTitleInfo info);

		long SaveOrderPayInfo(IEnumerable<OrderPayInfo> model, PlatformType platform);

		void SellerCloseOrder(long orderId, string sellerName);

		void SellerSendGood(long orderId, string sellerName, string companyName, string shipOrderNumber);

		void SellerUpdateAddress(long orderId, string sellerName, string shipTo, string cellPhone, int topRegionId, int regionId, string regionFullName, string address);

		void SellerUpdateItemDiscountAmount(long orderItemId, decimal discountAmount, string sellerName);

		void SellerUpdateOrderFreight(long orderId, decimal freight);

		void SetOrderExpressInfo(long shopId, string expressName, string startCode, IEnumerable<long> orderIds);

		void UpdateMemberOrderInfo(long userId, [DecimalConstant(0, 0, 0, 0, 0)] decimal addOrderAmount = default(decimal), int addOrderCount = 1);
	}
}