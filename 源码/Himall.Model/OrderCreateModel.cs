using Himall.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderCreateModel
	{
		public IEnumerable<long> CartItemIds
		{
			get;
			set;
		}

		public IEnumerable<long> CollPids
		{
			get;
			set;
		}

		public IEnumerable<int> Counts
		{
			get;
			set;
		}

		public IEnumerable<long> CouponIds
		{
			get;
			set;
		}

		public IEnumerable<string[]> CouponIdsStr
		{
			get;
			set;
		}

		[JsonIgnore]
		public UserMemberInfo CurrentUser
		{
			get;
			set;
		}

		public int Integral
		{
			get;
			set;
		}

		public InvoiceType Invoice
		{
			get;
			set;
		}

		public string InvoiceContext
		{
			get;
			set;
		}

		public string InvoiceTitle
		{
			get;
			set;
		}

		public bool IslimitBuy
		{
			get;
			set;
		}

		public Himall.Core.PlatformType PlatformType
		{
			get;
			set;
		}

		public long ReceiveAddressId
		{
			get;
			set;
		}

		public IEnumerable<string> SkuIds
		{
			get;
			set;
		}

		public OrderCreateModel()
		{
            PlatformType = Himall.Core.PlatformType.PC;
		}
	}
}