using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IShopBonusService : IService, IDisposable
	{
		void Add(ShopBonusInfo model, long shopid);

		long GenerateBonusDetail(ShopBonusInfo model, long createUserid, long orderid, string receiveurl);

		PageModel<ShopBonusInfo> Get(long shopid, string name, int state, int pageIndex, int pageSize);

		ShopBonusInfo Get(long id);

		ShopBonusInfo GetByGrantId(long grantid);

		ShopBonusGrantInfo GetByOrderId(long orderid);

		ShopBonusInfo GetByShopId(long shopid);

		List<ShopBonusReceiveInfo> GetCanUseDetailByUserId(long userid);

		PageModel<ShopBonusReceiveInfo> GetDetail(long bonusid, int pageIndex, int pageSize);

		List<ShopBonusReceiveInfo> GetDetailByGrantId(long grantid);

		ShopBonusReceiveInfo GetDetailById(long userid, long id);

		PageModel<ShopBonusReceiveInfo> GetDetailByQuery(CouponRecordQuery query);

		List<ShopBonusReceiveInfo> GetDetailByUserId(long userid);

		List<ShopBonusReceiveInfo> GetDetailToUse(long shopid, long userid, decimal sumprice);

		ShopBonusGrantInfo GetGrantByUserOrder(long orderid, long userid);

		long GetGrantIdByOrderId(long orderid);

		ActiveMarketServiceInfo GetShopBonusService(long shopId);

		decimal GetUsedPrice(long orderid, long userid);

		void Invalid(long id);

		bool IsAdd(long shopid);

		bool IsOverDate(DateTime bonusDateEnd, DateTime dateEnd, long shopid);

		object Receive(long grantid, string openId, string wxhead, string wxname);

		void SetBonusToUsed(long userid, List<OrderInfo> orders, long rid);

		void Update(ShopBonusInfo model);
	}
}