using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductDetailModelForWeb
	{
		public List<HotProductInfo> BoughtProducts
		{
			get;
			set;
		}

		public CashDepositsObligation CashDepositsServer
		{
			get;
			set;
		}

		public CollectionSKU Color
		{
			get;
			set;
		}

		public string DescriptiondSuffix
		{
			get;
			set;
		}

		public string DescriptionPrefix
		{
			get;
			set;
		}

		public List<HotProductInfo> HotAttentionProducts
		{
			get;
			set;
		}

		public List<HotProductInfo> HotSaleProducts
		{
			get;
			set;
		}

		public int MaxSaleCount
		{
			get;
			set;
		}

		public ProductInfo Product
		{
			get;
			set;
		}

		public string ProductDescription
		{
			get;
			set;
		}

		public ShopInfoModel Shop
		{
			get;
			set;
		}

		public List<CategoryJsonModel> ShopCategory
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

		public ProductDetailModelForWeb()
		{
		}
	}
}