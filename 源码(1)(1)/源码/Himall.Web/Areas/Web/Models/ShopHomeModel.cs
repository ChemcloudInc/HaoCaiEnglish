using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ShopHomeModel
	{
		public List<ShopHomeFloor> Floors
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

		public List<ImageAdInfo> ImageAds
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public List<BannerInfo> Navignations
		{
			get;
			set;
		}

		public List<ProductInfo> Products
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

		public List<SlideAdInfo> Slides
		{
			get;
			set;
		}

		public ShopHomeModel()
		{
		}
	}
}