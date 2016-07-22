using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.API
{
	public class ExpressDataController : BaseController
	{
		public ExpressDataController()
		{
		}

		public JsonResult Search(string expressCompanyName, string shipOrderNumber)
		{
			ExpressData expressData = ServiceHelper.Create<IExpressService>().GetExpressData(expressCompanyName, shipOrderNumber);
			if (expressData.Success)
			{
				expressData.ExpressDataItems = 
					from item in expressData.ExpressDataItems
					orderby item.Time descending
					select item;
			}
			var variable = new { success = expressData.Success, msg = expressData.Message, data = 
				from item in expressData.ExpressDataItems
				select new { time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"), content = item.Content } };
			return base.Json(variable);
		}

		public JsonResult SearchTop(string expressCompanyName, string shipOrderNumber)
		{
			ExpressData expressData = ServiceHelper.Create<IExpressService>().GetExpressData(expressCompanyName, shipOrderNumber);
			if (expressData.Success)
			{
				expressData.ExpressDataItems = 
					from item in expressData.ExpressDataItems
					orderby item.Time descending
					select item;
			}
			var variable = new { success = expressData.Success, msg = expressData.Message, data = 
				from item in expressData.ExpressDataItems.Take<ExpressDataItem>(1)
				select new { time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"), content = item.Content } };
			return base.Json(variable);
		}
	}
}