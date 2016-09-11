using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderComplaintInfo : BaseModel
	{
		private long _id;

		public DateTime ComplaintDate
		{
			get;
			set;
		}

		public string ComplaintReason
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

		public string SellerReply
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

		public string ShopPhone
		{
			get;
			set;
		}

		public OrderComplaintInfo.ComplaintStatus Status
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

		public string UserPhone
		{
			get;
			set;
		}

		public OrderComplaintInfo()
		{
		}

		public enum ComplaintStatus
		{
            [Description("WaitDeal")]
			WaitDeal = 1,
            [Description("Dealed")]
			Dealed = 2,
            [Description("Dispute")]
			Dispute = 3,
            [Description("End")]
			End = 4
		}
	}
}