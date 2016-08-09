using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile
{
	public class OrderDetail
	{
		public IEnumerable<OrderItem> OrderItems
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public OrderDetail()
		{
		}
	}
}