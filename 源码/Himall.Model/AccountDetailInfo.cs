using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AccountDetailInfo : BaseModel
	{
		private long _id;

		public long AccountId
		{
			get;
			set;
		}

		public decimal CommissionAmount
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public decimal FreightAmount
		{
			get;
			set;
		}

		public virtual AccountInfo Himall_Accounts
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

		public long OrderId
		{
			get;
			set;
		}

		public string OrderRefundsDates
		{
			get;
			set;
		}

		public AccountDetailInfo.EnumOrderType OrderType
		{
			get;
			set;
		}

		public decimal ProductActualPaidAmount
		{
			get;
			set;
		}

		public decimal RefundCommisAmount
		{
			get;
			set;
		}

		public decimal RefundTotalAmount
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public AccountDetailInfo()
		{
		}

		public enum EnumOrderType
		{
			[Description("退单列表")]
			ReturnOrder,
			[Description("订单列表")]
			FinishedOrder
		}
	}
}