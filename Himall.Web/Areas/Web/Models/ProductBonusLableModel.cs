using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ProductBonusLableModel
	{
		public int Count
		{
			get;
			set;
		}

		public decimal GrantPrice
		{
			get;
			set;
		}

		public decimal RandomAmountEnd
		{
			get;
			set;
		}

		public decimal RandomAmountStart
		{
			get;
			set;
		}

		public ProductBonusLableModel()
		{
		}
	}
}