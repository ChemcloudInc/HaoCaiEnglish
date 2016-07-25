using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class InviteRecordInfo : BaseModel
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

		public int InviteIntegral
		{
			get;
			set;
		}

		public virtual UserMemberInfo InviteMember
		{
			get;
			set;
		}

		public DateTime? RecordTime
		{
			get;
			set;
		}

		public int? RegIntegral
		{
			get;
			set;
		}

		public virtual UserMemberInfo RegMember
		{
			get;
			set;
		}

		public string RegName
		{
			get;
			set;
		}

		public DateTime? RegTime
		{
			get;
			set;
		}

		public long? RegUserId
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public InviteRecordInfo()
		{
		}
	}
}