using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopBonusReceiveInfo : BaseModel
	{
		private long _id;

		public long BonusGrantId
		{
			get;
			set;
		}

		public virtual UserMemberInfo Himall_Members
		{
			get;
			set;
		}

		public virtual ShopBonusGrantInfo Himall_ShopBonusGrant
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

		public string OpenId
		{
			get;
			set;
		}

		public decimal? Price
		{
			get;
			set;
		}

		public DateTime? ReceiveTime
		{
			get;
			set;
		}

		public ShopBonusReceiveInfo.ReceiveState State
		{
			get;
			set;
		}

		public long? UsedOrderId
		{
			get;
			set;
		}

		public DateTime? UsedTime
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public string WXHead
		{
			get;
			set;
		}

		public string WXName
		{
			get;
			set;
		}

		public ShopBonusReceiveInfo()
		{
		}

		public enum ReceiveState
		{
			[Description("未使用")]
			NotUse = 1,
			[Description("已使用")]
			Use = 2,
			[Description("已过期")]
			Expired = 3
		}
	}
}