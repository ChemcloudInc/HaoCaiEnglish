using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class ProductSearch
	{
		public List<string> AttrIds
		{
			get;
			set;
		}

		public long BrandId
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public decimal EndPrice
		{
			get;
			set;
		}

		public string Ex_Keyword
		{
			get;
			set;
		}

		public string Keyword
		{
			get;
			set;
		}

		public int OrderKey
		{
			get;
			set;
		}

		public bool OrderType
		{
			get;
			set;
		}

		public int PageNumber
		{
			get;
			set;
		}

		public int PageSize
		{
			get;
			set;
		}

		public long? ShopCategoryId
		{
			get;
			set;
		}

		public long shopId
		{
			get;
			set;
		}

		public decimal startPrice
		{
			get;
			set;
		}

		public ProductSearch()
		{
		}
	}
}