using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request;
using System;

namespace Hishop.Weixin.MP.Request.Event
{
	public class SubscribeEventRequest : EventRequest
	{
		public override RequestEventType Event
		{
			get
			{
				return RequestEventType.Subscribe;
			}
			set
			{
			}
		}

		public SubscribeEventRequest()
		{
		}
	}
}