using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IGiftsOrderService : IService, IDisposable
	{
		void AutoConfirmOrder();

		void CloseOrder(long id, string closeReason);

		void ConfirmOrder(long id, long userId);

		GiftOrderInfo CreateOrder(GiftOrderModel model);

		GiftOrderInfo GetOrder(long orderId);

		GiftOrderInfo GetOrder(long orderId, long userId);

		GiftOrderItemInfo GetOrderItemById(long id);

		PageModel<GiftOrderInfo> GetOrders(GiftsOrderQuery query);

		IEnumerable<GiftOrderInfo> GetOrders(IEnumerable<long> ids);

		int GetOwnBuyQuantity(long userid, long giftid);

		IEnumerable<GiftOrderInfo> OrderAddUserInfo(IEnumerable<GiftOrderInfo> orders);

		void SendGood(long id, string shipCompanyName, string shipOrderNumber);
	}
}