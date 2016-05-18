using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Util;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Script.Serialization;

namespace Hishop.Weixin.MP.Api
{
	public class TokenApi
	{
		public string AppId
		{
			get
			{
				return ConfigurationManager.AppSettings.Get("AppId");
			}
		}

		public string AppSecret
		{
			get
			{
				return ConfigurationManager.AppSettings.Get("AppSecret");
			}
		}

		public TokenApi()
		{
		}

		public static string GetToken(string appid, string secret)
		{
			string str = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
			return (new WebUtils()).DoGet(str, null);
		}

		public static string GetToken_Message(string appid, string secret)
		{
			string str = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
			string accessToken = (new WebUtils()).DoGet(str, null);
			if (accessToken.Contains("access_token"))
			{
				accessToken = (new JavaScriptSerializer()).Deserialize<Token>(accessToken).access_token;
			}
			return accessToken;
		}
	}
}