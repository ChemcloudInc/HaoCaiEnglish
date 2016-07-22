using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Product
{
	public class CategoryDropListModel
	{
		public int Depth
		{
			get;
			set;
		}

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

		public long ParentCategoryId
		{
			get;
			set;
		}

		public CategoryDropListModel()
		{
		}
	}
}