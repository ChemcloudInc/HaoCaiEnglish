using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class HomeFloorDetail
	{
		public IEnumerable<HomeFloorDetail.Brand> Brands
		{
			get;
			set;
		}

		public IEnumerable<HomeFloorDetail.Category> Categories
		{
			get;
			set;
		}

		public HomeFloorDetail.CategoryImage CategoryImg
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

		public string Name
		{
			get;
			set;
		}

		public IEnumerable<HomeFloorDetail.TextLink> ProductLinks
		{
			get;
			set;
		}

		public IEnumerable<HomeFloorDetail.ProductModule> ProductModules
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

		public IEnumerable<HomeFloorDetail.Tab> Tabs
		{
			get;
			set;
		}

		public IEnumerable<HomeFloorDetail.TextLink> TextLinks
		{
			get;
			set;
		}

		public HomeFloorDetail()
		{
            Brands = new Collection<HomeFloorDetail.Brand>();
            TextLinks = new Collection<HomeFloorDetail.TextLink>();
            ProductLinks = new Collection<HomeFloorDetail.TextLink>();
		}

		public class Brand
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

			public Brand()
			{
			}
		}

		public class Category
		{
			public int Depth
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

			public Category()
			{
			}
		}

		public class CategoryImage
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

			public string Url
			{
				get;
				set;
			}

			public CategoryImage()
			{
			}
		}

		public class ProductDetail
		{
			public long Id
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

		public class ProductModule
		{
			public int ModuleIndex
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public IEnumerable<long> ProductIds
			{
				get;
				set;
			}

			public IEnumerable<HomeFloorDetail.ProductModule.Topic> Topics
			{
				get;
				set;
			}

			public ProductModule()
			{
                ProductIds = new Collection<long>();
                Topics = new Collection<HomeFloorDetail.ProductModule.Topic>();
			}

			public class Topic
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

				public string Url
				{
					get;
					set;
				}

				public Topic()
				{
				}
			}
		}

		public class Tab
		{
			public int Count
			{
				get;
				set;
			}

			public IEnumerable<HomeFloorDetail.ProductDetail> Detail
			{
				get;
				set;
			}

			public long Id
			{
				get;
				set;
			}

			public string Ids
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

		public class TextLink
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

			public TextLink()
			{
			}
		}
	}
}