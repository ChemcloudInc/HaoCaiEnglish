using Himall.Core;
using Himall.IServices;
using Himall.Service;
using Himall.ServiceProvider;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;

namespace Himall.Service.Weixin
{
	public class WXHelper : ServiceBase
	{
		private IWXApiService ser_wxapi;

		public WXHelper()
		{
            ser_wxapi = Instance<IWXApiService>.Create;
		}

		public void ClearTicketCache(string ticket)
		{
		}

		public string GetAccessToken(string appid, string secret, bool IsGetNew = false)
		{
			string str = "";
			if (!string.IsNullOrWhiteSpace(appid) && !string.IsNullOrWhiteSpace(secret))
			{
				str = (!IsGetNew ? AccessTokenContainer.TryGetToken(appid, secret, false) : AccessTokenContainer.TryGetToken(appid, secret, true));
			}
			return str;
		}

		public string GetTicket(string appid, string secret, string type = "jsapi")
		{
			string ticketByToken = "";
			if (type != "jsapi")
			{
				try
				{
					ticketByToken = GetTicketByToken(GetAccessToken(appid, secret, false), type, false);
				}
				catch (Exception exception)
				{
					Log.Info("请求Ticket出错，强制刷新acess_token", exception);
					ticketByToken = GetTicketByToken(GetAccessToken(appid, secret, true), type, false);
				}
			}
			else
			{
				ticketByToken = ser_wxapi.GetTicket(appid, secret);
			}
			return ticketByToken;
		}

		public string GetTicketByToken(string accessToken, string type = "jsapi", bool IsRemote = false)
		{
			string str = "";
			if (!string.IsNullOrWhiteSpace(accessToken))
			{
				JsApiTicketResult ticket = CommonApi.GetTicket(accessToken, type);
				if (ticket.errcode != ReturnCode.请求成功)
				{
					object[] objArray = new object[] { "WXERR:[", ticket.errcode, "]", ticket.errmsg };
					throw new Exception(string.Concat(objArray));
				}
				str = ticket.ticket;
			}
			return str;
		}
	}
}