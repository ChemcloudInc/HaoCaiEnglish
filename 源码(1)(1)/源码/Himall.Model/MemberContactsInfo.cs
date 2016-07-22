using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberContactsInfo : BaseModel
	{
		private long _id;

		public string Contact
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

		public string ServiceProvider
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public MemberContactsInfo.UserTypes UserType
		{
			get;
			set;
		}

		public MemberContactsInfo()
		{
		}

		public enum UserTypes
		{
			[Description("普通用户")]
			General,
			[Description("店铺用户")]
			ShopManager
		}
	}
}