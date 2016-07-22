using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class AccountModel
	{
		public string AccountDate
		{
			get;
			set;
		}

		public decimal AdvancePaymentAmount
		{
			get;
			set;
		}

		public decimal CommissionAmount
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public decimal FreightAmount
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public decimal PeriodSettlement
		{
			get;
			set;
		}

		public decimal ProductActualPaidAmount
		{
			get;
			set;
		}

		public decimal RefundAmount
		{
			get;
			set;
		}

		public decimal RefundCommissionAmount
		{
			get;
			set;
		}

		public string Remark
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

		public DateTime StartDate
		{
			get;
			set;
		}

		public int Status
		{
			get;
			set;
		}

		public string TimeSlot
		{
			get;
			set;
		}

		public AccountModel()
		{
		}
	}
}