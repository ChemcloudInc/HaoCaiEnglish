using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShoppingCartItem
	{
		public DateTime AddTime
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public string SkuId
		{
			get;
			set;
		}

		public ShoppingCartItem()
		{
		}
	}
}