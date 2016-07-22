using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Himall.Service
{
	public class BindWXGenerateIntegral : ServiceBase, IConversionMemberIntegralBase
	{
		public BindWXGenerateIntegral()
		{
		}

		public int ConversionIntegral()
		{
			MemberIntegralRule memberIntegralRule = context.MemberIntegralRule.FirstOrDefault((MemberIntegralRule m) => m.TypeId == 6);
			if (memberIntegralRule != null)
			{
				return memberIntegralRule.Integral;
			}
			Log.Info(string.Format("找不到绑定微信产生会员积分的规则", new object[0]));
			return 0;
		}
	}
}