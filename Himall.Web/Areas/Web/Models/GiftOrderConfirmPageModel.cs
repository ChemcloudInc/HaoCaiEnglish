using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class GiftOrderConfirmPageModel
	{
		public List<GiftOrderItemInfo> GiftList
		{
			get;
			set;
		}

		public decimal GiftValueTotal
		{
			get;
			set;
		}

		public ShippingAddressInfo ShipAddress
		{
			get;
			set;
		}

		public int TotalAmount
		{
			get;
			set;
		}

		public GiftOrderConfirmPageModel()
		{
		}
	}
}