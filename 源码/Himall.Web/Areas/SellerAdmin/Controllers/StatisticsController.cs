using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class StatisticsController : BaseSellerController
	{
		public StatisticsController()
		{
		}

		public ActionResult DealConversionRate()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			IStatisticsService statisticsService = ServiceHelper.Create<IStatisticsService>();
			long shopId = base.CurrentSellerManager.ShopId;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			LineChartDataModel<float> dealConversionRateChart = statisticsService.GetDealConversionRateChart(shopId, year, now.Month);
			return View(new ChartDataViewModel(dealConversionRateChart));
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetDealConversionRateChartByMonth(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			LineChartDataModel<float> dealConversionRateChart = ServiceHelper.Create<IStatisticsService>().GetDealConversionRateChart(base.CurrentSellerManager.ShopId, year, month);
			return Json(new { successful = true, chart = dealConversionRateChart }, JsonRequestBehavior.AllowGet);
		}

		private List<SelectListItem> GetMonthDrop()
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			for (int i = 1; i < 13; i++)
			{
				SelectListItem selectListItem = new SelectListItem()
				{
					Selected = DateTime.Now.Month == i,
					Text = i.ToString(),
					Value = i.ToString()
				};
				selectListItems.Add(selectListItem);
			}
			return selectListItems;
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetSaleRankingChart(string day = "", int year = 0, int month = 0, int weekIndex = 0, int dimension = 1)
		{
			DateTime now;
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>();
			if (string.IsNullOrWhiteSpace(day))
			{
				if (year == 0)
				{
					year = DateTime.Now.Year;
				}
				if (month == 0)
				{
					month = DateTime.Now.Month;
				}
				lineChartDataModel = (weekIndex != 0 ? ServiceHelper.Create<IStatisticsService>().GetProductSaleRankingChart(base.CurrentSellerManager.ShopId, year, month, weekIndex, (SaleDimension)dimension, 15) : ServiceHelper.Create<IStatisticsService>().GetProductSaleRankingChart(base.CurrentSellerManager.ShopId, year, month, (SaleDimension)dimension, 15));
			}
			else
			{
				if (!DateTime.TryParse(day, out now))
				{
					now = DateTime.Now;
				}
				lineChartDataModel = ServiceHelper.Create<IStatisticsService>().GetProductSaleRankingChart(base.CurrentSellerManager.ShopId, now, (SaleDimension)dimension, 15);
			}
			return Json(new { successful = true, chart = lineChartDataModel }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetShopFlowChartByMonth(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			LineChartDataModel<int> shopFlowChart = ServiceHelper.Create<IStatisticsService>().GetShopFlowChart(base.CurrentSellerManager.ShopId, year, month);
			return Json(new { successful = true, chart = shopFlowChart }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetShopSaleChartByMonth(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			LineChartDataModel<int> shopSaleChart = ServiceHelper.Create<IStatisticsService>().GetShopSaleChart(base.CurrentSellerManager.ShopId, year, month);
			return Json(new { successful = true, chart = shopSaleChart }, JsonRequestBehavior.AllowGet);
		}

		private List<SelectListItem> GetWeekDrop(int year, int month)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, 1);
			for (int i = 1; i <= 4; i++)
			{
				SelectListItem selectListItem = new SelectListItem()
				{
					Selected = i == 1
				};
				string str = startDayOfWeeks.ToString("yyyy-MM-dd");
				DateTime dateTime = startDayOfWeeks.AddDays(6);
				selectListItem.Text = string.Format("{0} -- {1}", str, dateTime.ToString("yyyy-MM-dd"));
				selectListItem.Value = i.ToString();
				selectListItems.Add(selectListItem);
				startDayOfWeeks = startDayOfWeeks.AddDays(7);
			}
			return selectListItems;
		}

		private List<SelectListItem> GetYearDrop(int start, int end)
		{
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			for (int i = start; i < end; i++)
			{
				SelectListItem selectListItem = new SelectListItem()
				{
					Selected = DateTime.Now.Year == i,
					Text = i.ToString(),
					Value = i.ToString()
				};
				selectListItems.Add(selectListItem);
			}
			return selectListItems;
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult ProductSaleRanking()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			dynamic viewBag = base.ViewBag;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			viewBag.WeekDrop = GetWeekDrop(year, now.Month);
			return View();
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult ProductVisitRanking()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			dynamic viewBag = base.ViewBag;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			viewBag.WeekDrop = GetWeekDrop(year, now.Month);
			return View();
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult ProductVisitRankingChart(string day = "", int year = 0, int month = 0, int weekIndex = 0)
		{
			DateTime now;
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>();
			if (string.IsNullOrWhiteSpace(day))
			{
				if (year == 0)
				{
					year = DateTime.Now.Year;
				}
				if (month == 0)
				{
					month = DateTime.Now.Month;
				}
				lineChartDataModel = (weekIndex != 0 ? ServiceHelper.Create<IStatisticsService>().GetProductVisitRankingChart(base.CurrentSellerManager.ShopId, year, month, weekIndex, 15) : ServiceHelper.Create<IStatisticsService>().GetProductVisitRankingChart(base.CurrentSellerManager.ShopId, year, month, 15));
			}
			else
			{
				if (!DateTime.TryParse(day, out now))
				{
					now = DateTime.Now;
				}
				lineChartDataModel = ServiceHelper.Create<IStatisticsService>().GetProductVisitRankingChart(base.CurrentSellerManager.ShopId, now, 15);
			}
			return Json(new { successful = true, chart = lineChartDataModel }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ShopFlow()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			IStatisticsService statisticsService = ServiceHelper.Create<IStatisticsService>();
			long shopId = base.CurrentSellerManager.ShopId;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			LineChartDataModel<int> shopFlowChart = statisticsService.GetShopFlowChart(shopId, year, now.Month);
			return View(new ChartDataViewModel(shopFlowChart));
		}

		public ActionResult ShopSale()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			IStatisticsService statisticsService = ServiceHelper.Create<IStatisticsService>();
			long shopId = base.CurrentSellerManager.ShopId;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			LineChartDataModel<int> shopSaleChart = statisticsService.GetShopSaleChart(shopId, year, now.Month);
			return View(new ChartDataViewModel(shopSaleChart));
		}
	}
}