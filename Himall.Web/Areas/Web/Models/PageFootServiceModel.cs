using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class PageFootServiceModel
	{
		public IEnumerable<ArticleInfo> Articles
		{
			get;
			set;
		}

		public string CateogryName
		{
			get;
			set;
		}

		public PageFootServiceModel()
		{
		}
	}
}