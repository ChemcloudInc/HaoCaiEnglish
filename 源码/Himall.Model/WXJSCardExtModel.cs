using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXJSCardExtModel
	{
		public string nonce_str
		{
			get;
			set;
		}

		public int outer_id
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

		public WXJSCardExtModel()
		{
		}
	}
}