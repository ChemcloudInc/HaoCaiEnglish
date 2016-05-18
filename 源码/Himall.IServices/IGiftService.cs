using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IGiftService : IService, IDisposable
	{
		void AddGift(GiftInfo model);

		void ChangeStatus(long id, bool status);

		GiftInfo GetById(long id);

		GiftInfo GetByIdAsNoTracking(long id);

		PageModel<GiftModel> GetGifts(GiftQuery query);

		void UpdateGift(GiftInfo model);

		void UpdateSequence(long id, int sequence);
	}
}