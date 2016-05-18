using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Domain;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Response
{
	public class VoiceResponse : AbstractResponse
	{
		public override ResponseMsgType MsgType
		{
			get
			{
				return ResponseMsgType.Voice;
			}
		}

		public Hishop.Weixin.MP.Domain.Voice Voice
		{
			get;
			set;
		}

		public VoiceResponse()
		{
		}
	}
}