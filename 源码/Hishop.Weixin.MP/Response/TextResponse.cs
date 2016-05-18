using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Response
{
	public class TextResponse : AbstractResponse
	{
		public string Content
		{
			get;
			set;
		}

		public override ResponseMsgType MsgType
		{
			get
			{
				return ResponseMsgType.Text;
			}
		}

		public TextResponse()
		{
		}
	}
}