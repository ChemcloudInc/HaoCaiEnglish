using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class CapitalController : BaseMobileMemberController
	{
		public CapitalController()
		{
		}

		public JsonResult ApplyWithDrawSubmit(string nickname, decimal amount, string pwd)
		{
			if (ServiceHelper.Create<IMemberCapitalService>().GetMemberInfoByPayPwd(base.CurrentUser.Id, pwd) == null)
			{
				throw new HimallException("支付密码不对，请重新输入！");
			}
			CapitalInfo capitalInfo = ServiceHelper.Create<IMemberCapitalService>().GetCapitalInfo(base.CurrentUser.Id);
			decimal num = amount;
			decimal? balance = capitalInfo.Balance;
			if ((num <= balance.GetValueOrDefault() ? false : balance.HasValue))
			{
				throw new HimallException("提现金额不能超出可用金额！");
			}
			string cookie = WebHelper.GetCookie("Himall-User_OpenId");
			string empty = string.Empty;
			if (string.IsNullOrWhiteSpace(cookie))
			{
				throw new HimallException("数据异常,OpenId不能为空！");
			}
			cookie = SecureHelper.AESDecrypt(cookie, "Mobile");
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			if (!string.IsNullOrWhiteSpace(siteSettings.WeixinAppId) && !string.IsNullOrWhiteSpace(siteSettings.WeixinAppSecret))
			{
				string str = AccessTokenContainer.TryGetToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, false);
				WeixinUserInfoResult userInfo = CommonApi.GetUserInfo(str, cookie);
				if (userInfo != null)
				{
					empty = userInfo.nickname;
				}
			}
			ApplyWithDrawInfo applyWithDrawInfo = new ApplyWithDrawInfo()
			{
				ApplyAmount = amount,
				ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.WaitConfirm,
				ApplyTime = DateTime.Now,
				MemId = base.CurrentUser.Id,
				OpenId = cookie,
				NickName = empty
			};
			ServiceHelper.Create<IMemberCapitalService>().AddWithDrawApply(applyWithDrawInfo);
			return Json(new { success = true });
		}

		public ActionResult Index()
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalInfo capitalInfo = memberCapitalService.GetCapitalInfo(base.CurrentUser.Id);
			decimal num = new decimal(0);
			if (capitalInfo != null)
			{
				num = (
					from e in capitalInfo.Himall_CapitalDetail
					where e.SourceType == CapitalDetailInfo.CapitalDetailType.RedPacket
					select e).Sum<CapitalDetailInfo>((CapitalDetailInfo e) => e.Amount);
				dynamic viewBag = base.ViewBag;
				ICollection<CapitalDetailInfo> himallCapitalDetail = capitalInfo.Himall_CapitalDetail;
				viewBag.CapitalDetails = (
					from e in himallCapitalDetail
					orderby e.CreateTime descending
					select e).Take(15);
			}
			ViewBag.RedPacketAmount = num;
			base.ViewBag.IsSetPwd = (string.IsNullOrWhiteSpace(base.CurrentUser.PayPwd) ? false : true);
			return View(capitalInfo);
		}

		public JsonResult List(int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalDetailQuery capitalDetailQuery = new CapitalDetailQuery()
			{
				memberId = base.CurrentUser.Id,
				PageSize = rows,
				PageNo = page
			};
			PageModel<CapitalDetailInfo> capitalDetails = memberCapitalService.GetCapitalDetails(capitalDetailQuery);
			IEnumerable<CapitalDetailModel> list = 
				from e in capitalDetails.Models.ToList()
				select new CapitalDetailModel()
				{
					Id = e.Id,
					Amount = e.Amount,
					CapitalID = e.CapitalID,
					CreateTime = e.CreateTime.Value.ToString(),
					SourceData = e.SourceData,
					SourceType = e.SourceType,
					Remark = e.SourceType.ToDescription(),
					PayWay = e.Remark
				};
			return Json(list);
		}

		public JsonResult SetPayPwd(string pwd)
		{
			ServiceHelper.Create<IMemberCapitalService>().SetPayPwd(base.CurrentUser.Id, pwd);
			return Json(new { success = true, msg = "设置成功" });
		}
	}
}