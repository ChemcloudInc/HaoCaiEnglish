using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile
{
	public class CategoryModel
	{
		public int Depth
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public IEnumerable<CategoryModel> SubCategories
		{
			get;
			set;
		}

		public CategoryModel()
		{
		}
	}
}