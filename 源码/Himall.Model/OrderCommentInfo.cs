using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderCommentInfo : BaseModel
	{
		private long _id;

		public DateTime CommentDate
		{
			get;
			set;
		}

		public int DeliveryMark
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

		public int PackMark
		{
			get;
			set;
		}

		public int ServiceMark
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

		public OrderCommentInfo()
		{
		}
	}
}