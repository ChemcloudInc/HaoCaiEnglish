using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.OAuth.QQ
{
	internal class QQCore
	{
		public static string WorkDirectory
		{
			get;
			set;
		}

		public QQCore()
		{
		}

		public static OAuthQQConfig GetConfig()
		{
			OAuthQQConfig oAuthQQConfig;
			FileStream fileStream = new FileStream(string.Concat(QQCore.WorkDirectory, "\\QQ.config"), FileMode.Open);
			try
			{
				oAuthQQConfig = (OAuthQQConfig)(new XmlSerializer(typeof(OAuthQQConfig))).Deserialize(fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
			return oAuthQQConfig;
		}

		public static void SaveConfig(OAuthQQConfig config)
		{
			FileStream fileStream = new FileStream(string.Concat(QQCore.WorkDirectory, "\\QQ.config"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(OAuthQQConfig))).Serialize(fileStream, config);
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