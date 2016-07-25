using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class LoginController : Controller
	{
		private const int TIMES_WITHOUT_CHECKCODE = 3;

		public LoginController()
		{
		}

		private void CheckCheckCode(string username, string checkCode)
		{
			if (GetErrorTimes(username) >= 3)
			{
				if (string.IsNullOrWhiteSpace(checkCode))
				{
					throw new HimallException("30分钟内登录错误3次以上需要提供验证码");
				}
				if ((base.Session["checkCode"] as string).ToLower() != checkCode.ToLower())
				{
					throw new HimallException("验证码错误");
				}
				base.Session["checkCode"] = Guid.NewGuid().ToString();
			}
		}

		[HttpPost]
		public JsonResult CheckCode(string checkCode)
		{
			JsonResult jsonResult;
			try
			{
				string item = base.Session["checkCode"] as string;
				bool lower = item.ToLower() == checkCode.ToLower();
				jsonResult = Json(new { success = lower });
			}
			catch (HimallException himallException1)
			{
				HimallException himallException = himallException1;
				jsonResult = Json(new { success = false, msg = himallException.Message });
			}
			catch (Exception exception)
			{
				Log.Error("检验验证码时发生异常", exception);
				jsonResult = Json(new { success = false, msg = "未知错误" });
			}
			return jsonResult;
		}

		private void CheckInput(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new HimallException("请填写用户名");
			}
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new HimallException("请填写密码");
			}
		}

		private void ClearErrorTimes(string username)
		{
			Cache.Remove(CacheKeyCollection.ManagerLoginError(username));
		}

		public ActionResult GetCheckCode()
		{
			string str;
			MemoryStream memoryStream = ImageHelper.GenerateCheckCode(out str);
			base.Session["checkCode"] = str;
			return base.File(memoryStream.ToArray(), "image/png");
		}

		[HttpPost]
		public JsonResult GetErrorLoginTimes(string username)
		{
			return Json(new { errorTimes = GetErrorTimes(username) });
		}

		private int GetErrorTimes(string username)
		{
			object obj = Cache.Get(CacheKeyCollection.ManagerLoginError(username));
			return (obj == null ? 0 : (int)obj);
		}

		public ActionResult Index()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item == null || bool.Parse(item))
			{
				return View();
			}
			return RedirectToAction("Agreement", "Installer", new { area = "Web" });
		}

		[HttpPost]
		public JsonResult Login(string username, string password, string checkCode)
		{
			JsonResult jsonResult;
			string str = "";
			string host = System.Web.HttpContext.Current.Request.Url.Host;
			try
			{
				if(true)// (LicenseChecker.Check(out str, host))
				{
                    CheckInput(username, password);
                    CheckCheckCode(username, checkCode);
					ManagerInfo managerInfo = ServiceHelper.Create<IManagerService>().Login(username, password, true);
					if (managerInfo == null)
					{
						throw new HimallException("用户名和密码不匹配");
					}
                    ClearErrorTimes(username);
					jsonResult = Json(new { success = true, userId = UserCookieEncryptHelper.Encrypt(managerInfo.Id, "Admin") });
				}
				else
				{
					jsonResult = Json(new { success = false, msg = str });
				}
			}
			catch (HimallException himallException1)
			{
				HimallException himallException = himallException1;
				int num = SetErrorTimes(username);
				jsonResult = Json(new { success = false, msg = himallException.Message, errorTimes = num, minTimesWithoutCheckCode = 3 });
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				int num1 = SetErrorTimes(username);
				Log.Error(string.Concat("用户", username, "登录时发生异常"), exception);
				jsonResult = Json(new { success = false, msg = "未知错误", errorTimes = num1, minTimesWithoutCheckCode = 3 });
			}
			return jsonResult;
		}

		private int SetErrorTimes(string username)
		{
			int errorTimes = GetErrorTimes(username) + 1;
			string str = CacheKeyCollection.ManagerLoginError(username);
			object obj = errorTimes;
			DateTime now = DateTime.Now;
			Cache.Insert(str, obj, now.AddMinutes(30));
			return errorTimes;
		}
	}
}