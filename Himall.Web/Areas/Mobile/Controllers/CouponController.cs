using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class CouponController : BaseMobileMemberController
	{
		public CouponController()
		{
		}

		[HttpPost]
		public JsonResult AcceptCoupon(long vshopid, long couponid)
		{
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			CouponInfo couponInfo = couponService.GetCouponInfo(couponid);
			if (couponInfo.EndTime < DateTime.Now)
			{
				return Json(new { status = 2, success = false, msg = "优惠券已经过期." });
			}
			CouponRecordQuery couponRecordQuery = new CouponRecordQuery()
			{
				CouponId = new long?(couponid),
				UserId = new long?(base.CurrentUser.Id)
			};
			PageModel<CouponRecordInfo> couponRecordList = couponService.GetCouponRecordList(couponRecordQuery);
			if (couponInfo.PerMax != 0 && couponRecordList.Total >= couponInfo.PerMax)
			{
				return Json(new { status = 3, success = false, msg = "达到个人领取最大张数，不能再领取." });
			}
			couponRecordQuery = new CouponRecordQuery()
			{
				CouponId = new long?(couponid)
			};
			couponRecordList = couponService.GetCouponRecordList(couponRecordQuery);
			if (couponRecordList.Total >= couponInfo.Num)
			{
				return Json(new { status = 4, success = false, msg = "此优惠券已经领完了." });
			}
			if (couponInfo.ReceiveType == CouponInfo.CouponReceiveType.IntegralExchange && ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id).AvailableIntegrals < couponInfo.NeedIntegral)
			{
				int needIntegral = couponInfo.NeedIntegral;
				return Json(new { status = 5, success = false, msg = string.Concat("积分不足 ", needIntegral.ToString()) });
			}
			CouponRecordInfo couponRecordInfo = new CouponRecordInfo()
			{
				CouponId = couponid,
				UserId = base.CurrentUser.Id,
				UserName = base.CurrentUser.UserName,
				ShopId = couponInfo.ShopId
			};
			CouponRecordInfo couponRecordInfo1 = couponRecordInfo;
			couponService.AddCouponRecord(couponRecordInfo1);
			return Json(new { status = 0, success = true, msg = "领取成功", crid = couponRecordInfo1.Id });
		}

		public ActionResult Get()
		{
			return View();
		}

		private IEnumerable<CouponInfo> GetCouponList(long shopid)
		{
			IQueryable<CouponInfo> couponList = ServiceHelper.Create<ICouponService>().GetCouponList(shopid);
			IQueryable<long> vShopCouponSetting = 
				from item in ServiceHelper.Create<IVShopService>().GetVShopCouponSetting(shopid)
				select item.CouponID;
			if (couponList.Count() <= 0 || vShopCouponSetting.Count() <= 0)
			{
				return null;
			}
			IEnumerable<CouponInfo> array = 
				from item in couponList.ToArray()
                where vShopCouponSetting.Contains(item.Id)
				select item;
			return array;
		}

		public ActionResult Management()
		{
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			IVShopService vShopService = ServiceHelper.Create<IVShopService>();
			IQueryable<UserCouponInfo> userCouponList = couponService.GetUserCouponList(base.CurrentUser.Id);
			List<ShopBonusReceiveInfo> detailByUserId = ServiceHelper.Create<IShopBonusService>().GetDetailByUserId(base.CurrentUser.Id);
			if (userCouponList == null && detailByUserId == null)
			{
				throw new HimallException("没有领取记录!");
			}
			IEnumerable<UserCouponInfo> array = 
				from a in userCouponList.ToArray()
                select new UserCouponInfo()
				{
					UserId = a.UserId,
					ShopId = a.ShopId,
					CouponId = a.CouponId,
					Price = a.Price,
					PerMax = a.PerMax,
					OrderAmount = a.OrderAmount,
					Num = a.Num,
					StartTime = a.StartTime,
					EndTime = a.EndTime,
					CreateTime = a.CreateTime,
					CouponName = a.CouponName,
					UseStatus = a.UseStatus,
					UseTime = a.UseTime,
					VShop = vShopService.GetVShopByShopId(a.ShopId)
				};
			int num = array.Count((UserCouponInfo item) => {
				if (item.EndTime <= DateTime.Now)
				{
					return false;
				}
				return item.UseStatus == CouponRecordInfo.CounponStatuses.Unuse;
			});
			int num1 = detailByUserId.Count((ShopBonusReceiveInfo p) => {
				if (p.State == ShopBonusReceiveInfo.ReceiveState.NotUse)
				{
					return true;
				}
				return p.Himall_ShopBonusGrant.Himall_ShopBonus.BonusDateEnd < DateTime.Now;
			});
			ViewBag.NoUseCount = num + num1;
			ViewBag.UserCount = userCouponList.Count() - num + (detailByUserId.Count() - num1);
			ViewBag.ShopBonus = detailByUserId;
			return View(array);
		}

		public ActionResult ShopCouponList(long shopid)
		{
			long num;
			IEnumerable<CouponInfo> couponList = GetCouponList(shopid);
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(shopid);
			if (couponList != null)
			{
				dynamic viewBag = base.ViewBag;
				CouponInfo[] array = couponList.ToArray();
				viewBag.CouponList = 
					from a in array
                    select new UserCouponInfo()
					{
						ShopId = a.ShopId,
						CouponId = a.Id,
						Price = a.Price,
						PerMax = a.PerMax,
						OrderAmount = a.OrderAmount,
						Num = a.Num,
						StartTime = a.StartTime,
						EndTime = a.EndTime,
						CreateTime = a.CreateTime,
						CouponName = a.CouponName,
						VShop = vShopByShopId
					};
			}
			ViewBag.Shopid = shopid;
			dynamic obj = base.ViewBag;
			num = (vShopByShopId != null ? vShopByShopId.Id : 0);
			obj.VShopid = num;
			if (!ServiceHelper.Create<IShopService>().IsFavoriteShop(base.CurrentUser.Id, shopid))
			{
				ViewBag.FavText = "收藏店铺";
			}
			else
			{
				ViewBag.FavText = "已收藏";
			}
			WXShopInfo vShopSetting = ServiceHelper.Create<IVShopService>().GetVShopSetting(shopid) ?? new WXShopInfo()
			{
				FollowUrl = string.Empty
			};
			ViewBag.FollowUrl = vShopSetting.FollowUrl;
			return View();
		}
	}
}