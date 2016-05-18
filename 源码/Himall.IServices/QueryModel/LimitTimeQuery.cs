using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class LimitTimeQuery : QueryBase
	{
		public LimitTimeMarketInfo.LimitTimeMarketAuditStatus? AuditStatus
		{
			get;
			set;
		}

		public string CategoryName
		{
			get;
			set;
		}

		public bool CheckProductStatus
		{
			get;
			set;
		}

		public string ItemName
		{
			get;
			set;
		}

		public int OrderKey
		{
			get;
			set;
		}

		public int OrderType
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

		public LimitTimeQuery()
		{
		}
	}
}