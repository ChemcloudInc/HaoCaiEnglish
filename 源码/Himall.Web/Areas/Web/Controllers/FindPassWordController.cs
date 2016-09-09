using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class FindPassWordController : BaseWebController
	{
		public FindPassWordController()
		{
		}

		public ActionResult ChangePassWord(string passWord, string key)
		{
			UserMemberInfo userMemberInfo = Cache.Get(string.Concat(key, "3")) as UserMemberInfo;
			if (userMemberInfo == null)
			{
				return Json(new { success = false, flag = -1, msg = "Verify Timeout" });
			}
			long id = userMemberInfo.Id;
			ServiceHelper.Create<IMemberService>().ChangePassWord(id, passWord);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				SiteName = base.CurrentSiteSetting.SiteName,
				UserName = userMemberInfo.UserName
			};
			Task.Factory.StartNew(() => ServiceHelper.Create<IMessageService>().SendMessageOnFindPassWord(id, messageUserInfo));
			return Json(new { success = true, flag = 1, msg = "Password recovery success" });
		}

		[HttpPost]
		public ActionResult CheckPluginCode(string pluginId, string code, string key)
		{
			UserMemberInfo userMemberInfo = Cache.Get(key) as UserMemberInfo;
			string str = CacheKeyCollection.MemberFindPassWordCheck(userMemberInfo.UserName, pluginId);
			object obj = Cache.Get(str);
			if (obj == null || !(obj.ToString() == code))
			{
				Result result = new Result()
				{
					success = false,
					msg = "Verification code error or timeout"
				};
				return Json(result);
			}
			Cache.Remove(CacheKeyCollection.MemberFindPassWordCheck(userMemberInfo.UserName, pluginId));
			string str1 = string.Concat(key, "3");
			DateTime now = DateTime.Now;
			Cache.Insert(str1, userMemberInfo, now.AddMinutes(15));
			return Json(new { success = true, msg = "Verify Success", key = key });
		}

		[HttpPost]
		public ActionResult CheckUser(string userName, string checkCode)
		{
			Guid guid = Guid.NewGuid();
			string str = guid.ToString().Replace("-", "");
            VaildCode(checkCode);
			UserMemberInfo memberByContactInfo = ServiceHelper.Create<IMemberService>().GetMemberByContactInfo(userName);
			if (memberByContactInfo == null)
			{
				return Json(new { success = false, tag = "username", msg = "username has not exist or not bind email and mobile,please check and enter it again" });
			}
			DateTime now = DateTime.Now;
			Cache.Insert(str, memberByContactInfo, now.AddMinutes(15));
			return Json(new { success = true, key = str });
		}

		public ActionResult Error()
		{
			return View();
		}

		public ActionResult GetCheckCode()
		{
			string str;
			MemoryStream memoryStream = ImageHelper.GenerateCheckCode(out str);
			base.Session["FindPassWordcheckCode"] = str;
			return base.File(memoryStream.ToArray(), "image/png");
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SendCode(string pluginId, string key)
		{
			UserMemberInfo userMemberInfo = Cache.Get(key) as UserMemberInfo;
			if (userMemberInfo == null)
			{
				return Json(new { success = false, flag = -1, msg = "Verify Timetout！" });
			}
			string destination = ServiceHelper.Create<IMessageService>().GetDestination(userMemberInfo.Id, pluginId, MemberContactsInfo.UserTypes.General);
			if (Cache.Get(CacheKeyCollection.MemberPluginFindPassWordTime(userMemberInfo.UserName, pluginId)) != null)
			{
                return Json(new { success = false, flag = 0, msg = "Only allowed to request once in 120 seconds，Please wait and try it again!" });
			}
			int num = (new Random()).Next(10000, 99999);
			DateTime dateTime = DateTime.Now.AddMinutes(15);
			Cache.Insert(CacheKeyCollection.MemberFindPassWordCheck(userMemberInfo.UserName, pluginId), num, dateTime);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				UserName = userMemberInfo.UserName,
				SiteName = base.CurrentSiteSetting.SiteName,
				CheckCode = num.ToString()
			};
			ServiceHelper.Create<IMessageService>().SendMessageCode(destination, pluginId, messageUserInfo);
			string str = CacheKeyCollection.MemberPluginFindPassWordTime(userMemberInfo.UserName, pluginId);
			DateTime now = DateTime.Now;
			Cache.Insert(str, "0", now.AddSeconds(110));
			return Json(new { success = true, flag = 1, msg = "Send Success" });
		}

		public ActionResult Step2(string key)
		{
			UserMemberInfo userMemberInfo = Cache.Get(key) as UserMemberInfo;
			if (userMemberInfo == null)
			{
				return RedirectToAction("Error", "FindPassWord");
			}
			IEnumerable<Plugin<IMessagePlugin>> plugins = PluginsManagement.GetPlugins<IMessagePlugin>();
			IEnumerable<PluginsInfo> pluginsInfo = 
				from item in plugins
				select new PluginsInfo()
				{
					ShortName = item.Biz.ShortName,
					PluginId = item.PluginInfo.PluginId,
					Enable = item.PluginInfo.Enable,
					IsSettingsValid = item.Biz.IsSettingsValid,
					IsBind = !string.IsNullOrEmpty(ServiceHelper.Create<IMessageService>().GetDestination(userMemberInfo.Id, item.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General))
				};
			ViewBag.BindContactInfo = pluginsInfo;
			ViewBag.Key = key;
			return View(userMemberInfo);
		}

		public ActionResult Step3(string key)
		{
			if (!(Cache.Get(string.Concat(key, "3")) is UserMemberInfo))
			{
				return RedirectToAction("Error", "FindPassWord");
			}
			ViewBag.Key = key;
			return View();
		}

		public ActionResult Step4()
		{
			return View();
		}

		private void VaildCode(string checkCode)
		{
			if (string.IsNullOrWhiteSpace(checkCode))
			{
				throw new HimallException("Verification code is require");
			}
			string item = base.Session["FindPassWordcheckCode"] as string;
			if (string.IsNullOrEmpty(item))
			{
				throw new HimallException("verify timeout,please refresh");
			}
			if (item.ToLower() != checkCode.ToLower())
			{
				throw new HimallException("Verification code error");
			}
			base.Session["FindPassWordcheckCode"] = Guid.NewGuid().ToString();
		}
	}
}