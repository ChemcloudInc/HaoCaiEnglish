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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserInfoController : BaseMemberController
	{
		public UserInfoController()
		{
		}

		public ActionResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		public JsonResult ChangePassword(string oldpassword, string password)
		{
			if (string.IsNullOrWhiteSpace(oldpassword) || string.IsNullOrWhiteSpace(password))
			{
				Result result = new Result()
				{
					success = false,
					msg = "Password is require！"
				};
				return Json(result);
			}
			UserMemberInfo currentUser = base.CurrentUser;
			if (SecureHelper.MD5(string.Concat(SecureHelper.MD5(oldpassword), currentUser.PasswordSalt)) != currentUser.Password)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "Old password is error"
				};
				return Json(result1);
			}
			ServiceHelper.Create<IMemberService>().ChangePassWord(currentUser.Id, password);
			Result result2 = new Result()
			{
				success = true,
                msg = "Password Has Been Changed"
			};
			return Json(result2);
		}

		[HttpPost]
		public ActionResult CheckCode(string pluginId, string code, string destination)
		{
			string str = CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId);
			object obj = Cache.Get(str);
			UserMemberInfo currentUser = base.CurrentUser;
			if (obj == null || !(obj.ToString() == code))
			{
				Result result = new Result()
				{
					success = false,
					msg = "Verification code is error or timeout"
				};
				return Json(result);
			}
			Cache.Remove(CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId));
			string str1 = string.Concat("Rebind", currentUser.Id);
			DateTime now = DateTime.Now;
			Cache.Insert(str1, "step2", now.AddMinutes(30));
			return Json(new { success = true, msg = "Verify Sucess", key = currentUser.Id });
		}

		public JsonResult CheckOldPassWord(string password)
		{
			UserMemberInfo currentUser = base.CurrentUser;
			string str = SecureHelper.MD5(string.Concat(SecureHelper.MD5(password), currentUser.PasswordSalt));
			if (currentUser.Password == str)
			{
				return Json(new Result()
				{
					success = true
				});
			}
			return Json(new Result()
			{
				success = false
			});
		}

		[HttpPost]
		public JsonResult GetCurrentUserInfo()
		{
			UserMemberInfo currentUser = base.CurrentUser;
			return Json(new { success = true, name = (string.IsNullOrWhiteSpace(currentUser.Nick) ? currentUser.UserName : currentUser.Nick) });
		}

		public ActionResult Index()
		{
			UserMemberInfo currentUser = base.CurrentUser;
			PluginsManagement.GetPlugins<IMessagePlugin>();
			IEnumerable<Plugin<ISMSPlugin>> plugins = PluginsManagement.GetPlugins<ISMSPlugin>();
			PluginsInfo pluginsInfo = (
				from item in plugins
				select new PluginsInfo()
				{
					ShortName = item.Biz.ShortName,
					PluginId = item.PluginInfo.PluginId,
					Enable = item.PluginInfo.Enable,
					IsSettingsValid = item.Biz.IsSettingsValid,
					IsBind = !string.IsNullOrEmpty(ServiceHelper.Create<IMessageService>().GetDestination(base.CurrentUser.Id, item.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General))
				}).FirstOrDefault();
			IEnumerable<Plugin<IEmailPlugin>> plugins1 = PluginsManagement.GetPlugins<IEmailPlugin>();
			PluginsInfo pluginsInfo1 = (
				from item in plugins1
				select new PluginsInfo()
				{
					ShortName = item.Biz.ShortName,
					PluginId = item.PluginInfo.PluginId,
					Enable = item.PluginInfo.Enable,
					IsSettingsValid = item.Biz.IsSettingsValid,
					IsBind = !string.IsNullOrEmpty(ServiceHelper.Create<IMessageService>().GetDestination(base.CurrentUser.Id, item.PluginInfo.PluginId, MemberContactsInfo.UserTypes.General))
				}).FirstOrDefault();
			ViewBag.BindSMS = pluginsInfo;
			ViewBag.BindEmail = pluginsInfo1;
			return View(base.CurrentUser);
		}

		public ActionResult ReBind(string pluginId)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.ShortName = plugin.Biz.ShortName;
			ViewBag.id = pluginId;
			ViewBag.ContactInfo = ServiceHelper.Create<IMessageService>().GetDestination(base.CurrentUser.Id, pluginId, MemberContactsInfo.UserTypes.General);
			return View();
		}

		public ActionResult ReBindStep2(string pluginId, string key)
		{
			if (Cache.Get(string.Concat("Rebind", key)) as string != "step2")
			{
				RedirectToAction("ReBind", new { pluginId = pluginId });
			}
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.ShortName = plugin.Biz.ShortName;
			ViewBag.id = pluginId;
			ViewBag.ContactInfo = ServiceHelper.Create<IMessageService>().GetDestination(base.CurrentUser.Id, pluginId, MemberContactsInfo.UserTypes.General);
			return View();
		}

		public ActionResult ReBindStep3(string name)
		{
			ViewBag.ShortName = name;
			return View();
		}

		[HttpPost]
		public ActionResult SendCode(string pluginId, string destination)
		{
			if (Cache.Get(CacheKeyCollection.MemberPluginReBindTime(base.CurrentUser.UserName, pluginId)) != null)
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
			Cache.Insert(CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId), num, dateTime);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				UserName = base.CurrentUser.UserName,
				SiteName = base.CurrentSiteSetting.SiteName,
				CheckCode = num.ToString()
			};
			ServiceHelper.Create<IMessageService>().SendMessageCode(destination, pluginId, messageUserInfo);
			string str = CacheKeyCollection.MemberPluginReBindTime(base.CurrentUser.UserName, pluginId);
			DateTime now = DateTime.Now;
			Cache.Insert(str, "0", now.AddSeconds(110));
			Result result1 = new Result()
			{
				success = true,
				msg = "Send Success"
			};
			return Json(result1);
		}

		[HttpPost]
		public ActionResult SendCodeStep2(string pluginId, string destination)
		{
			if (Cache.Get(CacheKeyCollection.MemberPluginReBindStepTime(base.CurrentUser.UserName, pluginId)) != null)
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
			Cache.Insert(CacheKeyCollection.MemberPluginCheck(base.CurrentUser.UserName, pluginId), num, dateTime);
			MessageUserInfo messageUserInfo = new MessageUserInfo()
			{
				UserName = base.CurrentUser.UserName,
				SiteName = base.CurrentSiteSetting.SiteName,
				CheckCode = num.ToString()
			};
			ServiceHelper.Create<IMessageService>().SendMessageCode(destination, pluginId, messageUserInfo);
			string str = CacheKeyCollection.MemberPluginReBindStepTime(base.CurrentUser.UserName, pluginId);
			DateTime now = DateTime.Now;
			Cache.Insert(str, "0", now.AddSeconds(110));
			Result result1 = new Result()
			{
				success = true,
				msg = "Send Success"
			};
			return Json(result1);
		}

		public JsonResult UpdateUserInfo(UserMemberInfo model)
		{
			model.Id = base.CurrentUser.Id;
			ServiceHelper.Create<IMemberService>().UpdateMember(model);
			Result result = new Result()
			{
				success = true,
				msg = "Update Success"
			};
			return Json(result);
		}
	}
}