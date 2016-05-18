using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.IServices;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ExpressTemplateController : BaseAdminController
	{
		public ExpressTemplateController()
		{
		}

		public ActionResult Edit(string name)
		{
			return View(ServiceHelper.Create<IExpressService>().GetExpress(name));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetConfig(string name)
		{
			IExpress express = ServiceHelper.Create<IExpressService>().GetExpress(name);
			ExpressTemplateConfig expressTemplateConfig = new ExpressTemplateConfig()
			{
				width = express.Width,
				height = express.Height,
				data = (
					from item in ExpressPrintElement.AllPrintElements
					select new Element()
					{
						key = item.Key.ToString(),
						@value = item.Value
					}).ToArray()
			};
			ExpressTemplateConfig expressTemplateConfig1 = expressTemplateConfig;
			if (express.Elements != null)
			{
				int num = 0;
				foreach (ExpressPrintElement element in express.Elements)
				{
					Element element1 = ((IEnumerable<Element>)expressTemplateConfig1.data).FirstOrDefault((Element t) => t.key == element.PrintElementIndex.ToString());
					int[] x = new int[] { element.LeftTopPoint.X, element.LeftTopPoint.Y };
					element1.a = x;
					int[] numArray = new int[] { element.RightBottomPoint.X, element.RightBottomPoint.Y };
					element1.b = numArray;
					element1.selected = true;
					num++;
				}
				expressTemplateConfig1.selectedCount = num;
			}
			return Json(expressTemplateConfig1, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Management()
		{
			return View(ServiceHelper.Create<IExpressService>().GetAllExpress());
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Save(string elements, string name)
		{
			elements = elements.Replace("\"[", "[").Replace("]\"", "]");
			IEnumerable<ExpressElement> expressElements = JsonConvert.DeserializeObject<IEnumerable<ExpressElement>>(elements);
			ServiceHelper.Create<IExpressService>().GetExpress(name);
			IEnumerable<ExpressPrintElement> expressPrintElement = 
				from item in expressElements
				select new ExpressPrintElement()
				{
					LeftTopPoint = new ExpressPrintElement.Point()
					{
						X = item.a[0],
						Y = item.a[1]
					},
					RightBottomPoint = new ExpressPrintElement.Point()
					{
						X = item.b[0],
						Y = item.b[1]
					},
					PrintElementIndex = item.name
				};
			ServiceHelper.Create<IExpressService>().UpdatePrintElement(name, expressPrintElement);
			return Json(new { success = true });
		}
	}
}