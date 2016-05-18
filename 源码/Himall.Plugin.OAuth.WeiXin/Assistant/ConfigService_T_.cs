using System;
using System.IO;
using System.Xml.Serialization;

namespace Himall.Plugin.OAuth.WeiXin.Assistant
{
	public class ConfigService<T>
	where T : class
	{
		public ConfigService()
		{
		}

		public static T GetConfig(string filename)
		{
			T t;
			FileStream fileStream = new FileStream(filename, FileMode.Open);
			try
			{
				t = (T)(new XmlSerializer(typeof(T))).Deserialize(fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
			return t;
		}

		public static void SaveConfig(T config, string filename)
		{
			FileStream fileStream = new FileStream(filename, FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(T))).Serialize(fileStream, config);
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