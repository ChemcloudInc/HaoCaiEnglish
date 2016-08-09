using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
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

namespace Himall.Web.Areas.Admin.Controllers
{
	public class HomeController : BaseAdminController
	{
		public HomeController()
		{
		}

		[UnAuthorize]
		public ActionResult About()
		{
			return View();
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
			IPaltManager currentManager = base.CurrentManager;
			if (SecureHelper.MD5(string.Concat(SecureHelper.MD5(oldpassword), currentManager.PasswordSalt)) != currentManager.Password)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "旧密码错误"
				};
				return Json(result1);
			}
			ServiceHelper.Create<IManagerService>().ChangePlatformManagerPassword(currentManager.Id, password, 0);
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
			IPaltManager currentManager = base.CurrentManager;
			string str = SecureHelper.MD5(string.Concat(SecureHelper.MD5(password), currentManager.PasswordSalt));
			if (currentManager.Password == str)
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
			return View(ServiceHelper.Create<IShopService>().GetPlatConsoleMode());
		}

		[UnAuthorize]
		public ActionResult Copyright()
		{
			return View();
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetPlatfromMessage()
		{
			ProductQuery productQuery = new ProductQuery()
			{
				PageSize = 10000,
				PageNo = 1,
				AuditStatus = new ProductInfo.ProductAuditStatus[] { ProductInfo.ProductAuditStatus.WaitForAuditing },
				SaleStatus = new ProductInfo.ProductSaleStatus?(ProductInfo.ProductSaleStatus.OnSale)
			};
			ProductQuery productQuery1 = productQuery;
			int num = ServiceHelper.Create<IProductService>().GetProducts(productQuery1).Models.Count();
			ShopQuery shopQuery = new ShopQuery()
			{
				Status = new ShopInfo.ShopAuditStatus?(0),
				PageSize = 10000,
				PageNo = 1
			};
			int num1 = (
				from s in ServiceHelper.Create<IShopService>().GetShops(shopQuery).Models
				where (int)s.ShopStatus == 2 || (int)s.ShopStatus == 5 || (int)s.ShopStatus == 3
				select s).Count();
			long? nullable = null;
			int num2 = ServiceHelper.Create<IBrandService>().GetShopBrandApplys(nullable, new int?(0), 1, 10000, "").Models.Count();
			ComplaintQuery complaintQuery = new ComplaintQuery()
			{
				Status = new OrderComplaintInfo.ComplaintStatus?(OrderComplaintInfo.ComplaintStatus.Dispute),
				PageSize = 10000,
				PageNo = 1
			};
			ComplaintQuery complaintQuery1 = complaintQuery;
			int num3 = ServiceHelper.Create<IComplaintService>().GetOrderComplaints(complaintQuery1).Models.Count();
			RefundQuery refundQuery = new RefundQuery()
			{
				ConfirmStatus = new OrderRefundInfo.OrderRefundConfirmStatus?(OrderRefundInfo.OrderRefundConfirmStatus.UnConfirm),
				AuditStatus = new OrderRefundInfo.OrderRefundAuditStatus?(OrderRefundInfo.OrderRefundAuditStatus.Audited),
				PageSize = 10000,
				PageNo = 1
			};
			RefundQuery refundQuery1 = refundQuery;
			int num4 = ServiceHelper.Create<IRefundService>().GetOrderRefunds(refundQuery1).Models.Count();
			int num5 = num + num1 + num2 + num3 + num4;
			return Json(new { ProductWaitForAuditing = num, ShopWaitAudit = num1, BrandAudit = num2, ComplaintDispute = num3, RefundWaitAudit = num4, AllMessageCount = num5 }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult GetRecentMonthShopSaleRankChart()
		{
			LineChartDataModel<int> recentMonthShopSaleRankChart = ServiceHelper.Create<IStatisticsService>().GetRecentMonthShopSaleRankChart();
			return Json(new { successful = true, chart = recentMonthShopSaleRankChart }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public ActionResult Index()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item != null && !bool.Parse(item))
			{
				return RedirectToAction("Agreement", "Installer", new { area = "Web" });
			}
			ViewBag.Name = base.CurrentManager.UserName;
			dynamic viewBag = base.ViewBag;
			IEnumerable<int> adminPrivileges = 
				from a in base.CurrentManager.AdminPrivileges
				select (int)a;
			viewBag.Rights = string.Join<int>(",", 
				from a in adminPrivileges
				orderby a
				select a);
			return View();
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult ProductRecentMonthSaleRank()
		{
			LineChartDataModel<int> recentMonthSaleRankChart = ServiceHelper.Create<IStatisticsService>().GetRecentMonthSaleRankChart();
			return Json(new { successful = true, chart = recentMonthSaleRankChart }, JsonRequestBehavior.AllowGet);
		}
	}
}