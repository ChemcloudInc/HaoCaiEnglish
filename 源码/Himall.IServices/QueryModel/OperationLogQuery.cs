using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class OperationLogQuery : QueryBase
	{
		public DateTime? EndDate
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public OperationLogQuery()
		{
		}
	}
}