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
			[Description("领取红包")]
			RedPacket = 1,
			[Description("充值")]
			ChargeAmount = 2,
			[Description("提现")]
			WithDraw = 3,
			[Description("消费")]
			Consume = 4,
			[Description("退款")]
			Refund = 5
		}
	}
}