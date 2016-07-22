using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ApplyWithDrawModel
	{
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

		public string ApplyStatusDesc
		{
			get;
			set;
		}

		public string ApplyTime
		{
			get;
			set;
		}

		public string ConfirmTime
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string MemberName
		{
			get;
			set;
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

		public string PayTime
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public ApplyWithDrawModel()
		{
		}

        public long AccountId
        {
            get;
            set;
        }

        public string WithdrawType
        {
            get;
            set;
        }
	}
}