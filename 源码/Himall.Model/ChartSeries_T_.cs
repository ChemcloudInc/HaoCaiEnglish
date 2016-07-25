using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ChartSeries<T>
	where T : struct
	{
		public T[] Data
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public ChartSeries()
		{
		}
	}
}