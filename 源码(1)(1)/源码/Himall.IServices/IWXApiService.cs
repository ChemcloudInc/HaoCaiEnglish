using System;

namespace Himall.IServices
{
	public interface IWXApiService : IService, IDisposable
	{
		string GetTicket(string appid, string appsecret);

		void SendMessageByTemplate(string appid, string appsecret, string openId, string templateId, string topcolor, string url, object data);

		void Subscribe(string openId);

		void UnSubscribe(string openId);
	}
}