using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserCapitalController : BaseMemberController
	{
		public UserCapitalController()
		{
		}
        public ActionResult WithDrawType()
        {
            return View();
        }
        public ActionResult ApplyWithDraw()
        {
            SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            if (string.IsNullOrWhiteSpace(siteSettings.WeixinAppId) || string.IsNullOrWhiteSpace(siteSettings.WeixinAppSecret))
            {
                throw new HimallException("未配置公众号参数");
            }
            string str = AccessTokenContainer.TryGetToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, true);
            SceneModel sceneModel = new SceneModel(QR_SCENE_Type.WithDraw)
            {
                Object = base.CurrentUser.Id.ToString()
            };
            int num = (new SceneHelper()).SetModel(sceneModel, 600);
            CreateQrCodeResult createQrCodeResult = QrCodeApi.Create(str, 300, num, 10000);
            ViewBag.ticket = createQrCodeResult.ticket;
            ViewBag.Sceneid = num;
            IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
            CapitalInfo capitalInfo = memberCapitalService.GetCapitalInfo(base.CurrentUser.Id);
            if (capitalInfo == null)
            {
                ViewBag.ApplyWithMoney = 0;
            }
            else
            {
                dynamic viewBag = base.ViewBag;
                decimal? balance = capitalInfo.Balance;
                ViewBag.ApplyWithMoney = balance.Value;
            }
            string membersId = this.CurrentUser.UserName;
            IEnumerable<WithDrawInfo> WithDraws = ServiceHelper.Create<IWithDrawService>().GetWithDrawByMembersId(membersId);//因为UserName值唯一，所以没有登录账号ID去获取信息

            String[] Array = new String[WithDraws.Count()];
            int i = 0;
            foreach (var item in WithDraws)
            {
                Array[i] = item.WithdrawType + "【" + item.AccountNumber + "," + item.Name + "】";
                i++;
            }
            ViewBag.List = Array;
            ViewBag.Num = Array.Length;


            base.ViewBag.IsSetPwd = (string.IsNullOrWhiteSpace(base.CurrentUser.PayPwd) ? false : true);
            return View();
        }

		public JsonResult ApplyWithDrawList(int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			ApplyWithDrawQuery applyWithDrawQuery = new ApplyWithDrawQuery()
			{
				memberId = new long?(base.CurrentUser.Id),
				PageSize = rows,
				PageNo = page,
				Sort = "ApplyTime"
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

	/*	public JsonResult ApplyWithDrawSubmit(string openid, string nickname, decimal amount, string pwd)   //仅仅支持微信支付
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
			ApplyWithDrawInfo applyWithDrawInfo = new ApplyWithDrawInfo()
			{
				ApplyAmount = amount,
				ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.WaitConfirm,
				ApplyTime = DateTime.Now,
				MemId = base.CurrentUser.Id,
				OpenId = openid,
				NickName = nickname
			};
			ServiceHelper.Create<IMemberCapitalService>().AddWithDrawApply(applyWithDrawInfo);
			return Json(new { success = true });
		}*/

        public JsonResult ApplyWithDrawSubmit(string withdrawtype, string myaccount, string nickname, decimal amount, string pwd)   //扩展版本(提现方式、账号、人名)       
        {
            if (withdrawtype == null)
                return Json(new { success = true });

            if (ServiceHelper.Create<IMemberCapitalService>().GetMemberInfoByPayPwd(base.CurrentUser.Id, pwd) == null)
            {
                throw new HimallException("支付密码不对，请重新输入！");
            }
            CapitalInfo capitalInfo = ServiceHelper.Create<IMemberCapitalService>().GetCapitalInfo(base.CurrentUser.Id);
            decimal num = amount;
            decimal? balance = capitalInfo.Balance;
            if ((num <= balance.GetValueOrDefault() ? false : balance.HasValue))
            {
                throw new HimallException("提现金额不可超出可用金额！");
            }
            ApplyWithDrawInfo applyWithDrawInfo = new ApplyWithDrawInfo()
            {
                ApplyAmount = amount,
                ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.WaitConfirm,
                ApplyTime = DateTime.Now,
                MemId = base.CurrentUser.Id,
                AccountId = 0,
                WithdrawType = withdrawtype,
                Myaccount = myaccount,
                NickName = nickname,

            };
            ServiceHelper.Create<IMemberCapitalService>().AddWithDrawApply(applyWithDrawInfo);
            return Json(new { success = true });
        }
		public ActionResult CapitalCharge()
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalInfo capitalInfo = memberCapitalService.GetCapitalInfo(base.CurrentUser.Id);
			return View(capitalInfo);
		}

		public JsonResult ChargeList(int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			ChargeQuery chargeQuery = new ChargeQuery()
			{
				memberId = new long?(base.CurrentUser.Id),
				PageSize = rows,
				PageNo = page
			};
			PageModel<ChargeDetailInfo> chargeLists = memberCapitalService.GetChargeLists(chargeQuery);
			IEnumerable<ChargeDetailModel> list = 
				from e in chargeLists.Models.ToList()
				select new ChargeDetailModel()
				{
					Id = e.Id.ToString(),
					ChargeAmount = e.ChargeAmount,
					ChargeStatus = e.ChargeStatus,
					ChargeStatusDesc = e.ChargeStatus.ToDescription(),
					ChargeTime = e.ChargeTime.ToString(),
					CreateTime = e.CreateTime.ToString(),
					ChargeWay = e.ChargeWay,
					MemId = e.MemId
				};
			DataGridModel<ChargeDetailModel> dataGridModel = new DataGridModel<ChargeDetailModel>()
			{
				rows = list,
				total = chargeLists.Total
			};
			return Json(dataGridModel);
		}

		public JsonResult ChargeSubmit(decimal amount)
		{
			ChargeDetailInfo chargeDetailInfo = new ChargeDetailInfo()
			{
				ChargeAmount = amount,
				ChargeStatus = ChargeDetailInfo.ChargeDetailStatus.WaitPay,
				CreateTime = new DateTime?(DateTime.Now),
				MemId = base.CurrentUser.Id
			};
			long num = ServiceHelper.Create<IMemberCapitalService>().AddChargeApply(chargeDetailInfo);
			return Json(new { success = true, msg = num.ToString() });
		}

		private string DecodePaymentId(string paymentId)
		{
			return paymentId.Replace("-", ".");
		}

		private string EncodePaymentId(string paymentId)
		{
			return paymentId.Replace(".", "-");
		}

		public ActionResult Index()
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalInfo capitalInfo = memberCapitalService.GetCapitalInfo(base.CurrentUser.Id);
			return View(capitalInfo);
		}

		public JsonResult List(CapitalDetailInfo.CapitalDetailType capitalType, int page, int rows)
		{
			IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
			CapitalDetailQuery capitalDetailQuery = new CapitalDetailQuery()
			{
				memberId = base.CurrentUser.Id,
				capitalType = new CapitalDetailInfo.CapitalDetailType?(capitalType),
				PageSize = rows,
				PageNo = page
			};
			PageModel<CapitalDetailInfo> capitalDetails = memberCapitalService.GetCapitalDetails(capitalDetailQuery);
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

		public JsonResult PaymentList(decimal balance)
		{
			string str3;
			string scheme = base.Request.Url.Scheme;
			string host = base.HttpContext.Request.Url.Host;
			if (base.HttpContext.Request.Url.Port == 80)
			{
				str3 = "";
			}
			else
			{
				int port = base.HttpContext.Request.Url.Port;
				str3 = string.Concat(":", port.ToString());
			}
			string str4 = string.Concat(scheme, "://", host, str3);
			string str5 = string.Concat(str4, "/pay/CapitalChargeReturn/{0}");
			string str6 = string.Concat(str4, "/pay/CapitalChargeNotify/{0}");
			IEnumerable<Plugin<IPaymentPlugin>> plugins = 
				from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
				where item.Biz.SupportPlatforms.Contains<PlatformType>(PlatformType.PC)
				select item;
			long num = ServiceHelper.Create<IMemberCapitalService>().CreateCode(CapitalDetailInfo.CapitalDetailType.ChargeAmount);
			string str7 = num.ToString();
			IEnumerable<PaymentModel> paymentModels = plugins.Select<Plugin<IPaymentPlugin>, PaymentModel>((Plugin<IPaymentPlugin> item) => {
				string empty = string.Empty;
				try
				{
					IPaymentPlugin biz = item.Biz;
					string str = str5;
					string[] strArrays = new string[] { EncodePaymentId(item.PluginInfo.PluginId), "-", balance.ToString(), "-", null };
					strArrays[4] = CurrentUser.Id.ToString();
					string str1 = string.Format(str, string.Concat(strArrays));
					string str2 = str6;
					string[] strArrays1 = new string[] { EncodePaymentId(item.PluginInfo.PluginId), "-", balance.ToString(), "-", null };
					strArrays1[4] = CurrentUser.Id.ToString();
					empty = biz.GetRequestUrl(str1, string.Format(str2, string.Concat(strArrays1)), str7, balance, "预付款充值", null);
				}
				catch (Exception exception)
				{
					Log.Error("支付页面加载支付插件出错", exception);
				}
				return new PaymentModel()
				{
					Logo = string.Concat("/Plugins/Payment/", item.PluginInfo.ClassFullName.Split(new char[] { ',' })[1], "/", item.Biz.Logo),
					RequestUrl = empty,
					UrlType = item.Biz.RequestUrlType,
					Id = item.PluginInfo.PluginId
				};
			});
			paymentModels = 
				from item in paymentModels
				where !string.IsNullOrEmpty(item.RequestUrl)
				select item;
			return Json(paymentModels);
		}

		public JsonResult SavePayPwd(string pwd)
		{
			ServiceHelper.Create<IMemberCapitalService>().SetPayPwd(base.CurrentUser.Id, pwd);
			return Json(new { success = true, msg = "设置成功" });
		}
        public JsonResult SelectAccount(string account)
        {
            string[] array = account.Split('[', ']', ',', '【', '】', '，');
            if (array.Length < 3)                                                   //可在此添加账号判断
                return Json(new { success = false });
            return Json(new { success = true, type = array[0], myaccount = array[1], nikename = array[2] });
        }

		public ActionResult SetPayPwd()
		{
			return View();
		}
	}
}