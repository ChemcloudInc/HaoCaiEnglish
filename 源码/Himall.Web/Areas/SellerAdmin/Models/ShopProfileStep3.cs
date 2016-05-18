using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopProfileStep3
	{
		public List<BusinessCategoryInfo> BusinessCategory
		{
			get;
			set;
		}

		public long[] Categories
		{
			get;
			set;
		}

		public long ShopGrade
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public ShopProfileStep3()
		{
		}
	}
}