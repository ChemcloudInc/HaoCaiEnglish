using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.Service
{
	public class AttrComparer : IEqualityComparer<AttributeInfo>
	{
		public AttrComparer()
		{
		}

		public bool Equals(AttributeInfo x, AttributeInfo y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
			{
				return false;
			}
			if (x.Id != y.Id || x.Id == 0)
			{
				return false;
			}
			return y.Id != 0;
		}

		public int GetHashCode(AttributeInfo attr)
		{
			if (ReferenceEquals(attr, null))
			{
				return 0;
			}
			return (int)(attr.Id ^ attr.TypeId);
		}
	}
}