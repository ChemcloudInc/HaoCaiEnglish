using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP
{
	public class AbstractResponse
	{
		public DateTime CreateTime
		{
			get;
			set;
		}

		public string FromUserName
		{
			get;
			set;
		}

		public bool FuncFlag
		{
			get;
			set;
		}

		public virtual ResponseMsgType MsgType
		{
			get;
			set;
		}

		public string ToUserName
		{
			get;
			set;
		}

		public AbstractResponse()
		{
            CreateTime = DateTime.Now;
		}
	}
}