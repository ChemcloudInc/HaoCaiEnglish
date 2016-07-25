using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SpecificationValueInfo : BaseModel
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

		public virtual ICollection<Himall.Model.SellerSpecificationValueInfo> SellerSpecificationValueInfo
		{
			get;
			set;
		}

		public SpecificationType Specification
		{
			get;
			set;
		}

		public long TypeId
		{
			get;
			set;
		}

		public virtual ProductTypeInfo TypeInfo
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public SpecificationValueInfo()
		{
            SellerSpecificationValueInfo = new HashSet<Himall.Model.SellerSpecificationValueInfo>();
		}
	}
}