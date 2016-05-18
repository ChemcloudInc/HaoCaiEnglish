using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public abstract class EventRequest : AbstractRequest
	{
		public virtual RequestEventType Event
		{
			get;
			set;
		}

		protected EventRequest()
		{
		}
	}
}