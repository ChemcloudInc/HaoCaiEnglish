using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ExpressData
	{
		public IEnumerable<ExpressDataItem> ExpressDataItems
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public ExpressData()
		{
            ExpressDataItems = new ExpressDataItem[0];
		}
	}
}