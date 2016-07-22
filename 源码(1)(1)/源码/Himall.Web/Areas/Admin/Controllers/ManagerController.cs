using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ManagerController : BaseAdminController
	{
		public ManagerController()
		{
		}

		public JsonResult Add(ManagerInfoModel model)
		{
			ManagerInfo managerInfo = new ManagerInfo()
			{
				UserName = model.UserName,
				Password = model.Password,
				RoleId = model.RoleId
			};
			ServiceHelper.Create<IManagerService>().AddPlatformManager(managerInfo);
			Result result = new Result()
			{
				success = true,
				msg = "添加成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchDelete(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IManagerService>().BatchDeletePlatformManager(nums.ToArray());
			Result result = new Result()
			{
				success = true,
				msg = "批量删除成功！"
			};
			return Json(result);
		}

		[UnAuthorize]
		public JsonResult ChangePassWord(long id, string password, long roleId)
		{
			ServiceHelper.Create<IManagerService>().ChangePlatformManagerPassword(id, password, roleId);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<IManagerService>().DeletePlatformManager(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		[UnAuthorize]
		public JsonResult IsExistsUserName(string userName)
		{
			return Json(new { Exists = ServiceHelper.Create<IManagerService>().CheckUserNameExist(userName, true) });
		}

		[UnAuthorize]
		public JsonResult List(int page, string keywords, int rows, bool? status = null)
		{
			IManagerService managerService = ServiceHelper.Create<IManagerService>();
			ManagerQuery managerQuery = new ManagerQuery()
			{
				PageNo = page,
				PageSize = rows
			};
			PageModel<ManagerInfo> platformManagers = managerService.GetPlatformManagers(managerQuery);
			List<RoleInfo> list = ServiceHelper.Create<IPrivilegesService>().GetPlatformRoles().ToList();
			var collection = 
				from item in platformManagers.Models.ToList()
				select new { Id = item.Id, UserName = item.UserName, CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"), RoleName = (item.RoleId == 0 ? "系统管理员" : (
					from a in list
					where a.Id == item.RoleId
					select a).FirstOrDefault().RoleName), RoleId = item.RoleId };
			return Json(new { rows = collection, total = platformManagers.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult RoleList()
		{
			var platformRoles = 
				from item in ServiceHelper.Create<IPrivilegesService>().GetPlatformRoles()
				select new { Id = item.Id, RoleName = item.RoleName };
			return Json(platformRoles);
		}
	}
}