using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Domain;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Response
{
	public class ImageResponse : AbstractResponse
	{
		public Hishop.Weixin.MP.Domain.Image Image
		{
			get;
			set;
		}

		public override ResponseMsgType MsgType
		{
			get
			{
				return ResponseMsgType.Image;
			}
		}

		public ImageResponse()
		{
		}
	}
}