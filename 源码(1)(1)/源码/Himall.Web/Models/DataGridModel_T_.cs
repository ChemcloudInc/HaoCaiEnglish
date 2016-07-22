using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class DataGridModel<T>
	{
		public IEnumerable<T> rows
		{
			get;
			set;
		}

		public int total
		{
			get;
			set;
		}

		public DataGridModel()
		{
		}
	}
}