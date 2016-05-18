using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class QueryBase
	{
		public bool IsAsc
		{
			get;
			set;
		}

		public int PageNo
		{
			get;
			set;
		}

		public int PageSize
		{
			get;
			set;
		}

		public string Sort
		{
			get;
			set;
		}

		public QueryBase()
		{
		}
	}
}