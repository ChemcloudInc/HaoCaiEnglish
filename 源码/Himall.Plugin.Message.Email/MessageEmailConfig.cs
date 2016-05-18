using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.Message.Email
{
	public class MessageEmailConfig
	{
		public string DisplayName
		{
			get;
			set;
		}

		public string EmailName
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string SendAddress
		{
			get;
			set;
		}

		public string SmtpPort
		{
			get;
			set;
		}

		public string SmtpServer
		{
			get;
			set;
		}

		public MessageEmailConfig()
		{
		}
	}
}