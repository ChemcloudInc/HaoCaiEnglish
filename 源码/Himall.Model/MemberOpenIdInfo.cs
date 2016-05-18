using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MemberOpenIdInfo : BaseModel
	{
		private long _id;

		public MemberOpenIdInfo.AppIdTypeEnum AppIdType
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

		public virtual UserMemberInfo MemberInfo
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public string ServiceProvider
		{
			get;
			set;
		}

		public string UnionId
		{
			get;
			set;
		}

		public string UnionOpenId
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public MemberOpenIdInfo()
		{
		}

		public enum AppIdTypeEnum
		{
			Payment,
			Normal
		}
	}
}