using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web;
using Himall.Web.Areas.Admin.Models.Market;
using Himall.Web.Framework;
using Hishop.Weixin.MP.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class WXApiController : BaseMobileController
	{
		public WXApiController()
		{
		}

		private string DealTextMsg(Senparc.Weixin.MP.Entities.IRequestMessageBase doc, string msg)
		{
			string empty = string.Empty;
			string fromUserName = doc.FromUserName;
			string toUserName = doc.ToUserName;
			empty = string.Concat("", "<xml>");
			empty = string.Concat(empty, "<ToUserName><![CDATA[", fromUserName, "]]></ToUserName>");
			object obj = string.Concat(empty, "<FromUserName><![CDATA[", toUserName, "]]></FromUserName>");
			object[] weixinDateTime = new object[] { obj, "<CreateTime>", WXApiController.GetWeixinDateTime(DateTime.Now), "</CreateTime>" };
			empty = string.Concat(string.Concat(weixinDateTime), "<MsgType><![CDATA[text]]></MsgType>");
			empty = string.Concat(empty, "<Content><![CDATA[", msg, "]]></Content>");
			return string.Concat(empty, "</xml>");
		}

		private string GetResponseResult(string url)
		{
			string end;
			using (HttpWebResponse response = (HttpWebResponse)WebRequest.Create(url).GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						end = streamReader.ReadToEnd();
					}
				}
			}
			return end;
		}

		public static long GetWeixinDateTime(DateTime dateTime)
		{
			DateTime dateTime1 = new DateTime(1970, 1, 1);
			return (dateTime.Ticks - dateTime1.Ticks) / 10000000 - 28800;
		}

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[AllowAnonymous]
		public ActionResult Index()
		{
			Log.Info("进入微信API");
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			string weixinToken = "";
			string item = "";
			string str = "";
			string item1 = "";
			string str1 = "";
			weixinToken = siteSettings.WeixinToken;
			item = base.Request["signature"];
			str = base.Request["nonce"];
			item1 = base.Request["timestamp"];
			str1 = base.Request["echostr"];
			ActionResult user = base.Content("");
			if (base.Request.HttpMethod != "GET")
			{
				if (!Senparc.Weixin.MP.CheckSignature.Check(item, item1, str, weixinToken))
				{
					Log.Info("验证不通过");
				}
				XDocument xDocument = XDocument.Load(base.Request.InputStream);
				Senparc.Weixin.MP.Entities.IRequestMessageBase requestEntity = Senparc.Weixin.MP.RequestMessageFactory.GetRequestEntity(xDocument, null);
				SceneHelper sceneHelper = new SceneHelper();
				IWXCardService wXCardService = ServiceHelper.Create<IWXCardService>();
				if (requestEntity.MsgType == RequestMsgType.Event)
				{
					RequestMessageEventBase requestMessageEventBase = requestEntity as RequestMessageEventBase;
					Event @event = requestMessageEventBase.Event;
					switch (@event)
					{
						case Event.subscribe:
						{
							RequestMessageEvent_Subscribe requestMessageEventSubscribe = requestMessageEventBase as RequestMessageEvent_Subscribe;
							bool flag = false;
							if (requestMessageEventSubscribe.EventKey != string.Empty)
							{
								string str2 = requestMessageEventSubscribe.EventKey.Replace("qrscene_", string.Empty);
								SceneModel model = sceneHelper.GetModel(str2);
								if (model != null)
								{
									if (model.SceneType == QR_SCENE_Type.WithDraw)
									{
										flag = true;
										str1 = ProcessWithDrawScene(requestMessageEventSubscribe, str2, model);
										user = base.Content(str1);
									}
									else if (model.SceneType == QR_SCENE_Type.Bonus)
									{
										flag = true;
										user = SendActivityToUser(sceneHelper.GetModel(str2).Object, requestEntity);
									}
								}
							}
							if (!flag)
							{
								user = SendAttentionToUser(requestEntity);
							}
                                Subscribe(requestEntity.FromUserName);
							break;
						}
						case Event.unsubscribe:
						{
                                UnSubscribe(requestMessageEventBase.FromUserName);
							break;
						}
						case Event.CLICK:
						{
							break;
						}
						case Event.scan:
						{
							RequestMessageEvent_Scan requestMessageEventScan = requestMessageEventBase as RequestMessageEvent_Scan;
							if (string.IsNullOrWhiteSpace(requestMessageEventScan.EventKey))
							{
								break;
							}
							SceneModel sceneModel = sceneHelper.GetModel(requestMessageEventScan.EventKey);
							if (sceneModel == null || sceneModel.SceneType != QR_SCENE_Type.WithDraw)
							{
								break;
							}
							str1 = ProcessWithDrawScene(requestMessageEventScan, requestMessageEventScan.EventKey, sceneModel);
							user = base.Content(str1);
							break;
						}
						default:
						{
							switch (@event)
							{
								case Event.card_pass_check:
								{
									RequestMessageEvent_Card_Pass_Check requestMessageEventCardPassCheck = requestMessageEventBase as RequestMessageEvent_Card_Pass_Check;
									if (string.IsNullOrWhiteSpace(requestMessageEventCardPassCheck.CardId))
									{
										return user;
									}
									wXCardService.Event_Audit(requestMessageEventCardPassCheck.CardId, WXCardLogInfo.AuditStatusEnum.Audited);
									break;
								}
								case Event.card_not_pass_check:
								{
									RequestMessageEvent_Card_Pass_Check requestMessageEventCardPassCheck1 = requestMessageEventBase as RequestMessageEvent_Card_Pass_Check;
									if (string.IsNullOrWhiteSpace(requestMessageEventCardPassCheck1.CardId))
									{
										return user;
									}
									wXCardService.Event_Audit(requestMessageEventCardPassCheck1.CardId, WXCardLogInfo.AuditStatusEnum.AuditNot);
									break;
								}
								case Event.user_get_card:
								{
									RequestMessageEvent_User_Get_Card requestMessageEventUserGetCard = requestMessageEventBase as RequestMessageEvent_User_Get_Card;
									if (string.IsNullOrWhiteSpace(requestMessageEventUserGetCard.CardId) || string.IsNullOrWhiteSpace(requestMessageEventUserGetCard.UserCardCode))
									{
										return user;
									}
									wXCardService.Event_Send(requestMessageEventUserGetCard.CardId, requestMessageEventUserGetCard.UserCardCode, requestMessageEventUserGetCard.FromUserName, requestMessageEventUserGetCard.OuterId);
									break;
								}
								case Event.user_del_card:
								{
									RequestMessageEvent_User_Del_Card requestMessageEventUserDelCard = requestMessageEventBase as RequestMessageEvent_User_Del_Card;
									if (string.IsNullOrWhiteSpace(requestMessageEventUserDelCard.CardId) || string.IsNullOrWhiteSpace(requestMessageEventUserDelCard.UserCardCode))
									{
										return user;
									}
									wXCardService.Event_Unavailable(requestMessageEventUserDelCard.CardId, requestMessageEventUserDelCard.UserCardCode);
									break;
								}
							}
							break;
						}
					}
				}
			}
			else if (Senparc.Weixin.MP.CheckSignature.Check(item, item1, str, weixinToken))
			{
				user = base.Content(str1);
			}
			return user;
		}

		private string ProcessWithDrawScene(RequestMessageEventBase weixinMsg, string sceneid, SceneModel model)
		{
			try
			{
				string str = CacheKeyCollection.SceneReturn(sceneid);
				if (!(Cache.Get(str) is ApplyWithDrawInfo))
				{
					SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
					if (string.IsNullOrWhiteSpace(siteSettings.WeixinAppId) || string.IsNullOrWhiteSpace(siteSettings.WeixinAppSecret))
					{
						Log.Error("微信事件回调：未设置公众号配置参数！");
					}
					else
					{
						string str1 = AccessTokenContainer.TryGetToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, false);
						WeixinUserInfoResult userInfo = CommonApi.GetUserInfo(str1, weixinMsg.FromUserName);
						ApplyWithDrawInfo applyWithDrawInfo = new ApplyWithDrawInfo()
						{
							MemId = long.Parse(model.Object.ToString()),
							OpenId = weixinMsg.FromUserName,
							ApplyStatus = ApplyWithDrawInfo.ApplyWithDrawStatus.WaitConfirm,
							NickName = userInfo.nickname
						};
						ApplyWithDrawInfo applyWithDrawInfo1 = applyWithDrawInfo;
						if (Cache.Get(str) != null)
						{
							Cache.Remove(str);
						}
						Cache.Insert(str, applyWithDrawInfo1, 300);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(string.Concat("ProcessWithDrawScene:", exception.Message));
			}
			return string.Empty;
		}

		private ActionResult SendActivityToUser(BonusModel bonusModel, Senparc.Weixin.MP.Entities.IRequestMessageBase requestBaseMsg)
		{
			string str = (string.IsNullOrEmpty(base.Request.Url.Port.ToString()) ? "" : string.Concat(":", base.Request.Url.Port));
			object[] objArray = new object[] { "http://", base.Request.Url.Host.ToString(), str, "/m-weixin/Bonus/Index/", bonusModel.Id };
			string str1 = string.Concat(objArray);
			string str2 = string.Format(string.Concat("<a href='", str1, "'>您参加{0}，成功获得{1}赠送的红包</a>，点击去拆红包"), bonusModel.Name, bonusModel.MerchantsName);
			return new XmlResult(DealTextMsg(requestBaseMsg, str2));
		}

		private ActionResult SendActivityToUser(object sceneObj, Senparc.Weixin.MP.Entities.IRequestMessageBase requestBaseMsg)
		{
			ActionResult user;
			BonusModel bonusModel = sceneObj as BonusModel;
			if (bonusModel.Type == BonusInfo.BonusType.Activity)
			{
				try
				{
					user = SendActivityToUser(bonusModel, requestBaseMsg);
				}
				catch (Exception exception)
				{
					Log.Info("活动红包出错：", exception);
					return base.Content("");
				}
				return user;
			}
			return base.Content("");
		}

		private ActionResult SendAttentionToUser(Senparc.Weixin.MP.Entities.IRequestMessageBase requestBaseMsg)
		{
			string str = "";
			try
			{
				str = ServiceHelper.Create<IBonusService>().Receive(requestBaseMsg.FromUserName);
				if (!string.IsNullOrEmpty(str))
				{
					return new XmlResult(DealTextMsg(requestBaseMsg, str));
				}
			}
			catch (Exception exception)
			{
				Log.Info("关注红包出错：", exception);
			}
			return base.Content("");
		}

		private void Subscribe(string openId)
		{
			Task.Factory.StartNew(() => ServiceHelper.Create<IWXApiService>().Subscribe(openId));
		}

		private void UnSubscribe(string openId)
		{
			Task.Factory.StartNew(() => ServiceHelper.Create<IWXApiService>().UnSubscribe(openId));
		}

		public ContentResult VShopApi(long vshopId)
		{
			string token = ServiceHelper.Create<IVShopService>().GetVShopSetting(vshopId).Token;
			string item = base.Request["signature"];
			string str = base.Request["nonce"];
			string item1 = base.Request["timestamp"];
			string empty = base.Request["echostr"];
			if (base.Request.HttpMethod == "GET" && !Hishop.Weixin.MP.Util.CheckSignature.Check(item, item1, str, token))
			{
				empty = string.Empty;
			}
			return base.Content(empty);
		}

		public ActionResult WXAuthorize(string returnUrl = "")
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			string empty = string.Empty;
			if (!string.IsNullOrEmpty(siteSettings.WeixinAppId))
			{
				string item = base.HttpContext.Request["code"];
				if (string.IsNullOrEmpty(item))
				{
					string str = base.HttpContext.Request.Url.ToString();
					str = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect", siteSettings.WeixinAppId, HttpUtility.UrlEncode(str));
					return Redirect(str);
				}
				string responseResult = GetResponseResult(string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, item));
				if (responseResult.Contains("access_token"))
				{
					JObject jObjects = JsonConvert.DeserializeObject(responseResult) as JObject;
					string[] strArrays = new string[] { "https://api.weixin.qq.com/sns/userinfo?access_token=", jObjects["access_token"].ToString(), "&openid=", jObjects["openid"].ToString(), "&lang=zh_CN" };
					string responseResult1 = GetResponseResult(string.Concat(strArrays));
					if (responseResult1.Contains("nickname"))
					{
						JObject jObjects1 = JsonConvert.DeserializeObject(responseResult1) as JObject;
						if (base.Request["returnUrl"] != null)
						{
							string str1 = base.Request["returnUrl"].ToString();
							str1 = (!str1.Contains("?") ? "{0}?openId={1}&serviceProvider={2}&nickName={3}&headimgurl={4}&unionid={5}" : "{0}&openId={1}&serviceProvider={2}&nickName={3}&headimgurl={4}&unionid={5}");
							string str2 = str1;
							object[] objArray = new object[] { returnUrl, HttpUtility.UrlEncode(jObjects1["openid"].ToString()), HttpUtility.UrlEncode("Himall.Plugin.OAuth.WeiXin"), HttpUtility.UrlEncode(jObjects1["nickname"].ToString()), HttpUtility.UrlEncode(jObjects1["headimgurl"].ToString()), null };
							objArray[5] = (jObjects1["unionid"] != null ? jObjects1["unionid"].ToString() : jObjects1["openid"].ToString());
							str1 = string.Format(str2, objArray);
							return Redirect(str1);
						}
					}
				}
			}
			return base.Content("授权异常");
		}
	}
}