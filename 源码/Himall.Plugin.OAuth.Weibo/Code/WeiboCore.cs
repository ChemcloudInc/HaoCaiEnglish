using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.OAuth.Weibo.Code
{
	public class WeiboCore
	{
		public static string WorkDirectory
		{
			get;
			set;
		}

		public WeiboCore()
		{
		}

		public static OAuthWeiboConfig GetConfig()
		{
			OAuthWeiboConfig oAuthWeiboConfig;
			FileStream fileStream = new FileStream(string.Concat(WeiboCore.WorkDirectory, "\\Weibo.config"), FileMode.Open);
			try
			{
				oAuthWeiboConfig = (OAuthWeiboConfig)(new XmlSerializer(typeof(OAuthWeiboConfig))).Deserialize(fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
			return oAuthWeiboConfig;
		}

		public static void SaveConfig(OAuthWeiboConfig config)
		{
			FileStream fileStream = new FileStream(string.Concat(WeiboCore.WorkDirectory, "\\Weibo.config"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(OAuthWeiboConfig))).Serialize(fileStream, config);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
		}
	}
}