using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class OrderModel
	{
		public string IconSrc
		{
			get;
			set;
		}

		public string OrderDate
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public string OrderStatus
		{
			get;
			set;
		}

		public string PaymentTypeName
		{
			get;
			set;
		}

		public int PlatForm
		{
			get;
			set;
		}

		public string PlatformText
		{
			get;
			set;
		}

		public int? RefundStats
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public decimal TotalPrice
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public OrderModel()
		{
		}
	}
}