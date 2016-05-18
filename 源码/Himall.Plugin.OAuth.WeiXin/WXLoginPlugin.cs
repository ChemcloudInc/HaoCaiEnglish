using Himall.Core.Plugins;
using Himall.Core.Plugins.OAuth;
using Himall.Plugin.OAuth.WeiXin.Assistant;
using Himall.Plugin.OAuth.WeiXin.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.WeiXin
{
	public class WXLoginPlugin : IOAuthPlugin, IPlugin
	{
		public static string WXWorkDirectory;

		private static string ReturnUrl;

		public string Icon_Default
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WXLoginPlugin.WXWorkDirectory))
				{
					throw new MissingFieldException("未设置工作目录！");
				}
				return string.Concat(WXLoginPlugin.WXWorkDirectory, "/Resource/weixin1.png");
			}
		}

		public string Icon_Hover
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WXLoginPlugin.WXWorkDirectory))
				{
					throw new MissingFieldException("未设置工作目录！");
				}
				return string.Concat(WXLoginPlugin.WXWorkDirectory, "/Resource/weixin2.png");
			}
		}

		public string ShortName
		{
			get
			{
				return "微信";
			}
		}

		public string WorkDirectory
		{
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentNullException("WorkDirectory不能为空");
				}
				WXLoginPlugin.WXWorkDirectory = value;
			}
		}

		static WXLoginPlugin()
		{
			WXLoginPlugin.WXWorkDirectory = string.Empty;
			WXLoginPlugin.ReturnUrl = string.Empty;
		}

		public WXLoginPlugin()
		{
		}

		public void CheckCanEnable()
		{
			OAuthWXConfigInfo config = ConfigService<OAuthWXConfigInfo>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthWXConfig.config"));
			OAuthRule oAuthRule = ConfigService<OAuthRule>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthUrl.config"));
			if (string.IsNullOrEmpty(config.AppId))
			{
				throw new PluginException("未设置AppId！");
			}
			if (string.IsNullOrEmpty(config.AppSecret))
			{
				throw new PluginException("未设置AppSecret！");
			}
			if (string.IsNullOrEmpty(oAuthRule.GetCodeUrl))
			{
				throw new PluginException("未设置微信接口地址！");
			}
		}

		public FormData GetFormData()
		{
			OAuthWXConfigInfo config = ConfigService<OAuthWXConfigInfo>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthWXConfig.config"));
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[2];
			FormData.FormItem formItem = new FormData.FormItem();
			formItem.DisplayName="AppId";
			formItem.Name="AppId";
			formItem.IsRequired=true;
			formItem.Type=FormData.FormItemType.text;
			formItem.Value=config.AppId;
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem();
			formItem1.DisplayName="AppSecret";
			formItem1.Name="AppSecret";
			formItem1.IsRequired=true;
			formItem1.Type= FormData.FormItemType.text;
            formItem1.Value=config.AppSecret;
			formItemArray[1] = formItem1;
			formDatum.Items=formItemArray;
			return formDatum;
		}

		public string GetOpenLoginUrl(string returnUrl)
		{
			WXLoginPlugin.ReturnUrl = returnUrl;
			OAuthWXConfigInfo config = ConfigService<OAuthWXConfigInfo>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthWXConfig.config"));
			OAuthRule oAuthRule = ConfigService<OAuthRule>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthUrl.config"));
			if (string.IsNullOrEmpty(returnUrl))
			{
				throw new PluginException("未传入回调地址！");
			}
			if (string.IsNullOrEmpty(config.AppId))
			{
				throw new PluginException("未设置AppId！");
			}
			if (string.IsNullOrEmpty(config.AppSecret))
			{
				throw new PluginException("未设置AppSecret！");
			}
			if (string.IsNullOrEmpty(oAuthRule.GetCodeUrl))
			{
				throw new PluginException("未设置微信接口地址！");
			}
			string str = string.Format(oAuthRule.GetCodeUrl, config.AppId, WXLoginPlugin.ReturnUrl);
			return str;
		}

		public OAuthUserInfo GetUserInfo(NameValueCollection queryString)
		{
			OAuthUserInfo oAuthUserInfo = new OAuthUserInfo();
			string empty = string.Empty;
			string item = string.Empty;
			OAuthWXConfigInfo config = ConfigService<OAuthWXConfigInfo>.GetConfig(string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthWXConfig.config"));
			if ((queryString["code"] == null ? false : queryString["state"] != null))
			{
				empty = queryString["code"];
				item = queryString["state"];
				if (string.IsNullOrEmpty(config.AppSecret))
				{
					throw new MissingFieldException("未设置AppSecret！");
				}
				UserInfo userInfo = WeiXinApi.GetUserInfo(empty, config.AppId, config.AppSecret);
				oAuthUserInfo.OpenId=(string.IsNullOrWhiteSpace(userInfo.unionid) ? userInfo.openid : userInfo.unionid);
				oAuthUserInfo.NickName=userInfo.nickname;
				oAuthUserInfo.IsMale=new bool?((userInfo.sex == 0 ? false : true));
			}
			return oAuthUserInfo;
		}

		public string GetValidateContent()
		{
			return string.Empty;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppId");
			if (string.IsNullOrWhiteSpace(keyValuePair.Value))
			{
				throw new PluginException("Appid不能为空");
			}
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppSecret");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new PluginException("AppSecret不能为空");
			}
			OAuthWXConfigInfo oAuthWXConfigInfo = new OAuthWXConfigInfo()
			{
				AppId = keyValuePair.Value,
				AppSecret = keyValuePair1.Value
			};
			ConfigService<OAuthWXConfigInfo>.SaveConfig(oAuthWXConfigInfo, string.Concat(WXLoginPlugin.WXWorkDirectory, "\\Config\\OAuthWXConfig.config"));
		}
	}
}