using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class Element
	{
		public int[] a
		{
			get;
			set;
		}

		public int[] b
		{
			get;
			set;
		}

		public string key
		{
			get;
			set;
		}

		public bool selected
		{
			get;
			set;
		}

		public string @value
		{
			get;
			set;
		}

		public Element()
		{
		}
	}
}