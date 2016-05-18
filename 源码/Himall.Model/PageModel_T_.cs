using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class PageModel<T>
	{
		public IQueryable<T> Models
		{
			get;
			set;
		}

		public int Total
		{
			get;
			set;
		}

		public PageModel()
		{
		}
	}
}