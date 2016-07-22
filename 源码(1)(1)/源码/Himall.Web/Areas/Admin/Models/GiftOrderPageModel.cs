using Himall.Core;
using Himall.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class GiftOrderPageModel
	{
		public string Address
		{
			get;
			set;
		}

		public string CellPhone
		{
			get;
			set;
		}

		public string CloseReason
		{
			get;
			set;
		}

		public string ExpressCompanyName
		{
			get;
			set;
		}

		public DateTime? FinishDate
		{
			get;
			set;
		}

		public int FirstGiftBuyQuantity
		{
			get;
			set;
		}

		public long FirstGiftId
		{
			get;
			set;
		}

		public string FirstGiftName
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public DateTime OrderDate
		{
			get;
			set;
		}

		public GiftOrderInfo.GiftOrderStatus OrderStatus
		{
			get;
			set;
		}

		public string RegionFullName
		{
			get;
			set;
		}

		public int? RegionId
		{
			get;
			set;
		}

		public string ShipOrderNumber
		{
			get;
			set;
		}

		public DateTime? ShippingDate
		{
			get;
			set;
		}

		public string ShipTo
		{
			get;
			set;
		}

		[NotMapped]
		public string ShowOrderStatus
		{
			get
			{
				return OrderStatus.ToDescription();
			}
		}

		public int? TopRegionId
		{
			get;
			set;
		}

		public int? TotalIntegral
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

		public string UserRemark
		{
			get;
			set;
		}

		public GiftOrderPageModel()
		{
		}
	}
}