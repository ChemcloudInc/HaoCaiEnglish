using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class ChartDataViewModel
	{
		public List<SeriesViewModel> SeriesData
		{
			get;
			set;
		}

		public string xAxis
		{
			get;
			set;
		}

		public ChartDataViewModel()
		{
		}

		public ChartDataViewModel(LineChartDataModel<int> chart) : this()
		{
            SeriesData = new List<SeriesViewModel>();
			string[] xAxisData = chart.XAxisData;
			for (int i = 0; i < xAxisData.Length; i++)
			{
				string str = xAxisData[i];
				ChartDataViewModel chartDataViewModel = this;
				chartDataViewModel.xAxis = string.Concat(chartDataViewModel.xAxis, "'", str, "',");
			}
			foreach (ChartSeries<int> seriesDatum in chart.SeriesData)
			{
				SeriesViewModel seriesViewModel = new SeriesViewModel()
				{
					Name = seriesDatum.Name,
					SeriesData = ""
				};
				SeriesViewModel seriesViewModel1 = seriesViewModel;
				int[] data = seriesDatum.Data;
				for (int j = 0; j < data.Length; j++)
				{
					int num = data[j];
					SeriesViewModel seriesViewModel2 = seriesViewModel1;
					seriesViewModel2.SeriesData = string.Concat(seriesViewModel2.SeriesData, num.ToString(), ",");
				}
                SeriesData.Add(seriesViewModel1);
			}
		}

		public ChartDataViewModel(LineChartDataModel<float> chart) : this()
		{
            SeriesData = new List<SeriesViewModel>();
			string[] xAxisData = chart.XAxisData;
			for (int i = 0; i < xAxisData.Length; i++)
			{
				string str = xAxisData[i];
				ChartDataViewModel chartDataViewModel = this;
				chartDataViewModel.xAxis = string.Concat(chartDataViewModel.xAxis, "'", str, "',");
			}
			foreach (ChartSeries<float> seriesDatum in chart.SeriesData)
			{
				SeriesViewModel seriesViewModel = new SeriesViewModel()
				{
					Name = seriesDatum.Name,
					SeriesData = ""
				};
				SeriesViewModel seriesViewModel1 = seriesViewModel;
				float[] data = seriesDatum.Data;
				for (int j = 0; j < data.Length; j++)
				{
					float single = data[j];
					SeriesViewModel seriesViewModel2 = seriesViewModel1;
					seriesViewModel2.SeriesData = string.Concat(seriesViewModel2.SeriesData, single.ToString(), ",");
				}
                SeriesData.Add(seriesViewModel1);
			}
		}
	}
}