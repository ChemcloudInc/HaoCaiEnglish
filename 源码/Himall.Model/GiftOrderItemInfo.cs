using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class GiftOrderItemInfo : BaseModel
	{
		private long _id;

		public long GiftId
		{
			get;
			set;
		}

		public string GiftName
		{
			get;
			set;
		}

		public decimal? GiftValue
		{
			get;
			set;
		}

		public virtual GiftOrderInfo Himall_GiftsOrder
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

		public string ImagePath
		{
			get;
			set;
		}

		public long? OrderId
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public int? SaleIntegral
		{
			get;
			set;
		}

		public GiftOrderItemInfo()
		{
		}
	}
}