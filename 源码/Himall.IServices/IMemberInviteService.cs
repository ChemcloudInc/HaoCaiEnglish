using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IMemberInviteService : IService, IDisposable
	{
		void AddInviteIntegel(UserMemberInfo RegMember, UserMemberInfo InviteMember);

		void AddInviteRecord(InviteRecordInfo info);

		PageModel<InviteRecordInfo> GetInviteList(InviteRecordQuery query);

		InviteRuleInfo GetInviteRule();

		UserInviteModel GetMemberInviteInfo(long userId);

		bool HasInviteIntegralRecord(long RegId);

		void SetInviteRule(InviteRuleInfo model);
	}
}