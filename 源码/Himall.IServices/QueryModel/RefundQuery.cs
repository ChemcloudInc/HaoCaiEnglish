using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class RefundQuery : QueryBase
	{
		public OrderRefundInfo.OrderRefundAuditStatus? AuditStatus
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundConfirmStatus? ConfirmStatus
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public List<long> MoreOrderId
		{
			get;
			set;
		}

		public long? OrderId
		{
			get;
			set;
		}

		public string ProductName
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

		public int? ShowRefundType
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public int? Status
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

		public RefundQuery()
		{
		}
	}
}