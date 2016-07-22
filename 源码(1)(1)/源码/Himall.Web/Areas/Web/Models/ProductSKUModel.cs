using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductSKUModel
	{
		public decimal Price
		{
			get;
			set;
		}

		public string SKUId
		{
			get;
			set;
		}

		public int Stock
		{
			get;
			set;
		}

		public ProductSKUModel()
		{
		}
	}
}