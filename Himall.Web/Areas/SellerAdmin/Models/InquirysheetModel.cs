using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class InquirysheetModel
	{
		public IList<InquirysheetModel.Product> products;

		public string Address
		{
			get;
			set;
		}

		public string ContainTax
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string InvoiceName
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Phone
		{
			get;
			set;
		}

		public string PublishDate
		{
			get;
			set;
		}

		public string QualificationRequirement
		{
			get;
			set;
		}

		public string QuotationEndDate
		{
			get;
			set;
		}

		public string ReceivingName
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public string ShippingEndDate
		{
			get;
			set;
		}

		public int State
		{
			get;
			set;
		}

		public string StateName
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public decimal TotalPrice
		{
			get;
			set;
		}

		public InquirysheetModel()
		{
            products = new List<InquirysheetModel.Product>();
		}

		public class Product
		{
			public string Annex
			{
				get;
				set;
			}

			public long Id
			{
				get;
				set;
			}

			public string Industry
			{
				get;
				set;
			}

			public decimal Price
			{
				get;
				set;
			}

			public string ProductName
			{
				get;
				set;
			}

			public string Quantity
			{
				get;
				set;
			}

			public string ShortDescription
			{
				get;
				set;
			}

			public Product()
			{
			}
		}
	}
}