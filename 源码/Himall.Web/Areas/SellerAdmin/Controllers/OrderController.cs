using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Himall.Service;
using System.Transactions;
using Himall.Entity;



namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class OrderController : BaseSellerController
	{
        private OrderService orderService = new OrderService();
		public OrderController()
		{
		}

		[HttpPost]
		[ShopOperationLog(Message="商家取消订单")]
		public JsonResult CloseOrder(long orderId)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IOrderService>().SellerCloseOrder(orderId, base.CurrentSellerManager.UserName);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		[HttpPost]
		public JsonResult ConfirmSendGood(string ids, string companyNames, string shipOrderNumbers)
		{
			Result result = new Result();
			try
			{
				char[] chrArray = new char[] { ',' };
				IEnumerable<long> nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
				string[] strArrays = companyNames.Split(new char[] { ',' });
				string[] strArrays1 = shipOrderNumbers.Split(new char[] { ',' });
				ServiceHelper.Create<IOrderService>().GetOrders(nums);
				int num = 0;
				foreach (long num1 in nums)
				{
					ServiceHelper.Create<IOrderService>().SellerSendGood(num1, base.CurrentSellerManager.UserName, strArrays[num], strArrays1[num]);
					num++;
				}
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		private Cell CreateCell(int cellID, Row row)
		{
			Cell cell = row.GetCell(cellID) ?? row.CreateCell(cellID);
			return cell;
		}

		private Row CreateRow(int rowID, HSSFSheet excelSheet)
		{
			Row row = excelSheet.GetRow(rowID) ?? excelSheet.CreateRow(rowID);
			return row;
		}

		public ActionResult Detail(long id, bool updatePrice = false)
		{
			OrderInfo order = ServiceHelper.Create<IOrderService>().GetOrder(id);
			if (order == null || order.ShopId != base.CurrentSellerManager.ShopId)
			{
				throw new HimallException("订单已被删除，或者不属于该店铺！");
			}
			ViewBag.UpdatePrice = updatePrice;
			return View(order);
		}

		[UnAuthorize]
		public FileResult DownloadOrderList(string ids)
		{
			base.HttpContext.Response.BufferOutput = true;
			DateTime now = DateTime.Now;
			string str = string.Concat("ordergoods_", now.ToString("yyyyMMddHHmmss"), ".xls");
			if (base.Request.ServerVariables["http_user_agent"].ToLower().IndexOf("firefox") == -1)
			{
				str = HttpUtility.UrlEncode(str, Encoding.UTF8);
			}
			HSSFWorkbook excel = writeToExcel(ids);
			MemoryStream memoryStream = new MemoryStream();
			excel.Write(memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return File(memoryStream, "application/vnd.ms-excel", str);
		}

		public void DownloadProductList(string ids)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			Dictionary<long, OrderItemInfo> orderItems = ServiceHelper.Create<IOrderService>().GetOrderItems(nums);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=gb2312\"></head><body>");
			stringBuilder.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
			stringBuilder.AppendLine("<caption style='text-align:center;'>配货单(仓库拣货表)</caption>");
			stringBuilder.AppendLine("<tr style=\"font-weight: bold; white-space: nowrap;\">");
			stringBuilder.AppendLine("<td>商品名称</td>");
			stringBuilder.AppendLine("<td>货号</td>");
			stringBuilder.AppendLine("<td>规格</td>");
			stringBuilder.AppendLine("<td>拣货数量</td>");
			stringBuilder.AppendLine("<td>现库存数</td>");
			stringBuilder.AppendLine("</tr>");
			long stock = 0;
			foreach (OrderItemInfo value in orderItems.Values)
			{
				SKUInfo sku = ServiceHelper.Create<IProductService>().GetSku(value.SkuId);
				if (sku != null)
				{
					stock = sku.Stock;
				}
				stringBuilder.AppendLine("<tr>");
				stringBuilder.AppendLine(string.Concat("<td>", value.ProductName, "</td>"));
				stringBuilder.AppendLine(string.Concat("<td style=\"vnd.ms-excel.numberformat:@\">", value.SKU, "</td>"));
				string[] color = new string[] { "<td>", value.Color, value.Size, value.Version, "</td>" };
				stringBuilder.AppendLine(string.Concat(color));
				stringBuilder.AppendLine(string.Concat("<td>", value.Quantity, "</td>"));
				long quantity = stock + value.Quantity;
				stringBuilder.AppendLine(string.Concat("<td>", quantity.ToString(), "</td>"));
				stringBuilder.AppendLine("</tr>");
			}
			stringBuilder.AppendLine("</table>");
			stringBuilder.AppendLine("</body></html>");
			base.Response.Clear();
			base.Response.Buffer = false;
			base.Response.Charset = "GB2312";
			HttpResponseBase response = base.Response;
			DateTime now = DateTime.Now;
			response.AppendHeader("Content-Disposition", string.Concat("attachment;filename=productgoods_", now.ToString("yyyyMMddHHmmss"), ".xls"));
			base.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
			base.Response.ContentType = "application/ms-excel";
			base.Response.Write(stringBuilder.ToString());
			base.Response.End();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetExpressData(string expressCompanyName, string shipOrderNumber)
		{
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

		private string GetIconSrc(PlatformType platform)
		{
			if (platform == PlatformType.IOS || platform == PlatformType.Android)
			{
				return "/images/app.png";
			}
			return string.Format("/images/{0}.png", platform.ToString());
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetOrderPrint(string ids)
		{
			Result result = new Result();
			try
			{
				char[] chrArray = new char[] { ',' };
				IEnumerable<long> nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
				IEnumerable<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders(nums);
				string siteName = base.CurrentSiteSetting.SiteName;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (OrderInfo list in orders.ToList())
				{
					object id = list.Id;
					DateTime orderDate = list.OrderDate;
					stringBuilder.AppendFormat("<h3 class=\"table-hd\"><strong>{0}发货单</strong><span>订单号：{1}（{2}）</span></h3>", siteName, id, orderDate.ToLongDateString());
					stringBuilder.Append("<table class=\"table table-bordered\"><thead><tr><th>商品名称</th><th>规格</th><th>数量</th><th>单价</th><th>总价</th></tr></thead><tbody>");
					foreach (OrderItemInfo orderItemInfo in list.OrderItemInfo.ToList())
					{
						stringBuilder.Append("<tr>");
						stringBuilder.AppendFormat("<td style=\"text-align:left\">{0}</td>", orderItemInfo.ProductName);
						stringBuilder.AppendFormat("<td>{0} {1} {2}</td>", orderItemInfo.Color, orderItemInfo.Size, orderItemInfo.Version);
						stringBuilder.AppendFormat("<td>{0}</td>", orderItemInfo.Quantity);
						stringBuilder.AppendFormat("<td>￥{0}</td>", orderItemInfo.SalePrice);
						stringBuilder.AppendFormat("<td>￥{0}</td>", orderItemInfo.RealTotalPrice);
						stringBuilder.Append("</tr>");
					}
					stringBuilder.AppendFormat("<tr><td style=\"text-align:right\" colspan=\"6\"><span>商品总价：￥{0} &nbsp; 运费：￥{1}</span> &nbsp; <b>实付金额：￥{2}</b></td></tr>", list.ProductTotalAmount, list.Freight, list.OrderTotalAmount);
					stringBuilder.AppendLine("</tbody></table>");
				}
				result.success = true;
				result.msg = stringBuilder.ToString();
			}
			catch (Exception exception)
			{
				result.success = false;
			}
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetRegion(long? key = null, int? level = -1)
		{
			int? nullable = level;
			if ((nullable.GetValueOrDefault() != -1 ? false : nullable.HasValue))
			{
				key = new long?(0);
			}
			if (!key.HasValue)
			{
				return Json(new object[0]);
			}
			IEnumerable<KeyValuePair<long, string>> region = ServiceHelper.Create<IRegionService>().GetRegion(key.Value);
			return Json(region);
		}

		[UnAuthorize]
		public JsonResult GetRegionIdPath(long regionId)
		{
			return Json(ServiceHelper.Create<IRegionService>().GetRegionIdPath(regionId));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(DateTime? startDate, DateTime? endDate, long? orderId, int? orderStatus, string userName, int page, int rows, int? orderType)
		{
			OrderInfo.OrderOperateStatus? nullable;
			OrderQuery orderQuery = new OrderQuery()
			{
				StartDate = startDate,
				EndDate = endDate,
				OrderId = orderId
			};
			OrderQuery orderQuery1 = orderQuery;
			int? nullable1 = orderStatus;
			if (nullable1.HasValue)
			{
				nullable = new OrderInfo.OrderOperateStatus?((OrderInfo.OrderOperateStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			orderQuery1.Status = nullable;
			orderQuery.ShopId = new long?(base.CurrentSellerManager.ShopId);
			orderQuery.UserName = userName;
			orderQuery.OrderType = orderType;
			orderQuery.PageSize = rows;
			orderQuery.PageNo = page;
			PageModel<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
			IEnumerable<OrderModel> array = 
				from item in orders.Models.ToArray()
				select new OrderModel()
				{
					OrderId = item.Id,
					OrderStatus = item.OrderStatus.ToDescription(),
					OrderDate = item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
					ShopId = item.ShopId,
					ShopName = item.ShopName,
					UserId = item.UserId,
					UserName = item.UserName,
					TotalPrice = item.OrderTotalAmount,
					PaymentTypeName = item.PaymentTypeName,
					IconSrc = GetIconSrc(item.Platform),
					PlatForm = (int)item.Platform,
					PlatformText = item.Platform.ToDescription()
				};
			array = array.ToList();
			List<long> list = (
				from d in array
				select d.OrderId).ToList();
			if (list.Count > 0)
			{
				RefundQuery refundQuery = new RefundQuery()
				{
					OrderId = new long?(list[0]),
					MoreOrderId = list,
					PageNo = 1,
					PageSize = array.Count()
				};
				List<OrderRefundInfo> orderRefundInfos = (
					from d in ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery).Models
					where (int)d.RefundMode == 1 && (int)d.SellerAuditStatus != 4
					select d).ToList();
				if (orderRefundInfos.Count > 0)
				{
					foreach (OrderRefundInfo orderRefundInfo in orderRefundInfos)
					{
						OrderModel orderModel = array.FirstOrDefault((OrderModel d) => d.OrderId == orderRefundInfo.OrderId);
						if (orderModel == null || !(orderModel.OrderStatus != OrderInfo.OrderOperateStatus.Close.ToDescription()) || orderRefundInfo.SellerAuditStatus == OrderRefundInfo.OrderRefundAuditStatus.UnAudit)
						{
							continue;
						}
						orderModel.RefundStats = (int)orderRefundInfo.SellerAuditStatus;
					}
				}
			}
			DataGridModel<OrderModel> dataGridModel = new DataGridModel<OrderModel>()
			{
				rows = array,
				total = orders.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}

        [HttpPost]
        [UnAuthorize]
        public JsonResult ListAccountType(DateTime? startDate, DateTime? endDate, long? orderId, int? accountType, string userName, int page, int rows, int? orderType)
        {
            Boolean IsNowTime = true;
            if(accountType == 5)
            {
                accountType = 1;
                IsNowTime = false;
            }
            OrderInfo.AccountTypes? nullable;
            List<OrderInfo.OrderOperateStatus> a =new List<OrderInfo.OrderOperateStatus> { OrderInfo.OrderOperateStatus.WaitReceiving,OrderInfo.OrderOperateStatus.WaitDelivery };
            OrderQuery orderQuery = new OrderQuery()
            {
                StartDate = startDate,
                EndDate = endDate,
                OrderId = orderId
            };
            OrderQuery orderQuery1 = orderQuery;
            int? nullable1 = accountType;
            if (nullable1.HasValue)
            {
                nullable = new OrderInfo.AccountTypes?((OrderInfo.AccountTypes)nullable1.GetValueOrDefault());
            }
            else
            {
                nullable = null;
            }

            orderQuery1.AccountTypeStatus = nullable;
            orderQuery.MoreStatus = a;
            orderQuery.Status = OrderInfo.OrderOperateStatus.Finish;
            orderQuery.ShopId = new long?(base.CurrentSellerManager.ShopId);
            orderQuery.UserName = userName;
            orderQuery.OrderType = orderType;
            orderQuery.PageSize = rows;
            orderQuery.PageNo = page;
            IEnumerable<OrderModel> array;
            PageModel<OrderInfo> orders;
            SiteSettingsInfo siteSettings = (new SiteSettingService()).GetSiteSettings();
          //  int s = siteSettings.WeekSettlement;
            if (!IsNowTime)
            {
                DateTime time1 = DateTime.Now.Date;
                orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
                array =
                    from item in orders.Models.ToArray()
                    where (item.OrderDate.AddDays(siteSettings.WeekSettlement) < time1)
                    select new OrderModel()
                    {
                        OrderId = item.Id,
                        OrderStatus = item.OrderStatus.ToDescription(),
                        OrderDate = item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        ShopId = item.ShopId,
                        ShopName = item.ShopName,
                        UserId = item.UserId,
                        UserName = item.UserName,
                        TotalPrice = item.OrderTotalAmount,
                        PaymentTypeName = item.PaymentTypeName,
                        IconSrc = GetIconSrc(item.Platform),
                        PlatForm = (int)item.Platform,
                        PlatformText = item.Platform.ToDescription(),
                        AccountType = item.AccountType.ToDescription()
                    };
                array = array.ToList();
            }
            else
            {
                orders = ServiceHelper.Create<IOrderService>().GetOrders<OrderInfo>(orderQuery, null);
                array =
                    from item in orders.Models.ToArray()
                   // where (item.OrderDate.AddDays(9) < time1 || IsNowTime)
                    select new OrderModel()
                    {
                        OrderId = item.Id,
                        OrderStatus = item.OrderStatus.ToDescription(),
                        OrderDate = item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        ShopId = item.ShopId,
                        ShopName = item.ShopName,
                        UserId = item.UserId,
                        UserName = item.UserName,
                        TotalPrice = item.OrderTotalAmount,
                        PaymentTypeName = item.PaymentTypeName,
                        IconSrc = GetIconSrc(item.Platform),
                        PlatForm = (int)item.Platform,
                        PlatformText = item.Platform.ToDescription(),
                        AccountType = item.AccountType.ToDescription()
                    };
                array = array.ToList();
            }
            List<long> list = (
                from d in array
                select d.OrderId).ToList();
            if (list.Count > 0)
            {
                RefundQuery refundQuery = new RefundQuery()
                {
                    OrderId = new long?(list[0]),
                    MoreOrderId = list,
                    PageNo = 1,
                    PageSize = array.Count()
                };
                List<OrderRefundInfo> orderRefundInfos = (
                    from d in ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery).Models
                    where (int)d.RefundMode == 1 && (int)d.SellerAuditStatus != 4
                    select d).ToList();
                if (orderRefundInfos.Count > 0)
                {
                    foreach (OrderRefundInfo orderRefundInfo in orderRefundInfos)
                    {
                        OrderModel orderModel = array.FirstOrDefault((OrderModel d) => d.OrderId == orderRefundInfo.OrderId);
                        if (orderModel == null || !(orderModel.OrderStatus != OrderInfo.OrderOperateStatus.Close.ToDescription()) || orderRefundInfo.SellerAuditStatus == OrderRefundInfo.OrderRefundAuditStatus.UnAudit)
                        {
                            continue;
                        }
                        orderModel.RefundStats = (int)orderRefundInfo.SellerAuditStatus;
                    }
                }
            }
            DataGridModel<OrderModel> dataGridModel = new DataGridModel<OrderModel>()
            {
                rows = array,
                total = orders.Total
                //total=array.Count()
            };
            return Json(dataGridModel);
        }
        public ActionResult AccountTypeList()
        {
            return View();
        }
        public ActionResult ApplySettlement()
        {
            return View();
        }
		public ActionResult Print(string orderIds)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in orderIds.Split(chrArray)
				select long.Parse(item);
			ViewBag.OrdersCount = nums.Count();
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			ViewBag.Name = shop.SenderName;
			ViewBag.Address = shop.SenderAddress;
			ViewBag.Tel = shop.SenderPhone;
			if (!shop.SenderRegionId.HasValue)
			{
				ViewBag.RegionId = "";
				ViewBag.FullRegionPath = "";
			}
			else
			{
				string regionIdPath = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.SenderRegionId.Value);
				dynamic viewBag = base.ViewBag;
				int? senderRegionId = shop.SenderRegionId;
				viewBag.RegionId = senderRegionId.Value;
				ViewBag.FullRegionPath = regionIdPath;
			}
			IEnumerable<IExpress> recentExpress = ServiceHelper.Create<IExpressService>().GetRecentExpress(base.CurrentSellerManager.ShopId, 2147483647);
			return View(recentExpress);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Print(string orderIds, string expressName, string startNo, int regionId, string address, string senderName, string senderPhone)
		{
			IExpressService expressService = ServiceHelper.Create<IExpressService>();
			IExpress express = expressService.GetExpress(expressName);
			if (!express.CheckExpressCodeIsValid(startNo))
			{
				return Json(new { success = false, msg = "起始快递单号无效" });
			}
			ServiceHelper.Create<IShopService>().UpdateShopSenderInfo(base.CurrentSellerManager.ShopId, regionId, address, senderName, senderPhone);
			IEnumerable<int> elements = 
				from item in express.Elements
				select item.PrintElementIndex;
			List<PrintModel> printModels = new List<PrintModel>();
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in orderIds.Split(chrArray)
				select long.Parse(item);
			foreach (long num in nums)
			{
				PrintModel printModel = new PrintModel()
				{
					Width = express.Width,
					Height = express.Height,
					FontSize = 11
				};
				PrintModel printModel1 = printModel;
				IDictionary<int, string> printElementIndexAndOrderValue = expressService.GetPrintElementIndexAndOrderValue(base.CurrentSellerManager.ShopId, num, elements);
				printModel1.Elements = printElementIndexAndOrderValue.Select<KeyValuePair<int, string>, PrintModel.PrintElement>((KeyValuePair<int, string> item) => {
					ExpressPrintElement expressPrintElement = express.Elements.FirstOrDefault((ExpressPrintElement t) => t.PrintElementIndex == item.Key);
					return new PrintModel.PrintElement()
					{
						X = expressPrintElement.LeftTopPoint.X,
						Y = expressPrintElement.LeftTopPoint.Y,
						Height = expressPrintElement.RightBottomPoint.Y - expressPrintElement.LeftTopPoint.Y,
						Width = expressPrintElement.RightBottomPoint.X - expressPrintElement.LeftTopPoint.X,
						Value = item.Value
					};
				});
				printModels.Add(printModel1);
			}
			ServiceHelper.Create<IOrderService>().SetOrderExpressInfo(base.CurrentSellerManager.ShopId, expressName, startNo, nums);
			return Json(new { success = true, data = printModels });
		}
        public JsonResult submitApply(String ids)                         ///////////////////////////////////
        {
            String sd = ids;
            Result result = new Result();
            try
            {
                //修改数据库
                CalculationMoney(ids);
                result.success = true;
            }
            catch (Exception exception)
            {
                result.msg = exception.Message;
            }
            return Json(result);

        }

        private void CalculationMoney(string orderIds)
        {
            SiteSettingsInfo siteSettings = (new SiteSettingService()).GetSiteSettings();
            char[] chrArray = new char[] { ',' };
            IEnumerable<long> numsOrder =
                from item in orderIds.Split(chrArray)
                select long.Parse(item);
            DateTime? finishDate;
            string[] str;
            Entities entity = new Entities();
            var list = (
                from p in entity.OrderInfo
                join o in entity.OrderRefundInfo on p.Id equals o.OrderId
                join x in entity.OrderItemInfo on o.OrderId equals x.OrderId
                where numsOrder.Contains(p.Id)
                select new { Order = p, OrderRefund = o, OrderItem = x }).Distinct().ToList();
            var collection = (
                from p in entity.OrderInfo
                join o in entity.OrderItemInfo on p.Id equals o.OrderId
                where numsOrder.Contains(p.Id)
                select new { Order = p, OrderItem = o }).ToList();
            List<long> nums = new List<long>();
            nums.AddRange(
                from c in list
                select c.Order.ShopId);
            nums.AddRange(
                from c in collection
                select c.Order.ShopId);
            nums = nums.Distinct<long>().ToList();
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    foreach (long num in nums)
                    {
                        List<OrderInfo> orderInfos = (
                            from c in collection
                            where c.Order.ShopId == num
                            select c.Order).Distinct<OrderInfo>().ToList();
                        decimal num1 = orderInfos.Sum<OrderInfo>((OrderInfo c) => c.ProductTotalAmount) - orderInfos.Sum<OrderInfo>((OrderInfo c) => c.DiscountAmount);
                        decimal num2 = orderInfos.Sum<OrderInfo>((OrderInfo c) => c.Freight);
                        decimal num3 = CalculationTotalCommission((
                            from c in collection
                            where c.Order.ShopId == num
                            select c.OrderItem).Distinct<OrderItemInfo>().ToList());
                        decimal num4 = CalculationTotalRefundCommission((
                            from c in list
                            where c.OrderRefund.ShopId == num
                            select c.OrderItem).Distinct<OrderItemInfo>().ToList());
                        decimal num5 = (
                            from c in list
                            where c.OrderRefund.ShopId == num
                            select c.OrderRefund).Distinct<OrderRefundInfo>().Sum<OrderRefundInfo>((OrderRefundInfo c) => c.Amount);
                        decimal num6 = (((num1 + num2) - num3) - num5) + num4;
                        AccountInfo accountInfo = new AccountInfo()
                        {
                            ShopId = num,
                            ShopName = (
                                from c in entity.ShopInfo
                                where c.Id == num
                                select c).FirstOrDefault().ShopName,
                            AccountDate = DateTime.Now,
                            FinishDate = DateTime.Now,
                            StartDate = DateTime.Now.Date.AddDays(-siteSettings.WeekSettlement),
                            EndDate = DateTime.Now.Date,
                            Status = AccountInfo.AccountStatus.UnAccount,
                            ProductActualPaidAmount = num1,
                            FreightAmount = num2,
                            CommissionAmount = num3,
                            RefundCommissionAmount = num4,
                            RefundAmount = num5,
                            PeriodSettlement = num6,
                            Remark = string.Empty
                        };
                        entity.AccountInfo.Add(accountInfo);
                        foreach (OrderInfo orderInfo in (
                            from c in list
                            where c.Order.ShopId == num
                            select c.Order).Distinct<OrderInfo>().ToList())
                        {
                            AccountDetailInfo accountDetailInfo = new AccountDetailInfo()
                            {
                                Himall_Accounts = accountInfo,
                                ShopId = orderInfo.ShopId
                            };
                            finishDate = orderInfo.FinishDate;
                            accountDetailInfo.Date = finishDate.Value;
                            accountDetailInfo.OrderType = AccountDetailInfo.EnumOrderType.ReturnOrder;
                            accountDetailInfo.OrderId = orderInfo.Id;
                            accountDetailInfo.ProductActualPaidAmount = orderInfo.ProductTotalAmount - orderInfo.DiscountAmount;
                            accountDetailInfo.FreightAmount = orderInfo.Freight;
                            accountDetailInfo.CommissionAmount = CalculationTotalCommission((
                                from c in list
                                where c.OrderRefund.OrderId == orderInfo.Id
                                select c.OrderItem).Distinct<OrderItemInfo>().ToList());
                            accountDetailInfo.RefundCommisAmount = CalculationTotalRefundCommission((
                                from c in list
                                where c.OrderRefund.OrderId == orderInfo.Id
                                select c.OrderItem).Distinct<OrderItemInfo>().ToList());
                            accountDetailInfo.RefundTotalAmount = (
                                from c in list
                                where c.OrderRefund.OrderId == orderInfo.Id
                                select c.OrderRefund).Distinct<OrderRefundInfo>().Sum<OrderRefundInfo>((OrderRefundInfo c) => c.Amount);
                            accountDetailInfo.OrderDate = orderInfo.OrderDate;
                            accountDetailInfo.OrderRefundsDates = string.Join<DateTime>(";", (
                                from c in list
                                where c.OrderRefund.OrderId == orderInfo.Id
                                select c.OrderRefund.ManagerConfirmDate).Distinct<DateTime>());
                            entity.AccountDetailInfo.Add(accountDetailInfo);
                            UpateAccountType(orderInfo.Id);
                        }
                        foreach (OrderInfo orderInfo1 in orderInfos)
                        {
                            AccountDetailInfo value = new AccountDetailInfo()
                            {
                                Himall_Accounts = accountInfo,
                                ShopId = orderInfo1.ShopId
                            };
                            finishDate = orderInfo1.FinishDate;
                            value.Date = finishDate.Value;
                            value.OrderType = AccountDetailInfo.EnumOrderType.FinishedOrder;
                            value.OrderId = orderInfo1.Id;
                            value.ProductActualPaidAmount = orderInfo1.ProductTotalAmount - orderInfo1.DiscountAmount;
                            value.FreightAmount = orderInfo1.Freight;
                            value.CommissionAmount = CalculationTotalCommission((
                                from c in collection
                                where c.Order.Id == orderInfo1.Id
                                select c.OrderItem).Distinct<OrderItemInfo>().ToList());
                            value.RefundCommisAmount = new decimal(0);
                            value.RefundTotalAmount = new decimal(0);
                            value.OrderDate = orderInfo1.OrderDate;
                            value.OrderRefundsDates = string.Empty;
                            entity.AccountDetailInfo.Add(value);
                            UpateAccountType(orderInfo1.Id);
                        }
                    }
                    entity.SaveChanges();
                    transactionScope.Complete();
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    str = new string[] { "CalculationMoney ：startDate=", DateTime.Now.Date.AddDays(-siteSettings.WeekSettlement).ToString(), " endDate=", DateTime.Now.Date.ToString(), "/r/n", exception.Message };
                    Log.Error(string.Concat(str));
                }
            }
        }
        /// <summary>
        /// 更新Order状态为待结款
        /// </summary>
        /// <param name="orderId"></param>
        public void UpateAccountType(long orderId)
        {
            OrderInfo orderInfo = orderService.GetOrder(orderId);
            if (orderInfo != null)
            {
                orderInfo.AccountType = OrderInfo.AccountTypes.WaitAccout;
                orderService.UpdateOrderInfo(orderInfo);
            }
        }

        /// <summary>
        /// 计算总佣金
        /// </summary>
        /// <param name="orderItems"></param>
        /// <returns></returns>
        private decimal CalculationTotalCommission(IList<OrderItemInfo> orderItems)
        {
            decimal num = new decimal(0);
            return orderItems.Sum<OrderItemInfo>((OrderItemInfo c) => c.RealTotalPrice * c.CommisRate);
        }
        /// <summary>
        /// 计算退还的总佣金
        /// </summary>
        /// <param name="orderItems"></param>
        /// <returns></returns>
        private decimal CalculationTotalRefundCommission(IList<OrderItemInfo> orderItems)
        {
            decimal num = new decimal(0);
            return orderItems.Sum<OrderItemInfo>((OrderItemInfo c) => c.RefundPrice * c.CommisRate);
        }

		public ActionResult SendGood(string ids)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			OrderController.SendGoodMode sendGoodMode = new OrderController.SendGoodMode();
			IOrderedEnumerable<OrderInfo> orderInfos = ServiceHelper.Create<IOrderService>().GetOrders(nums).Where((OrderInfo a) => {
				if (a.OrderStatus != OrderInfo.OrderOperateStatus.WaitDelivery)
				{
					return false;
				}
				return a.ShopId == base.CurrentSellerManager.ShopId;
			}).OrderByDescending<OrderInfo, DateTime>((OrderInfo a) => a.OrderDate);
			if (orderInfos == null)
			{
				throw new HimallException(string.Concat("没有找到相关的订单", ids));
			}
			sendGoodMode.Orders = orderInfos;
			sendGoodMode.LogisticsCompanies = ServiceHelper.Create<IExpressService>().GetAllExpress();
			return View(sendGoodMode);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateAddress(long orderId, string shipTo, string cellPhone, int topRegionId, int regionId, string address)
		{
			Result result = new Result();
			try
			{
				string regionFullName = ServiceHelper.Create<IRegionService>().GetRegionFullName(regionId, " ");
				ServiceHelper.Create<IOrderService>().SellerUpdateAddress(orderId, base.CurrentSellerManager.UserName, shipTo, cellPhone, topRegionId, regionId, regionFullName, address);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateItemDiscountAmount(long orderItemId, decimal discountAmount)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IOrderService>().SellerUpdateItemDiscountAmount(orderItemId, discountAmount, base.CurrentSellerManager.UserName);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		[HttpPost]
		public JsonResult UpdateOrderFrieght(long orderId, decimal frieght)
		{
			ServiceHelper.Create<IOrderService>().SellerUpdateOrderFreight(orderId, frieght);
			return Json(new { success = true });
		}

		private HSSFWorkbook writeToExcel(string ids)
		{
			long stock = 0;
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			IEnumerable<OrderInfo> orders = ServiceHelper.Create<IOrderService>().GetOrders(nums);
			HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
			HSSFSheet hSSFSheet = (HSSFSheet)hSSFWorkbook.CreateSheet("sheet1");
			Row row = hSSFSheet.CreateRow(0);
			string[] strArrays = new string[] { "订单单号", "商品名称", "货号", "规格", "拣货数量", "现库存数", "备注" };
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
                CreateCell(i, row).SetCellValue(strArrays1[i]);
			}
			foreach (OrderInfo list in orders.ToList())
			{
				foreach (OrderItemInfo orderItemInfo in list.OrderItemInfo.ToList())
				{
					SKUInfo sku = ServiceHelper.Create<IProductService>().GetSku(orderItemInfo.SkuId);
					if (sku != null)
					{
						stock = sku.Stock;
					}
					Row row1 = CreateRow(hSSFSheet.PhysicalNumberOfRows, hSSFSheet);
					Cell cell = CreateCell(0, row1);
                    //CellStyle format = hSSFWorkbook.CreateCellStyle();
                    //format.DataFormat = hSSFWorkbook.CreateDataFormat().GetFormat("####################");
                    //cell.CellStyle = format;
					cell.SetCellValue(list.Id.ToString());
					cell = CreateCell(1, row1);
					cell.SetCellValue(orderItemInfo.ProductName);
					cell = CreateCell(2, row1);
					cell.SetCellValue(orderItemInfo.SKU);
					cell = CreateCell(3, row1);
					cell.SetCellValue(string.Concat(orderItemInfo.Color, orderItemInfo.Size, orderItemInfo.Version));
					cell = CreateCell(4, row1);
					cell.SetCellValue(orderItemInfo.Quantity);
					cell = CreateCell(5, row1);
					cell.SetCellValue((stock + orderItemInfo.Quantity).ToString());
					cell = CreateCell(6, row1);
					cell.SetCellValue(list.UserRemark);
				}
			}
			return hSSFWorkbook;
		}

		public class SendGoodMode
		{
			public IEnumerable<IExpress> LogisticsCompanies
			{
				get;
				set;
			}

			public IEnumerable<OrderInfo> Orders
			{
				get;
				set;
			}

			public SendGoodMode()
			{
			}
		}
	}
}