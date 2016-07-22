using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web
{
	public class FilterConfig
	{
		public FilterConfig()
		{
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new GZipAttribute());
			filters.Add(new HandleErrorAttribute());
		}
	}
}