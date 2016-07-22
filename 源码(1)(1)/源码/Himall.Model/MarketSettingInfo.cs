using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MarketSettingInfo : BaseModel
	{
		private int _id;

		public virtual ICollection<MarketSettingMetaInfo> Himall_MarketSettingMeta
		{
			get;
			set;
		}

		public new int Id
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

		public decimal Price
		{
			get;
			set;
		}

		public MarketType TypeId
		{
			get;
			set;
		}

		public MarketSettingInfo()
		{
            Himall_MarketSettingMeta = new HashSet<MarketSettingMetaInfo>();
		}
	}
}