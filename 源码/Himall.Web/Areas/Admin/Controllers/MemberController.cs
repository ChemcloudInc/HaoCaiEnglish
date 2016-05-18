using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
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
	public class MemberController : BaseAdminController
	{
		public MemberController()
		{
		}

		[HttpPost]
		public JsonResult BatchDelete(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IMemberService>().BatchDeleteMember(nums.ToArray());
			Result result = new Result()
			{
				success = true,
				msg = "批量删除成功！"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult BatchLock(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IMemberService>().BatchLock(nums.ToArray());
			Result result = new Result()
			{
				success = true,
				msg = "批量锁定成功！"
			};
			return Json(result);
		}

		public JsonResult ChangePassWord(long id, string password)
		{
			ServiceHelper.Create<IMemberService>().ChangePassWord(id, password);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result);
		}

		[HttpPost]
		[OperationLog(Message="删除会员", ParameterNameList="id")]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<IMemberService>().DeleteMember(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		public ActionResult Detail(long id)
		{
			UserMemberInfo member = ServiceHelper.Create<IMemberService>().GetMember(id);
			string regionFullName = ServiceHelper.Create<IRegionService>().GetRegionFullName(member.RegionId, " ");
			MemberModel memberModel = new MemberModel()
			{
				Id = member.Id,
				UserName = member.UserName,
				LastLoginDate = member.LastLoginDate,
				QQ = member.QQ,
				Points = member.Points,
				RealName = member.RealName,
				Email = member.Email,
				Disabled = member.Disabled,
				Expenditure = member.Expenditure,
				OrderNumber = member.OrderNumber,
				CellPhone = member.CellPhone,
				CreateDate = member.CreateDate,
				Address = member.Address
			};
			MemberModel memberModel1 = memberModel;
			ViewBag.Region = regionFullName;
			return PartialView("Detail", memberModel1);
		}

		public JsonResult GetMembers(bool? status, string keyWords)
		{
			IQueryable<UserMemberInfo> members = ServiceHelper.Create<IMemberService>().GetMembers(status, keyWords);
			var variable = 
				from item in members
				select new { key = item.Id, @value = item.UserName };
			return Json(variable);
		}

		[Description("分页获取会员管理JSON数据")]
		public JsonResult List(int page, string keywords, int rows, bool? status = null)
		{
			IMemberService memberService = ServiceHelper.Create<IMemberService>();
			MemberQuery memberQuery = new MemberQuery()
			{
				keyWords = keywords,
				Status = status,
				PageNo = page,
				PageSize = rows
			};
			PageModel<UserMemberInfo> members = memberService.GetMembers(memberQuery);
			IQueryable<MemberModel> models = 
				from item in members.Models
				select new MemberModel()
				{
					Id = item.Id,
					UserName = item.UserName,
					LastLoginDate = item.LastLoginDate,
					CreateDate = item.CreateDate,
					QQ = item.QQ,
					Points = item.Points,
					RealName = item.RealName,
					Email = item.Email,
					CellPhone = item.CellPhone,
					Disabled = item.Disabled
				};
			DataGridModel<MemberModel> dataGridModel = new DataGridModel<MemberModel>()
			{
				rows = models,
				total = members.Total
			};
			return Json(dataGridModel);
		}

		public JsonResult Lock(long id)
		{
			ServiceHelper.Create<IMemberService>().LockMember(id);
			Result result = new Result()
			{
				success = true,
				msg = "冻结成功！"
			};
			return Json(result);
		}

		[Description("会员管理页面")]
		public ActionResult Management()
		{
			return View();
		}

		public JsonResult UnLock(long id)
		{
			ServiceHelper.Create<IMemberService>().UnLockMember(id);
			Result result = new Result()
			{
				success = true,
				msg = "解冻成功！"
			};
			return Json(result);
		}
	}
}