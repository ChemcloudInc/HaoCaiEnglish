using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductShopCategoryInfo : BaseModel
	{
		private long _id;

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

		public long ProductId
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductInfo ProductInfo
		{
			get;
			set;
		}

		public long ShopCategoryId
		{
			get;
			set;
		}

		public virtual Himall.Model.ShopCategoryInfo ShopCategoryInfo
		{
			get;
			set;
		}

		public ProductShopCategoryInfo()
		{
		}
	}
}