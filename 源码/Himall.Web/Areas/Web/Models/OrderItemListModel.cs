using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class OrderItemListModel
	{
		public Himall.Model.CashDepositsObligation CashDepositsObligation
		{
			get;
			set;
		}

		public string Color
		{
			get;
			set;
		}

		public decimal CommisRate
		{
			get;
			set;
		}

		public decimal CostPrice
		{
			get;
			set;
		}

		public decimal DiscountAmount
		{
			get;
			set;
		}

		public decimal? EnabledRefundAmount
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public bool IsLimitBuy
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public long Quantity
		{
			get;
			set;
		}

		public decimal RealTotalPrice
		{
			get;
			set;
		}

		public decimal RefundPrice
		{
			get;
			set;
		}

		public long ReturnQuantity
		{
			get;
			set;
		}

		public decimal SalePrice
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Size
		{
			get;
			set;
		}

		public string SKU
		{
			get;
			set;
		}

		public string SkuId
		{
			get;
			set;
		}

		public string ThumbnailsUrl
		{
			get;
			set;
		}

		public string Version
		{
			get;
			set;
		}

		public OrderItemListModel()
		{
		}
	}
}