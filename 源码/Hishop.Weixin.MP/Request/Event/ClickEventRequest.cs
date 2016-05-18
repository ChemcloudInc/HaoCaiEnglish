using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request.Event
{
	public class ClickEventRequest : EventRequest
	{
		public override RequestEventType Event
		{
			get
			{
				return RequestEventType.Click;
			}
			set
			{
			}
		}

		public string EventKey
		{
			get;
			set;
		}

		public ClickEventRequest()
		{
		}
	}
}