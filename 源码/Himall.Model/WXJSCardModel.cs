using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXJSCardModel
	{
		public WXJSCardExtModel cardExt
		{
			get;
			set;
		}

		public string cardId
		{
			get;
			set;
		}

		public WXJSCardModel()
		{
            cardExt = new WXJSCardExtModel();
		}
	}
}