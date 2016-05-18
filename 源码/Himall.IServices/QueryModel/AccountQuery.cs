using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class AccountQuery : QueryBase
	{
		public long AccountId
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public AccountDetailInfo.EnumOrderType EnumOrderType
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

		public AccountInfo.AccountStatus? Status
		{
			get;
			set;
		}

		public AccountQuery()
		{
		}
	}
}