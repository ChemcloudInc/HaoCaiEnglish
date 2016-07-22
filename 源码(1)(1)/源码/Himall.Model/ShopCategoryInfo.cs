using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopCategoryInfo : BaseModel
	{
		private long _id;

		public long DisplaySequence
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

		public bool IsShow
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public long ParentCategoryId
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.ProductShopCategoryInfo> ProductShopCategoryInfo
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public ShopCategoryInfo()
		{
            ProductShopCategoryInfo = new HashSet<Himall.Model.ProductShopCategoryInfo>();
		}
	}
}