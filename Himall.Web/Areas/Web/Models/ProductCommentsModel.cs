using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductCommentsModel
	{
		public string content
		{
			get;
			set;
		}

		public int star
		{
			get;
			set;
		}

		public int subOrderId
		{
			get;
			set;
		}

		public ProductCommentsModel()
		{
		}
	}
}