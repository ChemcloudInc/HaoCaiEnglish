using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Domain
{
	public class Music : IThumbMedia
	{
		public string Description
		{
			get;
			set;
		}

		public string HQMusicUrl
		{
			get;
			set;
		}

		public string MusicUrl
		{
			get;
			set;
		}

		public int ThumbMediaId
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public Music()
		{
		}
	}
}