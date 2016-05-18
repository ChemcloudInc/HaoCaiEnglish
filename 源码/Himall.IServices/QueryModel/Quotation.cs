using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class Quotation : QueryBase
	{
		public DateTime EndDate
		{
			get;
			set;
		}

		public decimal MaxPrice
		{
			get;
			set;
		}

		public decimal MinPrice
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public int State
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public Quotation()
		{
		}
	}
}