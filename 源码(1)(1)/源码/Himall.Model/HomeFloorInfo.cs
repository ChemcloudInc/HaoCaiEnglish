using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class HomeFloorInfo : BaseModel
	{
		private long _id;

		public string DefaultTabName
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FloorBrandInfo> FloorBrandInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FloorCategoryInfo> FloorCategoryInfo
		{
			get;
			set;
		}

		public string FloorName
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FloorProductInfo> FloorProductInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FloorTopicInfo> FloorTopicInfo
		{
			get;
			set;
		}

		public virtual ICollection<FloorTablsInfo> Himall_FloorTabls
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

		public bool IsShow
		{
			get;
			set;
		}

		public long StyleLevel
		{
			get;
			set;
		}

		public string SubName
		{
			get;
			set;
		}

		public HomeFloorInfo()
		{
            FloorBrandInfo = new HashSet<Himall.Model.FloorBrandInfo>();
            FloorCategoryInfo = new HashSet<Himall.Model.FloorCategoryInfo>();
            FloorProductInfo = new HashSet<Himall.Model.FloorProductInfo>();
            FloorTopicInfo = new HashSet<Himall.Model.FloorTopicInfo>();
            Himall_FloorTabls = new HashSet<FloorTablsInfo>();
		}
	}
}