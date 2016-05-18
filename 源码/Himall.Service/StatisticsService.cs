using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class StatisticsService : ServiceBase, IStatisticsService, IService, IDisposable
	{
		public StatisticsService()
		{
		}

		private void ChechData(int year, int month)
		{
			if (year < 1600 || year > 9999)
			{
				throw new Exception("非法的年份");
			}
			if (month < 0 || month > 12)
			{
				throw new Exception("非法的月份");
			}
		}

		private string[] GenerateStringByDays(int days)
		{
			string[] strArrays = new string[days];
			for (int i = 1; i <= days; i++)
			{
				strArrays[i - 1] = string.Format("{0}", i);
			}
			return strArrays;
		}

		public MapChartDataModel GetAreaOrderChart(OrderDimension dimension, int year, int month)
		{
            ChechData(year, month);
			MapChartDataModel mapChartDataModel = new MapChartDataModel();
			switch (dimension)
			{
				case OrderDimension.OrderMemberCount:
				{
                        InitialOrderMemberCount(mapChartDataModel, year, month);
					break;
				}
				case OrderDimension.OrderCount:
				{
                        InitialOrderCount(mapChartDataModel, year, month);
					break;
				}
				case OrderDimension.OrderMoney:
				{
                        InitialOrderMoney(mapChartDataModel, year, month);
					break;
				}
			}
			return mapChartDataModel;
		}

		public LineChartDataModel<float> GetDealConversionRateChart(long shopId, int year, int month)
		{
			LineChartDataModel<float> lineChartDataModel = new LineChartDataModel<float>()
			{
				SeriesData = new List<ChartSeries<float>>()
			};
            ChechData(year, month);
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (month != 1)
			{
				month--;
			}
			else
			{
				month = 12;
				year--;
			}
			DateTime dateTime2 = new DateTime(year, month, 1);
			DateTime dateTime3 = dateTime2.AddMonths(1);
			lineChartDataModel.XAxisData = GenerateStringByDays(31);
			var list = (
				from m in context.ShopVistiInfo
				where (m.Date >= dateTime) && m.ShopId == shopId && (m.Date < dateTime1)
				select new { VistiCounts = m.VistiCounts, SaleCounts = m.SaleCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => (s.VistiCounts == 0 ? 0f : s.SaleCounts / (float)s.VistiCounts)), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<float> chartSeries = new ChartSeries<float>()
			{
				Name = string.Format("{0}月成交转化率", dateTime.Month),
				Data = new float[31]
			};
			ChartSeries<float> chartSeries1 = chartSeries;
			for (int i = 0; i < 31; i++)
			{
				DateTime dateTime4 = dateTime.AddDays(i);
				if (list.Any((d) => {
					if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
					{
						return false;
					}
					return d.Day == dateTime4.Day;
				}))
				{
					chartSeries1.Data[i] = (float)Math.Round(list.FirstOrDefault((d) =>
                    {
                        if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
                        {
                            return false;
                        }
                        return d.Day == dateTime4.Day;
                    }).Count * 100f, 2);
				}
			}
			lineChartDataModel.SeriesData.Add(chartSeries1);
			var collection = (
				from m in context.ShopVistiInfo
				where (m.Date >= dateTime2) && (m.Date < dateTime3) && m.ShopId == shopId
				select new { VistiCounts = m.VistiCounts, SaleCounts = m.SaleCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => (s.VistiCounts == 0 ? 0f : s.SaleCounts / (float)s.VistiCounts)), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<float> chartSeries2 = new ChartSeries<float>()
			{
				Name = string.Format("{0}月成交转化率", dateTime2.Month),
				Data = new float[31]
			};
			chartSeries1 = chartSeries2;
			for (int j = 0; j < 31; j++)
			{
				DateTime dateTime5 = dateTime2.AddDays(j);
				if (collection.Any((d) => {
					if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
					{
						return false;
					}
					return d.Day == dateTime5.Day;
				}))
				{
					chartSeries1.Data[j] = (float)Math.Round(collection.FirstOrDefault((d) =>
                    {
                        if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
                        {
                            return false;
                        }
                        return d.Day == dateTime5.Day;
                    }).Count * 100f, 2);
				}
			}
			lineChartDataModel.SeriesData.Add(chartSeries1);
			return lineChartDataModel;
		}

		public LineChartDataModel<int> GetMemberChart(int year, int month, int weekIndex)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>();
			string[] strArrays = new string[] { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天" };
			lineChartDataModel.XAxisData = strArrays;
			lineChartDataModel.SeriesData = new List<ChartSeries<int>>();
			LineChartDataModel<int> lineChartDataModel1 = lineChartDataModel;
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, weekIndex);
			if (DateTime.MinValue.Equals(startDayOfWeeks))
			{
				throw new ArgumentException("参数错误");
			}
			DateTime dateTime = startDayOfWeeks.AddDays(6);
			var list = (
				from m in context.UserMemberInfo
				orderby m.CreateDate
				where (m.CreateDate >= startDayOfWeeks) && (m.CreateDate <= dateTime)
				group m by new { Year = m.CreateDate.Year, Month = m.CreateDate.Month, Day = m.CreateDate.Day } into G
				select new { Key = G.Key, Count = G.Count() }).ToList();
			ChartSeries<int> chartSeries = new ChartSeries<int>()
			{
				Name = "新增会员",
				Data = new int[7]
			};
			ChartSeries<int> count = chartSeries;
			for (int i = 0; i < 7; i++)
			{
				DateTime dateTime1 = startDayOfWeeks.AddDays(i);
				if (list.Any((d) => {
					if (d.Key.Year != dateTime1.Year || d.Key.Month != dateTime1.Month)
					{
						return false;
					}
					return d.Key.Day == dateTime1.Day;
				}))
				{
					count.Data[i] = list.FirstOrDefault((d) => {
						if (d.Key.Year != dateTime1.Year || d.Key.Month != dateTime1.Month)
						{
							return false;
						}
						return d.Key.Day == dateTime1.Day;
					}).Count;
				}
			}
			lineChartDataModel1.SeriesData.Add(count);
			return lineChartDataModel1;
		}

		public LineChartDataModel<int> GetMemberChart(int year, int month)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				SeriesData = new List<ChartSeries<int>>()
			};
            ChechData(year, month);
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (month != 1)
			{
				month--;
			}
			else
			{
				month = 12;
				year--;
			}
			DateTime dateTime2 = new DateTime(year, month, 1);
			DateTime dateTime3 = dateTime2.AddMonths(1);
			lineChartDataModel.XAxisData = GenerateStringByDays(31);
			var list = (
				from m in context.UserMemberInfo
				where (m.CreateDate >= dateTime) && (m.CreateDate < dateTime1)
				select new { Year = m.CreateDate.Year, Month = m.CreateDate.Month, Day = m.CreateDate.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Count(), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries = new ChartSeries<int>()
			{
				Name = string.Format("{0}月新增会员", dateTime.Month),
				Data = new int[31]
			};
			ChartSeries<int> count = chartSeries;
			for (int i = 0; i < 31; i++)
			{
				DateTime dateTime4 = dateTime.AddDays(i);
				if (list.Any((d) => {
					if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
					{
						return false;
					}
					return d.Day == dateTime4.Day;
				}))
				{
					count.Data[i] = list.FirstOrDefault((d) => {
						if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
						{
							return false;
						}
						return d.Day == dateTime4.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			var collection = (
				from m in context.UserMemberInfo
				where (m.CreateDate >= dateTime2) && (m.CreateDate < dateTime3)
				select new { Year = m.CreateDate.Year, Month = m.CreateDate.Month, Day = m.CreateDate.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Count(), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries1 = new ChartSeries<int>()
			{
				Name = string.Format("{0}月新增会员", dateTime2.Month),
				Data = new int[31]
			};
			count = chartSeries1;
			for (int j = 0; j < 31; j++)
			{
				DateTime dateTime5 = dateTime2.AddDays(j);
				if (collection.Any((d) => {
					if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
					{
						return false;
					}
					return d.Day == dateTime5.Day;
				}))
				{
					count.Data[j] = collection.FirstOrDefault((d) => {
						if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
						{
							return false;
						}
						return d.Day == dateTime5.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			return lineChartDataModel;
		}

		public LineChartDataModel<int> GetMemberChart(DateTime day)
		{
			throw new NotImplementedException();
		}

		public LineChartDataModel<int> GetNewsShopChart(int year, int month)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				SeriesData = new List<ChartSeries<int>>()
			};
            ChechData(year, month);
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (month != 1)
			{
				month--;
			}
			else
			{
				month = 12;
				year--;
			}
			DateTime dateTime2 = new DateTime(year, month, 1);
			DateTime dateTime3 = dateTime2.AddMonths(1);
			lineChartDataModel.XAxisData = GenerateStringByDays(31);
			var list = (
				from m in context.ShopInfo
				where (m.CreateDate >= dateTime) && (m.CreateDate < dateTime1) && (int)m.Stage.Value == 5
				select new { Year = m.CreateDate.Year, Month = m.CreateDate.Month, Day = m.CreateDate.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Count(), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries = new ChartSeries<int>()
			{
				Name = string.Format("{0}月新增会员", dateTime.Month),
				Data = new int[31]
			};
			ChartSeries<int> count = chartSeries;
			for (int i = 0; i < 31; i++)
			{
				DateTime dateTime4 = dateTime.AddDays(i);
				if (list.Any((d) => {
					if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
					{
						return false;
					}
					return d.Day == dateTime4.Day;
				}))
				{
					count.Data[i] = list.FirstOrDefault((d) => {
						if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
						{
							return false;
						}
						return d.Day == dateTime4.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			var collection = (
				from m in context.ShopInfo
				where (m.CreateDate >= dateTime2) && (m.CreateDate < dateTime3) && (int)m.Stage.Value == 5
				select new { Year = m.CreateDate.Year, Month = m.CreateDate.Month, Day = m.CreateDate.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Count(), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries1 = new ChartSeries<int>()
			{
				Name = string.Format("{0}月新增店铺", dateTime2.Month),
				Data = new int[31]
			};
			count = chartSeries1;
			for (int j = 0; j < 31; j++)
			{
				DateTime dateTime5 = dateTime2.AddDays(j);
				if (collection.Any((d) => {
					if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
					{
						return false;
					}
					return d.Day == dateTime5.Day;
				}))
				{
					count.Data[j] = collection.FirstOrDefault((d) => {
						if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
						{
							return false;
						}
						return d.Day == dateTime5.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			return lineChartDataModel;
		}

		public LineChartDataModel<int> GetProductSaleRankingChart(long shopId, DateTime day, SaleDimension dimension = (SaleDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = day;
			DateTime dateTime1 = dateTime.AddHours(24);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialProductSaleChartBySales(shopId, dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialProductSaleChartBySaleCount(shopId, dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetProductSaleRankingChart(long shopId, int year, int month, SaleDimension dimension =(SaleDimension) 1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialProductSaleChartBySales(shopId, dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialProductSaleChartBySaleCount(shopId, dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetProductSaleRankingChart(long shopId, int year, int month, int weekIndex, SaleDimension dimension = (SaleDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, weekIndex);
			if (DateTime.MinValue.Equals(startDayOfWeeks))
			{
				throw new ArgumentException("参数错误");
			}
			DateTime dateTime = startDayOfWeeks.AddDays(6);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialProductSaleChartBySales(shopId, startDayOfWeeks, dateTime, 15, str);
			}
			else
			{
                InitialProductSaleChartBySaleCount(shopId, startDayOfWeeks, dateTime, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetProductVisitRankingChart(long shopId, DateTime day, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = day;
			DateTime dateTime1 = dateTime.AddHours(24);
            InitialProductVisit(shopId, dateTime, dateTime1, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetProductVisitRankingChart(long shopId, int year, int month, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
            InitialProductVisit(shopId, dateTime, dateTime1, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetProductVisitRankingChart(long shopId, int year, int month, int weekIndex, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, weekIndex);
			if (DateTime.MinValue.Equals(startDayOfWeeks))
			{
				throw new ArgumentException("参数错误");
			}
			DateTime dateTime = startDayOfWeeks.AddDays(6);
            InitialProductVisit(shopId, startDayOfWeeks, dateTime, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetRecentMonthSaleRankChart()
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[15],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[15]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= 15; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime now = DateTime.Now;
            InitialSaleChartBySaleCount(now.AddMonths(-1), DateTime.Now, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetRecentMonthSaleRankChart(long shopId)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[15],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[15]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= 15; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime now = DateTime.Now;
            InitialProductSaleChartBySaleCount(shopId, now.AddMonths(-1), DateTime.Now, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetRecentMonthShopSaleRankChart()
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[15],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[15]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= 15; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime now = DateTime.Now;
            InitialShopChartByOrderCount(now.AddMonths(-1), DateTime.Now, 15, str);
			return str;
		}

		public LineChartDataModel<int> GetSaleRankingChart(int year, int month, SaleDimension dimension = (SaleDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialSaleChartBySales(dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialSaleChartBySaleCount(dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetSaleRankingChart(int year, int month, int weekIndex, SaleDimension dimension =(SaleDimension) 1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, weekIndex);
			if (DateTime.MinValue.Equals(startDayOfWeeks))
			{
				throw new ArgumentException("参数错误");
			}
			DateTime dateTime = startDayOfWeeks.AddDays(6);
			dateTime = dateTime.Date.AddDays(1);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialSaleChartBySales(startDayOfWeeks, dateTime, 15, str);
			}
			else
			{
                InitialSaleChartBySaleCount(startDayOfWeeks, dateTime, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetSaleRankingChart(DateTime day, SaleDimension dimension = (SaleDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = day;
			DateTime dateTime1 = dateTime.AddHours(24);
			if (dimension != SaleDimension.SaleCount)
			{
                InitialSaleChartBySales(dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialSaleChartBySaleCount(dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetShopFlowChart(long shopId, int year, int month)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				SeriesData = new List<ChartSeries<int>>()
			};
            ChechData(year, month);
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (month != 1)
			{
				month--;
			}
			else
			{
				month = 12;
				year--;
			}
			DateTime dateTime2 = new DateTime(year, month, 1);
			DateTime dateTime3 = dateTime2.AddMonths(1);
			lineChartDataModel.XAxisData = GenerateStringByDays(31);
			var list = (
				from m in context.ShopVistiInfo
				where (m.Date >= dateTime) && m.ShopId == shopId && (m.Date < dateTime1)
				select new { VistiCounts = m.VistiCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => s.VistiCounts), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries = new ChartSeries<int>()
			{
				Name = string.Format("{0}月店铺总流量", dateTime.Month),
				Data = new int[31]
			};
			ChartSeries<int> count = chartSeries;
			for (int i = 0; i < 31; i++)
			{
				DateTime dateTime4 = dateTime.AddDays(i);
				if (list.Any((d) => {
					if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
					{
						return false;
					}
					return d.Day == dateTime4.Day;
				}))
				{
					count.Data[i] = (int)list.FirstOrDefault((d) => {
						if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
						{
							return false;
						}
						return d.Day == dateTime4.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			var collection = (
				from m in context.ShopVistiInfo
				where (m.Date >= dateTime2) && m.ShopId == shopId && (m.Date < dateTime3)
				select new { VistiCounts = m.VistiCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => s.VistiCounts), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries1 = new ChartSeries<int>()
			{
				Name = string.Format("{0}月店铺总流量", dateTime2.Month),
				Data = new int[31]
			};
			count = chartSeries1;
			for (int j = 0; j < 31; j++)
			{
				DateTime dateTime5 = dateTime2.AddDays(j);
				if (collection.Any((d) => {
					if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
					{
						return false;
					}
					return d.Day == dateTime5.Day;
				}))
				{
					count.Data[j] = (int)collection.FirstOrDefault((d) => {
						if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
						{
							return false;
						}
						return d.Day == dateTime5.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			return lineChartDataModel;
		}

		public LineChartDataModel<int> GetShopRankingChart(int year, int month, ShopDimension dimension = (ShopDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (dimension != ShopDimension.OrderCount)
			{
                InitialShopChartBySales(dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialShopChartByOrderCount(dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetShopRankingChart(int year, int month, int weekIndex, ShopDimension dimension =(ShopDimension) 1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime startDayOfWeeks = DateTimeHelper.GetStartDayOfWeeks(year, month, weekIndex);
			if (DateTime.MinValue.Equals(startDayOfWeeks))
			{
				throw new ArgumentException("参数错误");
			}
			DateTime dateTime = startDayOfWeeks.AddDays(6);
			if (dimension != ShopDimension.OrderCount)
			{
                InitialShopChartBySales(startDayOfWeeks, dateTime, 15, str);
			}
			else
			{
                InitialShopChartByOrderCount(startDayOfWeeks, dateTime, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetShopRankingChart(DateTime day, ShopDimension dimension = (ShopDimension)1, int rankSize = 15)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				XAxisData = new string[rankSize],
				SeriesData = new List<ChartSeries<int>>(),
				ExpandProp = new string[rankSize]
			};
			LineChartDataModel<int> str = lineChartDataModel;
			for (int i = 1; i <= rankSize; i++)
			{
				str.XAxisData[i - 1] = i.ToString();
			}
			DateTime dateTime = day;
			DateTime dateTime1 = dateTime.AddHours(24);
			if (dimension != ShopDimension.OrderCount)
			{
                InitialShopChartBySales(dateTime, dateTime1, 15, str);
			}
			else
			{
                InitialShopChartByOrderCount(dateTime, dateTime1, 15, str);
			}
			return str;
		}

		public LineChartDataModel<int> GetShopSaleChart(long shopId, int year, int month)
		{
			LineChartDataModel<int> lineChartDataModel = new LineChartDataModel<int>()
			{
				SeriesData = new List<ChartSeries<int>>()
			};
            ChechData(year, month);
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			if (month != 1)
			{
				month--;
			}
			else
			{
				month = 12;
				year--;
			}
			DateTime dateTime2 = new DateTime(year, month, 1);
			DateTime dateTime3 = dateTime2.AddMonths(1);
			lineChartDataModel.XAxisData = GenerateStringByDays(31);
			var list = (
				from sv in context.ShopVistiInfo
				where (sv.Date >= dateTime) && sv.ShopId == shopId && (sv.Date < dateTime1)
				select sv into m
				select new { SaleCounts = m.SaleCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => s.SaleCounts), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries = new ChartSeries<int>()
			{
				Name = string.Format("{0}月店铺总销量", dateTime.Month),
				Data = new int[31]
			};
			ChartSeries<int> count = chartSeries;
			for (int i = 0; i < 31; i++)
			{
				DateTime dateTime4 = dateTime.AddDays(i);
				if (list.Any((d) => {
					if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
					{
						return false;
					}
					return d.Day == dateTime4.Day;
				}))
				{
					count.Data[i] = (int)list.FirstOrDefault((d) => {
						if (d.Year != dateTime4.Year || d.Month != dateTime4.Month)
						{
							return false;
						}
						return d.Day == dateTime4.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			var collection = (
				from sv in context.ShopVistiInfo
				where (sv.Date >= dateTime2) && sv.ShopId == shopId && (sv.Date < dateTime3)
				select sv into m
				select new { SaleCounts = m.SaleCounts, Year = m.Date.Year, Month = m.Date.Month, Day = m.Date.Day } into m
				group m by new { Year = m.Year, Month = m.Month, Day = m.Day } into g
				select new { Count = g.Sum((s) => s.SaleCounts), Year = g.Key.Year, Month = g.Key.Month, Day = g.Key.Day }).ToList();
			ChartSeries<int> chartSeries1 = new ChartSeries<int>()
			{
				Name = string.Format("{0}月店铺总销量", dateTime2.Month),
				Data = new int[31]
			};
			count = chartSeries1;
			for (int j = 0; j < 31; j++)
			{
				DateTime dateTime5 = dateTime2.AddDays(j);
				if (collection.Any((d) => {
					if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
					{
						return false;
					}
					return d.Day == dateTime5.Day;
				}))
				{
					count.Data[j] = (int)collection.FirstOrDefault((d) => {
						if (d.Year != dateTime5.Year || d.Month != dateTime5.Month)
						{
							return false;
						}
						return d.Day == dateTime5.Day;
					}).Count;
				}
			}
			lineChartDataModel.SeriesData.Add(count);
			return lineChartDataModel;
		}

		private void InitialOrderCount(MapChartDataModel model, int year, int month)
		{
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			var list = (
				from o in context.OrderInfo
				orderby o.OrderDate
				where (o.OrderDate >= dateTime) && (o.OrderDate < dateTime1)
				select new { TopRegionId = o.TopRegionId } into t
				group t by t.TopRegionId into G
				select new { Name = G.Key, Count = G.Count() }).ToList();
			if (list != null && list.Count() > 0)
			{
				model.RangeMin = list.Min((t) => t.Count);
				model.RangeMax = list.Max((t) => t.Count);
				MapChartSeries mapChartSeries = new MapChartSeries()
				{
					Name = "下单量",
					Data = new MapChartSeriesData[list.Count()]
				};
				model.Series = mapChartSeries;
				int num = 0;
				foreach (var variable in list.ToList())
				{
					model.Series.Data[num] = new MapChartSeriesData();
					model.Series.Data[num].name = variable.Name.ToString();
					int num1 = num;
					num = num1 + 1;
					model.Series.Data[num1].@value = variable.Count;
				}
			}
		}

		private void InitialOrderMemberCount(MapChartDataModel model, int year, int month)
		{
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			var list = (
				from t in (
					from m in context.UserMemberInfo
					join o in context.OrderInfo on m.Id equals o.UserId
					orderby o.OrderDate
					where (o.OrderDate >= dateTime) && (o.OrderDate < dateTime1)
					select new { UserId = m.Id, TopRegionId = m.TopRegionId }).Distinct()
				group t by new { TopRegionId = t.TopRegionId } into G
				select new { Name = G.Key.TopRegionId, Count = G.Count() }).ToList();
			if (list != null && list.Count() > 0)
			{
				model.RangeMin = list.Min((t) => t.Count);
				model.RangeMax = list.Max((t) => t.Count);
				if (model.RangeMax == model.RangeMin)
				{
					model.RangeMin = 0;
				}
				MapChartSeries mapChartSeries = new MapChartSeries()
				{
					Name = "下单客户数",
					Data = new MapChartSeriesData[list.Count()]
				};
				model.Series = mapChartSeries;
				int num = 0;
				foreach (var variable in list.ToList())
				{
					model.Series.Data[num] = new MapChartSeriesData();
					model.Series.Data[num].name = variable.Name.ToString();
					int num1 = num;
					num = num1 + 1;
					model.Series.Data[num1].@value = variable.Count;
				}
			}
		}

		private void InitialOrderMoney(MapChartDataModel model, int year, int month)
		{
			DateTime dateTime = new DateTime(year, month, 1);
			DateTime dateTime1 = dateTime.AddMonths(1);
			IQueryable<decimal> orderInfo = 
				from i in context.OrderInfo
				select i.OrderTotalAmount;
			var list = (
				from o in context.OrderInfo
				orderby o.OrderDate
				where (o.OrderDate >= dateTime) && (o.OrderDate < dateTime1)
				select new { TopRegionId = o.TopRegionId, OrderTotalMoney = o.ProductTotalAmount + o.Freight + o.Tax } into t
				group t by t.TopRegionId into G
				select new { Name = G.Key, Money = G.Sum((g) => g.OrderTotalMoney) }).ToList();
			if (list != null && list.Count() > 0)
			{
				model.RangeMin = (int)list.Min((t) => t.Money);
				model.RangeMax = (int)list.Max((t) => t.Money);
				MapChartSeries mapChartSeries = new MapChartSeries()
				{
					Name = "下单金额",
					Data = new MapChartSeriesData[list.Count()]
				};
				model.Series = mapChartSeries;
				int num = 0;
				foreach (var variable in list.ToList())
				{
					model.Series.Data[num] = new MapChartSeriesData();
					model.Series.Data[num].name = variable.Name.ToString();
					int num1 = num;
					num = num1 + 1;
					model.Series.Data[num1].@value = variable.Money;
				}
			}
		}

		private void InitialProductSaleChartBySaleCount(long shopId, DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from pv in context.ProductVistiInfo
					join p in context.ProductInfo on pv.ProductId equals p.Id
					join s in context.ShopInfo on p.ShopId equals s.Id
					where (pv.Date >= start) && (pv.Date < end) && s.Id == shopId && (int)p.SaleStatus != 4
					select new { Date = pv.Date, Count = pv.SaleCounts, ProductId = pv.ProductId, ProductName = p.ProductName }).ToList()
				orderby t.Count
				group t by new { ProductId = t.ProductId } into G
				select new { ProductName = G.FirstOrDefault().ProductName, Count = G.Sum((t) => t.Count) } into x
				orderby x.Count descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize]
				};
				string[] str = new string[] { "月销售量Top", rankSize.ToString(), "      ", start.ToString("yyyy-MM-dd"), "至", end.ToString("yyyy-MM-dd") };
				chartSeries.Name = string.Concat(str);
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize]
			};
			string[] strArrays = new string[] { "月销售量Top", rankSize.ToString(), "      ", start.ToString("yyyy-MM-dd"), "至", end.ToString("yyyy-MM-dd") };
			chartSeries2.Name = string.Concat(strArrays);
			ChartSeries<int> count = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				count.Data[num] = (int)variable.Count;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ProductName;
			}
			model.SeriesData.Add(count);
		}

		private void InitialProductSaleChartBySales(long shopId, DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from pv in context.ProductVistiInfo
					join p in context.ProductInfo on pv.ProductId equals p.Id
					join s in context.ShopInfo on p.ShopId equals s.Id
					where (pv.Date >= start) && (pv.Date < end) && s.Id == shopId && (int)p.SaleStatus != 4
					select new { Date = pv.Date, ProductId = pv.ProductId, ProductName = p.ProductName, RealTotalPrice = pv.SaleAmounts }).ToList()
				orderby t.Date
				group t by new { ProductId = t.ProductId, ProductName = t.ProductName } into G
				select new { ProductName = G.FirstOrDefault().ProductName, Money = G.Sum((t) => t.RealTotalPrice) } into x
				orderby x.Money descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("商品销售额排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("商品销售额排行Top", rankSize.ToString())
			};
			ChartSeries<int> money = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				money.Data[num] = (int)variable.Money;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ProductName;
			}
			model.SeriesData.Add(money);
		}

		private void InitialProductVisit(long shopId, DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from s in context.ShopInfo
					join p in context.ProductInfo on s.Id equals p.ShopId
					join pv in context.ProductVistiInfo on p.Id equals pv.ProductId
					where (pv.Date >= start) && (pv.Date < end) && s.Id == shopId && (int)p.SaleStatus != 4
					select new { Date = pv.Date, Count = pv.VistiCounts, ProductId = p.Id, ProductName = p.ProductName }).ToList()
				orderby t.Date
				group t by new { Date = t.Date, ProductName = t.ProductName, ProductId = t.ProductId } into G
				select new { ProductName = G.FirstOrDefault().ProductName, Visit = G.Sum((t) => t.Count) } into x
				orderby x.Visit descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("商品浏览量排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("商品浏览量排行Top", rankSize.ToString())
			};
			ChartSeries<int> visit = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				visit.Data[num] = (int)variable.Visit;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ProductName;
			}
			model.SeriesData.Add(visit);
		}

		private void InitialSaleChartBySaleCount(DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from o in context.OrderInfo
					join oi in context.OrderItemInfo on o.Id equals oi.OrderId
					where (o.OrderDate >= start) && (o.OrderDate < end) && (int)o.OrderStatus == 5
					select new { OrderDate = o.OrderDate, ProductId = oi.ProductId, ProductName = oi.ProductName, Quantity = oi.Quantity - oi.ReturnQuantity }).ToList()
				orderby t.OrderDate
				group t by new { ProductId = t.ProductId, ProductName = t.ProductName } into G
				select new { ProductName = G.FirstOrDefault().ProductName, Count = G.Sum((t) => t.Quantity) } into x
				orderby x.Count descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("商品销售量排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("商品销售量排行Top", rankSize.ToString())
			};
			ChartSeries<int> count = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				count.Data[num] = (int)variable.Count;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ProductName;
			}
			model.SeriesData.Add(count);
		}

		private void InitialSaleChartBySales(DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from pv in context.ProductVistiInfo
					join p in context.ProductInfo on pv.ProductId equals p.Id
					where (pv.Date >= start) && (pv.Date < end) && (int)p.SaleStatus != 4
					select new { Date = pv.Date, ProductId = pv.ProductId, ProductName = p.ProductName, RealTotalPrice = pv.SaleAmounts }).ToList()
				orderby t.Date
				group t by new { ProductId = t.ProductId, ProductName = t.ProductName } into G
				select new { ProductId = G.FirstOrDefault().ProductId, ProductName = G.FirstOrDefault().ProductName, Money = G.Sum((t) => t.RealTotalPrice) } into pm
				join r in 
					from rm in (
						from item in context.OrderItemInfo
						join itemrefund in context.OrderRefundInfo on item.Id equals itemrefund.OrderItemId
						where (int)itemrefund.ManagerConfirmStatus == 7 && (itemrefund.ManagerConfirmDate >= start) && (itemrefund.ManagerConfirmDate < end)
						select new { ProductId = item.ProductId, Amount = itemrefund.Amount }).ToList()
					group rm by new { ProductId = rm.ProductId } into R
					select new { ProductId = R.FirstOrDefault().ProductId, RefundMoney = R.Sum((money) => money.Amount) } on pm.ProductId equals r.ProductId into ps
				from pd in ps.DefaultIfEmpty()
				select new { ProductName = pm.ProductName, Money = ((pm.Money - (pd == null ? new decimal(0) : pd.RefundMoney)) < new decimal(0) ? new decimal(0) : pm.Money - (pd == null ? new decimal(0) : pd.RefundMoney)) } into x
				orderby x.Money descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("商品销售额排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("商品销售额排行Top", rankSize.ToString())
			};
			ChartSeries<int> chartSeries3 = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				chartSeries3.Data[num] = (int)variable.Money;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ProductName;
			}
			model.SeriesData.Add(chartSeries3);
		}

		private void InitialShopChartByOrderCount(DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from o in context.OrderInfo
					join s in context.ShopInfo on o.ShopId equals s.Id
					where (o.OrderDate >= start) && (o.OrderDate < end)
					select new { OrderDate = o.OrderDate, ShopName = s.ShopName }).ToList()
				orderby t.OrderDate
				group t by new { ShopName = t.ShopName } into G
				select new { ShopName = G.FirstOrDefault().ShopName, Count = G.Count() } into x
				orderby x.Count descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("店铺订单量排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("店铺订单量排行Top", rankSize.ToString())
			};
			ChartSeries<int> count = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				count.Data[num] = variable.Count;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ShopName;
			}
			model.SeriesData.Add(count);
		}

		private void InitialShopChartBySales(DateTime start, DateTime end, int rankSize, LineChartDataModel<int> model)
		{
			int num = 0;
			var list = (
				from t in (
					from sv in context.ShopVistiInfo
					join s in context.ShopInfo on sv.ShopId equals s.Id
					where (sv.Date >= start) && (sv.Date < end)
					select new { Date = sv.Date, ShopName = s.ShopName, OrderMoney = sv.SaleAmounts }).ToList()
				orderby t.Date
				group t by new { ShopName = t.ShopName } into G
				select new { ShopName = G.FirstOrDefault().ShopName, Money = G.Sum((g) => g.OrderMoney) } into x
				orderby x.Money descending
				select x).ToList();
			if (list == null || 0 >= list.Count())
			{
				ChartSeries<int> chartSeries = new ChartSeries<int>()
				{
					Data = new int[rankSize],
					Name = string.Concat("店铺销售额排行Top", rankSize.ToString())
				};
				ChartSeries<int> chartSeries1 = chartSeries;
				for (int i = 0; i < rankSize; i++)
				{
					chartSeries1.Data[num] = 0;
					int num1 = num;
					num = num1 + 1;
					model.ExpandProp[num1] = "";
				}
				model.SeriesData.Add(chartSeries1);
				return;
			}
			ChartSeries<int> chartSeries2 = new ChartSeries<int>()
			{
				Data = new int[rankSize],
				Name = string.Concat("店铺销售额排行Top", rankSize.ToString())
			};
			ChartSeries<int> money = chartSeries2;
			foreach (var variable in list.Take(rankSize))
			{
				money.Data[num] = (int)variable.Money;
				int num2 = num;
				num = num2 + 1;
				model.ExpandProp[num2] = variable.ShopName;
			}
			model.SeriesData.Add(money);
		}
	}
}