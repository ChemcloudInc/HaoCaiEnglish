using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class CouponController : BaseSellerController
	{
		private ICouponService couponser;

		public CouponController()
		{
            couponser = ServiceHelper.Create<ICouponService>();
		}

		public ActionResult Add()
		{
			CouponInfo couponInfo = new CouponInfo();
			long shopId = base.CurrentSellerManager.ShopId;
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			couponInfo = new CouponInfo()
			{
				StartTime = DateTime.Now,
				EndTime = couponInfo.StartTime.AddMonths(1),
				ReceiveType = CouponInfo.CouponReceiveType.ShopIndex,
				CanVshopIndex = base.CurrentSellerManager.VShopId > 0
            };
			couponInfo.Himall_CouponSetting.Clear();
			if (couponInfo.CanVshopIndex)
			{
				ICollection<CouponSettingInfo> himallCouponSetting = couponInfo.Himall_CouponSetting;
				CouponSettingInfo couponSettingInfo = new CouponSettingInfo()
				{
					Display = new int?(1),
					PlatForm = PlatformType.Wap
				};
				himallCouponSetting.Add(couponSettingInfo);
			}
			ICollection<CouponSettingInfo> couponSettingInfos = couponInfo.Himall_CouponSetting;
			CouponSettingInfo couponSettingInfo1 = new CouponSettingInfo()
			{
				Display = new int?(1),
				PlatForm = PlatformType.PC
			};
			couponSettingInfos.Add(couponSettingInfo1);
			couponInfo.FormIsSyncWeiXin = false;
			couponInfo.ShopId = shopId;
			dynamic viewBag = base.ViewBag;
			DateTime dateTime = ServiceHelper.Create<IMarketService>().GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Coupon).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
			viewBag.EndTime = dateTime.ToString("yyyy-MM-dd");
			ViewBag.CanAddIntegralCoupon = couponService.CanAddIntegralCoupon(shopId, 0);
			return View(couponInfo);
		}

        public ActionResult BuyService()
        {
            ActiveMarketServiceInfo couponService = ServiceHelper.Create<ICouponService>().GetCouponService(base.CurrentSellerManager.ShopId);
            ((dynamic)base.ViewBag).Market = couponService;
            string str = null;
            DateTime date = DateTime.Now.Date;
            if ((couponService != null) && (couponService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime) < date))
            {
                str = "Your coupon service has expired, you can renew.";
            }
            else if ((couponService != null) && (couponService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime) >= date))
            {
                DateTime time2 = couponService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime);
                str = string.Format("{0} 年 {1} 月 {2} 日", time2.Year, time2.Month, time2.Day);
            }
            ViewBag.EndDate = str;
            ViewBag.Price = ServiceHelper.Create<IMarketService>().GetServiceSetting(MarketType.Coupon).Price;
            return View();
        }


        [HttpPost]
		[UnAuthorize]
		public JsonResult BuyService(int month)
		{
			Result result = new Result();
			IMarketService marketService = ServiceHelper.Create<IMarketService>();
			marketService.OrderMarketService(month, base.CurrentSellerManager.ShopId, MarketType.Coupon);
			result.success = true;
            result.msg = "Purchase successfully";
			return Json(result);
		}

		public JsonResult Cancel(long couponId)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<ICouponService>().CancelCoupon(couponId, shopId);
			Result result = new Result()
			{
				success = true,
				msg = "操作成功！"
			};
			return Json(result);
		}

		public ActionResult Detail(long Id)
		{
			string str;
			CouponInfo couponInfo = ServiceHelper.Create<ICouponService>().GetCouponInfo(base.CurrentSellerManager.ShopId, Id);
			if (couponInfo != null && couponInfo.IsSyncWeiXin == 1 && couponInfo.WXAuditStatus != 1)
			{
				throw new HimallException("同步微信优惠券未审核通过时不可修改。");
			}
			string host = base.Request.Url.Host;
			string str1 = host;
			if (base.Request.Url.Port != 80)
			{
				int port = base.Request.Url.Port;
				str = string.Concat(":", port.ToString());
			}
			else
			{
				str = "";
			}
			host = string.Concat(str1, str);
			ViewBag.Url = string.Format("http://{0}/m-wap/vshop/CouponInfo/{1}", host, couponInfo.Id);
			dynamic viewBag = QRCodeHelper.Create(ViewBag.Url);
			MemoryStream memoryStream = new MemoryStream();
			viewBag.Save(memoryStream, ImageFormat.Gif);
			string str2 = string.Concat("data:image/gif;base64,", Convert.ToBase64String(memoryStream.ToArray()));
			memoryStream.Dispose();
			ViewBag.Image = str2;
			dynamic obj = base.ViewBag;
			DateTime dateTime = ServiceHelper.Create<IMarketService>().GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Coupon).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
			obj.EndTime = dateTime.ToString("yyyy-MM-dd");
			return View(couponInfo);
		}

		public ActionResult Edit(long Id)
		{
			CouponInfo couponInfo = new CouponInfo();
			long shopId = base.CurrentSellerManager.ShopId;
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			couponInfo = couponService.GetCouponInfo(shopId, Id);
			if (couponInfo == null)
			{
				throw new HimallException("错误的优惠券编号。");
			}
			if (couponInfo.IsSyncWeiXin == 1 && couponInfo.WXAuditStatus != 1)
			{
				throw new HimallException("同步微信优惠券未审核通过时不可修改。");
			}
			couponInfo.FormIsSyncWeiXin = couponInfo.IsSyncWeiXin == 1;
			couponInfo.CanVshopIndex = base.CurrentSellerManager.VShopId > 0;
			dynamic viewBag = base.ViewBag;
			DateTime dateTime = ServiceHelper.Create<IMarketService>().GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Coupon).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
			viewBag.EndTime = dateTime.ToString("yyyy-MM-dd");
			ViewBag.CanAddIntegralCoupon = couponService.CanAddIntegralCoupon(shopId, couponInfo.Id);
			return View(couponInfo);
		}

		[HttpPost]
		public JsonResult Edit(CouponInfo info)
		{
			bool flag = false;
			if (info.Id == 0)
			{
				flag = true;
			}
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			long shopId = base.CurrentSellerManager.ShopId;
			info.ShopId = shopId;
			string shopName = ServiceHelper.Create<IShopService>().GetShop(shopId, false).ShopName;
			info.ShopName = shopName;
			if (flag)
			{
				info.CreateTime = DateTime.Now;
				if (info.StartTime >= info.EndTime)
				{
					Result result = new Result()
					{
						success = false,
						msg = "开始时间必须小于结束时间"
					};
					return Json(result);
				}
				info.IsSyncWeiXin = 0;
				if (info.FormIsSyncWeiXin)
				{
					info.IsSyncWeiXin = 1;
					if (string.IsNullOrWhiteSpace(info.FormWXColor))
					{
						Result result1 = new Result()
						{
							success = false,
							msg = "错误的卡券颜色"
						};
						return Json(result1);
					}
					if (string.IsNullOrWhiteSpace(info.FormWXCTit))
					{
						Result result2 = new Result()
						{
							success = false,
							msg = "请填写卡券标题"
						};
						return Json(result2);
					}
					if (!WXCardLogInfo.WXCardColors.Contains(info.FormWXColor))
					{
						Result result3 = new Result()
						{
							success = false,
							msg = "错误的卡券颜色"
						};
						return Json(result3);
					}
					Encoding @default = Encoding.Default;
					if (@default.GetBytes(info.FormWXCTit).Count() > 18)
					{
						Result result4 = new Result()
						{
							success = false,
							msg = "卡券标题不得超过9个汉字"
						};
						return Json(result4);
					}
					if (!string.IsNullOrWhiteSpace(info.FormWXCSubTit) && @default.GetBytes(info.FormWXCSubTit).Count() > 36)
					{
						Result result5 = new Result()
						{
							success = false,
							msg = "卡券副标题不得超过18个汉字"
						};
						return Json(result5);
					}
				}
			}
			string item = base.Request.Form["chkShow"];
			info.CanVshopIndex = base.CurrentSellerManager.VShopId > 0;
			switch (info.ReceiveType)
			{
				case CouponInfo.CouponReceiveType.IntegralExchange:
				{
					if (!couponService.CanAddIntegralCoupon(shopId, info.Id))
					{
						Result result6 = new Result()
						{
							success = false,
							msg = "当前己有积分优惠券，每商家只可以推广一张积分优惠券！"
						};
						return Json(result6);
					}
					info.Himall_CouponSetting.Clear();
					if (!info.EndIntegralExchange.HasValue)
					{
						Result result7 = new Result()
						{
							success = false,
							msg = "错误的兑换截止时间"
						};
						return Json(result7);
					}
					DateTime? endIntegralExchange = info.EndIntegralExchange;
					DateTime date = info.EndTime.AddDays(1).Date;
					if ((endIntegralExchange.HasValue ? endIntegralExchange.GetValueOrDefault() > date : false))
					{
						Result result8 = new Result()
						{
							success = false,
							msg = "错误的兑换截止时间"
						};
						return Json(result8);
					}
					if (info.NeedIntegral >= 10)
					{
						break;
					}
					Result result9 = new Result()
					{
						success = false,
						msg = "积分最少10分起兑"
					};
					return Json(result9);
				}
				case CouponInfo.CouponReceiveType.DirectHair:
				{
					info.Himall_CouponSetting.Clear();
					break;
				}
				default:
				{
					if (string.IsNullOrEmpty(item))
					{
						Result result10 = new Result()
						{
							success = false,
							msg = "必须选择一个推广类型"
						};
						return Json(result10);
					}
					info.Himall_CouponSetting.Clear();
					string[] strArrays = item.Split(new char[] { ',' });
					if (strArrays.Contains<string>("WAP") && info.CanVshopIndex)
					{
						ICollection<CouponSettingInfo> himallCouponSetting = info.Himall_CouponSetting;
						CouponSettingInfo couponSettingInfo = new CouponSettingInfo()
						{
							Display = new int?(1),
							PlatForm = PlatformType.Wap
						};
						himallCouponSetting.Add(couponSettingInfo);
					}
					if (!strArrays.Contains<string>("PC"))
					{
						break;
					}
					ICollection<CouponSettingInfo> couponSettingInfos = info.Himall_CouponSetting;
					CouponSettingInfo couponSettingInfo1 = new CouponSettingInfo()
					{
						Display = new int?(1),
						PlatForm = PlatformType.PC
					};
					couponSettingInfos.Add(couponSettingInfo1);
					break;
				}
			}
			Server.MapPath(string.Format("/Storage/Shop/{0}/Coupon/{1}", shopId, info.Id));
			if (!flag)
			{
				couponService.EditCoupon(info);
			}
			else
			{
				couponService.AddCoupon(info);
			}
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetItemList(int page, int rows, string couponName)
		{
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			CouponQuery couponQuery = new CouponQuery()
			{
				CouponName = couponName,
				ShopId = new long?(base.CurrentSellerManager.ShopId),
				IsShowAll = new bool?(true),
				PageSize = rows,
				PageNo = page
			};
			PageModel<CouponInfo> couponList = couponService.GetCouponList(couponQuery);
			var list = 
				from item in couponList.Models.ToList()
				select new { Id = item.Id, StartTime = item.StartTime.ToString("yyyy/MM/dd"), EndTime = item.EndTime.ToString("yyyy/MM/dd"), Price = Math.Round(item.Price, 2), CouponName = item.CouponName, PerMax = (item.PerMax == 0 ? "不限张" : string.Concat(item.PerMax.ToString(), "张/人")), OrderAmount = (item.OrderAmount == new decimal(0) ? "不限制" : string.Concat("满", item.OrderAmount)), Num = item.Num, ReceviceNum = item.Himall_CouponRecord.Count(), RecevicePeople = (
					from a in item.Himall_CouponRecord
					group a by a.UserId).Count<IGrouping<long, CouponRecordInfo>>(), Used = item.Himall_CouponRecord.Count((CouponRecordInfo a) => a.CounponStatus == CouponRecordInfo.CounponStatuses.Used), IsSyncWeiXin = item.IsSyncWeiXin, WXAuditStatus = (item.IsSyncWeiXin != 1 ? 1 : item.WXAuditStatus) };
			return Json(new { rows = list, total = couponList.Total });
		}

		[HttpPost]
		public ActionResult GetReceivers(long Id, int page, int rows)
		{
			CouponRecordQuery couponRecordQuery = new CouponRecordQuery()
			{
				CouponId = new long?(Id),
				ShopId = new long?(base.CurrentSellerManager.ShopId),
				PageNo = page,
				PageSize = rows
			};
			PageModel<CouponRecordInfo> couponRecordList = ServiceHelper.Create<ICouponService>().GetCouponRecordList(couponRecordQuery);
			var array = 
				from item in couponRecordList.Models.ToArray()
                select new { Id = item.Id, Price = Math.Round(item.Himall_Coupon.Price, 2), CreateTime = item.Himall_Coupon.CreateTime.ToString("yyyy-MM-dd"), CouponSN = item.CounponSN, UsedTime = (item.UsedTime.HasValue ? item.UsedTime.Value.ToString("yyyy-MM-dd") : ""), ReceviceTime = item.CounponTime.ToString("yyyy-MM-dd"), Recever = item.UserName, OrderId = item.OrderId, Status = (item.CounponStatus == CouponRecordInfo.CounponStatuses.Unuse ? (item.Himall_Coupon.EndTime < DateTime.Now.Date ? "已过期" : item.CounponStatus.ToDescription()) : item.CounponStatus.ToDescription()) };
			return Json(new { rows = array, total = couponRecordList.Total });
		}

		public ActionResult Management()
		{
            couponser.ClearErrorWeiXinCardSync();
			if (ServiceHelper.Create<IMarketService>().GetServiceSetting(MarketType.Coupon) == null)
			{
				return View("Nosetting");
			}
			ViewBag.Market = ServiceHelper.Create<ICouponService>().GetCouponService(base.CurrentSellerManager.ShopId);
			return View();
		}

		public ActionResult Receivers(long Id)
		{
			ViewBag.Id = Id;
			return View();
		}
	}
}