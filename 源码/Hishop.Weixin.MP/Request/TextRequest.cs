using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class TextRequest : AbstractRequest
	{
		public string Content
		{
			get;
			set;
		}

		public TextRequest()
		{
		}
	}
}