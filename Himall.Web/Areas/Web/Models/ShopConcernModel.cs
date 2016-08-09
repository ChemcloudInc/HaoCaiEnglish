using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ShopConcernModel
	{
		public ConcernShopInfo FavoriteShopInfo
		{
			get;
			set;
		}

		public List<HotProductInfo> HotSaleProducts
		{
			get;
			set;
		}

		public List<HotProductInfo> NewSaleProducts
		{
			get;
			set;
		}

		public ShopConcernModel()
		{
            FavoriteShopInfo = new ConcernShopInfo();
            HotSaleProducts = new List<HotProductInfo>();
            NewSaleProducts = new List<HotProductInfo>();
		}
	}
}