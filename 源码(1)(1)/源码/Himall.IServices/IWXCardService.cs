using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IWXCardService : IService, IDisposable
	{
		bool Add(WXCardLogInfo info);

		void Consume(string cardid, string code);

		void Consume(long id);

		void Consume(long couponcodeid, WXCardLogInfo.CouponTypeEnum coupontype);

		void Delete(string cardid);

		void Delete(long id);

		void EditGetLimit(int? num, string cardid);

		void EditGetLimit(int? num, long id);

		void EditStock(int num, string cardid);

		void EditStock(int num, long id);

		void Event_Audit(string cardid, WXCardLogInfo.AuditStatusEnum auditstatus);

		void Event_Send(string cardid, string code, string openid, int outerid);

		void Event_Unavailable(string cardid, string code);

		WXCardLogInfo Get(long id);

		WXCardLogInfo Get(string cardid);

		WXCardLogInfo Get(long couponId, WXCardLogInfo.CouponTypeEnum couponType);

		string GetCardReceiveUrl(string cardid, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType);

		string GetCardReceiveUrl(long id, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType);

		WXCardCodeLogInfo GetCodeInfo(long id);

		WXCardCodeLogInfo GetCodeInfo(string cardid, string code);

		WXCardCodeLogInfo GetCodeInfo(long couponCodeId, WXCardLogInfo.CouponTypeEnum couponType);

		WXJSCardModel GetJSWeiXinCard(long couponid, long couponcodeid, WXCardLogInfo.CouponTypeEnum couponType);

		WXSyncJSInfoByCard GetSyncWeiXin(long couponid, long couponcodeid, WXCardLogInfo.CouponTypeEnum couponType, string url);

		void Unavailable(string cardid, string code);

		void Unavailable(long id);
	}
}