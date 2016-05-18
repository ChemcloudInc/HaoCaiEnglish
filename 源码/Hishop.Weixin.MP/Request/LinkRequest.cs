using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class LinkRequest : AbstractRequest
	{
		public string Description
		{
			get;
			set;
		}

		public int Title
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public LinkRequest()
		{
		}
	}
}