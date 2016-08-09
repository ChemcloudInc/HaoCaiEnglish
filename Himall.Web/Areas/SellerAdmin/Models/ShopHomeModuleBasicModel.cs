using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopHomeModuleBasicModel
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

		public IEnumerable<long> ProductIds
		{
			get;
			set;
		}

		public ShopHomeModuleBasicModel()
		{
		}
	}
}