using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class RedirectController : BaseMobileMemberController
	{
		public RedirectController()
		{
		}

		public ActionResult Index(string redirectUrl)
		{
			return Redirect(redirectUrl);
		}
	}
}