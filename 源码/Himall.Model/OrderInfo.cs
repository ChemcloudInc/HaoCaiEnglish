using Himall.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderInfo : BaseModel
	{
		private long _id;

		public OrderInfo.ActiveTypes ActiveType
		{
			get;
			set;
		}

		public string Address
		{
			get;
			set;
		}

		public string CellPhone
		{
			get;
			set;
		}

		public string CloseReason
		{
			get;
			set;
		}

		[NotMapped]
		public decimal CommisAmount
		{
			get
			{
				return CommisTotalAmount - RefundCommisAmount;
			}
		}

		public decimal CommisTotalAmount
		{
			get;
			set;
		}

		public decimal DiscountAmount
		{
			get;
			set;
		}

		public string ExpressCompanyName
		{
			get;
			set;
		}

		public DateTime? FinishDate
		{
			get;
			set;
		}

		public decimal Freight
		{
			get;
			set;
		}

		public string GatewayOrderId
		{
			get;
			set;
		}

		[NotMapped]
		public bool HaveDelProduct
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public decimal IntegralDiscount
		{
			get;
			set;
		}

		public string InvoiceContext
		{
			get;
			set;
		}

		public string InvoiceTitle
		{
			get;
			set;
		}

		public Himall.Model.InvoiceType InvoiceType
		{
			get;
			set;
		}

		public bool IsPrinted
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.OrderCommentInfo> OrderCommentInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.OrderComplaintInfo> OrderComplaintInfo
		{
			get;
			set;
		}

		public DateTime OrderDate
		{
			get;
			set;
		}

		[NotMapped]
		public decimal OrderEnabledRefundAmount
		{
			get
			{
				decimal num = new decimal(0);
				switch (OrderStatus)
				{
					case OrderInfo.OrderOperateStatus.WaitDelivery:
					{
						num = (ProductTotalAmount + Freight) - DiscountAmount;
						return num;
					}
					case OrderInfo.OrderOperateStatus.WaitReceiving:
					case OrderInfo.OrderOperateStatus.Finish:
					{
						num = (ProductTotalAmount - DiscountAmount) - RefundTotalAmount;
						return num;
					}
					case OrderInfo.OrderOperateStatus.Close:
					{
						return num;
					}
					default:
					{
						return num;
					}
				}
			}
		}

		public virtual ICollection<Himall.Model.OrderItemInfo> OrderItemInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.OrderOperationLogInfo> OrderOperationLogInfo
		{
			get;
			set;
		}

		[NotMapped]
		public long OrderProductQuantity
		{
			get
			{
				long quantity = 0;
				foreach (Himall.Model.OrderItemInfo orderItemInfo in OrderItemInfo)
				{
					quantity = quantity + orderItemInfo.Quantity;
				}
				return quantity;
			}
		}

		[NotMapped]
		public long OrderReturnQuantity
		{
			get
			{
				long returnQuantity = 0;
				foreach (Himall.Model.OrderItemInfo orderItemInfo in OrderItemInfo)
				{
					returnQuantity = returnQuantity + orderItemInfo.ReturnQuantity;
				}
				return returnQuantity;
			}
		}

		public OrderInfo.OrderOperateStatus OrderStatus
		{
			get;
			set;
		}

		[NotMapped]
		public decimal OrderTotalAmount
		{
			get
			{
				return (((ProductTotalAmount + Freight) + Tax) - IntegralDiscount) - DiscountAmount;
			}
		}

		public OrderInfo.OrderTypes? OrderType
		{
			get;
			set;
		}

		public DateTime? PayDate
		{
			get;
			set;
		}

		public string PaymentTypeGateway
		{
			get;
			set;
		}

		public string PaymentTypeName
		{
			get;
			set;
		}

		public string PayRemark
		{
			get;
			set;
		}

		public PlatformType Platform
		{
			get;
			set;
		}

		public decimal ProductTotalAmount
		{
			get;
			set;
		}

		public decimal RefundCommisAmount
		{
			get;
			set;
		}

		[NotMapped]
		public int? RefundStats
		{
			get;
			set;
		}

		public decimal RefundTotalAmount
		{
			get;
			set;
		}

		public string RegionFullName
		{
			get;
			set;
		}

		public int RegionId
		{
			get;
			set;
		}

		public string SellerAddress
		{
			get;
			set;
		}

		public string SellerPhone
		{
			get;
			set;
		}

		public string SellerRemark
		{
			get;
			set;
		}

		public string ShipOrderNumber
		{
			get;
			set;
		}

		public DateTime? ShippingDate
		{
			get;
			set;
		}

		public string ShipTo
		{
			get;
			set;
		}

		[NotMapped]
		public decimal ShopAccountAmount
		{
			get
			{
				return (OrderTotalAmount - RefundTotalAmount) - CommisAmount;
			}
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		[NotMapped]
		public string ShowExpressCompanyName
		{
			get
			{
				string expressCompanyName = ExpressCompanyName;
				if (expressCompanyName == "-1")
				{
					expressCompanyName = "其他";
				}
				return expressCompanyName;
			}
		}

		public decimal Tax
		{
			get;
			set;
		}

		public int TopRegionId
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public string UserRemark
		{
			get;
			set;
		}

		public OrderInfo()
		{
            OrderComplaintInfo = new HashSet<Himall.Model.OrderComplaintInfo>();
            OrderItemInfo = new HashSet<Himall.Model.OrderItemInfo>();
            OrderOperationLogInfo = new HashSet<Himall.Model.OrderOperationLogInfo>();
            OrderCommentInfo = new HashSet<Himall.Model.OrderCommentInfo>();
		}

		public enum ActiveTypes
		{
			[Description("无活动")]
			None
		}

		public enum OrderOperateStatus
		{
			[Description("待付款")]
			WaitPay = 1,
			[Description("待发货")]
			WaitDelivery = 2,
			[Description("待收货")]
			WaitReceiving = 3,
			[Description("已关闭")]
			Close = 4,
			[Description("已完成")]
			Finish = 5
		}

		public enum OrderTypes
		{
			[Description("组合购")]
			Collocation = 1,
			[Description("限时购")]
			LimitBuy = 2
		}
	}
}