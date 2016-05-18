using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class SellerQuery : QueryBase
	{
		public IEnumerable<long> Ids
		{
			get;
			set;
		}

		public int? NextRegionId
		{
			get;
			set;
		}

		public int? RegionId
		{
			get;
			set;
		}

		public long? ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public SellerQuery()
		{
		}
	}
}