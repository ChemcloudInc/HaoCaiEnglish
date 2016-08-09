using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
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
	public class PrivilegeController : BaseAdminController
	{
		public PrivilegeController()
		{
		}

		public ActionResult Add()
		{
            SetPrivileges();
			return View();
		}

		[Description("角色添加")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult Add(string roleJson)
		{
			if (!base.ModelState.IsValid)
			{
				return Json(new { success = true, msg = "验证失败" });
			}
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
			RoleInfo roleInfo = JsonConvert.DeserializeObject<RoleInfo>(roleJson, jsonSerializerSetting);
			ServiceHelper.Create<IPrivilegesService>().AddPlatformRole(roleInfo);
			return Json(new { success = true });
		}

		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			IPrivilegesService privilegesService = ServiceHelper.Create<IPrivilegesService>();
			privilegesService.GetPlatformRole(id);
			if (ServiceHelper.Create<IManagerService>().GetPlatformManagerByRoleId(id).Count() > 0)
			{
				Result result = new Result()
				{
					success = false,
					msg = "该角色下还有管理员，不允许删除！"
				};
				return Json(result);
			}
			privilegesService.DeletePlatformRole(id);
			Result result1 = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result1);
		}

		public ActionResult Edit(long id)
		{
            SetPrivileges();
			RoleInfo platformRole = ServiceHelper.Create<IPrivilegesService>().GetPlatformRole(id);
			RoleInfoModel roleInfoModel = new RoleInfoModel()
			{
				ID = platformRole.Id,
				RoleName = platformRole.RoleName
			};
			RoleInfoModel roleInfoModel1 = roleInfoModel;
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			dynamic viewBag = base.ViewBag;
			ICollection<RolePrivilegeInfo> rolePrivilegeInfo = platformRole.RolePrivilegeInfo;
			viewBag.RolePrivilegeInfo = JsonConvert.SerializeObject(
				from item in rolePrivilegeInfo
				select new { Privilege = item.Privilege }, jsonSerializerSetting);
			return View(roleInfoModel1);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Edit(string roleJson, long id)
		{
			if (!base.ModelState.IsValid)
			{
				return Json(new { success = true, msg = "验证失败" });
			}
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
			RoleInfo roleInfo = JsonConvert.DeserializeObject<RoleInfo>(roleJson, jsonSerializerSetting);
			roleInfo.Id = id;
			ServiceHelper.Create<IPrivilegesService>().UpdatePlatformRole(roleInfo);
			foreach (ManagerInfo list in ServiceHelper.Create<IManagerService>().GetPlatformManagerByRoleId(id).ToList())
			{
				Cache.Remove(CacheKeyCollection.Manager(list.Id));
			}
			return Json(new { success = true });
		}

		[Description("角色列表显示")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult List()
		{
			IQueryable<RoleInfo> platformRoles = ServiceHelper.Create<IPrivilegesService>().GetPlatformRoles();
			var variable = 
				from item in platformRoles
				select new { Id = item.Id, Name = item.RoleName };
			return Json(new { rows = variable });
		}

		public ActionResult Management()
		{
			return View();
		}

		private void SetPrivileges()
		{
			ViewBag.Privileges = PrivilegeHelper.AdminPrivileges;
		}
	}
}