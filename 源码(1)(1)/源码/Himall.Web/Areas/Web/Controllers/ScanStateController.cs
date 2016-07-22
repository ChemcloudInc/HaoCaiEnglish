using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ScanStateController : BaseAsyncController
	{
		public ScanStateController()
		{
		}

		public void GetStateAsync(string sceneid)
		{
			base.AsyncManager.OutstandingOperations.Increment();
			int num1 = 200;
			int num2 = 10000;
			Task.Factory.StartNew(() => {
				int num = 0;
				while (true)
				{
					ApplyWithDrawInfo applyWithDrawInfo = Cache.Get(CacheKeyCollection.SceneReturn(sceneid)) as ApplyWithDrawInfo;
					if (applyWithDrawInfo != null)
					{
                        AsyncManager.Parameters["state"] = true;
                        AsyncManager.Parameters["model"] = applyWithDrawInfo;
						break;
					}
					else if (num < num2)
					{
						num = num + num1;
						Thread.Sleep(num1);
					}
					else
					{
                        AsyncManager.Parameters["state"] = false;
                        AsyncManager.Parameters["model"] = applyWithDrawInfo;
						break;
					}
				}
                AsyncManager.OutstandingOperations.Decrement();
			});
		}

		public JsonResult GetStateCompleted(bool state, ApplyWithDrawInfo model)
		{
			return Json(new { success = state, data = model }, JsonRequestBehavior.AllowGet);
		}
	}
}