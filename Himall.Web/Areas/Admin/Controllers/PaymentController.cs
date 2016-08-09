using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
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
	public class PaymentController : BaseAdminController
	{
		public PaymentController()
		{
		}

		public ActionResult Edit(string pluginId)
		{
			ViewBag.Id = pluginId;
			Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(pluginId);
			ViewBag.Name = plugin.PluginInfo.DisplayName;
			return View(plugin.Biz.GetFormData());
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Enable(string pluginId, bool enable)
		{
			Result result = new Result();
			PluginsManagement.EnablePlugin(pluginId, enable);
			result.success = true;
			return Json(result);
		}

		[UnAuthorize]
		public ActionResult Management()
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IPaymentPlugin>().Select<Plugin<IPaymentPlugin>, object>((Plugin<IPaymentPlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				return expandoObjects;
			});
			return View(objs);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Save(string pluginId, string values)
		{
			IPaymentPlugin biz = PluginsManagement.GetPlugin<IPaymentPlugin>(pluginId).Biz;
			biz.SetFormValues(JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values));
			return Json(new { success = true });
		}
	}
}