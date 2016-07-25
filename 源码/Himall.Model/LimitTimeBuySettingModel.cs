using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class LimitTimeBuySettingModel
	{
		public decimal Price
		{
			get;
			set;
		}

		public int ReviceDays
		{
			get;
			set;
		}

		public LimitTimeBuySettingModel()
		{
		}
	}
}