using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopProductCategoryModel
	{
		public List<CategoryJsonModel> Data
		{
			get;
			set;
		}

		public List<Himall.Web.Areas.SellerAdmin.Models.SelectedCategory> SelectedCategory
		{
			get;
			set;
		}

		public ShopProductCategoryModel()
		{
		}
	}
}