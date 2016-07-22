using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberIntegralExchangeRules : BaseModel
	{
		private long _id;

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

		public int IntegralPerMoney
		{
			get;
			set;
		}

		public int MoneyPerIntegral
		{
			get;
			set;
		}

		public MemberIntegralExchangeRules()
		{
		}
	}
}