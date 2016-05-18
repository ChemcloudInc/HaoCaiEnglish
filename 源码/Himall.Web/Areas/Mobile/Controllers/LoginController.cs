using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web;
using Himall.Web.Framework;
using System;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class LoginController : BaseMobileTemplatesController
	{
		public LoginController()
		{
		}

		public ActionResult BindUser(string returnUrl, string openId, string serviceProvider, string nickName, string realName, string headimgurl, string unionid = null)
		{
			return View(base.CurrentSiteSetting);
		}

		[HttpPost]
		public JsonResult BindUser(string username, string password, string headimgurl, string serviceProvider, string openId, MemberOpenIdInfo.AppIdTypeEnum appidtype = (MemberOpenIdInfo.AppIdTypeEnum)1, string unionid = null)
		{
			IMemberService memberService = ServiceHelper.Create<IMemberService>();
			UserMemberInfo userMemberInfo = memberService.Login(username, password);
			if (userMemberInfo == null)
			{
				throw new HimallException("用户名和密码不匹配");
			}
			headimgurl = HttpUtility.UrlDecode(headimgurl);
			memberService.BindMember(userMemberInfo.Id, serviceProvider, openId, appidtype, headimgurl, unionid);
			string str = UserCookieEncryptHelper.Encrypt(userMemberInfo.Id, "Mobile");
			WebHelper.SetCookie("Himall-User", str);
			return Json(new { success = true });
		}

		private void CheckInput(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new LoginException("请填写用户名", LoginException.ErrorTypes.UsernameError);
			}
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new LoginException("请填写密码", LoginException.ErrorTypes.PasswordError);
			}
		}

		public ActionResult Entrance(string returnUrl, string openId, string serviceProvider, string nickName, string headimgurl, string realName, string unionid = null)
		{
			return View();
		}

		[HttpPost]
		public JsonResult Index(string username, string password)
		{
			JsonResult jsonResult;
			try
			{
                CheckInput(username, password);
				UserMemberInfo userMemberInfo = ServiceHelper.Create<IMemberService>().Login(username, password);
				if (userMemberInfo == null)
				{
					throw new LoginException("用户名和密码不匹配", LoginException.ErrorTypes.PasswordError);
				}
				string str = UserCookieEncryptHelper.Encrypt(userMemberInfo.Id, "Mobile");
				if (base.PlatformType != Himall.Core.PlatformType.WeiXin)
				{
					WebHelper.SetCookie("Himall-User", str, DateTime.MaxValue);
				}
				else
				{
					WebHelper.SetCookie("Himall-User", str);
				}
				jsonResult = Json(new { success = true, memberId = userMemberInfo.Id });
			}
			catch (LoginException loginException1)
			{
				LoginException loginException = loginException1;
				jsonResult = Json(new { success = false, msg = loginException.Message });
			}
			catch (HimallException himallException1)
			{
				HimallException himallException = himallException1;
				jsonResult = Json(new { success = false, msg = himallException.Message });
			}
			catch (Exception exception)
			{
				Log.Error(string.Concat("用户", username, "登录时发生异常"), exception);
				jsonResult = Json(new { success = false, msg = "未知错误" });
			}
			return jsonResult;
		}
	}
}