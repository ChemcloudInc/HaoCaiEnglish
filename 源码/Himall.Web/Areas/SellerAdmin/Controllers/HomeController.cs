using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class HomeController : BaseSellerController
	{
		public HomeController()
		{
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult AnalysisEffectShop()
		{
			DateTime date = DateTime.Now.AddDays(-1).Date;
			DateTime dateTime = date.AddDays(1).AddMilliseconds(-1);
			ShopInfo.ShopVistis shopVistiInfo = ServiceHelper.Create<IShopService>().GetShopVistiInfo(date, dateTime, base.CurrentSellerManager.ShopId);
			IList<EchartsData> echartsDatas = new List<EchartsData>();
			if (shopVistiInfo == null)
			{
				string str = (new decimal(0)).ToString();
				EchartsData echartsDatum = new EchartsData()
				{
					name = "访问次数",
					@value = str
				};
				echartsDatas.Add(echartsDatum);
				EchartsData echartsDatum1 = new EchartsData()
				{
					name = "下单次数",
					@value = str
				};
				echartsDatas.Add(echartsDatum1);
				EchartsData echartsDatum2 = new EchartsData()
				{
					name = "支付金额",
					@value = str
				};
				echartsDatas.Add(echartsDatum2);
			}
			else
			{
				EchartsData echartsDatum3 = new EchartsData()
				{
					name = "访问次数",
					@value = shopVistiInfo.VistiCounts.ToString()
				};
				echartsDatas.Add(echartsDatum3);
				EchartsData echartsDatum4 = new EchartsData()
				{
					name = "下单次数",
					@value = shopVistiInfo.OrderCounts.ToString()
				};
				echartsDatas.Add(echartsDatum4);
				EchartsData echartsDatum5 = new EchartsData()
				{
					name = "支付金额",
					@value = shopVistiInfo.SaleAmounts.ToString()
				};
				echartsDatas.Add(echartsDatum5);
			}
			return Json(new { successful = true, chart = echartsDatas }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ChangePassword(string oldpassword, string password)
		{
			if (string.IsNullOrWhiteSpace(oldpassword) || string.IsNullOrWhiteSpace(password))
			{
				Result result = new Result()
				{
					success = false,
					msg = "密码不能为空！"
				};
				return Json(result);
			}
			ISellerManager currentSellerManager = base.CurrentSellerManager;
			if (SecureHelper.MD5(string.Concat(SecureHelper.MD5(oldpassword), currentSellerManager.PasswordSalt)) != currentSellerManager.Password)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "旧密码错误"
				};
				return Json(result1);
			}
			ServiceHelper.Create<IManagerService>().ChangeSellerManagerPassword(base.CurrentSellerManager.Id, base.CurrentSellerManager.ShopId, password, base.CurrentSellerManager.RoleId);
			Result result2 = new Result()
			{
				success = true,
				msg = "修改成功"
			};
			return Json(result2);
		}

		[UnAuthorize]
		public JsonResult CheckOldPassword(string password)
		{
			ISellerManager currentSellerManager = base.CurrentSellerManager;
			string str = SecureHelper.MD5(string.Concat(SecureHelper.MD5(password), currentSellerManager.PasswordSalt));
			if (currentSellerManager.Password == str)
			{
				return Json(new Result()
				{
					success = true
				});
			}
			return Json(new Result()
			{
				success = false
			});
		}

		[UnAuthorize]
		public ActionResult Console()
		{
			HomeModel homeModel = new HomeModel()
			{
				SellerConsoleModel = ServiceHelper.Create<IShopService>().GetSellerConsoleModel(base.CurrentSellerManager.ShopId),
				Articles = ServiceHelper.Create<IArticleService>().GetTopNArticle<ArticleInfo>(6, 4, null, false)
			};
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			if (shop != null)
			{
				ViewBag.Logo = base.CurrentSiteSetting.MemberLogo;
				homeModel.ShopId = shop.Id;
				homeModel.ShopLogo = shop.Logo;
				homeModel.ShopName = shop.ShopName;
				homeModel.ShopEndDate = (shop.EndDate.HasValue ? shop.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty);
				ShopGradeInfo shopGradeInfo = (
					from c in ServiceHelper.Create<IShopService>().GetShopGrades()
					where c.Id == shop.GradeId
					select c).FirstOrDefault();
				homeModel.ShopGradeName = (shopGradeInfo != null ? shopGradeInfo.Name : string.Empty);
				IQueryable<StatisticOrderCommentsInfo> shopStatisticOrderComments = ServiceHelper.Create<IShopService>().GetShopStatisticOrderComments(base.CurrentSellerManager.ShopId);
				StatisticOrderCommentsInfo statisticOrderCommentsInfo = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 1
					select c).FirstOrDefault();
				StatisticOrderCommentsInfo statisticOrderCommentsInfo1 = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 9
					select c).FirstOrDefault();
				StatisticOrderCommentsInfo statisticOrderCommentsInfo2 = (
					from c in shopStatisticOrderComments
					where (int)c.CommentKey == 9
					select c).FirstOrDefault();
				string str = "5";
				homeModel.ProductAndDescription = (statisticOrderCommentsInfo != null ? string.Format("{0:F}", statisticOrderCommentsInfo.CommentValue) : str);
				homeModel.SellerServiceAttitude = (statisticOrderCommentsInfo1 != null ? string.Format("{0:F}", statisticOrderCommentsInfo1.CommentValue) : str);
				homeModel.SellerDeliverySpeed = (statisticOrderCommentsInfo2 != null ? string.Format("{0:F}", statisticOrderCommentsInfo2.CommentValue) : str);
				homeModel.ProductsNumberIng = homeModel.SellerConsoleModel.ProductLimit.ToString();
				homeModel.ProductsNumber = homeModel.SellerConsoleModel.ProductsCount.ToString();
				homeModel.UseSpace = homeModel.SellerConsoleModel.ImageLimit.ToString();
				homeModel.UseSpaceing = homeModel.SellerConsoleModel.ProductImages.ToString();
				homeModel.OrderProductConsultation = homeModel.SellerConsoleModel.ProductConsultations.ToString();
				homeModel.OrderCounts = homeModel.SellerConsoleModel.OrderCounts.ToString();
				homeModel.OrderWaitPay = homeModel.SellerConsoleModel.WaitPayTrades.ToString();
				homeModel.OrderWaitDelivery = homeModel.SellerConsoleModel.WaitDeliveryTrades.ToString();
				ICommentService commentService = ServiceHelper.Create<ICommentService>();
				CommentQuery commentQuery = new CommentQuery()
				{
					PageNo = 1,
					PageSize = 2147483647,
					IsReply = new bool?(false),
					ShopID = base.CurrentSellerManager.ShopId
				};
				int total = commentService.GetComments(commentQuery).Total;
				homeModel.OrderReplyComments = total.ToString();
				homeModel.OrderHandlingComplaints = homeModel.SellerConsoleModel.Complaints.ToString();
				homeModel.OrderWithRefund = homeModel.SellerConsoleModel.RefundTrades.ToString();
				homeModel.OrderWithRefundAndRGoods = homeModel.SellerConsoleModel.RefundAndRGoodsTrades.ToString();
				homeModel.ProductsEvaluation = homeModel.SellerConsoleModel.ProductComments.ToString();
				int num = (
					from c in ServiceHelper.Create<IBrandService>().GetShopBrandApplys(base.CurrentSellerManager.ShopId)
					where c.AuditStatus == 1
					select c).Count();
				homeModel.ProductsBrands = num.ToString();
				homeModel.ProductsOnSale = homeModel.SellerConsoleModel.OnSaleProducts.ToString();
				IProductService productService = ServiceHelper.Create<IProductService>();
				ProductQuery productQuery = new ProductQuery()
				{
					PageNo = 1,
					PageSize = 2147483647,
					ShopId = new long?(base.CurrentSellerManager.ShopId),
					SaleStatus = new ProductInfo.ProductSaleStatus?(ProductInfo.ProductSaleStatus.InDraft)
				};
				int total1 = productService.GetProducts(productQuery).Total;
				homeModel.ProductsInDraft = total1.ToString();
				homeModel.ProductsWaitForAuditing = homeModel.SellerConsoleModel.WaitForAuditingProducts.ToString();
				homeModel.ProductsAuditFailed = homeModel.SellerConsoleModel.AuditFailureProducts.ToString();
				homeModel.ProductsInfractionSaleOff = homeModel.SellerConsoleModel.InfractionSaleOffProducts.ToString();
				homeModel.ProductsInStock = homeModel.SellerConsoleModel.InStockProducts.ToString();
				DateTime date = DateTime.Now.AddDays(-1).Date;
				DateTime dateTime = date.AddDays(1).AddMilliseconds(-1);
				ShopInfo.ShopVistis shopVistiInfo = ServiceHelper.Create<IShopService>().GetShopVistiInfo(date, dateTime, base.CurrentSellerManager.ShopId);
				List<EchartsData> echartsDatas = new List<EchartsData>();
				if (shopVistiInfo == null)
				{
					string str1 = (new decimal(0)).ToString();
					ViewBag.VistiCounts = str1;
					ViewBag.OrderCounts = str1;
					ViewBag.SaleAmounts = str1;
				}
				else
				{
					ViewBag.VistiCounts = shopVistiInfo.VistiCounts;
					ViewBag.OrderCounts = shopVistiInfo.OrderCounts;
					ViewBag.SaleAmounts = shopVistiInfo.SaleAmounts;
				}
			}
			return View(homeModel);
		}

		[UnAuthorize]
		public JsonResult GetsellerAdminMessage()
		{
			CommentQuery commentQuery = new CommentQuery()
			{
				PageNo = 1,
				PageSize = 100001,
				ShopID = base.CurrentSellerManager.ShopId,
				IsReply = new bool?(false)
			};
			CommentQuery commentQuery1 = commentQuery;
			int num = ServiceHelper.Create<ICommentService>().GetComments(commentQuery1).Models.Count();
			ConsultationQuery consultationQuery = new ConsultationQuery()
			{
				PageNo = 1,
				PageSize = 10000,
				ShopID = base.CurrentSellerManager.ShopId,
				IsReply = new bool?(false)
			};
			ConsultationQuery consultationQuery1 = consultationQuery;
			int num1 = ServiceHelper.Create<IConsultationService>().GetConsultations(consultationQuery1).Models.Count();
			IOrderService orderService = ServiceHelper.Create<IOrderService>();
			OrderQuery orderQuery = new OrderQuery()
			{
				PageNo = 1,
				PageSize = 10000,
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.WaitPay),
				ShopId = new long?(base.CurrentSellerManager.ShopId)
			};
			int num2 = orderService.GetOrders<OrderInfo>(orderQuery, null).Models.Count();
			IOrderService orderService1 = ServiceHelper.Create<IOrderService>();
			OrderQuery orderQuery1 = new OrderQuery()
			{
				PageNo = 1,
				PageSize = 10000,
				Status = new OrderInfo.OrderOperateStatus?(OrderInfo.OrderOperateStatus.WaitDelivery),
				ShopId = new long?(base.CurrentSellerManager.ShopId)
			};
			int num3 = orderService1.GetOrders<OrderInfo>(orderQuery1, null).Models.Count();
			IComplaintService complaintService = ServiceHelper.Create<IComplaintService>();
			ComplaintQuery complaintQuery = new ComplaintQuery()
			{
				PageNo = 1,
				PageSize = 10000,
				ShopId = new long?(base.CurrentSellerManager.ShopId),
				Status = new OrderComplaintInfo.ComplaintStatus?(OrderComplaintInfo.ComplaintStatus.WaitDeal)
			};
			int num4 = complaintService.GetOrderComplaints(complaintQuery).Models.Count();
			int num5 = num1 + num + num2 + num4 + num3;
			return Json(new { UnReplyConsultations = num1, UnReplyComments = num, UnPayOrder = num2, UnComplaints = num4, UnDeliveryOrder = num3, AllMessageCount = num5 }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public ActionResult Index()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item != null && !bool.Parse(item))
			{
				return RedirectToAction("Agreement", "Installer", new { area = "Web" });
			}
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			ViewBag.Name = base.CurrentSellerManager.UserName;
			dynamic viewBag = base.ViewBag;
			IEnumerable<int> sellerPrivileges = 
				from a in base.CurrentSellerManager.SellerPrivileges
				select (int)a;
			viewBag.Rights = string.Join<int>(",", 
				from a in sellerPrivileges
				orderby a
				select a);
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			ViewBag.Logo = base.CurrentSiteSetting.MemberLogo;
			return View(base.CurrentSellerManager);
		}

		private bool IsInstalled()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item == null)
			{
				return true;
			}
			return bool.Parse(item);
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult ProductRecentMonthSaleRank()
		{
			long shopId = base.CurrentSellerManager.ShopId;
			LineChartDataModel<int> recentMonthSaleRankChart = ServiceHelper.Create<IStatisticsService>().GetRecentMonthSaleRankChart(shopId);
			return Json(new { successful = true, chart = recentMonthSaleRankChart }, JsonRequestBehavior.AllowGet);
		}
	}
}