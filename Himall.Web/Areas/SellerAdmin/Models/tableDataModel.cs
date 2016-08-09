using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class tableDataModel
	{
		public List<SKUSpecModel> cost
		{
			get;
			set;
		}

		public List<SKUSpecModel> mallPrice
		{
			get;
			set;
		}

		public long productId
		{
			get;
			set;
		}

		public List<SKUSpecModel> sku
		{
			get;
			set;
		}

		public List<SKUSpecModel> stock
		{
			get;
			set;
		}

		public tableDataModel()
		{
		}
	}
}