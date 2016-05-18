using Himall.Core;
using Himall.Core.Plugins.Message;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.Message.SMS
{
	internal class SMSCore
	{
		public static string WorkDirectory
		{
			get;
			set;
		}

		public SMSCore()
		{
		}

		public static MessageSMSConfig GetConfig()
		{
			MessageSMSConfig messageSMSConfig;
			FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "\\Data\\SMS.config"), FileMode.Open);
			try
			{
				messageSMSConfig = (MessageSMSConfig)(new XmlSerializer(typeof(MessageSMSConfig))).Deserialize(fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
			return messageSMSConfig;
		}

		public static MessageContent GetMessageContentConfig()
		{
			MessageContent messageContent = Cache.Get("MessageContent") as MessageContent;
			if (messageContent == null)
			{
				FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "\\Data\\MessageContent.xml"), FileMode.Open);
				try
				{
					messageContent = (MessageContent)(new XmlSerializer(typeof(MessageContent))).Deserialize(fileStream);
					Cache.Insert("MessageContent", messageContent);
				}
				finally
				{
					if (fileStream != null)
					{
						((IDisposable)fileStream).Dispose();
					}
				}
			}
			return messageContent;
		}

		public static void SaveConfig(MessageSMSConfig config)
		{
			FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "\\Data\\SMS.config"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(MessageSMSConfig))).Serialize(fileStream, config);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
		}

		public static void SaveMessageContentConfig(MessageContent config)
		{
			FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "\\Data\\MessageContent.xml"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(MessageContent))).Serialize(fileStream, config);
				Cache.Insert("MessageContent", config);
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