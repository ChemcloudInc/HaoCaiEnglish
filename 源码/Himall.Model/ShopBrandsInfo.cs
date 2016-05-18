using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopBrandsInfo : BaseModel
	{
		private long _id;

		public long BrandId
		{
			get;
			set;
		}

		public virtual BrandInfo Himall_Brands
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
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

		public long ShopId
		{
			get;
			set;
		}

		public ShopBrandsInfo()
		{
		}
	}
}