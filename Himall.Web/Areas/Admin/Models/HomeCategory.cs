using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class HomeCategory
	{
		public IEnumerable<long> AllCategoryIds
		{
			get;
			set;
		}

		public int RowNumber
		{
			get;
			set;
		}

		public IEnumerable<string> TopCategoryNames
		{
			get;
			set;
		}

		public HomeCategory()
		{
		}
	}
}