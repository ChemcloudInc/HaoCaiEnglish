using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain
{
	public class Video : IMedia, IThumbMedia
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

		public Video()
		{
		}
	}
}