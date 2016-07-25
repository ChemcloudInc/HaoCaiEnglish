using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class IntegralRuleController : BaseAdminController
	{
		public IntegralRuleController()
		{
		}

		public ActionResult Change()
		{
			return View(ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule());
		}

		[HttpPost]
		public JsonResult Change(int IntegralPerMoney)
		{
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			if (integralChangeRule == null)
			{
				integralChangeRule = new MemberIntegralExchangeRules()
				{
					IntegralPerMoney = IntegralPerMoney
				};
			}
			else
			{
				integralChangeRule.IntegralPerMoney = IntegralPerMoney;
			}
			ServiceHelper.Create<IMemberIntegralService>().SetIntegralChangeRule(integralChangeRule);
			Result result = new Result()
			{
				success = true,
				msg = "保存成功"
			};
			return Json(result);
		}

		public ActionResult Management()
		{
			IEnumerable<MemberIntegralRule> integralRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralRule();
			IntegralRule moneyPerIntegral = new IntegralRule();
			MemberIntegralRule memberIntegralRule = integralRule.FirstOrDefault((MemberIntegralRule a) => a.TypeId == 6);
			MemberIntegralRule memberIntegralRule1 = integralRule.FirstOrDefault((MemberIntegralRule a) => a.TypeId == 9);
			MemberIntegralRule memberIntegralRule2 = integralRule.FirstOrDefault((MemberIntegralRule a) => a.TypeId == 5);
			MemberIntegralRule memberIntegralRule3 = integralRule.FirstOrDefault((MemberIntegralRule a) => a.TypeId == 7);
			moneyPerIntegral.BindWX = (memberIntegralRule == null ? 0 : memberIntegralRule.Integral);
			moneyPerIntegral.Reg = (memberIntegralRule1 == null ? 0 : memberIntegralRule1.Integral);
			moneyPerIntegral.Comment = (memberIntegralRule3 == null ? 0 : memberIntegralRule3.Integral);
			moneyPerIntegral.Login = (memberIntegralRule2 == null ? 0 : memberIntegralRule2.Integral);
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			if (integralChangeRule != null)
			{
				moneyPerIntegral.MoneyPerIntegral = integralChangeRule.MoneyPerIntegral;
			}
			return View(moneyPerIntegral);
		}

		[HttpPost]
		public JsonResult Management(IntegralRule rule)
		{
			List<MemberIntegralRule> memberIntegralRules = new List<MemberIntegralRule>();
			MemberIntegralRule memberIntegralRule = new MemberIntegralRule()
			{
				Integral = rule.Reg,
				TypeId = 9
			};
			memberIntegralRules.Add(memberIntegralRule);
			MemberIntegralRule memberIntegralRule1 = new MemberIntegralRule()
			{
				Integral = rule.BindWX,
				TypeId = 6
			};
			memberIntegralRules.Add(memberIntegralRule1);
			MemberIntegralRule memberIntegralRule2 = new MemberIntegralRule()
			{
				Integral = rule.Login,
				TypeId = 5
			};
			memberIntegralRules.Add(memberIntegralRule2);
			MemberIntegralRule memberIntegralRule3 = new MemberIntegralRule()
			{
				Integral = rule.Comment,
				TypeId = 7
			};
			memberIntegralRules.Add(memberIntegralRule3);
			ServiceHelper.Create<IMemberIntegralService>().SetIntegralRule(memberIntegralRules);
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			if (integralChangeRule == null)
			{
				integralChangeRule = new MemberIntegralExchangeRules()
				{
					MoneyPerIntegral = rule.MoneyPerIntegral
				};
			}
			else
			{
				integralChangeRule.MoneyPerIntegral = rule.MoneyPerIntegral;
			}
			ServiceHelper.Create<IMemberIntegralService>().SetIntegralChangeRule(integralChangeRule);
			Result result = new Result()
			{
				success = true,
				msg = "保存成功"
			};
			return Json(result);
		}
	}
}