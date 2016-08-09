using AutoMapper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class MemberInviteController : BaseAdminController
	{
		public MemberInviteController()
		{
		}

		public JsonResult GetMembers(bool? status, string keyWords)
		{
			IQueryable<UserMemberInfo> members = ServiceHelper.Create<IMemberService>().GetMembers(status, keyWords);
			var variable = 
				from item in members
				select new { key = item.Id, @value = item.UserName };
			return Json(variable);
		}

		public ActionResult List(int page, string keywords, int rows)
		{
			InviteRecordQuery inviteRecordQuery = new InviteRecordQuery()
			{
				PageNo = page,
				PageSize = rows,
				userName = keywords
			};
			PageModel<InviteRecordInfo> inviteList = ServiceHelper.Create<IMemberInviteService>().GetInviteList(inviteRecordQuery);
			var list = 
				from a in inviteList.Models.ToList()
				select new { Id = a.Id, InviteIntegral = a.InviteIntegral, RegTime = a.RegTime.Value.ToString("yyyy-MM-dd"), RegIntegral = a.RegIntegral, RegName = a.RegName, UserName = a.UserName };
			return Json(new { rows = list, total = inviteList.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SaveSetting(InviteRuleModel model)
		{
			if (!base.ModelState.IsValid)
			{
				Result result = new Result()
				{
					success = false,
					msg = "数据验证错误！"
				};
				return Json(result);
			}
			Mapper.CreateMap<InviteRuleModel, InviteRuleInfo>();
			InviteRuleInfo inviteRuleInfo = Mapper.Map<InviteRuleModel, InviteRuleInfo>(model);
			ServiceHelper.Create<IMemberInviteService>().SetInviteRule(inviteRuleInfo);
			Result result1 = new Result()
			{
				success = true,
				msg = "保存成功！"
			};
			return Json(result1);
		}

		public ActionResult Setting()
		{
			InviteRuleInfo inviteRule = ServiceHelper.Create<IMemberInviteService>().GetInviteRule();
			Mapper.CreateMap<InviteRuleInfo, InviteRuleModel>();
			return View(Mapper.Map<InviteRuleInfo, InviteRuleModel>(inviteRule));
		}
	}
}