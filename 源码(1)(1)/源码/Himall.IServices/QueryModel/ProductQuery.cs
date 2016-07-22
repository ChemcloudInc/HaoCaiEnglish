using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class ProductQuery : QueryBase
	{
		public IEnumerable<ProductInfo.ProductAuditStatus> AuditStatus
		{
			get;
			set;
		}

		public string BrandNameKeyword
		{
			get;
			set;
		}

		public long? CategoryId
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public IEnumerable<long> Ids
		{
			get;
			set;
		}

		public bool IsLimitTimeBuy
		{
			get;
			set;
		}

		public string KeyWords
		{
			get;
			set;
		}

		public bool NotIncludedInDraft
		{
			get;
			set;
		}

		public string ProductCode
		{
			get;
			set;
		}

		public ProductInfo.ProductSaleStatus? SaleStatus
		{
			get;
			set;
		}

		public long? ShopCategoryId
		{
			get;
			set;
		}

		public long? ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public ProductQuery()
		{
		}
	}
}