using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Domain;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Response
{
	public class VideoResponse : AbstractResponse
	{
		public override ResponseMsgType MsgType
		{
			get
			{
				return ResponseMsgType.Video;
			}
		}

		public Hishop.Weixin.MP.Domain.Video Video
		{
			get;
			set;
		}

		public VideoResponse()
		{
		}
	}
}