using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CommentQuery : QueryBase
	{
		public bool? IsReply
		{
			get;
			set;
		}

		public string KeyWords
		{
			get;
			set;
		}

		public long ProductID
		{
			get;
			set;
		}

		public long ShopID
		{
			get;
			set;
		}

		public long UserID
		{
			get;
			set;
		}

		public CommentQuery()
		{
		}
	}
}