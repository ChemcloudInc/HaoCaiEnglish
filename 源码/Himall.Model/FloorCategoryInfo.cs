using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FloorCategoryInfo : BaseModel
	{
		private long _id;

		public long CategoryId
		{
			get;
			set;
		}

		public virtual Himall.Model.CategoryInfo CategoryInfo
		{
			get;
			set;
		}

		public int Depth
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

		public FloorCategoryInfo()
		{
		}
	}
}