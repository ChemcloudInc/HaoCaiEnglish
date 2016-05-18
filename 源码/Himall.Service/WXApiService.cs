using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class WXApiService : ServiceBase, IWXApiService, IService, IDisposable
	{
		public WXApiService()
		{
		}

		public string GetTicket(string appid, string appsecret)
		{
			int num = context.WeiXinBasicInfo.ToList().Count();
			WeiXinBasicInfo weiXinBasicInfo = null;
			if (num != 0)
			{
				weiXinBasicInfo = context.WeiXinBasicInfo.FirstOrDefault();
			}
			else
			{
				WeiXinBasicInfo weiXinBasicInfo1 = new WeiXinBasicInfo()
				{
					TicketOutTime = DateTime.Now,
					Ticket = ""
				};
				weiXinBasicInfo = weiXinBasicInfo1;
				weiXinBasicInfo.AppId = "";
				weiXinBasicInfo.AccessToken = "";
				weiXinBasicInfo = context.WeiXinBasicInfo.Add(weiXinBasicInfo);
                context.SaveChanges();
			}
			if (weiXinBasicInfo.TicketOutTime > DateTime.Now)
			{
				return weiXinBasicInfo.Ticket;
			}
			JsApiTicketResult jsApiTicketResult = new JsApiTicketResult();
			try
			{
				string str = AccessTokenContainer.TryGetToken(appid, appsecret, false);
				weiXinBasicInfo.AccessToken = str;
				jsApiTicketResult = CommonApi.GetTicket(str, "jsapi");
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (jsApiTicketResult.errcode == ReturnCode.获取access_token时AppSecret错误或者access_token无效)
				{
					Log.Info("请求Ticket出错，强制刷新acess_token", exception);
					string str1 = AccessTokenContainer.TryGetToken(appid, appsecret, true);
					weiXinBasicInfo.AccessToken = str1;
					jsApiTicketResult = CommonApi.GetTicket(str1, "jsapi");
				}
			}
			if (jsApiTicketResult.errcode != ReturnCode.请求成功)
			{
				throw new HimallException("请求微信接口出错");
			}
			if (jsApiTicketResult.expires_in > 3600)
			{
				jsApiTicketResult.expires_in = 3600;
			}
			weiXinBasicInfo.AppId = appid;
			DateTime now = DateTime.Now;
			weiXinBasicInfo.TicketOutTime = now.AddSeconds(jsApiTicketResult.expires_in);
			weiXinBasicInfo.Ticket = jsApiTicketResult.ticket;
			try
			{
                context.Configuration.ValidateOnSaveEnabled = false;
                context.SaveChanges();
                context.Configuration.ValidateOnSaveEnabled = true;
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				Log.Info("保存失败", (exception2.InnerException != null ? exception2.InnerException : exception2));
			}
			return weiXinBasicInfo.Ticket;
		}

		public void SendMessageByTemplate(string appid, string appsecret, string openId, string templateId, string topcolor, string url, object data)
		{
			if (!string.IsNullOrWhiteSpace(templateId))
			{
				string str = AccessTokenContainer.TryGetToken(appid, appsecret, false);
				TemplateApi.SendTemplateMessage(str, openId, templateId, topcolor, url, data, 10000);
			}
		}

		public void Subscribe(string openId)
		{
			OpenIdsInfo openIdsInfo = context.OpenIdsInfo.FirstOrDefault((OpenIdsInfo p) => p.OpenId == openId);
			if (openIdsInfo != null)
			{
				if (!openIdsInfo.IsSubscribe)
				{
					openIdsInfo.IsSubscribe = true;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                    context.Configuration.ValidateOnSaveEnabled = true;
				}
				return;
			}
			openIdsInfo = new OpenIdsInfo()
			{
				OpenId = openId,
				SubscribeTime = DateTime.Now,
				IsSubscribe = true
			};
            context.OpenIdsInfo.Add(openIdsInfo);
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
		}

		public void UnSubscribe(string openId)
		{
			OpenIdsInfo openIdsInfo = context.OpenIdsInfo.FirstOrDefault((OpenIdsInfo p) => p.OpenId == openId);
			if (openIdsInfo != null)
			{
				openIdsInfo.IsSubscribe = false;
                context.SaveChanges();
				return;
			}
			openIdsInfo = new OpenIdsInfo()
			{
				OpenId = openId,
				SubscribeTime = DateTime.Now,
				IsSubscribe = false
			};
            context.OpenIdsInfo.Add(openIdsInfo);
            context.SaveChanges();
		}
	}
}