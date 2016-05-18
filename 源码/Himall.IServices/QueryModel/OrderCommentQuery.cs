using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class OrderCommentQuery : QueryBase
	{
		public DateTime? EndDate
		{
			get;
			set;
		}

		public long? OrderId
		{
			get;
			set;
		}

		public long? ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public OrderCommentQuery()
		{
		}
	}
}