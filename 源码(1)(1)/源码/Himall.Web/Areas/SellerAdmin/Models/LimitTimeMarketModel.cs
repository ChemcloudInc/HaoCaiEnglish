using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class LimitTimeMarketModel
	{
		public string AuditStatus
		{
			get;
			set;
		}

		public int AuditStatusNum
		{
			get;
			set;
		}

		public DateTime AuditTime
		{
			get;
			set;
		}

		public string CancelReson
		{
			get;
			set;
		}

		public string CategoryName
		{
			get;
			set;
		}

		public string EndTime
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public int MaxSaleCount
		{
			get;
			set;
		}

		public decimal Price
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

		public decimal ProductPrice
		{
			get;
			set;
		}

		public decimal RecentMonthPrice
		{
			get;
			set;
		}

		public int SaleCount
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

		public string StartTime
		{
			get;
			set;
		}

		public int Stock
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public LimitTimeMarketModel()
		{
		}
	}
}