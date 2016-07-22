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

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class PrivilegeController : BaseSellerController
	{
		public PrivilegeController()
		{
		}

		public ActionResult Add()
		{
            SetPrivileges();
			return View();
		}

		[Description("权限组添加")]
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
			RoleInfo shopId = JsonConvert.DeserializeObject<RoleInfo>(roleJson, jsonSerializerSetting);
			shopId.ShopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<IPrivilegesService>().AddSellerRole(shopId);
			return Json(new { success = true });
		}

		[ShopOperationLog(Message="删除商家权限组")]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			IPrivilegesService privilegesService = ServiceHelper.Create<IPrivilegesService>();
			privilegesService.GetPlatformRole(id);
			if (ServiceHelper.Create<IManagerService>().GetSellerManagerByRoleId(id, shopId).Count() > 0)
			{
				Result result = new Result()
				{
					success = false,
					msg = "该权限组下还有管理员，不允许删除！"
				};
				return Json(result);
			}
			privilegesService.DeleteSellerRole(id, shopId);
			Result result1 = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result1);
		}

		public ActionResult Edit(long id)
		{
			long shopId = base.CurrentSellerManager.ShopId;
            SetPrivileges();
			RoleInfo sellerRole = ServiceHelper.Create<IPrivilegesService>().GetSellerRole(id, shopId);
			RoleInfoModel roleInfoModel = new RoleInfoModel()
			{
				ID = sellerRole.Id,
				RoleName = sellerRole.RoleName
			};
			RoleInfoModel roleInfoModel1 = roleInfoModel;
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			dynamic viewBag = base.ViewBag;
			ICollection<RolePrivilegeInfo> rolePrivilegeInfo = sellerRole.RolePrivilegeInfo;
			viewBag.RolePrivilegeInfo = JsonConvert.SerializeObject(
				from item in rolePrivilegeInfo
				select new { Privilege = item.Privilege }, jsonSerializerSetting);
			return View(roleInfoModel1);
		}

		[HttpPost]
		[ShopOperationLog(Message="编辑商家权限组")]
		[UnAuthorize]
		public JsonResult Edit(string roleJson, long id)
		{
			if (!base.ModelState.IsValid)
			{
				return Json(new { success = true, msg = "验证失败" });
			}
			long shopId = base.CurrentSellerManager.ShopId;
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
			RoleInfo roleInfo = JsonConvert.DeserializeObject<RoleInfo>(roleJson, jsonSerializerSetting);
			roleInfo.Id = id;
			roleInfo.ShopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<IPrivilegesService>().UpdateSellerRole(roleInfo);
			List<ManagerInfo> list = ServiceHelper.Create<IManagerService>().GetSellerManagerByRoleId(id, shopId).ToList();
			foreach (ManagerInfo managerInfo in list)
			{
				Cache.Remove(CacheKeyCollection.Seller(managerInfo.Id));
			}
			return Json(new { success = true });
		}

		[Description("角色列表显示")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult List()
		{
			long shopId = base.CurrentSellerManager.ShopId;
			IQueryable<RoleInfo> sellerRoles = ServiceHelper.Create<IPrivilegesService>().GetSellerRoles(shopId);
			var variable = 
				from item in sellerRoles
				select new { Id = item.Id, Name = item.RoleName };
			return Json(new { rows = variable });
		}

		public ActionResult Management()
		{
			return View();
		}

		private void SetPrivileges()
		{
			ViewBag.Privileges = PrivilegeHelper.SellerAdminPrivileges;
		}
	}
}