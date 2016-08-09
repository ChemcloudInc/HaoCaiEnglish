using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Service;
using Himall.ServiceProvider;
using Himall.Web.Areas.Mobile.Models;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
    public class ShopBonusController : BaseMobileTemplatesController
    {
        private IShopBonusService _bonusService;

        private SiteSettingsInfo _siteSetting;

        private IWXCardService ser_wxcard;

        private WXCardLogInfo.CouponTypeEnum ThisCouponType = WXCardLogInfo.CouponTypeEnum.Bonus;

        public ShopBonusController()
        {
            _siteSetting = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            if (string.IsNullOrWhiteSpace(_siteSetting.WeixinAppId) || string.IsNullOrWhiteSpace(_siteSetting.WeixinAppSecret))
            {
                throw new HimallException("未配置公众号参数");
            }
            _bonusService = ServiceHelper.Create<IShopBonusService>();
            ser_wxcard = ServiceHelper.Create<IWXCardService>();
        }

        public ActionResult Completed(long id, string openId = "", [DecimalConstant(0, 0, 0, 0, 0)] decimal price = default(decimal), string user = "", long grantid = 0L, long rid = 0L, string wxhead = "", string host = "")
        {
            SetShareData();
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            ViewBag.GrantId = grantid;
            ViewBag.ShareHref = string.Concat(host, "/m-weixin/shopbonus/index/", grantid.ToString());
            ViewBag.HeadImg = wxhead;
            ViewBag.ShopAddress = GetShopAddress(shopBonusModel.ShopId);
            ViewBag.UserName = user;
            ViewBag.Price = price;
            ViewBag.HighPrice = price + 60;
            ViewBag.OpenId = openId;
            ViewBag.ShopName = ServiceHelper.Create<IShopService>().GetShopName(shopBonusModel.ShopId);
            shopBonusModel.ShareImg = string.Concat("http://", host, shopBonusModel.ShareImg);
            shopBonusModel.ReceiveId = rid;
            if (shopBonusModel.SynchronizeCard && base.PlatformType == Himall.Core.PlatformType.WeiXin)
            {
                shopBonusModel.WXJSInfo = ser_wxcard.GetSyncWeiXin(id, rid, ThisCouponType, base.Request.Url.AbsoluteUri);
                if (shopBonusModel.WXJSInfo != null)
                {
                    shopBonusModel.IsShowSyncWeiXin = true;
                }
            }
            return View(shopBonusModel);
        }

        public ActionResult CompletedNotUser(long id, long rid, string openId = "", [DecimalConstant(0, 0, 0, 0, 0)] decimal price = default(decimal), long grantid = 0L, string wxhead = "", string host = "")
        {
            SetShareData();
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            ViewBag.GrantId = grantid;
            ViewBag.ShareHref = string.Concat(host, "/m-weixin/shopbonus/index/", grantid.ToString());
            ViewBag.HeadImg = wxhead;
            ViewBag.ShopAddress = GetShopAddress(shopBonusModel.ShopId);
            ViewBag.Price = price;
            ViewBag.OpenId = openId;
            ViewBag.ShopName = ServiceHelper.Create<IShopService>().GetShopName(shopBonusModel.ShopId);
            shopBonusModel.ShareImg = string.Concat("http://", host, shopBonusModel.ShareImg);
            shopBonusModel.ReceiveId = rid;
            if (shopBonusModel.SynchronizeCard && base.PlatformType == Himall.Core.PlatformType.WeiXin)
            {
                shopBonusModel.WXJSInfo = ser_wxcard.GetSyncWeiXin(id, rid, ThisCouponType, base.Request.Url.AbsoluteUri);
                if (shopBonusModel.WXJSInfo != null)
                {
                    shopBonusModel.IsShowSyncWeiXin = true;
                }
            }
            return View(shopBonusModel);
        }

        public ActionResult Expired(long id, string openId = "", long grantid = 0L, string wxhead = "")
        {
            ViewBag.GrantId = grantid;
            ViewBag.HeadImg = wxhead;
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            ViewBag.ShopAddress = GetShopAddress(shopBonusModel.ShopId);
            shopBonusModel.ShareImg = string.Concat("http://", base.Request.Url.Host.ToString(), shopBonusModel.ShareImg);
            return View(shopBonusModel);
        }

        [HttpPost]
        public ActionResult GetOtherReceive(long id)
        {
            List<ShopBonusOtherReceiveModel> shopBonusOtherReceiveModels = new List<ShopBonusOtherReceiveModel>();
            List<ShopBonusReceiveInfo> detailByGrantId = _bonusService.GetDetailByGrantId(id);
            int num = 0;
            foreach (ShopBonusReceiveInfo shopBonusReceiveInfo in detailByGrantId)
            {
                ShopBonusOtherReceiveModel shopBonusOtherReceiveModel = new ShopBonusOtherReceiveModel()
                {
                    Name = shopBonusReceiveInfo.WXName,
                    HeadImg = shopBonusReceiveInfo.WXHead,
                    Copywriter = RandomStr(num),
                    Price = shopBonusReceiveInfo.Price.Value,
                    ReceiveTime = shopBonusReceiveInfo.ReceiveTime.Value.ToString("yyyy-MM-dd HH:mm")
                };
                shopBonusOtherReceiveModels.Add(shopBonusOtherReceiveModel);
                num++;
                if (num <= 4)
                {
                    continue;
                }
                num = 0;
            }
            return Json(shopBonusOtherReceiveModels);
        }

        private string GetShopAddress(long shopid)
        {
            VShopInfo vShopByShopId = Instance<IVShopService>.Create.GetVShopByShopId(shopid);
            if (vShopByShopId == null)
            {
                return "/";
            }
            return string.Concat("/m-weixin/vshop/Detail/", vShopByShopId.Id);
        }

        [HttpPost]
        public JsonResult GetWXCardData(long id, long rid)
        {
            WXJSCardModel wXJSCardModel = new WXJSCardModel();
            if (true && base.PlatformType == Himall.Core.PlatformType.WeiXin)
            {
                wXJSCardModel = ser_wxcard.GetJSWeiXinCard(id, rid, ThisCouponType);
            }
            return Json(wXJSCardModel);
        }

        private UserInfoJson GetWXUserHead(string openid)
        {
            SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
            if (string.IsNullOrEmpty(siteSettings.WeixinAppId) && string.IsNullOrEmpty(siteSettings.WeixinAppSecret))
            {
                throw new Exception("未配置公众号");
            }
            string str = AccessTokenContainer.TryGetToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret, false);
            UserInfoJson userInfoJson = UserApi.Info(str, openid, Language.zh_CN);
            if (userInfoJson.errcode != ReturnCode.请求成功)
            {
                throw new HimallException(userInfoJson.errmsg);
            }
            return userInfoJson;
        }

        public ActionResult HasReceive(long id, string openId = "", long grantid = 0L, string wxhead = "", string host = "")
        {
            SetShareData();
            ViewBag.GrantId = grantid;
            ViewBag.ShareHref = string.Concat(host, "/m-weixin/shopbonus/index/", grantid.ToString());
            ViewBag.OpenId = openId;
            ViewBag.HeadImg = wxhead;
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            ViewBag.ShopAddress = GetShopAddress(shopBonusModel.ShopId);
            shopBonusModel.ShareImg = string.Concat("http://", host, shopBonusModel.ShareImg);
            return View(shopBonusModel);
        }

        public ActionResult HaveNot(long id, string openId = "", long grantid = 0L, string wxhead = "")
        {
            ViewBag.GrantId = grantid;
            ViewBag.HeadImg = wxhead;
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            ViewBag.ShopAddress = GetShopAddress(shopBonusModel.ShopId);
            shopBonusModel.ShareImg = string.Concat("http://", base.Request.Url.Host.ToString(), shopBonusModel.ShareImg);
            return View(shopBonusModel);
        }

        public ActionResult Index(long id)
        {
            ActionResult actionResult;
            Log.Info("进入ShopBonus Index");
            if (base.PlatformType != Himall.Core.PlatformType.WeiXin)
            {
                Log.Info(base.PlatformType.ToString());
                return base.Content("只能在微信端访问");
            }
            ShopBonusInfo byGrantId = _bonusService.GetByGrantId(id);
            if (byGrantId == null)
            {
                Log.Info(string.Concat("红包失踪，id = ", id));
                return Redirect("/m-weixin/ShopBonus/Invalidtwo");
            }
            string item = base.HttpContext.Request["code"];
            OAuthAccessTokenResult accessToken = null;
            if (string.IsNullOrEmpty(item))
            {
                string absoluteUri = base.Request.Url.AbsoluteUri;
                string authorizeUrl = OAuthApi.GetAuthorizeUrl(_siteSetting.WeixinAppId.Trim(), absoluteUri, "STATE#wechat_redirect", OAuthScope.snsapi_userinfo, "code");
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
            Log.Info("------------------------------");
            OAuthUserInfo userInfo = OAuthApi.GetUserInfo(accessToken.access_token, accessToken.openid, Language.zh_CN);
            ShopBonusModel shopBonusModel = new ShopBonusModel(byGrantId);
            if (shopBonusModel.DateEnd <= DateTime.Now || shopBonusModel.IsInvalid || shopBonusModel.BonusDateEnd <= DateTime.Now)
            {
                object[] objArray = new object[] { "/m-weixin/ShopBonus/Expired/", shopBonusModel.Id, "?openId=", accessToken.openid, "&grantid=", id, "&wxhead=", userInfo.headimgurl };
                return Redirect(string.Concat(objArray));
            }
            if (shopBonusModel.DateStart > DateTime.Now)
            {
                return Redirect(string.Concat("/m-weixin/ShopBonus/NotStart/", shopBonusModel.Id));
            }
            ShopReceiveModel shopReceiveModel = (ShopReceiveModel)_bonusService.Receive(id, userInfo.openid, userInfo.headimgurl, userInfo.nickname);
            Log.Info(string.Concat("获取State=", shopReceiveModel.State.ToString()));
            if (shopReceiveModel.State == ShopReceiveStatus.CanReceive)
            {
                object[] objArray1 = new object[] { "/m-weixin/ShopBonus/Completed/", shopBonusModel.Id, "?openId=", userInfo.openid, "&price=", shopReceiveModel.Price, "&user=", shopReceiveModel.UserName, "&grantid=", id, "&rid=", shopReceiveModel.Id, "&wxhead=", userInfo.headimgurl, "&host=", base.Request.Url.Host };
                return Redirect(string.Concat(objArray1));
            }
            if (shopReceiveModel.State == ShopReceiveStatus.CanReceiveNotUser)
            {
                object[] objArray2 = new object[] { "/m-weixin/ShopBonus/CompletedNotUser/", shopBonusModel.Id, "?openId=", userInfo.openid, "&price=", shopReceiveModel.Price, "&grantid=", id, "&rid=", shopReceiveModel.Id, "&wxhead=", userInfo.headimgurl, "&host=", base.Request.Url.Host };
                return Redirect(string.Concat(objArray2));
            }
            if (shopReceiveModel.State == ShopReceiveStatus.Receive)
            {
                object[] objArray3 = new object[] { "/m-weixin/ShopBonus/HasReceive/", shopBonusModel.Id, "?openId=", userInfo.openid, "&grantid=", id, "&wxhead=", userInfo.headimgurl, "&host=", base.Request.Url.Host };
                return Redirect(string.Concat(objArray3));
            }
            if (shopReceiveModel.State == ShopReceiveStatus.HaveNot)
            {
                object[] objArray4 = new object[] { "/m-weixin/ShopBonus/HaveNot/", shopBonusModel.Id, "?openId=", userInfo.openid, "&grantid=", id, "&wxhead=", userInfo.headimgurl };
                return Redirect(string.Concat(objArray4));
            }
            if (shopReceiveModel.State != ShopReceiveStatus.Invalid)
            {
                throw new Exception("领取发生异常");
            }
            object[] objArray5 = new object[] { "/m-weixin/ShopBonus/Expired/", shopBonusModel.Id, "?openId=", userInfo.openid, "&grantid=", id, "&wxhead=", userInfo.headimgurl };
            return Redirect(string.Concat(objArray5));
        }

        public ActionResult Invalidtwo()
        {
            return View();
        }

        public ActionResult NotStart(long id)
        {
            ShopBonusModel shopBonusModel = new ShopBonusModel(_bonusService.Get(id));
            shopBonusModel.ShareImg = string.Concat("http://", base.Request.Url.Host.ToString(), shopBonusModel.ShareImg);
            return View(shopBonusModel);
        }

        private string RandomStr(int index)
        {
            string[] strArrays = new string[] { "手气不错，以后购物就来这家店了", "抢红包，姿势我最帅", "人品攒的好，红包来的早", "这个发红包的老板好帅", "多谢，老板和宝贝一样靠谱" };
            return strArrays[index];
        }

        private void SetShareData()
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
        }
    }
}