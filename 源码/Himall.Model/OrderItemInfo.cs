using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderItemInfo : BaseModel
	{
		private long _id;

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

		public virtual ICollection<ProductCommentInfo> Himall_ProductComments
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

		public virtual Himall.Model.OrderInfo OrderInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.OrderRefundInfo> OrderRefundInfo
		{
			get;
			set;
		}

		public string ProductCode
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

		public OrderItemInfo()
		{
            OrderRefundInfo = new HashSet<Himall.Model.OrderRefundInfo>();
            Himall_ProductComments = new HashSet<ProductCommentInfo>();
		}
	}
}