using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AccountPurchaseAgreementInfo : BaseModel
	{
		private long _id;

		public long? AccountId
		{
			get;
			set;
		}

		public decimal AdvancePayment
		{
			get;
			set;
		}

		public DateTime? ApplyDate
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public DateTime FinishDate
		{
			get;
			set;
		}

		public virtual AccountInfo Himall_Accounts
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

		public long PurchaseAgreementId
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public AccountPurchaseAgreementInfo()
		{
		}
	}
}