using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class OrderQuery : QueryBase
	{
		public bool? Commented
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public List<OrderInfo.OrderOperateStatus> MoreStatus
		{
			get;
			set;
		}

        public List<OrderInfo.AccountTypes> MoreAccountTypeStatus
        {
            get;
            set;
        }

		public long? OrderId
		{
			get;
			set;
		}

		public int? OrderType
		{
			get;
			set;
		}

		public string PaymentTypeGateway
		{
			get;
			set;
		}

		public string PaymentTypeName
		{
			get;
			set;
		}

		public string SearchKeyWords
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

		public OrderInfo.OrderOperateStatus? Status
		{
			get;
			set;
		}
        public OrderInfo.AccountTypes? AccountTypeStatus
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

		public OrderQuery()
		{
		}
	}
}