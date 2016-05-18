using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.Service
{
	public class SpecValueComparer : IEqualityComparer<SpecificationValueInfo>
	{
		public SpecValueComparer()
		{
		}

		public bool Equals(SpecificationValueInfo x, SpecificationValueInfo y)
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

		public int GetHashCode(SpecificationValueInfo spec)
		{
			if (ReferenceEquals(spec, null))
			{
				return 0;
			}
			return spec.Value.GetHashCode();
		}
	}
}