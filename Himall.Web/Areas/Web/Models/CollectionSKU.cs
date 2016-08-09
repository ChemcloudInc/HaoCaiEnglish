using System;
using System.Collections.Generic;

namespace Himall.Web.Areas.Web.Models
{
	public class CollectionSKU : List<ProductSKU>
	{
		public CollectionSKU()
		{
		}

		public override string ToString()
		{
			string str = "";
			foreach (ProductSKU productSKU in this)
			{
				str = string.Concat(str, productSKU.Value, ",");
			}
			return str.TrimEnd(new char[] { ',' });
		}
	}
}