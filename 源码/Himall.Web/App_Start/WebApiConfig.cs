using Himall.Web.Framework;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Himall.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			config.Filters.Add(new ApiExceptionFilterAttribute());
			config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { Controller = "Test", Action = "Get", id = RouteParameter.Optional });
		}
	}
}