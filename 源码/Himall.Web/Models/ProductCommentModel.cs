using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class ProductCommentModel
	{
		public string Color
		{
			get;
			set;
		}

		public string CommentContent
		{
			get;
			set;
		}

		public DateTime CommentDate
		{
			get;
			set;
		}

		public string CommentDateStr
		{
			get
			{
				return CommentDate.ToString("yyyy-MM-dd HH:mm");
			}
		}

		public int CommentMark
		{
			get;
			set;
		}

		public string Date
		{
			get
			{
				return CommentDate.ToString("yyyy-MM-dd HH:mm");
			}
		}

		public string Email
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string ImagePath
		{
			get;
			set;
		}

		public long? OderItemId
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

		public string ProductPic
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

		public string Size
		{
			get;
			set;
		}

		public bool Status
		{
			get
			{
				return ReplyDate.HasValue;
			}
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

		public string Version
		{
			get;
			set;
		}

		public ProductCommentModel()
		{
		}
	}
}