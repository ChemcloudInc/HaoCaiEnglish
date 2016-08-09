using Himall.Core;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class OrderListModel
	{
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

		public long Id
		{
			get;
			set;
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

		public DateTime OrderDate
		{
			get;
			set;
		}

		public IEnumerable<OrderItemListModel> OrderItemList
		{
			get;
			set;
		}

		public OrderInfo.OrderOperateStatus OrderStatus
		{
			get;
			set;
		}

		public decimal OrderTotalAmount
		{
			get;
			set;
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

		public ShopBonusGrantInfo ReceiveBonus
		{
			get;
			set;
		}

		public decimal RefundCommisAmount
		{
			get;
			set;
		}

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

		public OrderListModel()
		{
		}
	}
}