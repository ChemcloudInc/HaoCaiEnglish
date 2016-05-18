using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AttributeInfo : BaseModel
	{
		private long _id;

		public virtual ICollection<Himall.Model.AttributeValueInfo> AttributeValueInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string AttrValue
		{
			get;
			set;
		}

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

		public bool IsMulti
		{
			get;
			set;
		}

		public bool IsMust
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public virtual ICollection<ProductAttributeInfo> ProductAttributesInfo
		{
			get;
			set;
		}

		public long TypeId
		{
			get;
			set;
		}

		public virtual ProductTypeInfo TypesInfo
		{
			get;
			set;
		}

		public AttributeInfo()
		{
            AttributeValueInfo = new HashSet<Himall.Model.AttributeValueInfo>();
            ProductAttributesInfo = new HashSet<ProductAttributeInfo>();
		}
	}
}