using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class BrandInfo : BaseModel
	{
		private long _id;

		public string Description
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FloorBrandInfo> FloorBrandInfo
		{
			get;
			set;
		}

		public virtual ICollection<ShopBrandApplysInfo> Himall_ShopBrandApplys
		{
			get;
			set;
		}

		public virtual ICollection<ShopBrandsInfo> Himall_ShopBrands
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public bool IsRecommend
		{
			get;
			set;
		}

		private string logo
		{
			get;
			set;
		}

		public string Logo
		{
			get
			{
				return string.Concat(ImageServerUrl, logo);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    logo = value;
					return;
				}
                logo = value.Replace(ImageServerUrl, "");
			}
		}

		public string Meta_Description
		{
			get;
			set;
		}

		public string Meta_Keywords
		{
			get;
			set;
		}

		public string Meta_Title
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string RewriteName
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.TypeBrandInfo> TypeBrandInfo
		{
			get;
			set;
		}

		public BrandInfo()
		{
            TypeBrandInfo = new HashSet<Himall.Model.TypeBrandInfo>();
            FloorBrandInfo = new HashSet<Himall.Model.FloorBrandInfo>();
            Himall_ShopBrands = new HashSet<ShopBrandsInfo>();
            Himall_ShopBrandApplys = new HashSet<ShopBrandApplysInfo>();
		}
	}
}