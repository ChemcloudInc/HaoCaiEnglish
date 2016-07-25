using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class InviteRuleInfo : BaseModel
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

		public int? InviteIntegral
		{
			get;
			set;
		}

		public int? RegIntegral
		{
			get;
			set;
		}

		public string ShareDesc
		{
			get;
			set;
		}

		public string ShareIcon
		{
			get;
			set;
		}

		public string ShareRule
		{
			get;
			set;
		}

		public string ShareTitle
		{
			get;
			set;
		}

		public InviteRuleInfo()
		{
		}
	}
}