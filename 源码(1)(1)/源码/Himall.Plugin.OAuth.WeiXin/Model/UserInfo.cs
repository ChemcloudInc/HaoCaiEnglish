using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.WeiXin.Model
{
	public class UserInfo
	{
		public string city
		{
			get;
			set;
		}

		public string country
		{
			get;
			set;
		}

		public string headimgurl
		{
			get;
			set;
		}

		public string nickname
		{
			get;
			set;
		}

		public string openid
		{
			get;
			set;
		}

		public string[] privilege
		{
			get;
			set;
		}

		public string province
		{
			get;
			set;
		}

		public int sex
		{
			get;
			set;
		}

		public string unionid
		{
			get;
			set;
		}

		public UserInfo()
		{
		}
	}
}