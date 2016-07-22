using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface ICashDepositsService : IService, IDisposable
	{
		void AddCashDeposit(CashDepositInfo cashDeposit);

		void AddCashDepositDetails(CashDepositDetailInfo cashDepositDetail);

		void AddCategoryCashDeposits(CategoryCashDepositInfo model);

		void CloseNoReasonReturn(long id);

		void DeleteCategoryCashDeposits(long categoryId);

		CashDepositInfo GetCashDeposit(long id);

		CashDepositInfo GetCashDepositByShopId(long shopId);

		PageModel<CashDepositDetailInfo> GetCashDepositDetails(CashDepositDetailQuery query);

		PageModel<CashDepositInfo> GetCashDeposits(CashDepositQuery query);

		CashDepositsObligation GetCashDepositsObligation(long productId);

		IEnumerable<CategoryCashDepositInfo> GetCategoryCashDeposits();

		decimal GetNeedPayCashDepositByShopId(long shopId);

		void OpenNoReasonReturn(long id);

		void UpdateEnableLabels(long id, bool enableLabels);

		void UpdateNeedPayCashDeposit(long categoryId, decimal CashDeposit);
	}
}