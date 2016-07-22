using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CollocationQuery : QueryBase
	{
		public long? ShopId
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public CollocationQuery()
		{
		}
	}
}