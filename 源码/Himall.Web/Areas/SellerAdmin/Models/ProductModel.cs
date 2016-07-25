using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ProductModel
	{
		public string AuditReason
		{
			get;
			set;
		}

		public int AuditState
		{
			get;
			set;
		}

		public long BrandId
		{
			get;
			set;
		}

		public string BrandName
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public string CategoryName
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public bool IsLimitTimeBuy
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public string ProductCode
		{
			get;
			set;
		}

		public string PublishTime
		{
			get;
			set;
		}

		public string QrCode
		{
			get;
			set;
		}

		public long SaleCount
		{
			get;
			set;
		}

		public int SaleState
		{
			get;
			set;
		}

		public int Stock
		{
			get;
			set;
		}

		public long Uid
		{
			get;
			set;
		}

		public string Unit
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public ProductModel()
		{
		}
	}
}