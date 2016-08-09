using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile.Models
{
	public class HomeTopicModel
	{
		public long Id
		{
			get;
			set;
		}

		public string ImageUrl
		{
			get;
			set;
		}

		public IEnumerable<string> Tags
		{
			get;
			set;
		}

		public HomeTopicModel()
		{
		}
	}
}