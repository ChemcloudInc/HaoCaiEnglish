using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.Weibo
{
	public class WeiboConfig
	{
		public string AppId
		{
			get;
			set;
		}

		public string AppKey
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

		public WeiboConfig()
		{
		}
	}
}