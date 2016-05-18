using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberIntegralRule : BaseModel
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

		public int Integral
		{
			get;
			set;
		}

		public int TypeId
		{
			get;
			set;
		}

		public MemberIntegralRule()
		{
		}
	}
}