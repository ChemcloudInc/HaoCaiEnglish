using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductVistiInfo : BaseModel
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

		public long? OrderCounts
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductInfo ProductInfo
		{
			get;
			set;
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

		public long VistiCounts
		{
			get;
			set;
		}

		public ProductVistiInfo()
		{
		}
	}
}