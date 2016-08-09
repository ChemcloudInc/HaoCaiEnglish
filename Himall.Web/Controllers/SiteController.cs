using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Himall.Web.Controllers
{
	public class SiteController : Controller
	{
		public SiteController()
		{
		}

		public ActionResult Close()
		{
			return View();
		}

		public object GetAppDataUrl()
		{
			object obj;
			try
			{
				Configuration configuration = WebConfigurationManager.OpenWebConfiguration(base.Request.ApplicationPath);
				string value = configuration.AppSettings.Settings["AppDateUrl"].Value;
				obj = Json(new { Success = "true", Url = value }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception exception)
			{
				var variable = new { Success = "false", ErrorMsg = exception.Message };
				obj = Json(variable, JsonRequestBehavior.AllowGet);
			}
			return obj;
		}

		private bool GetSolutionDebugState()
		{
			return false;
		}

		public ActionResult State()
		{
			ViewBag.IsDebug = GetSolutionDebugState();
			return View();
		}

		public ActionResult Version()
		{
			string str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Himall.Web.dll");
			Assembly.GetExecutingAssembly();
			ViewBag.FileVersion = FileVersionInfo.GetVersionInfo(str).FileVersion;
			ViewBag.IsDebug = GetSolutionDebugState();
			return View();
		}
	}
}