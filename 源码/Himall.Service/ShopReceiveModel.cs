using System;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ShopReceiveModel
	{
		public long Id
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public ShopReceiveStatus State
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public ShopReceiveModel()
		{
		}
	}
}