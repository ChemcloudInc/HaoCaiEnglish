using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductInfoModel
	{
		public ProductInfo.ProductAuditStatus AuditStatus
		{
			get;
			set;
		}

		public int CommentCount
		{
			get;
			set;
		}

		public int Consultations
		{
			get;
			set;
		}

		public List<string> ImagePath
		{
			get;
			set;
		}

		public bool IsFavorite
		{
			get;
			set;
		}

		public bool IsOnLimitBuy
		{
			get;
			set;
		}

		public decimal MarketPrice
		{
			get;
			set;
		}

		public decimal MinSalePrice
		{
			get;
			set;
		}

		public int NicePercent
		{
			get;
			set;
		}

		public string ProductDescription
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public ProductInfo.ProductSaleStatus ProductSaleStatus
		{
			get;
			set;
		}

		public string ShortDescription
		{
			get;
			set;
		}

		public ProductInfoModel()
		{
		}
	}
}