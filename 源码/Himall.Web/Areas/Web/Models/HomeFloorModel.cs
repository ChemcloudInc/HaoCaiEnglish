using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class HomeFloorModel
	{
		public List<HomeFloorModel.WebFloorBrand> Brands
		{
			get;
			set;
		}

		public string DefaultTabName
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public long Index
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<HomeFloorModel.WebFloorProductLinks> Products
		{
			get;
			set;
		}

		public List<HomeFloorModel.WebFloorProductLinks> RightBottons
		{
			get;
			set;
		}

		public List<HomeFloorModel.WebFloorProductLinks> RightTops
		{
			get;
			set;
		}

		public List<HomeFloorModel.WebFloorProductLinks> Scrolls
		{
			get;
			set;
		}

		public long StyleLevel
		{
			get;
			set;
		}

		public string SubName
		{
			get;
			set;
		}

		public List<HomeFloorModel.Tab> Tabs
		{
			get;
			set;
		}

		public List<HomeFloorModel.WebFloorTextLink> TextLinks
		{
			get;
			set;
		}

		public HomeFloorModel()
		{
            Brands = new List<HomeFloorModel.WebFloorBrand>();
            TextLinks = new List<HomeFloorModel.WebFloorTextLink>();
            Products = new List<HomeFloorModel.WebFloorProductLinks>();
            Tabs = new List<HomeFloorModel.Tab>();
		}

		public class ProductDetail
		{
			public string ImagePath
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

			public long ProductId
			{
				get;
				set;
			}

			public ProductDetail()
			{
			}
		}

		public class Tab
		{
			public List<HomeFloorModel.ProductDetail> Detail
			{
				get;
				set;
			}

			public long Id
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public Tab()
			{
			}
		}

		public class WebFloorBrand
		{
			public long Id
			{
				get;
				set;
			}

			public string Img
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			public WebFloorBrand()
			{
			}
		}

		public class WebFloorProductLinks
		{
			public long Id
			{
				get;
				set;
			}

			public string ImageUrl
			{
				get;
				set;
			}

			public Position Type
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			public WebFloorProductLinks()
			{
			}
		}

		public class WebFloorTextLink
		{
			public long Id
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			public WebFloorTextLink()
			{
			}
		}
	}
}