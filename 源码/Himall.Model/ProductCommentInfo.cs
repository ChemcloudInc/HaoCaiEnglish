using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductCommentInfo : BaseModel
	{
		private long _id;

		public string Email
		{
			get;
			set;
		}

		public virtual UserMemberInfo Himall_Members
		{
			get;
			set;
		}

		public virtual OrderItemInfo Himall_OrderItems
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
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

		public long ProductId
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductInfo ProductInfo
		{
			get;
			set;
		}

		public string ReplyContent
		{
			get;
			set;
		}

		public DateTime? ReplyDate
		{
			get;
			set;
		}

		public string ReviewContent
		{
			get;
			set;
		}

		public DateTime ReviewDate
		{
			get;
			set;
		}

		public int ReviewMark
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

		public long? SubOrderId
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

		public ProductCommentInfo()
		{
		}
	}
}