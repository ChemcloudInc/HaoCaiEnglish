using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FloorBrandInfo : BaseModel
	{
		private long _id;

		public long BrandId
		{
			get;
			set;
		}

		public virtual Himall.Model.BrandInfo BrandInfo
		{
			get;
			set;
		}

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

		public FloorBrandInfo()
		{
		}
	}
}