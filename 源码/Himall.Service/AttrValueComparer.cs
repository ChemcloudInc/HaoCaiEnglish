using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.Service
{
	public class AttrValueComparer : IEqualityComparer<AttributeValueInfo>
	{
		public AttrValueComparer()
		{
		}

		public bool Equals(AttributeValueInfo x, AttributeValueInfo y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
			{
				return false;
			}
			return x.Value == y.Value;
		}

		public int GetHashCode(AttributeValueInfo attr)
		{
			if (ReferenceEquals(attr, null))
			{
				return 0;
			}
			return attr.Value.GetHashCode();
		}
	}
}