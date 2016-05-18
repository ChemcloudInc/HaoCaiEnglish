using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXSyncJSInfoCardInfo
	{
		public string card_id
		{
			get;
			set;
		}

		public string nonce_str
		{
			get;
			set;
		}

		public int outerid
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

		public WXSyncJSInfoCardInfo()
		{
		}
	}
}