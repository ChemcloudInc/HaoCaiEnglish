using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class Specifications
	{
		public long Id
		{
			get;
			set;
		}

		public bool isPlatform
		{
			get;
			set;
		}

		public string newValue
		{
			get;
			set;
		}

		public string oldValue
		{
			get;
			set;
		}

		public bool selected
		{
			get;
			set;
		}

		public Specifications()
		{
		}
	}
}