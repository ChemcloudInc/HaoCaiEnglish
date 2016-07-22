using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class OrderPartialController : BaseMemberController
	{
		public OrderPartialController()
		{
		}

		private void InitTopBar()
		{
			ViewBag.Member = base.CurrentUser;
		}

		public ActionResult TopBar()
		{
            InitTopBar();
			return base.PartialView("~/Areas/Web/Views/Shared/OrderTopBar.cshtml");
		}
	}
}