using Himall.Core;
using Himall.Core.Plugins.Message;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.Message.Email
{
	internal class EmailCore
	{
		public static string WorkDirectory
		{
			get;
			set;
		}

		public EmailCore()
		{
		}

		public static MessageEmailConfig GetConfig()
		{
			MessageEmailConfig messageEmailConfig;
			FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "\\Data\\Email.config"), FileMode.Open);
			try
			{
				messageEmailConfig = (MessageEmailConfig)(new XmlSerializer(typeof(MessageEmailConfig))).Deserialize(fileStream);
			}
			finally
			{
				if (fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
			return messageEmailConfig;
		}

		public static MessageContent GetMessageContentConfig()
		{
			MessageContent messageContent = Cache.Get("MessageContent") as MessageContent;
			if (messageContent == null)
			{
				FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "\\Data\\MessageContent.xml"), FileMode.Open);
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

		public static void SaveConfig(MessageEmailConfig config)
		{
			FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "\\Data\\Email.config"), FileMode.Create);
			try
			{
				(new XmlSerializer(typeof(MessageEmailConfig))).Serialize(fileStream, config);
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
			FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "\\Data\\MessageContent.xml"), FileMode.Create);
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