using Hishop.Weixin.MP;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request
{
	public class ImageRequest : AbstractRequest
	{
		public int MediaId
		{
			get;
			set;
		}

		public string PicUrl
		{
			get;
			set;
		}

		public ImageRequest()
		{
		}
	}
}