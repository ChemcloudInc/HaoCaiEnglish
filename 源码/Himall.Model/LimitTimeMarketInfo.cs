using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class LimitTimeMarketInfo : BaseModel
	{
		private long _id;

		public LimitTimeMarketInfo.LimitTimeMarketAuditStatus AuditStatus
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

		public DateTime EndTime
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

		public string ImagePath
		{
			get;
			set;
		}

		public int MaxSaleCount
		{
			get;
			set;
		}

		public decimal MinPrice
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public string ProductAd
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

		public DateTime StartTime
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

		public LimitTimeMarketInfo()
		{
		}

		public enum LimitTimeMarketAuditStatus : short
		{
			[Description("待审核")]
			WaitForAuditing = 1,
			[Description("进行中")]
			Ongoing = 2,
			[Description("未通过")]
			AuditFailed = 3,
			[Description("已结束")]
			Ended = 4,
			[Description("已取消")]
			Cancelled = 5
		}
	}
}