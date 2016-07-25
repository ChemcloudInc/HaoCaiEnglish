using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderPayInfo : BaseModel
	{
		private long _id;

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

		public long OrderId
		{
			get;
			set;
		}

		public long PayId
		{
			get;
			set;
		}

		public bool PayState
		{
			get;
			set;
		}

		public DateTime? PayTime
		{
			get;
			set;
		}

		public OrderPayInfo()
		{
		}
	}
}