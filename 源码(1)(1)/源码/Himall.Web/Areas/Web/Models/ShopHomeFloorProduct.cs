using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ShopHomeFloorProduct
	{
		public long Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Pic
		{
			get;
			set;
		}

		public string Price
		{
			get;
			set;
		}

		public int SaleCount
		{
			get;
			set;
		}

		public ShopHomeFloorProduct()
		{
		}
	}
}