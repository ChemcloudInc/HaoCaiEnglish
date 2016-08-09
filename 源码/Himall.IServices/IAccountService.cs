using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IAccountService : IService, IDisposable
	{
		void ConfirmAccount(long id, string managerRemark);

		AccountInfo GetAccount(long id);

        IQueryable<AccountDetailInfo> GetAccountDetail(long accountid);

		PageModel<AccountDetailInfo> GetAccountDetails(AccountQuery query);

		PageModel<AccountMetaModel> GetAccountMeta(AccountQuery query);

		PageModel<AccountPurchaseAgreementInfo> GetAccountPurchaseAgreements(AccountQuery query);

		PageModel<AccountInfo> GetAccounts(AccountQuery query);
	}
}