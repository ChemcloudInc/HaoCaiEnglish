using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class MemberGradeController : BaseAdminController
	{
		public MemberGradeController()
		{
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Add(MemberGrade model)
		{
			string str;
			if (!CheckMemberGrade(model, out str))
			{
				Result result = new Result()
				{
					success = false,
					msg = str
				};
				return Json(result);
			}
			ServiceHelper.Create<IMemberGradeService>().AddMemberGrade(model);
			Result result1 = new Result()
			{
				success = true,
				msg = "添加成功！"
			};
			return Json(result1);
		}

		private bool CheckMemberGrade(MemberGrade model, out string erroMsg)
		{
			bool flag = true;
			erroMsg = "";
			if (string.IsNullOrWhiteSpace(model.GradeName))
			{
				erroMsg = "会员等级名称不能为空";
				flag = false;
			}
			if (model.Integral < 0)
			{
				erroMsg = "积分不能小于0";
				flag = false;
			}
			return flag;
		}

		[Description("删除会员等级")]
		[HttpPost]
		public JsonResult Delete(int id)
		{
			ServiceHelper.Create<IMemberGradeService>().DeleteMemberGrade(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		public ActionResult Edit(long id)
		{
			return View(ServiceHelper.Create<IMemberGradeService>().GetMemberGrade(id));
		}

		[HttpPost]
		public JsonResult Edit(MemberGrade model)
		{
			string str;
			if (!CheckMemberGrade(model, out str))
			{
				Result result = new Result()
				{
					success = false,
					msg = str
				};
				return Json(result);
			}
			ServiceHelper.Create<IMemberGradeService>().UpdateMemberGrade(model);
			Result result1 = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result1);
		}

		public ActionResult Management()
		{
			return View(ServiceHelper.Create<IMemberGradeService>().GetMemberGradeList());
		}
	}
}