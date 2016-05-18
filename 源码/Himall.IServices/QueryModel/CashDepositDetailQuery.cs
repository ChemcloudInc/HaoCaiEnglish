using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CashDepositDetailQuery : QueryBase
	{
		public long CashDepositId
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public string Operator
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public CashDepositDetailQuery()
		{
		}
	}
}