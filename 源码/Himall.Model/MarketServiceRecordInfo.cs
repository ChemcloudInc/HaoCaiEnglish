using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MarketServiceRecordInfo : BaseModel
	{
		private long _id;

		public virtual Himall.Model.ActiveMarketServiceInfo ActiveMarketServiceInfo
		{
			get;
			set;
		}

		public DateTime EndTime
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

		public long MarketServiceId
		{
			get;
			set;
		}

		public long SettlementFlag
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public MarketServiceRecordInfo()
		{
		}
	}
}