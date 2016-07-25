using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class VideoRequest : AbstractRequest
	{
		public int MediaId
		{
			get;
			set;
		}

		public int ThumbMediaId
		{
			get;
			set;
		}

		public VideoRequest()
		{
		}
	}
}