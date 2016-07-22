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
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class IntegralMallController : BaseWebController
	{
		private ICouponService couponser;

		private IGiftService giftser;

		public IntegralMallController()
		{
            couponser = ServiceHelper.Create<ICouponService>();
            giftser = ServiceHelper.Create<IGiftService>();
		}

		public ActionResult Coupon(int page = 1)
		{
			int num = 12;
			PageModel<CouponInfo> integralCoupons = couponser.GetIntegralCoupons(page, num);
			List<CouponInfo> list = integralCoupons.Models.ToList();
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = page,
				ItemsPerPage = num,
				TotalItems = integralCoupons.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(list);
		}

		[HttpPost]
		public JsonResult CouponList(int page, int rows = 6)
		{
			PageModel<CouponInfo> integralCoupons = couponser.GetIntegralCoupons(page, rows);
			return Json(integralCoupons.Models.ToList());
		}

		private int GetMaxPage(int total, int pagesize)
		{
			int num = 1;
			if (total > 0 && pagesize > 0)
			{
				num = (int)Math.Ceiling(total / (double)pagesize);
			}
			return num;
		}

		[HttpPost]
		public JsonResult GiftList(int page, int rows = 12)
		{
			GiftQuery giftQuery = new GiftQuery()
			{
				skey = "",
				status = new GiftInfo.GiftSalesStatus?(GiftInfo.GiftSalesStatus.Normal),
				PageSize = rows,
				PageNo = page
			};
			PageModel<GiftModel> gifts = giftser.GetGifts(giftQuery);
			var list = (
				from d in gifts.Models.ToList()
				select new { Id = d.Id, GiftName = d.GiftName, NeedIntegral = d.NeedIntegral, LimtQuantity = d.LimtQuantity, StockQuantity = d.StockQuantity, EndDate = d.EndDate, NeedGrade = d.NeedGrade, SumSales = d.SumSales, SalesStatus = d.SalesStatus, ImagePath = d.ImagePath, GiftValue = d.GiftValue, ShowImagePath = d.ShowImagePath, NeedGradeName = d.NeedGradeName }).ToList();
			return Json(list);
		}

		public ActionResult Index()
		{
			IntegralMallPageModel integralMallPageModel = new IntegralMallPageModel()
			{
				CouponPageSize = 6
			};
			PageModel<CouponInfo> integralCoupons = couponser.GetIntegralCoupons(1, integralMallPageModel.CouponPageSize);
			integralMallPageModel.CouponList = integralCoupons.Models.ToList();
			integralMallPageModel.CouponTotal = integralCoupons.Total;
			integralMallPageModel.CouponMaxPage = GetMaxPage(integralMallPageModel.CouponTotal, integralMallPageModel.CouponPageSize);
			integralMallPageModel.GiftPageSize = 12;
			GiftQuery giftQuery = new GiftQuery()
			{
				skey = "",
				status = new GiftInfo.GiftSalesStatus?(GiftInfo.GiftSalesStatus.Normal),
				PageSize = integralMallPageModel.GiftPageSize,
				PageNo = 1
			};
			PageModel<GiftModel> gifts = giftser.GetGifts(giftQuery);
			integralMallPageModel.GiftList = gifts.Models.ToList();
			integralMallPageModel.GiftTotal = gifts.Total;
			integralMallPageModel.GiftMaxPage = GetMaxPage(integralMallPageModel.GiftTotal, integralMallPageModel.GiftPageSize);
			return View(integralMallPageModel);
		}
	}
}