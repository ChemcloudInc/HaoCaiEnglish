using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class RegisterController : BaseMobileTemplatesController
	{
		private const string CHECK_CODE_KEY = "checkCode";

		public RegisterController()
		{
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

		public ActionResult GetCheckCode()
		{
			string str;
			MemoryStream memoryStream = ImageHelper.GenerateCheckCode(out str);
			base.Session["checkCode"] = str;
			return base.File(memoryStream.ToArray(), "image/png");
		}

		public ActionResult Index(long id = 0L, string openid = "")
		{
			string str;
			ViewBag.Introducer = id;
			if (id <= 0 || !string.IsNullOrWhiteSpace(openid))
			{
				return View();
			}
			string scheme = base.Request.Url.Scheme;
			string host = base.HttpContext.Request.Url.Host;
			if (base.HttpContext.Request.Url.Port == 80)
			{
				str = "";
			}
			else
			{
				int port = base.HttpContext.Request.Url.Port;
				str = string.Concat(":", port.ToString());
			}
			string str1 = string.Concat(scheme, "://", host, str);
			object[] platformType = new object[] { str1, "/m-", base.PlatformType, "/Register/InviteRegist?id=", id };
			string str2 = string.Concat(platformType);
			return Redirect(string.Concat("/m-", base.PlatformType.ToString(), "/WXApi/WXAuthorize?returnUrl=", str2));
		}

		[HttpPost]
		public JsonResult Index(string serviceProvider, string openId, string username, string password, string checkCode, string headimgurl, long introducer = 0L, string unionid = null)
		{
			UserMemberInfo userMemberInfo;
			if ((base.Session["checkCode"] as string).ToLower() != checkCode.ToLower())
			{
				throw new HimallException("验证码错误");
			}
			headimgurl = HttpUtility.UrlDecode(headimgurl);
			userMemberInfo = (string.IsNullOrWhiteSpace(serviceProvider) || string.IsNullOrWhiteSpace(openId) ? ServiceHelper.Create<IMemberService>().Register(username, password, "", introducer) : ServiceHelper.Create<IMemberService>().Register(username, password, serviceProvider, openId, headimgurl, introducer, null, unionid));
			if (userMemberInfo != null)
			{
				base.Session.Remove("checkCode");
			}
			ServiceHelper.Create<IBonusService>().DepositToRegister(userMemberInfo.Id);
			string str = UserCookieEncryptHelper.Encrypt(userMemberInfo.Id, "Mobile");
			WebHelper.SetCookie("Himall-User", str);
			return Json(new { success = true, memberId = userMemberInfo.Id });
		}

		public ActionResult InviteRegist(long id = 0L, string openId = "", string serviceProvider = "")
		{
			ViewBag.Introducer = id;
			UserMemberInfo memberByOpenId = ServiceHelper.Create<IMemberService>().GetMemberByOpenId(serviceProvider, openId);
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			InviteRuleInfo inviteRule = ServiceHelper.Create<IMemberInviteService>().GetInviteRule();
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			int num = (integralChangeRule == null ? 0 : integralChangeRule.IntegralPerMoney);
			ViewBag.WXLogo = siteSettings.WXLogo;
			if (!inviteRule.InviteIntegral.HasValue || num <= 0)
			{
				ViewBag.Money = "0.0";
			}
			else
			{
				dynamic viewBag = base.ViewBag;
				int value = inviteRule.InviteIntegral.Value / num;
				viewBag.Money = value.ToString("f1");
			}
			ViewBag.IsRegist = 0;
			if (memberByOpenId != null)
			{
				ViewBag.IsRegist = 1;
			}
			return View(inviteRule);
		}

		[HttpPost]
		public JsonResult InviteRegist(string serviceProvider, string openId, string username, string password, string nickName, string headimgurl, long introducer, string unionid = null)
		{
			UserMemberInfo userMemberInfo;
			headimgurl = HttpUtility.UrlDecode(headimgurl);
			nickName = HttpUtility.UrlDecode(nickName);
			username = HttpUtility.UrlDecode(username);
			userMemberInfo = (string.IsNullOrWhiteSpace(serviceProvider) || string.IsNullOrWhiteSpace(openId) ? ServiceHelper.Create<IMemberService>().Register(username, password, "", introducer) : ServiceHelper.Create<IMemberService>().Register(username, password, serviceProvider, openId, headimgurl, introducer, nickName, unionid));
			ServiceHelper.Create<IBonusService>().DepositToRegister(userMemberInfo.Id);
			string str = UserCookieEncryptHelper.Encrypt(userMemberInfo.Id, "Mobile");
			WebHelper.SetCookie("Himall-User", str);
			return Json(new { success = true, memberId = userMemberInfo.Id });
		}

		[HttpPost]
		public JsonResult Skip(string serviceProvider, string openId, string nickName, string realName, string headimgurl, MemberOpenIdInfo.AppIdTypeEnum appidtype = (MemberOpenIdInfo.AppIdTypeEnum)1, string unionid = null)
		{
			string str = DateTime.Now.ToString("yyMMddHHmmssffffff");
			nickName = HttpUtility.UrlDecode(nickName);
			realName = HttpUtility.UrlDecode(realName);
			headimgurl = HttpUtility.UrlDecode(headimgurl);
			UserMemberInfo userMemberInfo = ServiceHelper.Create<IMemberService>().QuickRegister(str, realName, nickName, serviceProvider, openId, headimgurl, appidtype, unionid, null);
			ServiceHelper.Create<IBonusService>().DepositToRegister(userMemberInfo.Id);
			string str1 = UserCookieEncryptHelper.Encrypt(userMemberInfo.Id, "Mobile");
			WebHelper.SetCookie("Himall-User", str1);
			return Json(new { success = true });
		}
	}
}