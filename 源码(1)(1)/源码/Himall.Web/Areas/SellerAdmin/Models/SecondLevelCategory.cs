using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class SecondLevelCategory
	{
		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<ThirdLevelCategoty> SubCategory
		{
			get;
			set;
		}

		public SecondLevelCategory()
		{
		}
	}
}