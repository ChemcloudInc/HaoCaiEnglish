using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class HomeCategorySet
	{
		public IEnumerable<HomeCategoryInfo> HomeCategories
		{
			get;
			set;
		}

		public IEnumerable<HomeCategorySet.HomeCategoryTopic> HomeCategoryTopics
		{
			get;
			set;
		}

		public int RowNumber
		{
			get;
			set;
		}

		public HomeCategorySet()
		{
		}

		public class HomeCategoryTopic
		{
			public string ImageUrl
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			public HomeCategoryTopic()
			{
			}
		}
	}
}