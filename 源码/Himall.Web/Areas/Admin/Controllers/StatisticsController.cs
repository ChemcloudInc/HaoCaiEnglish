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

namespace Himall.Web.Areas.Admin.Controllers
{
	public class StatisticsController : BaseAdminController
	{
		public StatisticsController()
		{
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetAreaMapBySearch(int dimension, int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			MapChartDataModel areaOrderChart = ServiceHelper.Create<IStatisticsService>().GetAreaOrderChart((OrderDimension)dimension, year, month);
			return Json(new { successful = true, chart = areaOrderChart }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetMemberChartByMonth(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			LineChartDataModel<int> memberChart = ServiceHelper.Create<IStatisticsService>().GetMemberChart(year, month);
			return Json(new { successful = true, chart = memberChart }, JsonRequestBehavior.AllowGet);
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
		public JsonResult GetNewShopChartByMonth(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			LineChartDataModel<int> newsShopChart = ServiceHelper.Create<IStatisticsService>().GetNewsShopChart(year, month);
			return Json(new { successful = true, chart = newsShopChart }, JsonRequestBehavior.AllowGet);
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
				lineChartDataModel = (weekIndex != 0 ? ServiceHelper.Create<IStatisticsService>().GetSaleRankingChart(year, month, weekIndex, (SaleDimension)dimension, 15) : ServiceHelper.Create<IStatisticsService>().GetSaleRankingChart(year, month, (SaleDimension)dimension, 15));
			}
			else
			{
				if (!DateTime.TryParse(day, out now))
				{
					now = DateTime.Now;
				}
				lineChartDataModel = ServiceHelper.Create<IStatisticsService>().GetSaleRankingChart(now, (SaleDimension)dimension, 15);
			}
			return Json(new { successful = true, chart = lineChartDataModel }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetShopRankingChart(string day = "", int year = 0, int month = 0, int weekIndex = 0, int dimension = 1)
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
				lineChartDataModel = (weekIndex != 0 ? ServiceHelper.Create<IStatisticsService>().GetShopRankingChart(year, month, weekIndex, (ShopDimension)dimension, 15) : ServiceHelper.Create<IStatisticsService>().GetShopRankingChart(year, month, (ShopDimension)dimension, 15));
			}
			else
			{
				if (!DateTime.TryParse(day, out now))
				{
					now = DateTime.Now;
				}
				lineChartDataModel = ServiceHelper.Create<IStatisticsService>().GetShopRankingChart(now, (ShopDimension)dimension, 15);
			}
			return Json(new { successful = true, chart = lineChartDataModel }, JsonRequestBehavior.AllowGet);
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

		[UnAuthorize]
		public JsonResult GetWeekList(int year = 0, int month = 0)
		{
			if (year == 0)
			{
				year = DateTime.Now.Year;
			}
			if (month == 0)
			{
				month = DateTime.Now.Month;
			}
			List<SelectListItem> weekDrop = GetWeekDrop(year, month);
			return Json(new { successful = true, week = weekDrop }, JsonRequestBehavior.AllowGet);
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

		[UnAuthorize]
		public ActionResult Member()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			LineChartDataModel<int> memberChart = ServiceHelper.Create<IStatisticsService>().GetMemberChart(DateTime.Now.Year, DateTime.Now.Month);
			return View(new ChartDataViewModel(memberChart));
		}

		[UnAuthorize]
		public ActionResult NewShop()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			LineChartDataModel<int> newsShopChart = ServiceHelper.Create<IStatisticsService>().GetNewsShopChart(DateTime.Now.Year, DateTime.Now.Month);
			return View(new ChartDataViewModel(newsShopChart));
		}

		[UnAuthorize]
		public ActionResult OrderAreaMap()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			dynamic viewBag = base.ViewBag;
			DateTime now = DateTime.Now;
			viewBag.Year = now.Year;
			dynamic month = base.ViewBag;
			DateTime dateTime = DateTime.Now;
			month.Month = dateTime.Month;
			return View();
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
		public ActionResult ShopRanking()
		{
			ViewBag.YearDrop = GetYearDrop(2014, 2024);
			ViewBag.MonthDrop = GetMonthDrop();
			dynamic viewBag = base.ViewBag;
			int year = DateTime.Now.Year;
			DateTime now = DateTime.Now;
			viewBag.WeekDrop = GetWeekDrop(year, now.Month);
			return View();
		}
	}
}