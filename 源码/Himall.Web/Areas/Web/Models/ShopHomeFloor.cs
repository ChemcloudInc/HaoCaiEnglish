using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ShopHomeFloor
	{
		public string FloorName
		{
			get;
			set;
		}

		public List<ShopHomeFloorProduct> Products
		{
			get;
			set;
		}

		public ShopHomeFloor()
		{
		}
	}
}