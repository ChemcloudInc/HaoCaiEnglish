using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class LineChartDataModel<T>
	where T : struct
	{
		public string[] ExpandProp
		{
			get;
			set;
		}

		public IList<ChartSeries<T>> SeriesData
		{
			get;
			set;
		}

		public string[] XAxisData
		{
			get;
			set;
		}

		public LineChartDataModel()
		{
		}
	}
}