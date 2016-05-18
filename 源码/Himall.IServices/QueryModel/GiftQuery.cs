using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class GiftQuery : QueryBase
	{
		public bool? isShowAll
		{
			get;
			set;
		}

		public string skey
		{
			get;
			set;
		}

		public new GiftQuery.GiftSortEnum Sort
		{
			get;
			set;
		}

		public GiftInfo.GiftSalesStatus? status
		{
			get;
			set;
		}

		public GiftQuery()
		{
		}

		public enum GiftSortEnum
		{
			Default,
			SalesNumber,
			RealSalesNumber
		}
	}
}