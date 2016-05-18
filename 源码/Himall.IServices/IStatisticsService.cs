using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IStatisticsService : IService, IDisposable
	{
		MapChartDataModel GetAreaOrderChart(OrderDimension dimension, int year, int month);

		LineChartDataModel<float> GetDealConversionRateChart(long shopId, int year, int month);

		LineChartDataModel<int> GetMemberChart(int year, int month, int weekIndex);

		LineChartDataModel<int> GetMemberChart(int year, int month);

		LineChartDataModel<int> GetMemberChart(DateTime day);

		LineChartDataModel<int> GetNewsShopChart(int year, int month);

		LineChartDataModel<int> GetProductSaleRankingChart(long shopId, DateTime day, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetProductSaleRankingChart(long shopId, int year, int month, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetProductSaleRankingChart(long shopId, int year, int month, int weekIndex, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetProductVisitRankingChart(long shopId, DateTime day, int rankSize = 15);

		LineChartDataModel<int> GetProductVisitRankingChart(long shopId, int year, int month, int rankSize = 15);

		LineChartDataModel<int> GetProductVisitRankingChart(long shopId, int year, int month, int weekIndex, int rankSize = 15);

		LineChartDataModel<int> GetRecentMonthSaleRankChart();

		LineChartDataModel<int> GetRecentMonthSaleRankChart(long shopId);

		LineChartDataModel<int> GetRecentMonthShopSaleRankChart();

		LineChartDataModel<int> GetSaleRankingChart(int year, int month, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetSaleRankingChart(int year, int month, int weekIndex, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetSaleRankingChart(DateTime day, SaleDimension dimension = (SaleDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetShopFlowChart(long shopId, int year, int month);

		LineChartDataModel<int> GetShopRankingChart(int year, int month, ShopDimension dimension = (ShopDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetShopRankingChart(int year, int month, int weekIndex, ShopDimension dimension = (ShopDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetShopRankingChart(DateTime day, ShopDimension dimension = (ShopDimension)1, int rankSize = 15);

		LineChartDataModel<int> GetShopSaleChart(long shopId, int year, int month);
	}
}