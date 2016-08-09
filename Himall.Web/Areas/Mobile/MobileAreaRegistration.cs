using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile
{
	public class MobileAreaRegistration : AreaRegistrationOrder
	{
		public override string AreaName
		{
			get
			{
				return "Mobile";
			}
		}

		public override int Order
		{
			get
			{
				return 2;
			}
		}

		public MobileAreaRegistration()
		{
		}

		public override void RegisterAreaOrder(AreaRegistrationContext context)
		{
			context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			context.MapRoute("Mobile_default", "m-{platform}/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
			context.MapRoute("Mobile_default2", "m/{controller}/{action}/{id}", new { controller = "Home", action = "Index", platform = "Mobile", id = UrlParameter.Optional });
			context.MapRoute("Mobile_isshare", "m-{platform}/{controller}/{action}/{id}/{isShare}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
			context.MapRoute("Mobile_openid_isshare", "m-{platform}/{controller}/{action}/{id}/{openId}/{isShare}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}
	}
}