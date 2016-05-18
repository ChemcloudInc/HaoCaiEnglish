using Himall.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderRefundInfo : BaseModel
	{
		private long _id;

		public decimal Amount
		{
			get;
			set;
		}

		public string Applicant
		{
			get;
			set;
		}

		public DateTime ApplyDate
		{
			get;
			set;
		}

		public DateTime? BuyerDeliverDate
		{
			get;
			set;
		}

		public string ContactCellPhone
		{
			get;
			set;
		}

		public string ContactPerson
		{
			get;
			set;
		}

		public decimal EnabledRefundAmount
		{
			get;
			set;
		}

		public string ExpressCompanyName
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

		public bool IsReturn
		{
			get;
			set;
		}

		public DateTime ManagerConfirmDate
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundConfirmStatus ManagerConfirmStatus
		{
			get;
			set;
		}

		public string ManagerRemark
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public long OrderItemId
		{
			get;
			set;
		}

		public virtual Himall.Model.OrderItemInfo OrderItemInfo
		{
			get;
			set;
		}

		public string Payee
		{
			get;
			set;
		}

		public string PayeeAccount
		{
			get;
			set;
		}

		public string Reason
		{
			get;
			set;
		}

		public string RefundAccount
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundMode RefundMode
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundPayStatus? RefundPayStatus
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundPayType? RefundPayType
		{
			get;
			set;
		}

		public string RefundStatus
		{
			get
			{
				string description = "";
				description = SellerAuditStatus.ToDescription();
				if (SellerAuditStatus == OrderRefundInfo.OrderRefundAuditStatus.Audited)
				{
					description = ManagerConfirmStatus.ToDescription();
				}
				return description;
			}
		}

		[NotMapped]
		public int RefundType
		{
			get;
			set;
		}

		[NotMapped]
		public int ReturnQuantity
		{
			get;
			set;
		}

		public DateTime SellerAuditDate
		{
			get;
			set;
		}

		public OrderRefundInfo.OrderRefundAuditStatus SellerAuditStatus
		{
			get;
			set;
		}

		public DateTime? SellerConfirmArrivalDate
		{
			get;
			set;
		}

		public string SellerRemark
		{
			get;
			set;
		}

		public string ShipOrderNumber
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

		public long UserId
		{
			get;
			set;
		}

		public OrderRefundInfo()
		{
		}

		public enum OrderRefundAuditStatus
		{
			[Description("待商家审核")]
			WaitAudit = 1,
			[Description("待买家寄货")]
			WaitDelivery = 2,
			[Description("待商家收货")]
			WaitReceiving = 3,
			[Description("商家拒绝")]
			UnAudit = 4,
			[Description("商家通过审核")]
			Audited = 5
		}

		public enum OrderRefundConfirmStatus
		{
			[Description("待平台确认")]
			UnConfirm = 6,
			[Description("退款成功")]
			Confirmed = 7
		}

		public enum OrderRefundMode
		{
			[Description("订单退款")]
			OrderRefund = 1,
			[Description("货品退款")]
			OrderItemRefund = 2,
			[Description("退货退款")]
			ReturnGoodsRefund = 3
		}

		public enum OrderRefundPayStatus
		{
			[Description("支付成功")]
			PaySuccess = 1,
			[Description("支付失败")]
			PayFail = 2
		}

		public enum OrderRefundPayType
		{
			[Description("原路返回")]
			BackOut = 1,
			[Description("线下支付")]
			OffLine = 2,
			[Description("退到预付款")]
			BackCapital = 3
		}
	}
}