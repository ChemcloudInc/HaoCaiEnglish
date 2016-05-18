using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CouponRecordQuery : QueryBase
	{
		public long? CouponId
		{
			get;
			set;
		}

		public long? ShopId
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

		public CouponRecordQuery()
		{
		}
	}
}