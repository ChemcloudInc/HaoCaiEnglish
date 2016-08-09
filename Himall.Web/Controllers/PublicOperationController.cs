using Himall.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Controllers
{
	public class PublicOperationController : Controller
	{
		public PublicOperationController()
		{
		}

		public ActionResult TestCache()
		{
			string str = "无";
			if (Cache.Get("tt") == null)
			{
				str = "失效";
				Log.Info("缓存已经失效");
				Cache.Insert("tt", "zhangsan", 7000);
			}
			return Json(str, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult UploadFile()
		{
			string str = "NoFile";
			if (base.Request.Files.Count > 0)
			{
				HttpPostedFileBase item = base.Request.Files[0];
				if (item.ContentLength != 0 && IsAllowExt(item))
				{
					Random random = new Random();
					DateTime now = DateTime.Now;
					string str1 = string.Concat(now.ToString("yyyyMMddHHmmssfff"), random.Next(1000, 9999), item.FileName.Substring(item.FileName.LastIndexOf("\\") + 1));
					string str2 = Server.MapPath("~/temp/");
					if (!Directory.Exists(str2))
					{
						Directory.CreateDirectory(str2);
					}
					string str3 = str1;
					try
					{
						object obj = Cache.Get("Cache-UserImportOpCount");
						if (obj != null)
						{
							Cache.Insert("Cache-UserImportOpCount", int.Parse(obj.ToString()) + 1);
						}
						else
						{
							Cache.Insert("Cache-UserImportOpCount", 1);
						}
						item.SaveAs(Path.Combine(str2, str1));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						object obj1 = Cache.Get("Cache-UserImportOpCount");
						if (obj1 != null)
						{
							Cache.Insert("Cache-UserImportOpCount", int.Parse(obj1.ToString()) - 1);
						}
						Log.Error(string.Concat("商品导入上传文件异常：", exception.Message));
						str3 = "Error";
					}
					str = str3;
				}
				else
				{
					str = "文件长度为0,格式异常。";
				}
			}
			return base.Content(str, "text/html");
		}

		[HttpPost]
		public ActionResult UploadPic()
		{
			string str = "";
			string str1 = "";
			List<string> strs = new List<string>();
			if (base.Request.Files.Count == 0)
			{
				return base.Content("NoFile", "text/html");
			}
			for (int i = 0; i < base.Request.Files.Count; i++)
			{
				HttpPostedFileBase item = base.Request.Files[i];
				if (item == null || item.ContentLength <= 0 || !IsAllowExt(item))
				{
					return base.Content("格式不正确！", "text/html");
				}
				Random random = new Random();
				DateTime now = DateTime.Now;
				str1 = string.Concat(now.ToString("yyyyMMddHHmmssffffff"), i, Path.GetExtension(item.FileName));
				string str2 = Server.MapPath("~/temp/");
				if (!Directory.Exists(str2))
				{
					Directory.CreateDirectory(str2);
				}
				str = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "/temp/");
				strs.Add(string.Concat("/temp/", str1));
				try
				{
					item.SaveAs(Path.Combine(str, str1));
				}
				catch (Exception exception)
				{
				}
			}
			return base.Content(string.Join(",", strs), "text/html");
		}

		public ActionResult UploadPictures()
		{
			return View();
		}

        public bool IsAllowExt(HttpPostedFileBase item)
        {
            var notAllowExt = "," + ConfigurationManager.AppSettings["NotAllowExt"] + ",";
            var ext = "," + Path.GetExtension(item.FileName).ToLower() + ",";
            if (notAllowExt.Contains(ext))
                return false;
            return true;
        }
    }
}