using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShoppingCartInfo
	{
		public IEnumerable<ShoppingCartItem> Items
		{
			get;
			set;
		}

		public long MemberId
		{
			get;
			set;
		}

		public ShoppingCartInfo()
		{
            Items = new ShoppingCartItem[0];
		}
	}
}