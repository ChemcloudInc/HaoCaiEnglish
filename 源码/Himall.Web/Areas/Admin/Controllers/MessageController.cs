using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class MessageController : BaseAdminController
	{
		public MessageController()
		{
		}

		public ActionResult Edit(string pluginId)
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			ViewBag.messagePlugins = objs;
			ViewBag.Id = pluginId;
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.Name = plugin.PluginInfo.DisplayName;
			ViewBag.ShortName = plugin.Biz.ShortName;
			FormData formData = plugin.Biz.GetFormData();
			Plugin<ISMSPlugin> plugin1 = PluginsManagement.GetPlugins<ISMSPlugin>().FirstOrDefault<Plugin<ISMSPlugin>>();
			ViewBag.ShowSMS = false;
			ViewBag.ShowBuy = false;
			if (plugin1 != null && pluginId == plugin1.PluginInfo.PluginId)
			{
				ViewBag.ShowSMS = true;
				ViewBag.LoginLink = plugin1.Biz.GetLoginLink();
				ViewBag.BuyLink = plugin1.Biz.GetBuyLink();
				if (plugin1.Biz.IsSettingsValid)
				{
					ViewBag.Amount = plugin1.Biz.GetSMSAmount();
					ViewBag.ShowBuy = true;
				}
			}
			return View(formData);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Enable(string pluginId, MessageTypeEnum messageType, bool enable)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (!enable)
			{
				plugin.Biz.Disable(messageType);
			}
			else
			{
				plugin.Biz.Enable(messageType);
			}
			return Json(new { success = true });
		}

		public ActionResult Management()
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			return View(objs);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Save(string pluginId, string values)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			IEnumerable<KeyValuePair<string, string>> keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values);
			plugin.Biz.SetFormValues(keyValuePairs);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Send(string pluginId, string destination)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (string.IsNullOrEmpty(destination))
			{
				Result result = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不能为空！")
				};
				return Json(result);
			}
			if (!plugin.Biz.CheckDestination(destination))
			{
				Result result1 = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不正确")
				};
				return Json(result1);
			}
			string siteName = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().SiteName;
			string str = plugin.Biz.SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
			if (str == "发送成功")
			{
				return Json(new { success = true });
			}
			Result result2 = new Result()
			{
				success = false,
				msg = str
			};
			return Json(result2);
		}
	}
}