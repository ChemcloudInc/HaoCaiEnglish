using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model.Models
{
	public class WXCategory
	{
		public List<WXCategory> Child
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public WXCategory()
		{
            Child = new List<WXCategory>();
		}
	}
}