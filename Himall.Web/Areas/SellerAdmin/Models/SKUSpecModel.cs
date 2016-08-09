using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	[Serializable]
	public class SKUSpecModel
	{
		public string index
		{
			get;
			set;
		}

		public List<string> ValueSet
		{
			get;
			set;
		}

		public SKUSpecModel()
		{
		}
	}
}