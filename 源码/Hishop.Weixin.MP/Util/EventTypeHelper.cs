using Hishop.Weixin.MP;
using System;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Util
{
	public static class EventTypeHelper
	{
		public static RequestEventType GetEventType(XDocument doc)
		{
			RequestEventType eventType = EventTypeHelper.GetEventType(doc.Root.Element("Event").Value);
			return eventType;
		}

		public static RequestEventType GetEventType(string str)
		{
			return (RequestEventType)Enum.Parse(typeof(RequestEventType), str, true);
		}
	}
}