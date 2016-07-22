using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Framework
{
	internal class MobileOAuthUserInfo
	{
		public string Headimgurl
		{
			get;
			set;
		}

		public string LoginProvider
		{
			get;
			set;
		}

		public string NickName
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public string RealName
		{
			get;
			set;
		}

		public string UnionId
		{
			get;
			set;
		}

		public MobileOAuthUserInfo()
		{
		}
	}
}