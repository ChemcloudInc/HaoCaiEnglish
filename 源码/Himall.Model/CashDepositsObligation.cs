using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CashDepositsObligation
	{
		public bool IsCustomerSecurity
		{
			get;
			set;
		}

		public bool IsSevenDayNoReasonReturn
		{
			get;
			set;
		}

		public bool IsTimelyShip
		{
			get;
			set;
		}

		public CashDepositsObligation()
		{
		}
	}
}