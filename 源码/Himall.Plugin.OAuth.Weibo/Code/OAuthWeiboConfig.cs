using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.Weibo.Code
{
	public class OAuthWeiboConfig
	{
		public string AppKey
		{
			get;
			set;
		}

		public string AppSecret
		{
			get;
			set;
		}

		public string AuthorizeURL
		{
			get;
			set;
		}

		public string ValidateContent
		{
			get;
			set;
		}

		public OAuthWeiboConfig()
		{
		}
	}
}