using Hishop.Weixin.MP;
using System;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Util
{
	public static class MsgTypeHelper
	{
		public static RequestMsgType GetMsgType(XDocument doc)
		{
			RequestMsgType msgType = MsgTypeHelper.GetMsgType(doc.Root.Element("MsgType").Value);
			return msgType;
		}

		public static RequestMsgType GetMsgType(string str)
		{
			return (RequestMsgType)Enum.Parse(typeof(RequestMsgType), str, true);
		}
	}
}