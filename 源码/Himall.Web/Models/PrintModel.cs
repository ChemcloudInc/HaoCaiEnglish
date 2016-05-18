using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class PrintModel
	{
		public IEnumerable<PrintModel.PrintElement> Elements
		{
			get;
			set;
		}

		public int FontSize
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public int Width
		{
			get;
			set;
		}

		public PrintModel()
		{
		}

		public class PrintElement
		{
			public int Height
			{
				get;
				set;
			}

			public string Value
			{
				get;
				set;
			}

			public int Width
			{
				get;
				set;
			}

			public int X
			{
				get;
				set;
			}

			public int Y
			{
				get;
				set;
			}

			public PrintElement()
			{
			}
		}
	}
}