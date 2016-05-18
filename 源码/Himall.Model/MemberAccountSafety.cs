using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberAccountSafety
	{
		public int AccountSafetyLevel
		{
			get;
			set;
		}

		public bool BindEmail
		{
			get;
			set;
		}

		public bool BindPhone
		{
			get;
			set;
		}

		public bool PayPassword
		{
			get;
			set;
		}

		public MemberAccountSafety()
		{
		}
	}
}