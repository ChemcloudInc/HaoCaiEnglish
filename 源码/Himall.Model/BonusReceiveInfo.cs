using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class BonusReceiveInfo : BaseModel
	{
		private long _id;

		public long BonusId
		{
			get;
			set;
		}

		public virtual BonusInfo Himall_Bonus
		{
			get;
			set;
		}

		public virtual UserMemberInfo Himall_Members
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

		public bool? IsShare
		{
			get;
			set;
		}

		public bool IsTransformedDeposit
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public DateTime? ReceiveTime
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public BonusReceiveInfo()
		{
		}
	}
}