using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CapitalDetailInfo : BaseModel
	{
		private long _id;

		public decimal Amount
		{
			get;
			set;
		}

		public long CapitalID
		{
			get;
			set;
		}

		public DateTime? CreateTime
		{
			get;
			set;
		}

		public virtual CapitalInfo Himall_Capital
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

		public string Remark
		{
			get;
			set;
		}

		public string SourceData
		{
			get;
			set;
		}

		public CapitalDetailInfo.CapitalDetailType SourceType
		{
			get;
			set;
		}

		public CapitalDetailInfo()
		{
		}

		public enum CapitalDetailType
		{
			[Description("CashCoupon")]
			RedPacket = 1,
            [Description("ChargeAmount")]
			ChargeAmount = 2,
            [Description("WithDraw")]
			WithDraw = 3,
            [Description("Consume")]
			Consume = 4,
            [Description("Refund")]
			Refund = 5
		}
	}
}