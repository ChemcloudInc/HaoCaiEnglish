using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MapChartDataModel
	{
		public int RangeMax
		{
			get;
			set;
		}

		public int RangeMin
		{
			get;
			set;
		}

		public MapChartSeries Series
		{
			get;
			set;
		}

		public MapChartDataModel()
		{
		}
	}
}