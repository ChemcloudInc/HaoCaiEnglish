using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class GiftController : BaseWebController
	{
		private IGiftService giftser;

		private IGiftsOrderService orderser;

		public GiftController()
		{
            giftser = ServiceHelper.Create<IGiftService>();
            orderser = ServiceHelper.Create<IGiftsOrderService>();
		}

		[HttpPost]
		public JsonResult CanBuy(long id, int count)
		{
			Result result = new Result();
			bool flag = true;
			if (base.CurrentUser == null)
			{
				flag = false;
				result.success = false;
				result.msg = "Please Sign in！";
				result.status = -1;
				return Json(result);
			}
			UserMemberInfo member = ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id);
			GiftInfo byId = giftser.GetById(id);
			if (flag && byId == null)
			{
				flag = false;
				result.success = false;
				result.msg = "Gift isn't exist！";
				result.status = -2;
			}
			if (flag && byId.GetSalesStatus != GiftInfo.GiftSalesStatus.Normal)
			{
				flag = false;
				result.success = false;
                result.msg = "Gift Has Expired ！";
				result.status = -2;
			}
			if (flag && count > byId.StockQuantity)
			{
				flag = false;
				result.success = false;
				int stockQuantity = byId.StockQuantity;
                result.msg = string.Concat("Gift inventory shortage, only remain ", stockQuantity.ToString(), " items！");
				result.status = -3;
			}
			if (flag && byId.NeedIntegral < 1)
			{
				flag = false;
				result.success = false;
                result.msg = "Gifts associated level information is wrong or points wrong！";
				result.status = -5;
				return Json(result);
			}
			if (flag && byId.LimtQuantity > 0 && orderser.GetOwnBuyQuantity(base.CurrentUser.Id, id) + count > byId.LimtQuantity)
			{
				flag = false;
				result.success = false;
                result.msg = "Exceed gift exchange quantity！";
				result.status = -4;
			}
			if (flag && byId.NeedIntegral * count > member.AvailableIntegrals)
			{
				flag = false;
				result.success = false;
                result.msg = "Lack of points！";
				result.status = -6;
			}
			if (flag && member.HistoryIntegral < byId.GradeIntegral)
			{
				flag = false;
				result.success = false;
                result.msg = "Lack of level！";
				result.status = -6;
			}
			if (flag)
			{
				result.success = true;
				result.msg = "Can Buy！";
				result.status = 1;
			}
			return Json(result);
		}

		public ActionResult Detail(long id)
		{
			GiftDetailPageModel giftDetailPageModel = new GiftDetailPageModel()
			{
				GiftData = giftser.GetById(id)
			};
			if (giftDetailPageModel.GiftData == null)
			{
				throw new HimallException("Gift information invalid！");
			}
			int num = 10;
			GiftQuery giftQuery = new GiftQuery()
			{
				skey = "",
				status = new GiftInfo.GiftSalesStatus?(GiftInfo.GiftSalesStatus.Normal),
				PageSize = num,
				PageNo = 1,
				Sort = GiftQuery.GiftSortEnum.SalesNumber,
				IsAsc = false
			};
			PageModel<GiftModel> gifts = giftser.GetGifts(giftQuery);
			giftDetailPageModel.HotGifts = gifts.Models.ToList();
			giftDetailPageModel.GiftCanBuy = true;
			if (giftDetailPageModel.GiftCanBuy && giftDetailPageModel.GiftData.GetSalesStatus != GiftInfo.GiftSalesStatus.Normal)
			{
				giftDetailPageModel.GiftCanBuy = false;
			}
			if (giftDetailPageModel.GiftCanBuy && giftDetailPageModel.GiftData.StockQuantity < 1)
			{
				giftDetailPageModel.GiftCanBuy = false;
			}
			if (giftDetailPageModel.GiftCanBuy && giftDetailPageModel.GiftData.NeedIntegral < 1)
			{
				giftDetailPageModel.GiftCanBuy = false;
			}
			if (giftDetailPageModel.GiftCanBuy && base.CurrentUser != null && giftDetailPageModel.GiftData.LimtQuantity > 0 && orderser.GetOwnBuyQuantity(base.CurrentUser.Id, id) >= giftDetailPageModel.GiftData.LimtQuantity)
			{
				giftDetailPageModel.GiftCanBuy = false;
			}
			return View(giftDetailPageModel);
		}
	}
}