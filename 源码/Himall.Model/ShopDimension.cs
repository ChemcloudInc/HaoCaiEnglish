using System;
using System.ComponentModel;

namespace Himall.Model
{
	public enum ShopDimension
	{
		[Description("订单量")]
		OrderCount = 1,
		[Description("销售额")]
		Sales = 2
	}
}