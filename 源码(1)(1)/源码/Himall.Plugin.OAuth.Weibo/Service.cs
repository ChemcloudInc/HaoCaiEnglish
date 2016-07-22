using Himall.Core.Plugins;
using Himall.Core.Plugins.OAuth;
using Himall.Plugin.OAuth.Weibo.Code;
using NetDimension.Weibo;
using NetDimension.Weibo.Entities.user;
using NetDimension.Weibo.Interface;
using NetDimension.Weibo.Interface.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.Weibo
{
	public class Service : IOAuthPlugin, IPlugin
	{
		private static string ReturnUrl;

		public string Icon_Default
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WeiboCore.WorkDirectory))
				{
					throw new MissingFieldException("没有设置插件工作目录");
				}
				return string.Concat(WeiboCore.WorkDirectory, "/weibo1.png");
			}
		}

		public string Icon_Hover
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WeiboCore.WorkDirectory))
				{
					throw new MissingFieldException("没有设置插件工作目录");
				}
				return string.Concat(WeiboCore.WorkDirectory, "/weibo2.png");
			}
		}

		public string ShortName
		{
			get
			{
				return "新浪微博";
			}
		}

		public string WorkDirectory
		{
			set
			{
				WeiboCore.WorkDirectory = value;
			}
		}

		static Service()
		{
			Service.ReturnUrl = string.Empty;
		}

		public Service()
		{
		}

		public void CheckCanEnable()
		{
		}

		public FormData GetFormData()
		{
			OAuthWeiboConfig config = WeiboCore.GetConfig();
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[3];
			FormData.FormItem formItem = new FormData.FormItem();
			formItem.DisplayName="AppKey";
			formItem.Name="AppKey";
			formItem.IsRequired=true;
			formItem.Type=FormData.FormItemType.text;
			formItem.Value=config.AppKey;
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem();
			formItem1.DisplayName="AppSecret";
			formItem1.Name="AppSecret";
			formItem1.IsRequired=true;
			formItem1.Type= FormData.FormItemType.text;
            formItem1.Value=config.AppSecret;
			formItemArray[1] = formItem1;
			FormData.FormItem formItem2 = new FormData.FormItem();
			formItem2.DisplayName="验证内容";
			formItem2.Name="ValidateContent";
			formItem2.IsRequired=true;
			formItem2.Type= FormData.FormItemType.text;
            formItem2.Value=config.ValidateContent;
			formItemArray[2] = formItem2;
			formDatum.Items=formItemArray;
			return formDatum;
		}

		public string GetOpenLoginUrl(string returnUrl)
		{
			Service.ReturnUrl = returnUrl;
			OAuthWeiboConfig config = WeiboCore.GetConfig();
			string str = string.Format(string.Concat(config.AuthorizeURL, "?client_id={0}&response_type=code&redirect_uri={1}"), config.AppKey, returnUrl);
			return str;
		}

		public OAuthUserInfo GetUserInfo(NameValueCollection queryString)
		{
			OAuthWeiboConfig config = WeiboCore.GetConfig();
            NetDimension.Weibo.OAuth oAuth = new NetDimension.Weibo.OAuth(config.AppKey, config.AppSecret, Service.ReturnUrl);
			Client client = new Client(oAuth);
			oAuth.GetAccessTokenByAuthorizationCode(queryString["code"]);
			OAuthUserInfo oAuthUserInfo = null;
			if (oAuth != null)
			{
				oAuthUserInfo = new OAuthUserInfo();
				oAuthUserInfo.OpenId=client.API.Entity.Account.GetUID();
				Entity entity = client.API.Entity.Users.Show(oAuthUserInfo.OpenId, "");
				string name = entity.Name;
				string str = name;
				oAuthUserInfo.RealName=name;
				oAuthUserInfo.NickName=str;
				oAuthUserInfo.IsMale=new bool?(entity.Gender == "m");
			}
			return oAuthUserInfo;
		}

		public string GetValidateContent()
		{
			return WeiboCore.GetConfig().ValidateContent;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppKey");
			if (string.IsNullOrWhiteSpace(keyValuePair.Value))
			{
				throw new ArgumentNullException("AppKey不能为空");
			}
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppSecret");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new ArgumentNullException("AppSecret不能为空");
			}
			KeyValuePair<string, string> keyValuePair2 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "ValidateContent");
			if (!string.IsNullOrWhiteSpace(keyValuePair2.Value))
			{
				string lower = keyValuePair2.Value.ToLower();
				if (!lower.StartsWith("<meta "))
				{
					throw new PluginException("验证内容必须以meta标签开头");
				}
				if (!lower.EndsWith(" />"))
				{
					throw new PluginException("验证内容必须以 /> 结尾");
				}
			}
			OAuthWeiboConfig config = WeiboCore.GetConfig();
			config.AppSecret = keyValuePair1.Value;
			config.AppKey = keyValuePair.Value;
			config.ValidateContent = keyValuePair2.Value;
			WeiboCore.SaveConfig(config);
		}
	}
}