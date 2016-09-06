using Himall.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class GiftOrderInfo : BaseModel
	{
		private long _id;

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

		public virtual ICollection<GiftOrderItemInfo> Himall_GiftOrderItem
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
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
		public string ShowExpressCompanyName
		{
			get
			{
				string expressCompanyName = ExpressCompanyName;
				if (expressCompanyName == "-1")
				{
					expressCompanyName = "其他";
				}
				return expressCompanyName;
			}
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

		public GiftOrderInfo()
		{
            Himall_GiftOrderItem = new HashSet<GiftOrderItemInfo>();
		}

		public enum GiftOrderStatus
		{
            [Description("WaitDelivery")]
			WaitDelivery = 2,
            [Description("WaitReceiving")]
			WaitReceiving = 3,
            [Description("Finished")]
			Finish = 5
		}
	}
}