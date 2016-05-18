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
			[Description("交易获得")]
			Consumption = 1,
			[Description("积分抵扣")]
			Exchange = 2,
			[Description("会员邀请")]
			InvitationMemberRegiste = 3,
			[Description("每日登录")]
			Login = 5,
			[Description("绑定微信")]
			BindWX = 6,
			[Description("订单评价")]
			Comment = 7,
			[Description("管理员操作")]
			SystemOper = 8,
			[Description("完善用户信息")]
			Reg = 9,
			[Description("取消订单")]
			Cancel = 10,
			[Description("其他")]
			Others = 11
		}

		public enum VirtualItemType
		{
			[Description("兑换")]
			Exchange = 1,
			[Description("邀请会员")]
			InvitationMember = 2,
			[Description("返利消费")]
			ProportionRebate = 3,
			[Description("评论")]
			Comment = 4,
			[Description("交易获得")]
			Consumption = 5,
			[Description("取消订单")]
			Cancel = 6
		}
	}
}