using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class RegisterController : BaseController
	{
		private const string CHECK_CODE_KEY = "regist_CheckCode";

		public RegisterController()
		{
		}

		[HttpPost]
		public JsonResult CheckCheckCode(string checkCode)
		{
			return Json(new { success = true, result = (base.Session["regist_CheckCode"] == null ? false : checkCode.ToLower() == base.Session["regist_CheckCode"].ToString().ToLower()) });
		}

		[HttpPost]
		public JsonResult CheckCode(string pluginId, string code, string destination)
		{
			object obj = Cache.Get(CacheKeyCollection.MemberPluginCheck(destination, pluginId));
			if (obj != null && obj.ToString() == code)
			{
				Result result = new Result()
				{
					success = true,
					msg = "Verify Success"
				};
				return Json(result);
			}
			Result result1 = new Result()
			{
				success = false,
				msg = "Verification code error or timeout"
			};
			return Json(result1);
		}

		[HttpPost]
		public JsonResult CheckManagerUser(string username)
		{
			bool flag = ServiceHelper.Create<IManagerService>().CheckUserNameExist(username, false);
			return Json(new { success = true, result = flag });
		}

		[HttpPost]
		public JsonResult CheckMobile(string mobile)
		{
			bool flag = ServiceHelper.Create<IMemberService>().CheckMobileExist(mobile);
			return Json(new { success = true, result = flag });
		}
        public JsonResult CheckEmail(string email)
        {
            bool flag = ServiceHelper.Create<IMemberService>().CheckEmailExist(email);

            return Json(new { success = true, result = flag });
        }
        
		[HttpPost]
		public JsonResult CheckUserName(string username)
		{
			bool flag = ServiceHelper.Create<IMemberService>().CheckMemberExist(username);
			return Json(new { success = true, result = flag });
		}

		[ValidateInput(false)]
		public ActionResult GetCheckCode()
		{
			string str;
			MemoryStream memoryStream = ImageHelper.GenerateCheckCode(out str);
			base.Session["regist_CheckCode"] = str;
			return base.File(memoryStream.ToArray(), "image/png");
		}

		public ActionResult Index(long id = 0L)
		{
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			ViewBag.MobileVerifOpen = base.CurrentSiteSetting.MobileVerifOpen;
			ViewBag.Introducer = id;
			return View();
		}

		[HttpGet]
		public ActionResult RegBusiness()
		{
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			return View();
		}

		public ActionResult RegisterAgreement()
		{
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			return View(ServiceHelper.Create<ISystemAgreementService>().GetAgreement(AgreementInfo.AgreementTypes.Buyers));
		}

		[HttpPost]
		public JsonResult RegisterUser(string username, string password, string email, long introducer = 0L)
		{
			UserMemberInfo userMemberInfo = ServiceHelper.Create<IMemberService>().Register(username, password, email, introducer);
			if (userMemberInfo != null)
			{
				base.Session.Remove("regist_CheckCode");
				if (!string.IsNullOrEmpty(email))
				{
					Cache.Remove(CacheKeyCollection.MemberPluginCheck(email, "Himall.Plugin.Message.Email"));
				}
			}
            long m = 200;
			ServiceHelper.Create<IBonusService>().DepositToRegister((long)userMemberInfo.Id);
            //ServiceHelper.Create<IBonusService>().DepositToRegister(m);
			return Json(new { success = true, memberId = userMemberInfo.Id });
		}

		[HttpPost]
		public JsonResult SendCode(string pluginId, string destination)
		{
			//ServiceHelper.Create<IMemberService>().CheckContactInfoHasBeenUsed(pluginId, destination, MemberContactsInfo.UserTypes.General);
			if(ServiceHelper.Create<IMemberService>().CheckEmailExist(destination))
            {
                Result result0 = new Result()
                {
                    success = false,
                    msg = "Email has been bind"
                };
                return Json(result0);
            }

            if (Cache.Get(CacheKeyCollection.MemberPluginCheckTime(destination, pluginId)) != null)
			{
				Result result = new Result()
				{
					success = false,
                    msg = "Only allowed to request once in 120 seconds，Please wait and try it again!"
				};
				return Json(result);
			}
			int num = (new Random()).Next(10000, 99999);
			DateTime dateTime = DateTime.Now.AddMinutes(15);
			if (pluginId.ToLower().Contains("email"))
			{
				dateTime = DateTime.Now.AddHours(24);
			}
			Cache.Insert(CacheKeyCollection.MemberPluginCheck(destination, pluginId), num, dateTime);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				UserName = "",
				SiteName = base.CurrentSiteSetting.SiteName,
				CheckCode = num.ToString()
			};
			ServiceHelper.Create<IMessageService>().SendMessageCode(destination, pluginId, messageUserInfo);
			string str = CacheKeyCollection.MemberPluginCheckTime(destination, pluginId);
			DateTime now = DateTime.Now;
			Cache.Insert(str, "0", now.AddSeconds(110));
			Result result1 = new Result()
			{
				success = true,
				msg = "Send Success"
			};
			return Json(result1);
		}
	}
}