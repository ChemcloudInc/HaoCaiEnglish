using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IMemberCapitalService : IService, IDisposable
	{
		void AddCapital(CapitalDetailModel model);

		long AddChargeApply(ChargeDetailInfo model);

		void AddWithDrawApply(ApplyWithDrawInfo model);

		void ConfirmApplyWithDraw(ApplyWithDrawInfo info);

		long CreateCode(CapitalDetailInfo.CapitalDetailType type);

		PageModel<ApplyWithDrawInfo> GetApplyWithDraw(ApplyWithDrawQuery query);

		CapitalDetailInfo GetCapitalDetailInfo(long id);

		PageModel<CapitalDetailInfo> GetCapitalDetails(CapitalDetailQuery query);

		CapitalInfo GetCapitalInfo(long userid);

		PageModel<CapitalInfo> GetCapitals(CapitalQuery query);

		ChargeDetailInfo GetChargeDetail(long id);

		PageModel<ChargeDetailInfo> GetChargeLists(ChargeQuery query);

		UserMemberInfo GetMemberInfoByPayPwd(long memid, string payPwd);

		void RefuseApplyWithDraw(long id, ApplyWithDrawInfo.ApplyWithDrawStatus status, string opuser, string remark);

		void SetPayPwd(long memid, string pwd);

		void UpdateCapitalAmount(long memid, decimal amount, decimal freezeAmount, decimal chargeAmount);

		void UpdateChargeDetail(ChargeDetailInfo model);
	}
}