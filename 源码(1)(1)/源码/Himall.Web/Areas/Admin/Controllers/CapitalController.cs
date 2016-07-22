using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class CapitalController : BaseAdminController
	{
		public CapitalController()
		{
		}

		public JsonResult ApplyWithDrawList(ApplyWithDrawInfo.ApplyWithDrawStatus capitalType, string withdrawno, string user, int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			IMemberService memberService = ServiceHelper.Create<IMemberService>();
			long? nullable = null;
			if (!string.IsNullOrWhiteSpace(user))
			{
				nullable = new long?((memberService.GetMemberByName(user) ?? new UserMemberInfo()
				{
					Id = 0
                }).Id);
			}
			ApplyWithDrawQuery applyWithDrawQuery = new ApplyWithDrawQuery()
			{
				memberId = nullable,
				PageSize = rows,
				PageNo = page,
				withDrawStatus = new ApplyWithDrawInfo.ApplyWithDrawStatus?(capitalType)
			};
			ApplyWithDrawQuery nullable1 = applyWithDrawQuery;
			if (!string.IsNullOrWhiteSpace(withdrawno))
			{
				nullable1.withDrawNo = new long?(long.Parse(withdrawno));
			}
			PageModel<ApplyWithDrawInfo> applyWithDraw = memberCapitalService.GetApplyWithDraw(nullable1);
			IEnumerable<ApplyWithDrawModel> applyWithDrawModels = applyWithDraw.Models.ToList().Select<ApplyWithDrawInfo, ApplyWithDrawModel>((ApplyWithDrawInfo e) => {
				string description = e.ApplyStatus.ToDescription();
				UserMemberInfo member = memberService.GetMember(e.MemId);
				return new ApplyWithDrawModel()
				{
					Id = e.Id,
					ApplyAmount = e.ApplyAmount,
					ApplyStatus = e.ApplyStatus,
					ApplyStatusDesc = description,
					ApplyTime = e.ApplyTime.ToString(),
					NickName = e.NickName,
					MemberName = member.UserName,
					ConfirmTime = e.ConfirmTime.ToString(),
					MemId = e.MemId,
					OpUser = e.OpUser,
					PayNo = e.PayNo,
					PayTime = e.PayTime.ToString(),
					Remark = e.Remark
				};
			});
			DataGridModel<ApplyWithDrawModel> dataGridModel = new DataGridModel<ApplyWithDrawModel>()
			{
				rows = applyWithDrawModels,
				total = applyWithDraw.Total
			};
			return Json(dataGridModel);
		}

		public JsonResult ApplyWithDrawListByUser(long userid, int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			ApplyWithDrawQuery applyWithDrawQuery = new ApplyWithDrawQuery()
			{
				memberId = new long?(userid),
				PageSize = rows,
				PageNo = page
			};
			PageModel<ApplyWithDrawInfo> applyWithDraw = memberCapitalService.GetApplyWithDraw(applyWithDrawQuery);
			IEnumerable<ApplyWithDrawModel> applyWithDrawModels = applyWithDraw.Models.ToList().Select<ApplyWithDrawInfo, ApplyWithDrawModel>((ApplyWithDrawInfo e) => {
				string empty = string.Empty;
				if (e.ApplyStatus == ApplyWithDrawInfo.ApplyWithDrawStatus.PayFail || e.ApplyStatus == ApplyWithDrawInfo.ApplyWithDrawStatus.WaitConfirm)
				{
					empty = "提现中";
				}
				else if (e.ApplyStatus == ApplyWithDrawInfo.ApplyWithDrawStatus.Refuse)
				{
					empty = "提现失败";
				}
				else if (e.ApplyStatus == ApplyWithDrawInfo.ApplyWithDrawStatus.WithDrawSuccess)
				{
					empty = "提现成功";
				}
				return new ApplyWithDrawModel()
				{
					Id = e.Id,
					ApplyAmount = e.ApplyAmount,
					ApplyStatus = e.ApplyStatus,
					ApplyStatusDesc = empty,
					ApplyTime = e.ApplyTime.ToString()
				};
			});
			DataGridModel<ApplyWithDrawModel> dataGridModel = new DataGridModel<ApplyWithDrawModel>()
			{
				rows = applyWithDrawModels,
				total = applyWithDraw.Total
			};
			return Json(dataGridModel);
		}

		public JsonResult ConfirmPay(long id, ApplyWithDrawInfo.ApplyWithDrawStatus status, string remark)
		{
			JsonResult jsonResult;
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			if (status == ApplyWithDrawInfo.ApplyWithDrawStatus.Refuse)
			{
				memberCapitalService.RefuseApplyWithDraw(id, status, base.CurrentManager.UserName, remark);
				return Json(new { success = true, msg = "审核成功！" });
			}
			ApplyWithDrawQuery applyWithDrawQuery = new ApplyWithDrawQuery()
			{
				withDrawNo = new long?(id),
				PageNo = 1,
				PageSize = 1
			};
			ApplyWithDrawQuery applyWithDrawQuery1 = applyWithDrawQuery;
			ApplyWithDrawInfo applyWithDrawInfo = memberCapitalService.GetApplyWithDraw(applyWithDrawQuery1).Models.FirstOrDefault();
			Plugin<IPaymentPlugin> plugin = (
				from e in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
				where e.PluginInfo.PluginId.ToLower().Contains("weixin")
				select e).FirstOrDefault<Plugin<IPaymentPlugin>>();
			if (plugin == null)
			{
				return Json(new { success = false, msg = "未找到支付插件" });
			}
			try
			{
				EnterprisePayPara enterprisePayPara = new EnterprisePayPara()
				{
					amount = applyWithDrawInfo.ApplyAmount,
					check_name = false,
					openid = applyWithDrawInfo.OpenId,
					out_trade_no = applyWithDrawInfo.Id.ToString(),
					desc = "提现"
				};
				PaymentInfo paymentInfo = plugin.Biz.EnterprisePay(enterprisePayPara);
				ApplyWithDrawInfo applyWithDrawInfo1 = new ApplyWithDrawInfo()
				{
					PayNo = paymentInfo.TradNo,
					ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.WithDrawSuccess,
					Remark = plugin.PluginInfo.Description,
					PayTime = new DateTime?((paymentInfo.TradeTime.HasValue ? paymentInfo.TradeTime.Value : DateTime.Now)),
					ConfirmTime = new DateTime?(DateTime.Now),
					OpUser = base.CurrentManager.UserName,
					ApplyAmount = applyWithDrawInfo.ApplyAmount,
					Id = applyWithDrawInfo.Id
				};
				memberCapitalService.ConfirmApplyWithDraw(applyWithDrawInfo1);
				return Json(new { success = true, msg = "付款成功" });
			}
			catch (Exception exception)
			{
				Log.Error(string.Concat("调用企业付款接口异常：", exception.Message));
				ApplyWithDrawInfo applyWithDrawInfo2 = new ApplyWithDrawInfo()
				{
					ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.PayFail,
					Remark = plugin.PluginInfo.Description,
					ConfirmTime = new DateTime?(DateTime.Now),
					OpUser = base.CurrentManager.UserName,
					ApplyAmount = applyWithDrawInfo.ApplyAmount,
					Id = applyWithDrawInfo.Id
				};
				memberCapitalService.ConfirmApplyWithDraw(applyWithDrawInfo2);
				jsonResult = Json(new { success = false, msg = "付款接口异常" });
			}
			return jsonResult;
		}

		public ActionResult Detail(long id)
		{
			ViewBag.UserId = id;
			return View();
		}

		public JsonResult GetMemberCapitals(string user, int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			IMemberService memberService = ServiceHelper.Create<IMemberService>();
			long? nullable = null;
			if (!string.IsNullOrWhiteSpace(user))
			{
				nullable = new long?((memberService.GetMemberByName(user) ?? new UserMemberInfo()
				{
					Id = 0
                }).Id);
			}
			CapitalQuery capitalQuery = new CapitalQuery()
			{
				PageNo = page,
				PageSize = rows,
				Sort = "Balance",
				memberId = nullable
			};
			PageModel<CapitalInfo> capitals = memberCapitalService.GetCapitals(capitalQuery);
			IEnumerable<CapitalModel> capitalModels = capitals.Models.ToList().Select<CapitalInfo, CapitalModel>((CapitalInfo e) => {
				UserMemberInfo member = memberService.GetMember(e.MemId);
				return new CapitalModel()
				{
					Balance = e.Balance.Value,
					ChargeAmount = (e.ChargeAmount.HasValue ? e.ChargeAmount.Value : new decimal(0, 0, 0, false, 2)),
					FreezeAmount = (e.FreezeAmount.HasValue ? e.FreezeAmount.Value : new decimal(0, 0, 0, false, 2)),
					Id = e.Id,
					UserId = e.MemId,
					UserCode = member.UserName,
					UserName = (string.IsNullOrWhiteSpace(member.RealName) ? member.UserName : member.RealName)
				};
			});
			DataGridModel<CapitalModel> dataGridModel = new DataGridModel<CapitalModel>()
			{
				rows = capitalModels,
				total = capitals.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Index()
		{
			return View();
		}

		public JsonResult List(CapitalDetailInfo.CapitalDetailType capitalType, long userid, string startTime, string endTime, int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalDetailQuery capitalDetailQuery = new CapitalDetailQuery()
			{
				memberId = userid,
				capitalType = new CapitalDetailInfo.CapitalDetailType?(capitalType),
				PageSize = rows,
				PageNo = page
			};
			CapitalDetailQuery nullable = capitalDetailQuery;
			if (!string.IsNullOrWhiteSpace(startTime))
			{
				nullable.startTime = new DateTime?(DateTime.Parse(startTime));
			}
			if (!string.IsNullOrWhiteSpace(endTime))
			{
				DateTime dateTime = DateTime.Parse(endTime).AddDays(1);
				nullable.endTime = new DateTime?(dateTime.AddSeconds(-1));
			}
			PageModel<CapitalDetailInfo> capitalDetails = memberCapitalService.GetCapitalDetails(nullable);
			List<CapitalDetailModel> list = (
				from e in capitalDetails.Models.ToList()
				select new CapitalDetailModel()
				{
					Id = e.Id,
					Amount = e.Amount,
					CapitalID = e.CapitalID,
					CreateTime = e.CreateTime.Value.ToString(),
					SourceData = e.SourceData,
					SourceType = e.SourceType,
					Remark = string.Concat(e.SourceType.ToDescription(), ",单号：", e.Id),
					PayWay = e.Remark
				}).ToList();
			DataGridModel<CapitalDetailModel> dataGridModel = new DataGridModel<CapitalDetailModel>()
			{
				rows = list,
				total = capitalDetails.Total
			};
			return Json(dataGridModel);
		}

		public JsonResult Pay(long id)
		{
			return Json(new { success = true, msg = "付款成功" });
		}

		public ActionResult WithDraw()
		{
			return View();
		}
	}
}