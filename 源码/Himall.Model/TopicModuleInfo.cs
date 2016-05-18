using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class TopicModuleInfo : BaseModel
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

		public virtual ICollection<Himall.Model.ModuleProductInfo> ModuleProductInfo
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public long TopicId
		{
			get;
			set;
		}

		public virtual Himall.Model.TopicInfo TopicInfo
		{
			get;
			set;
		}

		public TopicModuleInfo()
		{
            ModuleProductInfo = new HashSet<Himall.Model.ModuleProductInfo>();
		}
	}
}