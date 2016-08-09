using Himall.Core;
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
	public class OrderRefundController : BaseMemberController
	{
		public OrderRefundController()
		{
		}

		public ActionResult Detail(long id)
		{
			OrderRefundInfo orderRefund = ServiceHelper.Create<IRefundService>().GetOrderRefund(id, base.CurrentUser.Id);
			ViewBag.UserName = base.CurrentUser.UserName;
			return View(orderRefund);
		}

		[HttpGet]
		public JsonResult GetShopInfo(long shopId)
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(shopId, false);
			var variable = new { SenderAddress = shop.SenderAddress, SenderPhone = shop.SenderPhone, SenderName = shop.SenderName };
			return Json(variable, JsonRequestBehavior.AllowGet);
		}

		public static string HTMLEncode(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return string.Empty;
			}
			string str = txt.Replace(" ", "&nbsp;");
			str = str.Replace("<", "&lt;");
			str = str.Replace(">", "&gt;");
			str = str.Replace("\"", "&quot;");
			return str.Replace("'", "&#39;").Replace("\n", "<br>");
		}

		[ValidateInput(false)]
		public ActionResult Index(string orderDate, string keywords, int pageNo = 1, int pageSize = 10)
		{
			DateTime? nullable = null;
			DateTime? nullable1 = null;
			if (!string.IsNullOrEmpty(orderDate) && orderDate.ToLower() != "all")
			{
				string lower = orderDate.ToLower();
				string str = lower;
				if (lower != null)
				{
					if (str == "threemonth")
					{
						nullable = new DateTime?(DateTime.Now.AddMonths(-3));
					}
					else if (str == "halfyear")
					{
						nullable = new DateTime?(DateTime.Now.AddMonths(-6));
					}
					else if (str == "year")
					{
						nullable = new DateTime?(DateTime.Now.AddYears(-1));
					}
					else if (str == "yearago")
					{
						nullable1 = new DateTime?(DateTime.Now.AddYears(-1));
					}
				}
			}
			OrderQuery orderQuery = new OrderQuery()
			{
				StartDate = nullable,
				EndDate = nullable1,
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.Finish),
				UserId = new long?(base.CurrentUser.Id),
				SearchKeyWords = keywords,
				PageSize = pageSize,
				PageNo = pageNo
			};
			OrderQuery orderOperateStatuses = orderQuery;
			orderOperateStatuses.MoreStatus = new List<OrderInfo.OrderOperateStatus>()
			{
				OrderInfo.OrderOperateStatus.WaitReceiving
			};
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderOperateStatuses, null);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orders.Total
			};
			PagingInfo pagingInfo1 = pagingInfo;
			ViewBag.UserId = base.CurrentUser.Id;
			ViewBag.pageInfo = pagingInfo1;
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			ViewBag.SalesRefundTimeout = siteSettings.SalesReturnTimeout;
			return View(orders.Models.ToList());
		}

		public ActionResult List(string applyDate, int? auditStatus, int pageNo = 1, int pageSize = 10, int showtype = 0)
		{
			DateTime? nullable = null;
			DateTime? nullable1 = null;
			if (!string.IsNullOrEmpty(applyDate) && applyDate.ToLower() != "all")
			{
				string lower = applyDate.ToLower();
				string str = lower;
				if (lower != null)
				{
					if (str == "threemonth")
					{
						nullable = new DateTime?(DateTime.Now.AddMonths(-3));
					}
					else if (str == "threemonthago")
					{
						nullable1 = new DateTime?(DateTime.Now.AddMonths(-3));
					}
				}
			}
			if (auditStatus.HasValue)
			{
				int? nullable2 = auditStatus;
				if ((nullable2.GetValueOrDefault() != 0 ? false : nullable2.HasValue))
				{
					auditStatus = null;
				}
			}
			RefundQuery refundQuery = new RefundQuery()
			{
				StartDate = nullable,
				EndDate = nullable1,
				Status = auditStatus,
				UserId = new long?(base.CurrentUser.Id),
				PageSize = pageSize,
				PageNo = pageNo,
				ShowRefundType = new int?(showtype)
			};
			PageModel<OrderRefundInfo> orderRefunds = ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orderRefunds.Total
			};
			ViewBag.pageInfo = pagingInfo;
			ViewBag.UserId = base.CurrentUser.Id;
			ViewBag.ShowType = showtype;
			return View(orderRefunds.Models);
		}

		public ActionResult RefundApply(long id, long? itemId)
		{
			decimal? nullable1;
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			OrderInfo order = orderService.GetOrder(id, base.CurrentUser.Id);
			if (order == null)
			{
				throw new HimallException("该订单已删除或不属于该用户");
			}
			if (order.OrderStatus < OrderInfo.OrderOperateStatus.WaitDelivery)
			{
				throw new HimallException("错误的售后申请,订单状态有误");
			}
			if (!itemId.HasValue && order.OrderStatus != OrderInfo.OrderOperateStatus.WaitDelivery)
			{
				throw new HimallException("错误的订单退款申请,订单状态有误");
			}
			orderService.CalculateOrderItemRefund(id, false);
			OrderItemInfo orderItemInfo = new OrderItemInfo();
			ViewBag.MaxRGDNumber = 0;
			ViewBag.MaxRefundAmount = order.OrderEnabledRefundAmount;
			if (itemId.HasValue)
			{
				orderItemInfo = order.OrderItemInfo.Where((OrderItemInfo a) => {
					long num = a.Id;
					long? nullable = itemId;
					if (num != nullable.GetValueOrDefault())
					{
						return false;
					}
					return nullable.HasValue;
				}).FirstOrDefault();
				ViewBag.MaxRGDNumber = orderItemInfo.Quantity - orderItemInfo.ReturnQuantity;
				dynamic viewBag = base.ViewBag;
				decimal? enabledRefundAmount = orderItemInfo.EnabledRefundAmount;
				decimal refundPrice = orderItemInfo.RefundPrice;
				if (enabledRefundAmount.HasValue)
				{
					nullable1 = new decimal?(enabledRefundAmount.GetValueOrDefault() - refundPrice);
				}
				else
				{
					nullable1 = null;
				}
				//viewBag.MaxRefundAmount = nullable1;
			}
			else
			{
				orderItemInfo = order.OrderItemInfo.FirstOrDefault();
			}
			bool flag = false;
			IRefundService refundService = ServiceHelper.Create<IRefundService>();
			flag = (order.OrderStatus != OrderInfo.OrderOperateStatus.WaitDelivery ? refundService.CanApplyRefund(id, orderItemInfo.Id, new bool?(false)) : refundService.CanApplyRefund(id, orderItemInfo.Id, null));
			if (!flag)
			{
				throw new HimallException("您己申请过售后，不可重复申请");
			}
			ViewBag.UserName = base.CurrentUser.RealName;
			ViewBag.Phone = base.CurrentUser.CellPhone;
			ViewBag.OrderInfo = order;
			ViewBag.OrderItemId = itemId;
			ViewBag.RefundWay = "";
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Text = OrderRefundInfo.OrderRefundPayType.BackCapital.ToDescription(),
				Value = 3.ToString()
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			if (!string.IsNullOrWhiteSpace(order.PaymentTypeGateway) && order.PaymentTypeGateway.ToLower().Contains("weixin"))
			{
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Text = OrderRefundInfo.OrderRefundPayType.BackOut.ToDescription(),
					Value = 1.ToString()
				};
				selectListItems1.Add(selectListItem1);
			}
			ViewBag.RefundWay = selectListItems1;
			return View(orderItemInfo);
		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult RefundApply(OrderRefundInfo info)
		{
			decimal? nullable;
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(info.OrderId, base.CurrentUser.Id);
			if (order == null)
			{
				throw new HimallException("该订单已删除或不属于该用户");
			}
			if (order.OrderStatus < OrderInfo.OrderOperateStatus.WaitDelivery)
			{
				throw new HimallException("错误的售后申请,订单状态有误");
			}
			if (order.OrderStatus == OrderInfo.OrderOperateStatus.WaitDelivery)
			{
				info.RefundMode = OrderRefundInfo.OrderRefundMode.OrderRefund;
				info.ReturnQuantity = 0;
			}
			if (info.RefundType == 1)
			{
				info.ReturnQuantity = 0;
				info.IsReturn = false;
			}
			if (info.ReturnQuantity < 0)
			{
				throw new HimallException("错误的退货数量");
			}
			OrderItemInfo orderItemInfo = order.OrderItemInfo.FirstOrDefault((OrderItemInfo a) => a.Id == info.OrderItemId);
			if (orderItemInfo == null && info.RefundMode != OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				throw new HimallException("该订单条目已删除或不属于该用户");
			}
			if (info.RefundMode != OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				decimal amount = info.Amount;
				decimal? enabledRefundAmount = orderItemInfo.EnabledRefundAmount;
				decimal refundPrice = orderItemInfo.RefundPrice;
				if (enabledRefundAmount.HasValue)
				{
					nullable = new decimal?(enabledRefundAmount.GetValueOrDefault() - refundPrice);
				}
				else
				{
					nullable = null;
				}
				decimal? nullable1 = nullable;
				if ((amount <= nullable1.GetValueOrDefault() ? false : nullable1.HasValue))
				{
					throw new HimallException("退款金额不能超过订单的可退金额");
				}
				if (info.ReturnQuantity > orderItemInfo.Quantity - orderItemInfo.ReturnQuantity)
				{
					throw new HimallException("退货数量不可以超出可退数量");
				}
			}
			else
			{
				if (order.OrderStatus != OrderInfo.OrderOperateStatus.WaitDelivery)
				{
					throw new HimallException("错误的订单退款申请,订单状态有误");
				}
				info.IsReturn = false;
				info.ReturnQuantity = 0;
				if (info.Amount > order.OrderEnabledRefundAmount)
				{
					throw new HimallException("退款金额不能超过订单的实际支付金额");
				}
			}
			info.IsReturn = false;
			if (info.ReturnQuantity > 0)
			{
				info.IsReturn = true;
			}
			if (info.RefundType == 2)
			{
				info.IsReturn = true;
			}
			if (info.IsReturn && info.ReturnQuantity < 1)
			{
				throw new HimallException("错误的退货数量");
			}
			if (info.Amount <= new decimal(0))
			{
				throw new HimallException("错误的退款金额");
			}
			info.ShopId = order.ShopId;
			info.ShopName = order.ShopName;
			info.UserId = base.CurrentUser.Id;
			info.Applicant = base.CurrentUser.UserName;
			info.ApplyDate = DateTime.Now;
			info.Reason = OrderRefundController.HTMLEncode(info.Reason.Replace("'", "‘").Replace("\"", "”"));
			ServiceHelper.Create<IRefundService>().AddOrderRefund(info);
			return Json(new { success = true, msg = "提交成功", id = info.Id });
		}

		[HttpPost]
		public JsonResult UpdateRefund(long id, string expressCompanyName, string shipOrderNumber)
		{
			ServiceHelper.Create<IRefundService>().UserConfirmRefundGood(id, base.CurrentUser.UserName, expressCompanyName, shipOrderNumber);
			return Json(new { success = true, msg = "提交成功" });
		}
	}
}