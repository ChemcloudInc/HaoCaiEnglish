using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductAttributeInfo : BaseModel
	{
		private long _id;

		public long AttributeId
		{
			get;
			set;
		}

		public virtual AttributeInfo AttributesInfo
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

		public long ProductId
		{
			get;
			set;
		}

		public virtual ProductInfo ProductsInfo
		{
			get;
			set;
		}

		public long ValueId
		{
			get;
			set;
		}

		public ProductAttributeInfo()
		{
		}
	}
}