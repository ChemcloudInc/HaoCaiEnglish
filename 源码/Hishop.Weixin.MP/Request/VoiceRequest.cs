using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class VoiceRequest : AbstractRequest
	{
		public string Format
		{
			get;
			set;
		}

		public int MediaId
		{
			get;
			set;
		}

		public VoiceRequest()
		{
		}
	}
}