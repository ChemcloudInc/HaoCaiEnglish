using System;
using System.Runtime.CompilerServices;

namespace System.Web.Mvc
{
	public class PagingInfo
	{
		public int CurrentPage
		{
			get;
			set;
		}

		public int ItemsPerPage
		{
			get;
			set;
		}

		public int TotalItems
		{
			get;
			set;
		}

		public int TotalPages
		{
			get
			{
				return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
			}
		}

		public PagingInfo()
		{
		}
	}
}