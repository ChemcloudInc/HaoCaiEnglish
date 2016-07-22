using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile.Models
{
	public class WeixinPassiveReply
	{
		public string Content
		{
			get;
			private set;
		}

		public DateTime CreateTime
		{
			get;
			private set;
		}

		public string FromUserName
		{
			get;
			private set;
		}

		public string MsgType
		{
			get
			{
				return "text";
			}
		}

		public string ToUserName
		{
			get;
			private set;
		}

		public WeixinPassiveReply(string to, string form, DateTime time, string content)
		{
            ToUserName = to;
            FromUserName = form;
            CreateTime = time;
            Content = content;
		}
	}
}