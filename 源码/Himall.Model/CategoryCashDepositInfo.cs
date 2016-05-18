using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CategoryCashDepositInfo : BaseModel
	{
		private long _id;

		public virtual CategoryInfo CategoriesInfo
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public bool EnableNoReasonReturn
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

		public decimal NeedPayCashDeposit
		{
			get;
			set;
		}

		public CategoryCashDepositInfo()
		{
		}
	}
}