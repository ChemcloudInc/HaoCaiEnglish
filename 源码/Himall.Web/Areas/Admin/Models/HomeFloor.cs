using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class HomeFloor
	{
		public long DisplaySequence
		{
			get;
			set;
		}

		public bool Enable
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public long StyleLevel
		{
			get;
			set;
		}

		public HomeFloor()
		{
		}
	}
}