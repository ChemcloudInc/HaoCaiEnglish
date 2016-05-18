using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ActiveMarketServiceInfo : BaseModel
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

		public virtual ICollection<Himall.Model.MarketServiceRecordInfo> MarketServiceRecordInfo
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public MarketType TypeId
		{
			get;
			set;
		}

		public ActiveMarketServiceInfo()
		{
            MarketServiceRecordInfo = new HashSet<Himall.Model.MarketServiceRecordInfo>();
		}
	}
}