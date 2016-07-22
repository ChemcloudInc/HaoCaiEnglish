using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile
{
	public class OrderItem
	{
		public long Count
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductImage
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public OrderItem()
		{
		}
	}
}