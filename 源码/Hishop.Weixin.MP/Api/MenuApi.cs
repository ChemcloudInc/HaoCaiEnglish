using Hishop.Weixin.MP.Util;
using System;

namespace Hishop.Weixin.MP.Api
{
	public class MenuApi
	{
		public MenuApi()
		{
		}

		public static string CreateMenus(string accessToken, string json)
		{
			string str = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", accessToken);
			return (new WebUtils()).DoPost(str, json);
		}

		public static string DeleteMenus(string accessToken)
		{
			string str = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", accessToken);
			return (new WebUtils()).DoGet(str, null);
		}

		public static string GetMenus(string accessToken)
		{
			string str = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", accessToken);
			return (new WebUtils()).DoGet(str, null);
		}
	}
}