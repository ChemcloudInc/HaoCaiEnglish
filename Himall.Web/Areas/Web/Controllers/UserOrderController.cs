using Himall.Core;
using Himall.Core.Plugins;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserOrderController : BaseMemberController
	{
		public UserOrderController()
		{
		}

		[HttpPost]
		public JsonResult CloseOrder(long orderId)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(orderId, base.CurrentUser.Id);
			if (order == null)
			{
				Result result = new Result()
				{
					success = false,
					msg = "取消失败，该订单已删除或者不属于当前用户！"
				};
				return Json(result);
			}
			ServiceHelper.Create<IOrderService>().MemberCloseOrder(orderId, base.CurrentUser.UserName, false);
			foreach (OrderItemInfo orderItemInfo in order.OrderItemInfo)
			{
				ServiceHelper.Create<IProductService>().UpdateStock(orderItemInfo.SkuId, orderItemInfo.Quantity);
			}
			Result result1 = new Result()
			{
				success = true,
				msg = "取消成功"
			};
			return Json(result1);
		}

		[HttpPost]
		public JsonResult ConfirmOrder(long orderId)
		{
			ServiceHelper.Create<IOrderService>().MembeConfirmOrder(orderId, base.CurrentUser.UserName);
			Result result = new Result()
			{
				success = true,
				msg = "操作成功！"
			};
			return Json(result);
		}

		[ChildActionOnly]
		public ActionResult CustmerServices(long shopId)
		{
			List<CustomerServiceInfo> list = (
				from m in ServiceHelper.Create<ICustomerService>().GetCustomerService(shopId)
				orderby m.Tool
				select m).ToList();
			List<CustomerServiceInfo> customerServiceInfos = new List<CustomerServiceInfo>();
			CustomerServiceInfo customerServiceInfo = list.Where((CustomerServiceInfo a) => {
				if (a.Tool != CustomerServiceInfo.ServiceTool.QQ)
				{
					return false;
				}
				return a.Type == CustomerServiceInfo.ServiceType.AfterSale;
			}).OrderBy<CustomerServiceInfo, string>((CustomerServiceInfo t) => Guid.NewGuid().ToString()).FirstOrDefault();
			CustomerServiceInfo customerServiceInfo1 = list.Where((CustomerServiceInfo a) => {
				if (a.Tool != CustomerServiceInfo.ServiceTool.Wangwang)
				{
					return false;
				}
				return a.Type == CustomerServiceInfo.ServiceType.AfterSale;
			}).OrderBy<CustomerServiceInfo, string>((CustomerServiceInfo t) => Guid.NewGuid().ToString()).FirstOrDefault();
			if (customerServiceInfo != null)
			{
				customerServiceInfos.Add(customerServiceInfo);
			}
			if (customerServiceInfo1 != null)
			{
				customerServiceInfos.Add(customerServiceInfo1);
			}
			return base.PartialView(customerServiceInfos);
		}

		public ActionResult Detail(long id)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(id, base.CurrentUser.Id);
			IEnumerable<long> nums = (
				from d in order.OrderItemInfo
				select d.ProductId).AsEnumerable<long>();
			var list = (
				from d in ServiceHelper.Create<IProductService>().GetProductByIds(nums)
				select new { Id = d.Id, ProductCode = d.ProductCode }).ToList();
			foreach (OrderItemInfo orderItemInfo in order.OrderItemInfo)
			{
				var variable = list.Find((d) => d.Id == orderItemInfo.ProductId);
				if (variable == null)
				{
					continue;
				}
				orderItemInfo.ProductCode = variable.ProductCode;
			}
			ViewBag.Coupon = 0;
			CouponRecordInfo couponRecordInfo = ServiceHelper.Create<ICouponService>().GetCouponRecordInfo(order.UserId, order.Id);
			decimal usedPrice = ServiceHelper.Create<IShopBonusService>().GetUsedPrice(order.Id, order.UserId);
			if (couponRecordInfo == null)
			{
				ViewBag.Coupon = usedPrice;
			}
			else
			{
				ViewBag.Coupon = couponRecordInfo.Himall_Coupon.Price;
			}
			return View(order);
		}

		[HttpPost]
		public JsonResult GetExpressData(string expressCompanyName, string shipOrderNumber)
		{
			if (string.IsNullOrWhiteSpace(expressCompanyName) || string.IsNullOrWhiteSpace(shipOrderNumber))
			{
				throw new HimallException("错误的订单信息");
			}
			string kuaidi100Code = ServiceHelper.Create<IExpressService>().GetExpress(expressCompanyName).Kuaidi100Code;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.kuaidi100.com/query?type={0}&postid={1}", kuaidi100Code, shipOrderNumber));
			httpWebRequest.Timeout = 8000;
			string end = "暂时没有此快递单号的信息";
			try
			{
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					Stream responseStream = response.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
					end = streamReader.ReadToEnd();
					end = end.Replace("&amp;", "");
					end = end.Replace("&nbsp;", "");
					end = end.Replace("&", "");
				}
			}
			catch
			{
			}
			return Json(end);
		}

		[ValidateInput(false)]
		public ActionResult Index(string orderDate, string keywords, string orderids, DateTime? startDateTime, DateTime? endDateTime, int? orderStatus, int pageNo = 1, int pageSize = 10)
		{
			OrderInfo.OrderOperateStatus? nullable;
			ViewBag.Grant = (object)null;
			if (!string.IsNullOrEmpty(orderids) && orderids.IndexOf(',') <= 0)
			{
				ViewBag.Grant = ServiceHelper.Create<IShopBonusService>().GetByOrderId(long.Parse(orderids));
			}
			DateTime? nullable1 = startDateTime;
			DateTime? nullable2 = endDateTime;
			if (!string.IsNullOrEmpty(orderDate) && orderDate.ToLower() != "all")
			{
				string lower = orderDate.ToLower();
				string str = lower;
				if (lower != null)
				{
					if (str == "threemonth")
					{
						nullable1 = new DateTime?(DateTime.Now.AddMonths(-3));
					}
					else if (str == "halfyear")
					{
						nullable1 = new DateTime?(DateTime.Now.AddMonths(-6));
					}
					else if (str == "year")
					{
						nullable1 = new DateTime?(DateTime.Now.AddYears(-1));
					}
					else if (str == "yearago")
					{
						nullable2 = new DateTime?(DateTime.Now.AddYears(-1));
					}
				}
			}
			if (orderStatus.HasValue)
			{
				int? nullable3 = orderStatus;
				if ((nullable3.GetValueOrDefault() != 0 ? false : nullable3.HasValue))
				{
					orderStatus = null;
				}
			}
			OrderQuery orderQuery = new OrderQuery()
			{
				StartDate = nullable1,
				EndDate = nullable2
			};
			OrderQuery orderQuery1 = orderQuery;
			int? nullable4 = orderStatus;
			if (nullable4.HasValue)
			{
				nullable = new OrderInfo.OrderOperateStatus?((OrderInfo.OrderOperateStatus)nullable4.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			orderQuery1.Status = nullable;
			orderQuery.UserId = new long?(base.CurrentUser.Id);
			orderQuery.SearchKeyWords = keywords;
			orderQuery.PageSize = pageSize;
			orderQuery.PageNo = pageNo;
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = orders.Total
			};
			ViewBag.pageInfo = pagingInfo;
			ViewBag.UserId = base.CurrentUser.Id;
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			IShopBonusService shopBonusService = ServiceHelper.Create<IShopBonusService>();
			ViewBag.SalesRefundTimeout = siteSettings.SalesReturnTimeout;
			List<OrderInfo> list = orders.Models.ToList();
			ICashDepositsService create = Instance<ICashDepositsService>.Create;
			IEnumerable<OrderListModel> orderListModel = 
				from item in list
				select new OrderListModel()
				{
					Id = item.Id,
					ActiveType = item.ActiveType,
					OrderType = item.OrderType,
					Address = item.Address,
					CellPhone = item.CellPhone,
					CloseReason = item.CloseReason,
					CommisTotalAmount = item.CommisAmount,
					DiscountAmount = item.DiscountAmount,
					ExpressCompanyName = item.ExpressCompanyName,
					FinishDate = item.FinishDate,
					Freight = item.Freight,
					GatewayOrderId = item.GatewayOrderId,
					IntegralDiscount = item.IntegralDiscount,
					UserId = item.UserId,
					ShopId = item.ShopId,
					ShopName = item.ShopName,
					ShipTo = item.ShipTo,
					OrderTotalAmount = item.OrderTotalAmount,
					PaymentTypeName = item.PaymentTypeName,
					OrderStatus = item.OrderStatus,
					RefundStats = item.RefundStats,
					OrderCommentInfo = item.OrderCommentInfo,
					OrderDate = item.OrderDate,
					OrderItemList = 
						from oItem in item.OrderItemInfo
						select new OrderItemListModel()
						{
							ProductId = oItem.ProductId,
							Color = oItem.Color,
							Size = oItem.Size,
							Version = oItem.Version,
							ProductName = oItem.ProductName,
							ThumbnailsUrl = oItem.ThumbnailsUrl,
							SalePrice = oItem.SalePrice,
							SkuId = oItem.SkuId,
							Quantity = oItem.Quantity,
							CashDepositsObligation = create.GetCashDepositsObligation(oItem.ProductId)
						},
					ReceiveBonus = shopBonusService.GetGrantByUserOrder(item.Id, CurrentUser.Id)
				};
			List<long> nums = (
				from d in list
				select d.Id).ToList();
			if (nums.Count > 0)
			{
				RefundQuery refundQuery = new RefundQuery()
				{
					OrderId = new long?(nums[0]),
					MoreOrderId = nums,
					PageNo = 1,
					PageSize = list.Count
				};
				List<OrderRefundInfo> orderRefundInfos = (
					from d in ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery).Models
					where (int)d.RefundMode == 1
					select d).ToList();
				if (orderRefundInfos.Count > 0)
				{
					foreach (OrderRefundInfo orderRefundInfo in orderRefundInfos)
					{
						OrderInfo orderInfo = list.FirstOrDefault((OrderInfo d) => d.Id == orderRefundInfo.OrderId);
						if (orderInfo == null)
						{
							continue;
						}
						orderInfo.RefundStats = (int)(orderRefundInfo.SellerAuditStatus);
					}
				}
			}
			return View(orderListModel.ToList());
		}
	}
}