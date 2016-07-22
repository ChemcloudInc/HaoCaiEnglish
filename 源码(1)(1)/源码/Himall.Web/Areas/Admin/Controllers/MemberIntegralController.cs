using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
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
	public class MemberIntegralController : BaseAdminController
	{
		public MemberIntegralController()
		{
		}

		public ActionResult Detail(int id)
		{
			ViewBag.UserId = id;
			return View();
		}

		private string GetMemberGrade(IEnumerable<MemberGrade> memberGrade, int historyIntegrals)
		{
			MemberGrade memberGrade1 = (
				from a in memberGrade
				where a.Integral <= historyIntegrals
				orderby a.Integral descending
				select a).FirstOrDefault();
			if (memberGrade1 == null)
			{
				return "Vip0";
			}
			return memberGrade1.GradeName;
		}

		[HttpPost]
		public JsonResult GetMemberIntegralDetail(int page, int? userId, MemberIntegral.IntegralType? type, DateTime? startDate, DateTime? endDate, int rows)
		{
			long? nullable;
			IntegralRecordQuery integralRecordQuery = new IntegralRecordQuery()
			{
				StartDate = startDate,
				EndDate = endDate,
				IntegralType = type
			};
			IntegralRecordQuery integralRecordQuery1 = integralRecordQuery;
			int? nullable1 = userId;
			if (nullable1.HasValue)
			{
				nullable = new long?(nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			integralRecordQuery1.UserId = nullable;
			integralRecordQuery.PageNo = page;
			integralRecordQuery.PageSize = rows;
			PageModel<MemberIntegralRecord> integralRecordList = ServiceHelper.Create<IMemberIntegralService>().GetIntegralRecordList(integralRecordQuery);
			var list = 
				from item in integralRecordList.Models.ToList()
				select new { Id = item.Id, UserName = item.UserName, RecordDate = item.RecordDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), Integral = item.Integral, Type = item.TypeId.ToDescription(), Remark = GetRemarkFromIntegralType(item.TypeId, item.Himall_MemberIntegralRecordAction, item.ReMark) };
			return Json(new { rows = list, total = integralRecordList.Total });
		}

		[HttpPost]
		public JsonResult GetMembers(bool? status, string keyWords)
		{
			IQueryable<UserMemberInfo> members = ServiceHelper.Create<IMemberService>().GetMembers(status, keyWords);
			var variable = 
				from item in members
				select new { key = item.Id, @value = item.UserName };
			return Json(variable);
		}

		private string GetRemarkFromIntegralType(MemberIntegral.IntegralType type, ICollection<MemberIntegralRecordAction> recordAction, string remark = "")
		{
			if (recordAction == null || recordAction.Count == 0)
			{
				return remark;
			}
			MemberIntegral.IntegralType integralType = type;
			if (integralType == MemberIntegral.IntegralType.Consumption)
			{
				string str = "";
				foreach (MemberIntegralRecordAction memberIntegralRecordAction in recordAction)
				{
					str = string.Concat(str, memberIntegralRecordAction.VirtualItemId, ",");
				}
				char[] chrArray = new char[] { ',' };
				remark = string.Concat("使用订单号(", str.TrimEnd(chrArray), ")");
			}
			else
			{
				if (integralType != MemberIntegral.IntegralType.Comment)
				{
					return remark;
				}
				remark = string.Concat("商品评价（商品ID：", recordAction.FirstOrDefault().VirtualItemId, ")");
			}
			return remark;
		}

		[Description("分页获取会员积分JSON数据")]
		public JsonResult List(int page, string userName, DateTime? startDate, DateTime? endDate, int rows)
		{
			IEnumerable<MemberGrade> memberGradeList = ServiceHelper.Create<IMemberGradeService>().GetMemberGradeList();
			IMemberIntegralService memberIntegralService = ServiceHelper.Create<IMemberIntegralService>();
			IntegralQuery integralQuery = new IntegralQuery()
			{
				UserName = userName,
				StartDate = startDate,
				EndDate = endDate,
				PageNo = page,
				PageSize = rows
			};
			PageModel<MemberIntegral> memberIntegralList = memberIntegralService.GetMemberIntegralList(integralQuery);
			var list = 
				from item in memberIntegralList.Models.ToList()
				select new { Id = item.Id, UserName = item.UserName, UserId = item.MemberId, AvailableIntegrals = item.AvailableIntegrals, MemberGrade = GetMemberGrade(memberGradeList, item.HistoryIntegrals), HistoryIntegrals = item.HistoryIntegrals, RegDate = item.Himall_Members.CreateDate.ToString("yyyy-MM-dd") };
			return Json(new { rows = list, total = memberIntegralList.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Management(string Operation, int Integral, string userName, int? userId, string reMark)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				throw new HimallException("该用户不存在");
			}
			UserMemberInfo memberByName = ServiceHelper.Create<IMemberService>().GetMemberByName(userName);
			if (memberByName == null)
			{
				throw new HimallException("该用户不存在");
			}
			if (Integral <= 0)
			{
				throw new HimallException("积分必须为大于0的整数");
			}
			MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
			{
				UserName = userName,
				MemberId = memberByName.Id,
				RecordDate = new DateTime?(DateTime.Now),
				TypeId = MemberIntegral.IntegralType.SystemOper,
				ReMark = reMark
			};
			if (Operation == "sub")
			{
				Integral = -Integral;
			}
			IConversionMemberIntegralBase conversionMemberIntegralBase = ServiceHelper.Create<IMemberIntegralConversionFactoryService>().Create(MemberIntegral.IntegralType.SystemOper, Integral);
			ServiceHelper.Create<IMemberIntegralService>().AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
			Result result = new Result()
			{
				success = true,
				msg = "操作成功"
			};
			return Json(result);
		}

		public ActionResult Search()
		{
			return View();
		}
	}
}