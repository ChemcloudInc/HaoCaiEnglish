using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.OAuth;
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
	public class OAuthController : BaseAdminController
	{
		public OAuthController()
		{
		}

		public ActionResult Edit(string pluginId)
		{
			ViewBag.Id = pluginId;
			Plugin<IOAuthPlugin> plugin = PluginsManagement.GetPlugin<IOAuthPlugin>(pluginId);
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

		public ActionResult Management()
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IOAuthPlugin>().Select<Plugin<IOAuthPlugin>, object>((Plugin<IOAuthPlugin> item) => {
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
		[ValidateInput(false)]
		public JsonResult Save(string pluginId, string values)
		{
			Plugin<IOAuthPlugin> plugin = PluginsManagement.GetPlugin<IOAuthPlugin>(pluginId);
			IEnumerable<KeyValuePair<string, string>> keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values);
			plugin.Biz.SetFormValues(keyValuePairs);
			return Json(new { success = true });
		}
	}
}