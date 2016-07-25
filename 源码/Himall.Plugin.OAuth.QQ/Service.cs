using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.OAuth;
using QConnectSDK;
using QConnectSDK.Config;
using QConnectSDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.OAuth.QQ
{
	public class Service : IOAuthPlugin, IPlugin
	{
		private static string ReturnUrl;

		public string Icon_Default
		{
			get
			{
				if (string.IsNullOrWhiteSpace(QQCore.WorkDirectory))
				{
					throw new MissingFieldException("没有设置插件工作目录");
				}
				return string.Concat(QQCore.WorkDirectory, "/qq1.png");
			}
		}

		public string Icon_Hover
		{
			get
			{
				if (string.IsNullOrWhiteSpace(QQCore.WorkDirectory))
				{
					throw new MissingFieldException("没有设置插件工作目录");
				}
				return string.Concat(QQCore.WorkDirectory, "/qq2.png");
			}
		}

		public string ShortName
		{
			get
			{
				return "QQ";
			}
		}

		public string WorkDirectory
		{
			set
			{
				QQCore.WorkDirectory = value;
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
			OAuthQQConfig config = QQCore.GetConfig();
			if (string.IsNullOrWhiteSpace(config.AppId))
			{
				throw new PluginConfigException("未设置AppId");
			}
			if (string.IsNullOrWhiteSpace(config.AppKey))
			{
				throw new PluginConfigException("未设置AppKey");
			}
			if (string.IsNullOrWhiteSpace(config.AuthorizeURL))
			{
				throw new PluginConfigException("未设置授权地址(AuthorizeURL)");
			}
		}

		public FormData GetFormData()
		{
			OAuthQQConfig config = QQCore.GetConfig();
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[3];
			FormData.FormItem formItem = new FormData.FormItem();
			formItem.DisplayName="AppId";
			formItem.Name="AppId";
			formItem.IsRequired=true;
			formItem.Type= FormData.FormItemType.text;
			formItem.Value=config.AppId;
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem();
			formItem1.DisplayName="AppKey";
			formItem1.Name="AppKey";
			formItem1.IsRequired=true;
			formItem1.Type= FormData.FormItemType.text;
            formItem1.Value=config.AppKey;
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
			OAuthQQConfig config = QQCore.GetConfig();
			if (string.IsNullOrWhiteSpace(config.AppId))
			{
				throw new MissingFieldException("未配置AppId");
			}
			if (string.IsNullOrWhiteSpace(config.AppKey))
			{
				throw new MissingFieldException("未配置AppKey");
			}
			string str = "test";
			string str1 = "get_user_info";
			string str2 = string.Concat(config.AuthorizeURL, "?response_type=code&client_id={0}&redirect_uri={1}&scope={2}&state={3}");
			object[] appId = new object[] { config.AppId, returnUrl, str1, str };
			return string.Format(str2, appId);
		}

		public OAuthUserInfo GetUserInfo(NameValueCollection queryString)
		{
			bool flag;
			QOpenClient qOpenClient = null;
			string item = queryString["code"];
			string str = queryString["state"];
			OAuthQQConfig config = QQCore.GetConfig();
			string str1 = string.Concat(config.AuthorizeURL, "?grant_type=authorization_code&client_id={0}&state={2}&client_secret={3}&code={4}&redirect_uri={1}");
			object[] appId = new object[] { config.AppId, Service.ReturnUrl, str, config.AppKey, item };
			string.Format(str1, appId);
			QQConnectConfig.SetCallBackUrl(Service.ReturnUrl);
			qOpenClient = new QOpenClient(config.AuthorizeURL, config.AppId, config.AppKey, item, str);
			OAuthUserInfo oAuthUserInfo = null;
			if (qOpenClient != null)
			{
				oAuthUserInfo = new OAuthUserInfo();
				User currentUser = qOpenClient.GetCurrentUser();
				oAuthUserInfo.NickName=currentUser.Nickname;
				oAuthUserInfo.RealName=currentUser.Nickname;
				if (string.IsNullOrWhiteSpace(currentUser.Gender))
				{
					flag = true;
				}
				else
				{
					flag = (currentUser.Gender == "男" ? false : !(currentUser.Gender == "女"));
				}
				if (!flag)
				{
					oAuthUserInfo.IsMale=new bool?((currentUser.Gender == "男" ? true : false));
				}
				oAuthUserInfo.OpenId=qOpenClient.OAuthToken.OpenId;
			}
			return oAuthUserInfo;
		}

		public string GetValidateContent()
		{
			return QQCore.GetConfig().ValidateContent;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppId");
			if (string.IsNullOrWhiteSpace(keyValuePair.Value))
			{
				throw new PluginException("AppId不能为空");
			}
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppKey");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new PluginException("AppKey不能为空");
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
			OAuthQQConfig config = QQCore.GetConfig();
			config.AppId = keyValuePair.Value;
			config.AppKey = keyValuePair1.Value;
			config.ValidateContent = keyValuePair2.Value;
			QQCore.SaveConfig(config);
		}
	}
}