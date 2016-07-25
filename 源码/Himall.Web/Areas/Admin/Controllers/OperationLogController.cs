using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class OperationLogController : BaseAdminController
	{
		public OperationLogController()
		{
		}

		[Description("关键字获取管理员用户名列表")]
		[UnAuthorize]
		public JsonResult GetManagers(string keyWords)
		{
			IQueryable<ManagerInfo> managers = 
				from item in ServiceHelper.Create<IManagerService>().GetManagers(keyWords)
				where item.ShopId == 0
                select item;
			var variable = 
				from item in managers
				select new { key = item.Id, @value = item.UserName };
			return Json(variable);
		}

		[Description("分页获取日志的JSON数据")]
		[UnAuthorize]
		public JsonResult List(int page, string userName, int rows, DateTime? startDate, DateTime? endDate)
		{
			OperationLogQuery operationLogQuery = new OperationLogQuery()
			{
				UserName = userName,
				PageNo = page,
				PageSize = rows,
				StartDate = startDate,
				EndDate = endDate
			};
			PageModel<LogInfo> platformOperationLogs = ServiceHelper.Create<IOperationLogService>().GetPlatformOperationLogs(operationLogQuery);
			var list = 
				from item in platformOperationLogs.Models.ToList()
				select new { Id = item.Id, UserName = item.UserName, PageUrl = item.PageUrl, Description = item.Description, Date = item.Date.ToString("yyyy-MM-dd HH:mm"), IPAddress = item.IPAddress };
			return Json(new { rows = list, total = platformOperationLogs.Total });
		}

		[Description("平台日志管理页面")]
		public ActionResult Management()
		{
			return View();
		}
	}
}