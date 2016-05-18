using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class ManagerQuery : QueryBase
	{
		public long ShopID
		{
			get;
			set;
		}

		public long userID
		{
			get;
			set;
		}

		public ManagerQuery()
		{
		}
	}
}