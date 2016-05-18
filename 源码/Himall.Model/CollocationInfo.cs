using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CollocationInfo : BaseModel
	{
		private long _id;

		public DateTime? CreateTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public virtual ICollection<CollocationPoruductInfo> Himall_CollocationPoruducts
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

		[NotMapped]
		public long ProductId
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		[NotMapped]
		public string ShopName
		{
			get;
			set;
		}

		public string ShortDesc
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public string Status
		{
			get
			{
				if (EndTime < DateTime.Now)
				{
					return "已结束";
				}
				if (StartTime > DateTime.Now)
				{
					return "未开始";
				}
				return "进行中";
			}
		}

		public string Title
		{
			get;
			set;
		}

		public CollocationInfo()
		{
            Himall_CollocationPoruducts = new HashSet<CollocationPoruductInfo>();
		}
	}
}