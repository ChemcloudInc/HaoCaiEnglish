using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class OrderModel
	{
		public int orderCount
		{
			get;
			set;
		}

		public ProductInfo productInfo
		{
			get;
			set;
		}

		public OrderModel()
		{
		}
	}
}