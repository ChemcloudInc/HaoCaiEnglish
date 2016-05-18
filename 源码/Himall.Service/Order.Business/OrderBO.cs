using Himall.Core;
using Himall.Model;
using System;

namespace Himall.Service.Order.Business
{
	public class OrderBO
	{
		private static object obj;

		private static object objpay;

		static OrderBO()
		{
			OrderBO.obj = new object();
			OrderBO.objpay = new object();
		}

		public OrderBO()
		{
		}

		public void CloseOrder(OrderInfo order)
		{
			if (order.OrderStatus != OrderInfo.OrderOperateStatus.WaitPay)
			{
				throw new HimallException("只有待付款状态的订单才能进行取消操作");
			}
			order.OrderStatus = OrderInfo.OrderOperateStatus.Close;
		}

		public long GenerateOrderNumber()
		{
			long num;
			lock (OrderBO.obj)
			{
				string empty = string.Empty;
				Guid guid = Guid.NewGuid();
				Random random = new Random(BitConverter.ToInt32(guid.ToByteArray(), 0));
				for (int i = 0; i < 5; i++)
				{
					int num1 = random.Next();
					char chr = (char)(48 + (ushort)(num1 % 10));
					empty = string.Concat(empty, chr.ToString());
				}
				DateTime now = DateTime.Now;
				num = long.Parse(string.Concat(now.ToString("yyyyMMddfff"), empty));
			}
			return num;
		}

		public long GetOrderPayId()
		{
			long num;
			lock (OrderBO.objpay)
			{
				string empty = string.Empty;
				Guid guid = Guid.NewGuid();
				Random random = new Random(BitConverter.ToInt32(guid.ToByteArray(), 0));
				for (int i = 0; i < 6; i++)
				{
					int num1 = random.Next();
					char chr = (char)(48 + (ushort)(num1 % 10));
					empty = string.Concat(empty, chr.ToString());
				}
				DateTime now = DateTime.Now;
				num = long.Parse(string.Concat(now.ToString("yyMMddmmHHss"), empty));
			}
			return num;
		}

		public decimal GetRealTotalPrice(OrderInfo order, OrderItemInfo item, decimal discountAmount)
		{
			if ((item.RealTotalPrice - discountAmount) < new decimal(0))
			{
				throw new HimallException("优惠金额不能大于商品总金额！");
			}
			if ((order.OrderTotalAmount - discountAmount) < new decimal(0))
			{
				throw new HimallException("减价不能导致订单总金额为负值！");
			}
			return item.RealTotalPrice - discountAmount;
		}

		public bool IsFullFreeFreight(ShopInfo shop, decimal OrderPaidAmount)
		{
			bool flag = false;
			if (shop != null && shop.FreeFreight > new decimal(0) && OrderPaidAmount >= shop.FreeFreight)
			{
				flag = true;
			}
			return flag;
		}

		public void SetFreight(OrderInfo order, decimal freight)
		{
			if (freight < new decimal(0))
			{
				throw new HimallException("运费不能为负值！");
			}
			order.Freight = freight;
		}

		public void SetStateToConfirm(OrderInfo order)
		{
			if (order == null)
			{
				throw new HimallException("处理订单错误，请确认该订单状态正确");
			}
			if (order.OrderStatus != OrderInfo.OrderOperateStatus.WaitReceiving)
			{
				throw new HimallException("只有等待收货状态的订单才能进行确认操作");
			}
			order.OrderStatus = OrderInfo.OrderOperateStatus.Finish;
			order.FinishDate = new DateTime?(DateTime.Now);
		}
	}
}