using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class SlideAdModel
	{
		public string Description
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public long ID
		{
			get;
			set;
		}

		public string imgUrl
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public SlideAdModel()
		{
		}
	}
}