using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ApplyWithDrawInfo : BaseModel
	{
		private long _id;

		public decimal ApplyAmount
		{
			get;
			set;
		}

		public ApplyWithDrawInfo.ApplyWithDrawStatus ApplyStatus
		{
			get;
			set;
		}

		public DateTime ApplyTime
		{
			get;
			set;
		}

		public DateTime? ConfirmTime
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

		public string NickName
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public string OpUser
		{
			get;
			set;
		}

		public string PayNo
		{
			get;
			set;
		}

		public DateTime? PayTime
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public ApplyWithDrawInfo()
		{
		}

		public enum ApplyWithDrawStatus
		{
			[Description("待处理")]
			WaitConfirm = 1,
			[Description("付款失败")]
			PayFail = 2,
			[Description("提现成功")]
			WithDrawSuccess = 3,
			[Description("已拒绝")]
			Refuse = 4
		}


        public long AccountId                                   //账号id;
        {
            get;
            set;
        }

	}
}