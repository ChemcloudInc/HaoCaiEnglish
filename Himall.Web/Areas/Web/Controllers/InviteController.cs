using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class InviteController : Controller
	{
		public InviteController()
		{
		}

		public ActionResult Index()
		{
			return View(ServiceHelper.Create<IMemberInviteService>().GetInviteRule());
		}
	}
}