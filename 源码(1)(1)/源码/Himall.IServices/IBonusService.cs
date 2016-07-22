using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IBonusService : IService, IDisposable
	{
		void Add(BonusInfo model, string baseAddress);

		bool CanAddBonus();

		void DepositToRegister(long userid);

		PageModel<BonusInfo> Get(int type, int state, string name, int pageIndex, int pageSize);

		BonusInfo Get(long id);

		PageModel<BonusReceiveInfo> GetDetail(long bonusId, int pageIndex, int pageSize);

		decimal GetReceivePriceByOpendId(long id, string openId);

		void Invalid(long id);

		object Receive(long id, string openId);

		string Receive(string openId);

		void SetShare(long id, string openId);

		void Update(BonusInfo model);
	}
}