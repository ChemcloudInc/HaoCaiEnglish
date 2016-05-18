using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IMemberIntegralService : IService, IDisposable
	{
		void AddMemberIntegral(MemberIntegralRecord model, IConversionMemberIntegralBase conversionMemberIntegralEntity = null);

		MemberIntegralExchangeRules GetIntegralChangeRule();

		PageModel<MemberIntegralRecord> GetIntegralRecordList(IntegralRecordQuery query);

		IEnumerable<MemberIntegralRule> GetIntegralRule();

		MemberIntegral GetMemberIntegral(long userId);

		PageModel<MemberIntegral> GetMemberIntegralList(IntegralQuery query);

		UserIntegralGroupModel GetUserHistroyIntegralGroup(long userId);

		bool HasLoginIntegralRecord(long userId);

		void SetIntegralChangeRule(MemberIntegralExchangeRules info);

		void SetIntegralRule(IEnumerable<MemberIntegralRule> info);
	}
}