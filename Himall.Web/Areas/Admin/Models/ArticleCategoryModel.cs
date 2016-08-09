using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class ArticleCategoryModel
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

		public bool HasChild
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public bool IsDefault
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public long ParentId
		{
			get;
			set;
		}

		public ArticleCategoryModel()
		{
		}
	}
}