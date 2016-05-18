using Himall.Core.Plugins.Message;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IMessageService : IService, IDisposable
	{
		string GetDestination(long userId, string pluginId, MemberContactsInfo.UserTypes type);

		MemberContactsInfo GetMemberContactsInfo(string pluginId, string contact, MemberContactsInfo.UserTypes type);

		void SendMessageCode(string destination, string pluginId, MessageUserInfo info);

		void SendMessageOnFindPassWord(long userId, MessageUserInfo info);

		void SendMessageOnOrderCreate(long userId, MessageOrderInfo info);

		void SendMessageOnOrderPay(long userId, MessageOrderInfo info);

		void SendMessageOnOrderRefund(long userId, MessageOrderInfo info);

		void SendMessageOnOrderShipping(long userId, MessageOrderInfo info);

		void SendMessageOnShopAudited(long userId, MessageShopInfo info);

		void SendMessageOnShopSuccess(long userId, MessageShopInfo info);

		void UpdateMemberContacts(MemberContactsInfo info);
	}
}