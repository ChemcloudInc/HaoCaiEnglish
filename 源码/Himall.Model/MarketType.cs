using System;
using System.ComponentModel;

namespace Himall.Model
{
	public enum MarketType
	{
		[Description("限时购")]
		LimitTimeBuy = 1,
		[Description("优惠券")]
		Coupon = 2,
		[Description("组合购")]
		Collocation = 3,
		[Description("随机红包")]
		RandomlyBonus = 4
	}
}