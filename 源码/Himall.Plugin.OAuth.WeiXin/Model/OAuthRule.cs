using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.WeiXin.Model
{
	public class OAuthRule
	{
		public string GetCodeUrl
		{
			get;
			set;
		}

		public string GetTokenUrl
		{
			get;
			set;
		}

		public string GetUserInfoUrl
		{
			get;
			set;
		}

		public OAuthRule()
		{
		}
	}
}