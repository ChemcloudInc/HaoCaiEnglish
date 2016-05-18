using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ErrorController : BaseController
	{
		public ErrorController()
		{
		}

		public ActionResult Error404()
		{
			return View();
		}
	}
}