using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductDetailModelForMobie
	{
		public CollectionSKU Color
		{
			get;
			set;
		}

		public int MaxSaleCount
		{
			get;
			set;
		}

		public ProductInfoModel Product
		{
			get;
			set;
		}

		public ShopInfoModel Shop
		{
			get;
			set;
		}

		public CollectionSKU Size
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public CollectionSKU Version
		{
			get;
			set;
		}

		public ProductDetailModelForMobie()
		{
		}
	}
}