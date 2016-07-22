using System;
using System.IO;
using System.Xml.Serialization;

namespace Himall.PaymentPlugin
{
	public static class Utility<T>
	where T : ConfigBase, new()
	{
		public static T GetConfig(string workDirectory)
		{
			T t = Activator.CreateInstance<T>();
			FileStream fileStream = new FileStream(string.Concat(workDirectory, "\\data.config"), FileMode.Open);
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

		public static void SaveConfig(T config, string workDirectory)
		{
			FileStream fileStream = new FileStream(string.Concat(workDirectory, "\\data.config"), FileMode.Create);
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