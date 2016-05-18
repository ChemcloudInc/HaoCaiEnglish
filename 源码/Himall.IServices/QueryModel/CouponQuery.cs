using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CouponQuery : QueryBase
	{
		public string CouponName
		{
			get;
			set;
		}

		public bool? IsShowAll
		{
			get;
			set;
		}

		public long? ShopId
		{
			get;
			set;
		}

		public CouponQuery()
		{
		}
	}
}