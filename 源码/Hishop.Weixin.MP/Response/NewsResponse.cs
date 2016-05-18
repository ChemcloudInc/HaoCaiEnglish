using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Response
{
	public class NewsResponse : AbstractResponse
	{
		public int ArticleCount
		{
			get
			{
				return (Articles == null ? 0 : Articles.Count);
			}
		}

		public IList<Article> Articles
		{
			get;
			set;
		}

		public override ResponseMsgType MsgType
		{
			get
			{
				return ResponseMsgType.News;
			}
		}

		public NewsResponse()
		{
		}
	}
}