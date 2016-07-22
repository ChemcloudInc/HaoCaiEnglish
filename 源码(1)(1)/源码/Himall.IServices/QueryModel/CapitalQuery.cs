using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class CapitalQuery : QueryBase
	{
		public long? memberId
		{
			get;
			set;
		}

		public CapitalQuery()
		{
		}
	}
}