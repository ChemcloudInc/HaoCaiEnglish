using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserCouponController : BaseMemberController
	{
		public UserCouponController()
		{
		}

		public ActionResult Index(int? status, int pageSize = 10, int pageNo = 1)
		{
			if (!status.HasValue)
			{
				status = new int?(0);
			}
			CouponRecordQuery couponRecordQuery = new CouponRecordQuery()
			{
				UserId = new long?(base.CurrentUser.Id),
				PageNo = pageNo,
				PageSize = pageSize,
				Status = status
			};
			PageModel<CouponRecordInfo> couponRecordList = ServiceHelper.Create<ICouponService>().GetCouponRecordList(couponRecordQuery);
			PageModel<ShopBonusReceiveInfo> detailByQuery = ServiceHelper.Create<IShopBonusService>().GetDetailByQuery(couponRecordQuery);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = couponRecordList.Total + detailByQuery.Total
			};
			ViewBag.pageInfo = pagingInfo;
			ViewBag.Bonus = detailByQuery.Models.ToList();
			ViewBag.State = couponRecordQuery.Status;
			return View(couponRecordList.Models.ToList());
		}
	}
}