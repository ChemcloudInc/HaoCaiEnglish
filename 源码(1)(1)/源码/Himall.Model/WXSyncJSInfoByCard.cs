using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXSyncJSInfoByCard
	{
		public string apiticket
		{
			get;
			set;
		}

		public string appid
		{
			get;
			set;
		}

		public string nonceStr
		{
			get;
			set;
		}

		public string signature
		{
			get;
			set;
		}

		public string timestamp
		{
			get;
			set;
		}

		public WXSyncJSInfoByCard()
		{
		}
	}
}