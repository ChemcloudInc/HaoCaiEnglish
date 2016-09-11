using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberIntegral : BaseModel
	{
		private long _id;

		public int AvailableIntegrals
		{
			get;
			set;
		}

		public virtual UserMemberInfo Himall_Members
		{
			get;
			set;
		}

		public int HistoryIntegrals
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

		public long? MemberId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public MemberIntegral()
		{
		}

		public enum IntegralType
		{
            [Description("Consumption")]
			Consumption = 1,
            [Description("Exchange")]
			Exchange = 2,
            [Description("InvitationMemberRegiste")]
			InvitationMemberRegiste = 3,
            [Description("Login")]
			Login = 5,
            [Description("BindWX")]
			BindWX = 6,
            [Description("Comment")]
			Comment = 7,
            [Description("SystemOper")]
			SystemOper = 8,
            [Description("Reg")]
			Reg = 9,
            [Description("Cancel")]
			Cancel = 10,
            [Description("Others")]
			Others = 11
		}

		public enum VirtualItemType
		{
            [Description("Exchange")]
			Exchange = 1,
            [Description("InvitationMember")]
			InvitationMember = 2,
            [Description("ProportionRebate")]
			ProportionRebate = 3,
            [Description("Comment")]
			Comment = 4,
            [Description("Consumption")]
			Consumption = 5,
            [Description("Cancel")]
			Cancel = 6
		}
	}
}