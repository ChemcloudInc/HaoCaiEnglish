using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CashDepositDetailInfo : BaseModel
	{
		private long _id;

		public DateTime AddDate
		{
			get;
			set;
		}

		public decimal Balance
		{
			get;
			set;
		}

		public long CashDepositId
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public virtual CashDepositInfo Himall_CashDeposit
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

		public string Operator
		{
			get;
			set;
		}

		public int? RechargeWay
		{
			get;
			set;
		}

		public CashDepositDetailInfo()
		{
		}
	}
}