using Himall.Core;
using Himall.IServices;
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
	public class GiftOrderController : BaseMemberController
	{
		private IGiftService giftser;

		private IGiftsOrderService orderser;

		public GiftOrderController()
		{
            giftser = ServiceHelper.Create<IGiftService>();
            orderser = ServiceHelper.Create<IGiftsOrderService>();
		}

		private ShippingAddressInfo GetShippingAddress(long? regionId)
		{
			ShippingAddressInfo shippingAddressInfo = null;
			shippingAddressInfo = (!regionId.HasValue ? ServiceHelper.Create<IShippingAddressService>().GetDefaultUserShippingAddressByUserId(base.CurrentUser.Id) : ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(regionId.Value));
			return shippingAddressInfo;
		}

		public ActionResult OrderSuccess(long id)
		{
			GiftOrderInfo order = orderser.GetOrder(id, base.CurrentUser.Id);
			if (order == null)
			{
				throw new HimallException("错误的订单编号！");
			}
			ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
			ViewBag.Step = 3;
			return View(order);
		}

		public ActionResult SubmitOrder(long id, long? regionId, int count = 1)
		{
			GiftOrderConfirmPageModel giftOrderConfirmPageModel = new GiftOrderConfirmPageModel();
			List<GiftOrderItemInfo> giftOrderItemInfos = new List<GiftOrderItemInfo>();
			GiftInfo byId = giftser.GetById(id);
			if (byId == null)
			{
				throw new HimallException("错误的礼品编号！");
			}
			GiftOrderItemInfo giftOrderItemInfo = new GiftOrderItemInfo()
			{
				GiftId = byId.Id,
				GiftName = byId.GiftName,
				GiftValue = byId.GiftValue,
				ImagePath = byId.ImagePath,
				OrderId = new long?(0),
				Quantity = count,
				SaleIntegral = new int?(byId.NeedIntegral)
			};
			giftOrderItemInfos.Add(giftOrderItemInfo);
			giftOrderConfirmPageModel.GiftList = giftOrderItemInfos;
			GiftOrderConfirmPageModel value = giftOrderConfirmPageModel;
			decimal? nullable = giftOrderConfirmPageModel.GiftList.Sum<GiftOrderItemInfo>((GiftOrderItemInfo d) => {
				decimal quantity = d.Quantity;
				decimal? giftValue = d.GiftValue;
				if (!giftValue.HasValue)
				{
					return null;
				}
				return new decimal?(quantity * giftValue.GetValueOrDefault());
			});
			value.GiftValueTotal = nullable.Value;
			GiftOrderConfirmPageModel value1 = giftOrderConfirmPageModel;
			int? nullable1 = giftOrderConfirmPageModel.GiftList.Sum<GiftOrderItemInfo>((GiftOrderItemInfo d) => {
				int? saleIntegral = d.SaleIntegral;
				int quantity = d.Quantity;
				if (!saleIntegral.HasValue)
				{
					return null;
				}
				return new int?(saleIntegral.GetValueOrDefault() * quantity);
			});
			value1.TotalAmount = nullable1.Value;
			giftOrderConfirmPageModel.ShipAddress = GetShippingAddress(regionId);
			ViewBag.Logo = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().Logo;
			ViewBag.Step = 2;
			return View(giftOrderConfirmPageModel);
		}

		[HttpPost]
		public JsonResult SubmitOrder(long id, long regionId, int count)
		{
			Result result = new Result()
			{
				success = false,
				msg = "未知错误",
				status = 0
			};
			Result str = result;
			bool flag = true;
			if (count < 1)
			{
				flag = false;
				str.success = false;
				str.msg = "错误的兑换数量！";
				str.status = -8;
				return Json(str);
			}
			List<GiftOrderItemModel> giftOrderItemModels = new List<GiftOrderItemModel>();
			UserMemberInfo member = ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id);
			GiftInfo byId = giftser.GetById(id);
			if (byId == null)
			{
				flag = false;
				str.success = false;
				str.msg = "礼品不存在！";
				str.status = -2;
				return Json(str);
			}
			if (byId.GetSalesStatus != GiftInfo.GiftSalesStatus.Normal)
			{
				flag = false;
				str.success = false;
				str.msg = "礼品己失效！";
				str.status = -2;
				return Json(str);
			}
			if (count > byId.StockQuantity)
			{
				flag = false;
				str.success = false;
				int stockQuantity = byId.StockQuantity;
				str.msg = string.Concat("礼品库存不足,仅剩 ", stockQuantity.ToString(), " 件！");
				str.status = -3;
				return Json(str);
			}
			if (byId.NeedIntegral < 1)
			{
				flag = false;
				str.success = false;
				str.msg = "礼品关联等级信息有误或礼品积分数据有误！";
				str.status = -5;
				return Json(str);
			}
			if (byId.LimtQuantity > 0 && orderser.GetOwnBuyQuantity(base.CurrentUser.Id, id) + count > byId.LimtQuantity)
			{
				flag = false;
				str.success = false;
				str.msg = "超过礼品限兑数量！";
				str.status = -4;
				return Json(str);
			}
			if (byId.NeedIntegral * count > member.AvailableIntegrals)
			{
				flag = false;
				str.success = false;
				str.msg = "积分不足！";
				str.status = -6;
				return Json(str);
			}
			if (member.HistoryIntegral < byId.GradeIntegral)
			{
				flag = false;
				str.success = false;
				str.msg = "用户等级不足！";
				str.status = -6;
				return Json(str);
			}
			ShippingAddressInfo shippingAddress = GetShippingAddress(new long?(regionId));
			if (shippingAddress == null)
			{
				flag = false;
				str.success = false;
				str.msg = "错误的收货人地址信息！";
				str.status = -6;
				return Json(str);
			}
			if (flag)
			{
				GiftOrderItemModel giftOrderItemModel = new GiftOrderItemModel()
				{
					GiftId = byId.Id,
					Counts = count
				};
				giftOrderItemModels.Add(giftOrderItemModel);
				GiftOrderModel giftOrderModel = new GiftOrderModel()
				{
					Gifts = giftOrderItemModels,
					CurrentUser = member,
					ReceiveAddress = shippingAddress
				};
				GiftOrderInfo giftOrderInfo = orderser.CreateOrder(giftOrderModel);
				str.success = true;
				str.msg = giftOrderInfo.Id.ToString();
				str.status = 1;
			}
			return Json(str);
		}
	}
}