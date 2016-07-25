using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class CategoryJsonModel
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

		public List<SecondLevelCategory> SubCategory
		{
			get;
			set;
		}

		public CategoryJsonModel()
		{
		}
	}
}