using Himall.Web.App_Code.UEditor;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Controllers
{
	public class UEditorController : Controller
	{
		public UEditorController()
		{
		}

		public ContentResult Handle()
		{
			IUEditorHandle configHandler = null;
			string item = base.Request["action"];
			string str = item;
			if (item != null)
			{
				if (str == "config")
				{
					configHandler = new ConfigHandler();
					return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
				}
				else
				{
					if (str != "uploadimage")
					{
						configHandler = new NotSupportedHandler();
						return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
					}
					UploadConfig uploadConfig = new UploadConfig()
					{
						AllowExtensions = Config.GetStringList("imageAllowFiles"),
						PathFormat = Config.GetString("imagePathFormat"),
						SizeLimit = Config.GetInt("imageMaxSize"),
						UploadFieldName = Config.GetString("imageFieldName")
					};
					configHandler = new UploadHandle(uploadConfig);
					return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
				}
			}
			configHandler = new NotSupportedHandler();
			return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
		}

		[HttpPost]
		public ContentResult Handle(string action)
		{
			IUEditorHandle configHandler = null;
			action = base.Request["action"];
			string str = action;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "config")
				{
					configHandler = new ConfigHandler();
					return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
				}
				else
				{
					if (str1 != "uploadimage")
					{
						configHandler = new NotSupportedHandler();
						return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
					}
					UploadConfig uploadConfig = new UploadConfig()
					{
						AllowExtensions = Config.GetStringList("imageAllowFiles"),
						PathFormat = Config.GetString("imagePathFormat"),
						SizeLimit = Config.GetInt("imageMaxSize"),
						UploadFieldName = Config.GetString("imageFieldName")
					};
					configHandler = new UploadHandle(uploadConfig);
					return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
				}
			}
			configHandler = new NotSupportedHandler();
			return base.Content(JsonConvert.SerializeObject(configHandler.Process()));
		}

		[HttpGet]
		public ActionResult Upload()
		{
			string item = base.Request.QueryString["url"] ?? "";
			base.ViewData["url"] = item;
			return View();
		}

		[HttpPost]
		public ActionResult UploadImage(HttpPostedFileBase filename)
		{
			return View();
		}
	}
}