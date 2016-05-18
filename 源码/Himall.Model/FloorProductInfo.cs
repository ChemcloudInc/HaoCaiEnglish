using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FloorProductInfo : BaseModel
	{
		private long _id;

		public long FloorId
		{
			get;
			set;
		}

		public virtual Himall.Model.HomeFloorInfo HomeFloorInfo
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

		public int Tab
		{
			get;
			set;
		}

		public FloorProductInfo()
		{
		}
	}
}