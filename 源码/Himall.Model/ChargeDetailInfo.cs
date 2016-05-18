using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ChargeDetailInfo : BaseModel
	{
		private long _id;

		public decimal ChargeAmount
		{
			get;
			set;
		}

		public ChargeDetailInfo.ChargeDetailStatus ChargeStatus
		{
			get;
			set;
		}

		public DateTime? ChargeTime
		{
			get;
			set;
		}

		public string ChargeWay
		{
			get;
			set;
		}

		public DateTime? CreateTime
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

		public long MemId
		{
			get;
			set;
		}

		public ChargeDetailInfo()
		{
		}

		public enum ChargeDetailStatus
		{
			[Description("未付款")]
			WaitPay = 1,
			[Description("充值成功")]
			ChargeSuccess = 2
		}
	}
}