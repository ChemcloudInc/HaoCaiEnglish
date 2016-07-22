using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopVistiInfo : BaseModel
	{
		private long _id;

		public DateTime Date
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public decimal SaleAmounts
		{
			get;
			set;
		}

		public long SaleCounts
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public long VistiCounts
		{
			get;
			set;
		}

		public ShopVistiInfo()
		{
		}
	}
}