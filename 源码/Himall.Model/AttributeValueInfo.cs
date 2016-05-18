using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AttributeValueInfo : BaseModel
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

		public string Value
		{
			get;
			set;
		}

		public AttributeValueInfo()
		{
		}
	}
}