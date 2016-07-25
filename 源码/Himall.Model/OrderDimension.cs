using System;
using System.ComponentModel;

namespace Himall.Model
{
	public enum OrderDimension
	{
		[Description("下单客户数")]
		OrderMemberCount = 1,
		[Description("下单量")]
		OrderCount = 2,
		[Description("下单金额")]
		OrderMoney = 3
	}
}