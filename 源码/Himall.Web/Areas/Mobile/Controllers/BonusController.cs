using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Service;
using Himall.Web;
using Himall.Web.Areas.Admin.Models.Market;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
    public class BonusController : BaseMobileTemplatesController
    {
        private IBonusService _bonusService;

        private SiteSettingsInfo _siteSetting;

        public BonusController()
        {
            _siteSetting = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            if (string.IsNullOrWhiteSpace(_siteSetting.WeixinAppId) || string.IsNullOrWhiteSpace(_siteSetting.WeixinAppSecret))
            {
                throw new HimallException("未配置公众号参数");
            }
            _bonusService = ServiceHelper.Create<IBonusService>();
        }

        public ActionResult Completed(long id, string openId = "", [DecimalConstant(0, 0, 0, 0, 0)] decimal price = default(decimal))
        {
            IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
            string ticket = wXApiService.GetTicket(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret);
            JSSDKHelper jSSDKHelper = new JSSDKHelper();
            string timestamp = JSSDKHelper.GetTimestamp();
            string noncestr = JSSDKHelper.GetNoncestr();
            string signature = jSSDKHelper.GetSignature(ticket, noncestr, timestamp, base.Request.Url.AbsoluteUri);
            ViewBag.Timestamp = timestamp;
            ViewBag.NonceStr = noncestr;
            ViewBag.Signature = signature;
            ViewBag.AppId = _siteSetting.WeixinAppId;
            ViewBag.Price = price;
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id))
            {
            };
            bonusModel.ImagePath = string.Concat("http://", base.Request.Url.Host.ToString(), bonusModel.ImagePath);

            ViewBag.OpenId = openId;
            return View(bonusModel);
        }

        public ActionResult HasReceive(long id)
        {
            IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
            string ticket = wXApiService.GetTicket(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret);
            JSSDKHelper jSSDKHelper = new JSSDKHelper();
            string timestamp = JSSDKHelper.GetTimestamp();
            string noncestr = JSSDKHelper.GetNoncestr();
            string signature = jSSDKHelper.GetSignature(ticket, noncestr, timestamp, base.Request.Url.AbsoluteUri);
            ViewBag.Timestamp = timestamp;
            ViewBag.NonceStr = noncestr;
            ViewBag.Signature = signature;
            ViewBag.AppId = _siteSetting.WeixinAppId;
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id))
            {
            };
            bonusModel.ImagePath = string.Concat("http://", base.Request.Url.Host.ToString(), bonusModel.ImagePath);

            return View(bonusModel);
        }

        public ActionResult HaveNot(long id)
        {
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id));
            return View(bonusModel);
        }

        public ActionResult Index(long id)
        {
            ActionResult actionResult;
            if (base.PlatformType != Himall.Core.PlatformType.WeiXin)
            {
                return base.Content("只能在微信端访问");
            }
            BonusInfo bonusInfo = _bonusService.Get(id);
            if (bonusInfo == null)
            {
                return Redirect("/m-weixin/Bonus/Invalidtwo");
            }
            BonusModel bonusModel = new BonusModel(bonusInfo);
            string item = base.HttpContext.Request["code"];
            OAuthAccessTokenResult accessToken = null;
            if (string.IsNullOrEmpty(item))
            {
                string absoluteUri = base.Request.Url.AbsoluteUri;
                string authorizeUrl = OAuthApi.GetAuthorizeUrl(_siteSetting.WeixinAppId.Trim(), absoluteUri, "123321#wechat_redirect", OAuthScope.snsapi_base, "code");
                return Redirect(authorizeUrl);
            }
            try
            {
                accessToken = OAuthApi.GetAccessToken(_siteSetting.WeixinAppId.Trim(), _siteSetting.WeixinAppSecret.Trim(), item, "authorization_code");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                actionResult = base.Content(((exception.InnerException == null ? exception : exception.InnerException)).Message);
                return actionResult;
            }
            if (bonusModel.Type == BonusInfo.BonusType.Attention)
            {
                throw new Exception("红包异常");
            }
            if (bonusModel.EndTime <= DateTime.Now || bonusModel.IsInvalid)
            {
                return Redirect(string.Concat("/m-weixin/Bonus/Invalid/", bonusModel.Id));
            }
            if (bonusModel.StartTime > DateTime.Now)
            {
                object[] objArray = new object[] { "/m-weixin/Bonus/NotStart/", bonusModel.Id, "?openId=", accessToken.openid };
                return Redirect(string.Concat(objArray));
            }
            IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
            string ticket = wXApiService.GetTicket(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret);
            JSSDKHelper jSSDKHelper = new JSSDKHelper();
            string timestamp = JSSDKHelper.GetTimestamp();
            string noncestr = JSSDKHelper.GetNoncestr();
            string signature = jSSDKHelper.GetSignature(ticket, noncestr, timestamp, base.Request.Url.AbsoluteUri);
            ViewBag.Timestamp = timestamp;
            ViewBag.NonceStr = noncestr;
            ViewBag.Signature = signature;
            ViewBag.AppId = _siteSetting.WeixinAppId;
            ViewBag.OpenId = accessToken.openid;
            bonusModel.ImagePath = string.Concat("http://", base.Request.Url.Host.ToString(), bonusModel.ImagePath);
            return View(bonusModel);
        }

        public ActionResult Invalid(long id)
        {
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id));
            return View(bonusModel);
        }

        public ActionResult Invalidtwo()
        {
            return View();
        }

        public ActionResult NotAttention(long id)
        {
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id));
            string str = AccessTokenContainer.TryGetToken(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret, false);
            SceneHelper sceneHelper = new SceneHelper();
            int num = sceneHelper.SetModel(new SceneModel(QR_SCENE_Type.Bonus, bonusModel), 600);
            string str1 = QrCodeApi.Create(str, 86400, num, 10000).ticket;
            ViewBag.ticket = str1;
            return View("~/Areas/Mobile/Templates/Default/Views/Bonus/NotAttention.cshtml", bonusModel);
        }

        public ActionResult NotStart(long id, string openId = "")
        {
            IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
            string ticket = wXApiService.GetTicket(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret);
            JSSDKHelper jSSDKHelper = new JSSDKHelper();
            string timestamp = JSSDKHelper.GetTimestamp();
            string noncestr = JSSDKHelper.GetNoncestr();
            string signature = jSSDKHelper.GetSignature(ticket, noncestr, timestamp, base.Request.Url.AbsoluteUri);
            ViewBag.Timestamp = timestamp;
            ViewBag.NonceStr = noncestr;
            ViewBag.Signature = signature;
            ViewBag.AppId = _siteSetting.WeixinAppId;
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id))
            {
            };
            bonusModel.ImagePath = string.Concat("http://", base.Request.Url.Host.ToString(), bonusModel.ImagePath);

            ViewBag.OpenId = openId;
            return View(bonusModel);
        }

        [HttpPost]
        public ActionResult Receive(long id, string openId = "")
        {
            ReceiveModel receiveModel = (ReceiveModel)_bonusService.Receive(id, openId);
            BonusModel bonusModel = new BonusModel(_bonusService.Get(id));
            if (receiveModel.State == ReceiveStatus.CanReceive)
            {
                object[] objArray = new object[] { "/m-weixin/Bonus/Completed/", bonusModel.Id, "?openId=", openId, "&price=", receiveModel.Price };
                return Redirect(string.Concat(objArray));
            }
            if (receiveModel.State == ReceiveStatus.Receive)
            {
                return Redirect(string.Concat("/m-weixin/Bonus/HasReceive/", bonusModel.Id));
            }
            if (receiveModel.State == ReceiveStatus.HaveNot)
            {
                return Redirect(string.Concat("/m-weixin/Bonus/HaveNot/", bonusModel.Id));
            }
            if (receiveModel.State == ReceiveStatus.NotAttention)
            {
                return Redirect(string.Concat("/m-weixin/Bonus/NotAttention/", bonusModel.Id));
            }
            if (receiveModel.State != ReceiveStatus.Invalid)
            {
                throw new Exception("领取发生异常");
            }
            return Redirect(string.Concat("/m-weixin/Bonus/Invalid/", bonusModel.Id));
        }

        [HttpPost]
        public ActionResult SetShare(long id, string openId = "")
        {
            _bonusService.SetShare(id, openId);
            return Json(true);
        }
    }
}