using System;
using System.ComponentModel;

namespace Himall.Model
{
	public enum SaleDimension
	{
		[Description("销售量")]
		SaleCount = 1,
		[Description("销售额")]
		Sales = 2
	}
}