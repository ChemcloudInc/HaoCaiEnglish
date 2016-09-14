using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class PayController : BaseWebController
	{
		public PayController()
		{
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
						Description = "Recharge",
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
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string message = exception.Message;
				Log.Error("CashPayNotify_Post", exception);
			}
			return base.Content(empty1);
		}

		private string DecodePaymentId(string paymentId)
		{
			return paymentId.Replace("-", ".");
		}

		[ActionName("CapitalChargeNotify")]
		[ValidateInput(false)]
		public ContentResult PayNotify_Charge(string id)
		{
			string empty = string.Empty;
			try
			{
				id = DecodePaymentId(id);
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				if (plugin != null)
				{
					PaymentInfo paymentInfo = plugin.Biz.ProcessNotify(base.Request);
					IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
					ChargeDetailInfo chargeDetail = memberCapitalService.GetChargeDetail(paymentInfo.OrderIds.FirstOrDefault());
					if (chargeDetail != null && chargeDetail.ChargeStatus != ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess)
					{
						chargeDetail.ChargeWay = plugin.PluginInfo.DisplayName;
						chargeDetail.ChargeStatus = ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess;
						chargeDetail.ChargeTime = new DateTime?((paymentInfo.TradeTime.HasValue ? paymentInfo.TradeTime.Value : DateTime.Now));
						memberCapitalService.UpdateChargeDetail(chargeDetail);
						empty = plugin.Biz.ConfirmPayResult();
						string str = CacheKeyCollection.PaymentState(chargeDetail.Id.ToString());
						Cache.Insert(str, true, 15);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(string.Concat("Recharge failed：", exception.Message));
			}
			return base.Content(empty);
		}

		[ActionName("Notify")]
		[ValidateInput(false)]
		public ContentResult PayNotify_Post(string id)
		{
			id = DecodePaymentId(id);
			string empty = string.Empty;
			string str = string.Empty;
			try
			{
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				PaymentInfo paymentInfo = plugin.Biz.ProcessNotify(base.HttpContext.Request);
				DateTime? tradeTime = paymentInfo.TradeTime;
				long num = paymentInfo.OrderIds.FirstOrDefault();
				List<long> list = (
					from item in ServiceHelper.Create<IOrderService>().GetOrderPay(num)
					select item.OrderId).ToList();
				try
				{
					IOrderService orderService = ServiceHelper.Create<IOrderService>();
					DateTime? nullable = paymentInfo.TradeTime;
					orderService.PaySucceed(list, id, nullable.Value, paymentInfo.TradNo, num);
					str = plugin.Biz.ConfirmPayResult();
					string str1 = CacheKeyCollection.PaymentState(string.Join<long>(",", list));
					Cache.Insert(str1, true, 15);
					Dictionary<long, ShopBonusInfo> nums = new Dictionary<long, ShopBonusInfo>();
					string str2 = string.Concat("http://", base.Request.Url.Host.ToString(), "/m-weixin/shopbonus/index/");
					IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
					foreach (OrderInfo order in ServiceHelper.Create<IOrderService>().GetOrders(list.AsEnumerable<long>()))
					{
						Log.Info(string.Concat("ShopID = ", order.ShopId));
						ShopBonusInfo byShopId = shopBonusService.GetByShopId(order.ShopId);
						Log.Info(string.Concat("商家活动价格：", byShopId.GrantPrice));
						Log.Info(string.Concat("买家支付价格：", order.OrderTotalAmount));
						if (byShopId.GrantPrice > order.OrderTotalAmount)
						{
							continue;
						}
						object[] objArray = new object[] { byShopId.Id, order.UserId, order.Id, str2 };
						Log.Info(string.Format("{0} , {1} , {2} , {3} ", objArray));
						long num1 = shopBonusService.GenerateBonusDetail(byShopId, order.UserId, order.Id, str2);
						Log.Info(string.Concat("生成红包组，红包Grantid = ", num1));
						nums.Add(num1, byShopId);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string str3 = string.Concat(id, " ", num.ToString());
					if (paymentInfo.TradeTime.HasValue)
					{
						DateTime value = paymentInfo.TradeTime.Value;
						str3 = string.Concat(str3, " TradeTime:", value.ToString());
					}
					str3 = string.Concat(str3, " TradNo:", paymentInfo.TradNo);
					Log.Error(str3, exception);
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				string message = exception2.Message;
				Log.Error("PayNotify_Post", exception2);
			}
			return base.Content(str);
		}

		[ActionName("CapitalChargeReturn")]
		[ValidateInput(false)]
		public ActionResult PayReturn_Charge(string id)
		{
			string empty = string.Empty;
			try
			{
				id = DecodePaymentId(id);
				Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
				if (plugin != null)
				{
					PaymentInfo paymentInfo = plugin.Biz.ProcessReturn(base.Request);
					IMemberCapitalService memberCapitalService = ServiceHelper.Create<IMemberCapitalService>();
					ChargeDetailInfo chargeDetail = memberCapitalService.GetChargeDetail(paymentInfo.OrderIds.FirstOrDefault());
					if (chargeDetail != null && chargeDetail.ChargeStatus != ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess)
					{
						chargeDetail.ChargeWay = plugin.PluginInfo.DisplayName;
						chargeDetail.ChargeStatus = ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess;
						chargeDetail.ChargeTime = new DateTime?((paymentInfo.TradeTime.HasValue ? paymentInfo.TradeTime.Value : DateTime.Now));
						memberCapitalService.UpdateChargeDetail(chargeDetail);
						plugin.Biz.ConfirmPayResult();
						string str = CacheKeyCollection.PaymentState(chargeDetail.Id.ToString());
						Cache.Insert(str, true, 15);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(string.Concat("Recharge Failed：", exception.Message));
			}
			return View();
		}

		public ActionResult QRPay(string url, string id)
		{
			ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
			Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
			ViewBag.Title = string.Concat(plugin.PluginInfo.DisplayName, "Payment");
			ViewBag.Name = plugin.PluginInfo.DisplayName;
			Bitmap bitmap = QRCodeHelper.Create(url);
			DateTime now = DateTime.Now;
			string str = string.Concat(now.ToString("yyMMddHHmmssffffff"), ".jpg");
			string str1 = string.Concat("/temp/", str);
			bitmap.Save(string.Concat(Server.MapPath("~/temp/"), str));
			ViewBag.QRCode = str1;
			dynamic viewBag = base.ViewBag;
			string classFullName = plugin.PluginInfo.ClassFullName;
			char[] chrArray = new char[] { ',' };
			viewBag.HelpImage = string.Concat("/Plugins/Payment/", classFullName.Split(chrArray)[1], "/", plugin.Biz.HelpImage);
			ViewBag.Step = 2;
			return View();
		}

        public ActionResult CashDepositWeiXinPay(string url, string id)
        {
            ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
            Plugin<IPaymentPlugin> plugin = PluginsManagement.GetPlugin<IPaymentPlugin>(id);
            ViewBag.Title = string.Concat(plugin.PluginInfo.DisplayName, "Payment");
            ViewBag.Name = plugin.PluginInfo.DisplayName;
            Bitmap bitmap = QRCodeHelper.Create(url);
            DateTime now = DateTime.Now;
            string str = string.Concat(now.ToString("yyMMddHHmmssffffff"), ".jpg");
            string str1 = string.Concat("/temp/", str);
            bitmap.Save(string.Concat(Server.MapPath("~/temp/"), str));
            ViewBag.QRCode = str1;
            dynamic viewBag = base.ViewBag;
            string classFullName = plugin.PluginInfo.ClassFullName;
            char[] chrArray = new char[] { ',' };
            viewBag.HelpImage = string.Concat("/Plugins/Payment/", classFullName.Split(chrArray)[1], "/", plugin.Biz.HelpImage);
            ViewBag.Step = 2;
            return View();
        }

        public ActionResult Return(string orderIds)
		{
           
	
			string empty = string.Empty;
			try
			{

                char[] chrArray = new char[] { ',' };
                IEnumerable<long> num =
                    from item in orderIds.Split(chrArray)
                    select long.Parse(item);
				List<long> list = (
                    from item in ServiceHelper.Create<IOrderService>().GetOrders(num)
					select item.Id).ToList();
				ViewBag.OrderIds = string.Join<long>(",", list);
				IOrderService orderService = ServiceHelper.Create<IOrderService>();
				DateTime? nullable = DateTime.Now;

                orderService.PayPalSucceed(list, nullable.Value, null, list.FirstOrDefault());
                string str = CacheKeyCollection.PaymentState(string.Join<long>(",", list));
				Cache.Insert(str, true, 15);
				Dictionary<long, ShopBonusInfo> nums = new Dictionary<long, ShopBonusInfo>();
				string str1 = string.Concat("http://", base.Request.Url.Host.ToString(), "/m-weixin/shopbonus/index/");
				IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
				foreach (OrderInfo order in ServiceHelper.Create<IOrderService>().GetOrders(list.AsEnumerable<long>()))
				{
					Log.Info(string.Concat("ShopID = ", order.ShopId));
					ShopBonusInfo byShopId = shopBonusService.GetByShopId(order.ShopId);
                    if (byShopId != null)
                    {
                        Log.Info(string.Concat("商家活动价格：", byShopId.GrantPrice));
                        Log.Info(string.Concat("买家支付价格：", order.OrderTotalAmount));
                        if (byShopId.GrantPrice > order.OrderTotalAmount)
                        {
                            continue;
                        }
                        object[] objArray = new object[] { byShopId.Id, order.UserId, order.Id, str1 };
                        Log.Info(string.Format("{0} , {1} , {2} , {3} ", objArray));
                        long num1 = shopBonusService.GenerateBonusDetail(byShopId, order.UserId, order.Id, str1);
                        Log.Info(string.Concat("生成红包组，红包Grantid = ", num1));
                        nums.Add(num1, byShopId);
                    }
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

		public ActionResult ReturnSuccess(string id)
		{
			ViewBag.OrderIds = base.Request.QueryString[id];
			ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
			return View("Return");
		}
	}
}