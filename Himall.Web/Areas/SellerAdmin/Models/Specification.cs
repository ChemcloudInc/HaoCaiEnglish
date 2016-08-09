using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class Specification
	{
		public string Id
		{
			get;
			set;
		}

		public bool isPlatform
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool Selected
		{
			get;
			set;
		}

		public Specification()
		{
		}
	}
}