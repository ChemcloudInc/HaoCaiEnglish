using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class CashDepositController : BaseSellerController
	{
		public CashDepositController()
		{
		}

		public JsonResult CashDepositDetail(long cashDepositId, int pageNo = 1, int pageSize = 10)
		{
			CashDepositDetailQuery cashDepositDetailQuery = new CashDepositDetailQuery()
			{
				CashDepositId = cashDepositId,
				PageNo = pageNo,
				PageSize = pageSize
			};
			PageModel<CashDepositDetailInfo> cashDepositDetails = ServiceHelper.Create<ICashDepositsService>().GetCashDepositDetails(cashDepositDetailQuery);
			var array = 
				from item in cashDepositDetails.Models.ToArray()
                select new { Id = item.Id, Date = item.AddDate.ToString("yyyy-MM-dd HH:mm"), Balance = item.Balance, Operator = item.Operator, Description = item.Description };
			return Json(new { rows = array, total = cashDepositDetails.Total });
		}

		[ActionName("CashNotify")]
		[ValidateInput(false)]
		public ContentResult CashPayNotify_Post(string id, string str)
		{
			char[] chrArray = new char[] { '-' };
			decimal num = decimal.Parse(str.Split(chrArray)[0]);
			char[] chrArray1 = new char[] { '-' };
			string str1 = str.Split(chrArray1)[1];
			char[] chrArray2 = new char[] { '-' };
			long num1 = long.Parse(str.Split(chrArray2)[2]);
			id = DecodePaymentId(id);
			string empty = string.Empty;
			string empty1 = string.Empty;
			try
			{
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				PaymentInfo paymentInfo = plugin.Biz.ProcessReturn(base.HttpContext.Request);
				if ((Cache.Get(CacheKeyCollection.PaymentState(string.Join<long>(",", paymentInfo.OrderIds))) == null ? true : false))
				{
					ICashDepositsService cashDepositsService = ServiceHelper.Create<ICashDepositsService>();
					CashDepositDetailInfo cashDepositDetailInfo = new CashDepositDetailInfo()
					{
						AddDate = DateTime.Now,
						Balance = num,
						Description = "充值",
						Operator = str1
					};
					List<CashDepositDetailInfo> cashDepositDetailInfos = new List<CashDepositDetailInfo>()
					{
						cashDepositDetailInfo
					};
					if (cashDepositsService.GetCashDepositByShopId(num1) != null)
					{
						cashDepositDetailInfo.CashDepositId = cashDepositsService.GetCashDepositByShopId(num1).Id;
						ServiceHelper.Create<ICashDepositsService>().AddCashDepositDetails(cashDepositDetailInfo);
					}
					else
					{
						CashDepositInfo cashDepositInfo = new CashDepositInfo()
						{
							CurrentBalance = num,
							Date = DateTime.Now,
							ShopId = num1,
							TotalBalance = num,
							EnableLabels = true,
							Himall_CashDepositDetail = cashDepositDetailInfos
						};
						cashDepositsService.AddCashDeposit(cashDepositInfo);
					}
					empty1 = plugin.Biz.ConfirmPayResult();
					string str2 = CacheKeyCollection.PaymentState(string.Join<long>(",", paymentInfo.OrderIds));
					Cache.Insert(str2, true);
				}
			}
			catch (Exception exception)
			{
				string message = exception.Message;
			}
			return base.Content(empty1);
		}

		private string DecodePaymentId(string paymentId)
		{
			return paymentId.Replace("-", ".");
		}

		private string EncodePaymentId(string paymentId)
		{
			return paymentId.Replace(".", "-");
		}

		public ActionResult Management()
		{
			CashDepositInfo cashDepositByShopId = ServiceHelper.Create<ICashDepositsService>().GetCashDepositByShopId(base.CurrentSellerManager.ShopId);
			ViewBag.NeedPayCashDeposit = ServiceHelper.Create<ICashDepositsService>().GetNeedPayCashDepositByShopId(base.CurrentSellerManager.ShopId);
			return View(cashDepositByShopId);
		}

		public JsonResult PaymentList(decimal balance)
		{
			string str;
			decimal needPayCashDepositByShopId = ServiceHelper.Create<ICashDepositsService>().GetNeedPayCashDepositByShopId(base.CurrentSellerManager.ShopId);
			if (balance < needPayCashDepositByShopId)
			{
				throw new HimallException("缴纳保证金必须大于应缴保证金");
			}
			string scheme = base.Request.Url.Scheme;
			string host = base.HttpContext.Request.Url.Host;
			if (base.HttpContext.Request.Url.Port == 80)
			{
				str = "";
			}
			else
			{
				int port = base.HttpContext.Request.Url.Port;
				str = string.Concat(":", port.ToString());
			}
			string str1 = string.Concat(scheme, "://", host, str);
			string str2 = string.Concat(str1, "/SellerAdmin/CashDeposit/Return/{0}?balance={1}");
			string str3 = string.Concat(str1, "/pay/CashNotify/{0}?str={1}");
			IEnumerable<Plugin<IPaymentPlugin>> plugins = 
				from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
				where item.Biz.SupportPlatforms.Contains<PlatformType>(PlatformType.PC)
				select item;
			string str4 = DateTime.Now.ToString("yyyyMMddmmss");
			long shopId = base.CurrentSellerManager.ShopId;
			string str5 = string.Concat(str4, shopId.ToString());
			IEnumerable<PaymentModel> paymentModels = plugins.Select<Plugin<IPaymentPlugin>, PaymentModel>((Plugin<IPaymentPlugin> item) => {
				string empty = string.Empty;
				try
				{
					empty = item.Biz.GetRequestUrl(string.Format(str2, EncodePaymentId(item.PluginInfo.PluginId), balance), string.Format(str3, EncodePaymentId(item.PluginInfo.PluginId), string.Concat(new object[] { balance, "-", CurrentSellerManager.UserName, "-", CurrentSellerManager.ShopId })), str5, balance, "保证金充值", null);
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
			paymentModels = paymentModels.Where((PaymentModel item) => {
				if (string.IsNullOrEmpty(item.RequestUrl))
				{
					return false;
				}
				return item.Id != "Himall.Plugin.Payment.WeiXinPay";
			});
			return Json(paymentModels);
		}

		public ActionResult Return(string id, decimal balance)
		{
			id = DecodePaymentId(id);
			string empty = string.Empty;
			try
			{
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				PaymentInfo paymentInfo = plugin.Biz.ProcessReturn(base.HttpContext.Request);
				ICashDepositsService cashDepositsService = ServiceHelper.Create<ICashDepositsService>();
				CashDepositDetailInfo cashDepositDetailInfo = new CashDepositDetailInfo();
				if ((Cache.Get(CacheKeyCollection.PaymentState(string.Join<long>(",", paymentInfo.OrderIds))) == null ? true : false))
				{
					cashDepositDetailInfo.AddDate = DateTime.Now;
					cashDepositDetailInfo.Balance = balance;
					cashDepositDetailInfo.Description = "充值";
					cashDepositDetailInfo.Operator = base.CurrentSellerManager.UserName;
					List<CashDepositDetailInfo> cashDepositDetailInfos = new List<CashDepositDetailInfo>()
					{
						cashDepositDetailInfo
					};
					if (cashDepositsService.GetCashDepositByShopId(base.CurrentSellerManager.ShopId) != null)
					{
						cashDepositDetailInfo.CashDepositId = cashDepositsService.GetCashDepositByShopId(base.CurrentSellerManager.ShopId).Id;
						ServiceHelper.Create<ICashDepositsService>().AddCashDepositDetails(cashDepositDetailInfo);
					}
					else
					{
						CashDepositInfo cashDepositInfo = new CashDepositInfo()
						{
							CurrentBalance = balance,
							Date = DateTime.Now,
							ShopId = base.CurrentSellerManager.ShopId,
							TotalBalance = balance,
							EnableLabels = true,
							Himall_CashDepositDetail = cashDepositDetailInfos
						};
						cashDepositsService.AddCashDeposit(cashDepositInfo);
					}
					string str = CacheKeyCollection.PaymentState(string.Join<long>(",", paymentInfo.OrderIds));
					Cache.Insert(str, true);
				}
			}
			catch (Exception exception)
			{
				empty = exception.Message;
			}
			ViewBag.Error = empty;
			ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
			return View();
		}
	}
}