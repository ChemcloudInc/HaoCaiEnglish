using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class ExpressTemplateConfig
	{
		public Element[] data
		{
			get;
			set;
		}

		public int height
		{
			get;
			set;
		}

		public int selectedCount
		{
			get;
			set;
		}

		public int width
		{
			get;
			set;
		}

		public ExpressTemplateConfig()
		{
		}
	}
}