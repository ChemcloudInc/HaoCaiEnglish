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
	public class UserIntegralController : BaseMemberController
	{
		private IGiftsOrderService orderser;

		public UserIntegralController()
		{
            orderser = ServiceHelper.Create<IGiftsOrderService>();
		}

		private string ClearHtmlString(string str)
		{
			string str1 = str;
			if (!string.IsNullOrWhiteSpace(str1))
			{
				str1 = str1.Replace("'", "&#39;");
				str1 = str1.Replace("\"", "&#34;");
				str1 = str1.Replace(">", "&gt;");
				str1 = str1.Replace("<", "&lt;");
			}
			return str1;
		}

		[HttpPost]
		public JsonResult ConfirmOrder(long id)
		{
			Result result = new Result();
            orderser.ConfirmOrder(id, base.CurrentUser.Id);
			result.success = true;
			result.status = 1;
			result.msg = "Order Completed";
			return Json(result);
		}

		private string GetRemarkFromIntegralType(MemberIntegral.IntegralType type, ICollection<MemberIntegralRecordAction> recordAction, string remark = "")
		{
			if (recordAction == null || recordAction.Count == 0)
			{
				return remark;
			}
			if (type != MemberIntegral.IntegralType.Consumption)
			{
				return remark;
			}
			string str = "";
			foreach (MemberIntegralRecordAction memberIntegralRecordAction in recordAction)
			{
				str = string.Concat(str, memberIntegralRecordAction.VirtualItemId, ",");
			}
			char[] chrArray = new char[] { ',' };
			remark = string.Concat("OrderId (", str.TrimEnd(chrArray), ")");
			return remark;
		}

		public ActionResult Index(int? type, int pageSize = 10, int pageNo = 1)
		{
			int num;
			int num1;
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			dynamic viewBag = base.ViewBag;
			num = (integralChangeRule == null ? 0 : integralChangeRule.IntegralPerMoney);
			viewBag.IntegralPerMoney = num;
			MemberIntegral memberIntegral = ServiceHelper.Create<IMemberIntegralService>().GetMemberIntegral(base.CurrentUser.Id);
			dynamic obj = base.ViewBag;
			num1 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
			obj.Integral = num1;
			MemberIntegral.IntegralType? nullable = null;
			if (type.HasValue)
			{
				nullable = new MemberIntegral.IntegralType?((MemberIntegral.IntegralType)type.Value);
			}
			IntegralRecordQuery integralRecordQuery = new IntegralRecordQuery()
			{
				IntegralType = nullable,
				UserId = new long?(base.CurrentUser.Id),
				PageNo = pageNo,
				PageSize = pageSize
			};
			PageModel<MemberIntegralRecord> integralRecordList = ServiceHelper.Create<IMemberIntegralService>().GetIntegralRecordList(integralRecordQuery);
			IEnumerable<MemberIntegralRecord> list = 
				from item in integralRecordList.Models.ToList()
				select new MemberIntegralRecord()
				{
					Id = item.Id,
					UserName = item.UserName,
					RecordDate = item.RecordDate,
					Integral = item.Integral,
					TypeId = item.TypeId,
					ReMark = GetRemarkFromIntegralType(item.TypeId, item.Himall_MemberIntegralRecordAction, item.ReMark)
				};
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = integralRecordList.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(list);
		}

		public ActionResult IntegralRule()
		{
			UserIntegralGroupModel userHistroyIntegralGroup = ServiceHelper.Create<IMemberIntegralService>().GetUserHistroyIntegralGroup(base.CurrentUser.Id);
			return View(userHistroyIntegralGroup);
		}

		public ActionResult OrderList(string skey, GiftOrderInfo.GiftOrderStatus? status, int page = 1)
		{
			int num = 12;
			GiftsOrderQuery giftsOrderQuery = new GiftsOrderQuery()
			{
				skey = skey
			};
			if (status.HasValue && status.Value != 0)
			{
				giftsOrderQuery.status = status;
			}
			giftsOrderQuery.UserId = new long?(base.CurrentUser.Id);
			giftsOrderQuery.PageSize = num;
			giftsOrderQuery.PageNo = page;
			PageModel<GiftOrderInfo> orders = orderser.GetOrders(giftsOrderQuery);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = page,
				ItemsPerPage = num,
				TotalItems = orders.Total
			};
			PagingInfo pagingInfo1 = pagingInfo;
			List<GiftOrderInfo> list = orders.Models.ToList();
			ViewBag.pageInfo = pagingInfo1;
            orderser.OrderAddUserInfo(list);
			List<GiftOrderInfo> giftOrderInfos = list.ToList();
			foreach (GiftOrderInfo giftOrderInfo in giftOrderInfos)
			{
				giftOrderInfo.Address = ClearHtmlString(giftOrderInfo.Address);
				giftOrderInfo.CloseReason = ClearHtmlString(giftOrderInfo.CloseReason);
				giftOrderInfo.UserRemark = ClearHtmlString(giftOrderInfo.UserRemark);
			}
			return View(giftOrderInfos);
		}
	}
}