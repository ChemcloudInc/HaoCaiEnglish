using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ManagerController : BaseSellerController
	{
		public ManagerController()
		{
		}

		[ShopOperationLog(Message="添加卖家子帐号")]
		public JsonResult Add(ManagerInfoModel model)
		{
			string userName = base.CurrentSellerManager.UserName;
			char[] chrArray = new char[] { ':' };
			string str = userName.Split(chrArray)[0];
			long shopId = base.CurrentSellerManager.ShopId;
			string str1 = string.Concat(str, ":", model.UserName);
			ManagerInfo managerInfo = new ManagerInfo()
			{
				UserName = str1,
				Password = model.Password,
				RoleId = model.RoleId,
				ShopId = shopId,
				Remark = model.Remark,
				RealName = model.RealName
			};
			ServiceHelper.Create<IManagerService>().AddSellerManager(managerInfo, str);
			Result result = new Result()
			{
				success = true,
				msg = "添加成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[ShopOperationLog(Message="批量删除管理员")]
		public JsonResult BatchDelete(string ids)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IManagerService>().BatchDeleteSellerManager(nums.ToArray(), shopId);
			Result result = new Result()
			{
				success = true,
				msg = "批量删除成功！"
			};
			return Json(result);
		}

		[ShopOperationLog(Message="修改商家管理员")]
		public JsonResult Change(long id, string password, long roleId, string realName, string reMark)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ManagerInfo managerInfo = new ManagerInfo()
			{
				Id = id,
				Password = password,
				RoleId = roleId,
				RealName = realName,
				Remark = reMark,
				ShopId = shopId
			};
			ServiceHelper.Create<IManagerService>().ChangeSellerManager(managerInfo);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[ShopOperationLog(Message="删除卖家子帐号")]
		public JsonResult Delete(long id)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			if (base.CurrentSellerManager.Id == id)
			{
				Result result = new Result()
				{
					success = false,
					msg = "不能删除自身！"
				};
				return Json(result);
			}
			ServiceHelper.Create<IManagerService>().DeleteSellerManager(id, shopId);
			Result result1 = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result1);
		}

		[UnAuthorize]
		public JsonResult IsExistsUserName(string userName)
		{
			string str = base.CurrentSellerManager.UserName;
			char[] chrArray = new char[] { ':' };
			string str1 = str.Split(chrArray)[0];
			userName = string.Concat(str1, ":", userName);
			return Json(new { Exists = ServiceHelper.Create<IManagerService>().CheckUserNameExist(userName, false) });
		}

		public JsonResult List(int page, string keywords, int rows, bool? status = null)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			long id = base.CurrentSellerManager.Id;
			IManagerService managerService = ServiceHelper.Create<IManagerService>();
			ManagerQuery managerQuery = new ManagerQuery()
			{
				PageNo = page,
				PageSize = rows,
				ShopID = shopId,
				userID = id
			};
			PageModel<ManagerInfo> sellerManagers = managerService.GetSellerManagers(managerQuery);
			List<RoleInfo> list = ServiceHelper.Create<IPrivilegesService>().GetSellerRoles(shopId).ToList();
			var collection = 
				from item in sellerManagers.Models.ToList()
				select new { Id = item.Id, UserName = item.UserName, CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"), RoleName = (
					from a in list
					where a.Id == item.RoleId
					select a).FirstOrDefault().RoleName, RoleId = item.RoleId, realName = item.RealName, reMark = item.Remark };
			return Json(new { rows = collection, total = sellerManagers.Total });
		}

		public ActionResult Management()
		{
			string userName = base.CurrentSellerManager.UserName;
			dynamic viewBag = base.ViewBag;
			char[] chrArray = new char[] { ':' };
			viewBag.MainUserName = userName.Split(chrArray)[0];
			ViewBag.UserId = base.CurrentSellerManager.Id;
			return View();
		}

		[HttpPost]
		public JsonResult RoleList()
		{
			long shopId = base.CurrentSellerManager.ShopId;
			var sellerRoles = 
				from item in ServiceHelper.Create<IPrivilegesService>().GetSellerRoles(shopId)
				select new { Id = item.Id, RoleName = item.RoleName };
			return Json(sellerRoles);
		}
	}
}