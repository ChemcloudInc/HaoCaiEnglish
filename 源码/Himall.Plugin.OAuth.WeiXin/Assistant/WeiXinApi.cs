using Himall.Core.Plugins;
using Himall.Plugin.OAuth.WeiXin;
using Himall.Plugin.OAuth.WeiXin.Model;
using System;

namespace Himall.Plugin.OAuth.WeiXin.Assistant
{
	public class WeiXinApi
	{
		public WeiXinApi()
		{
		}

		public static UserInfo GetUserInfo(string code, string appid, string appsecret)
		{
			UserInfo responseResult = null;
			OAuthRule config = ConfigService<OAuthRule>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthUrl.config"));
			if (string.IsNullOrEmpty(config.GetTokenUrl))
			{
				throw new MissingFieldException("未设置微信接口地址:GetTokenUrl");
			}
			if (string.IsNullOrEmpty(config.GetUserInfoUrl))
			{
				throw new MissingFieldException("未设置微信接口地址:GetUserInfoUrl");
			}
			string str = string.Format(config.GetTokenUrl, appid, appsecret, code);
			HttpHandler.ClientRequest clientRequest = new HttpHandler.ClientRequest(str)
			{
				HttpMethod = "get"
			};
			ErrResult errResult = new ErrResult();
			TokenResult tokenResult = HttpHandler.GetResponseResult<TokenResult, ErrResult>(clientRequest, errResult);
			if (errResult.errcode > 0)
			{
				throw new PluginException(string.Concat("微信登录接口GetToken出错: ", errResult.errmsg));
			}
			if (string.IsNullOrEmpty(tokenResult.access_token))
			{
				throw new PluginException("微信登录接口返回access_Token为空");
			}
			str = string.Format(config.GetUserInfoUrl, tokenResult.access_token, tokenResult.openid);
			HttpHandler.ClientRequest clientRequest1 = new HttpHandler.ClientRequest(str)
			{
				HttpMethod = "get"
			};
			responseResult = HttpHandler.GetResponseResult<UserInfo, ErrResult>(clientRequest1, errResult);
			if (errResult.errcode > 0)
			{
				throw new PluginException(string.Concat("微信登录接口GetUserInfo出错: ", errResult.errmsg));
			}
			return responseResult;
		}
	}
}