using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Himall.Service.Weixin;
using Himall.ServiceProvider;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
    public class WXCardService : ServiceBase, IWXCardService, IService, IDisposable
    {
        private int MaxStock = 1000000;

        private WXHelper wxhelper;

        public WXCardService()
        {
        }

        public bool Add(WXCardLogInfo info)
        {
            string str;
            bool flag = false;
            string accessToken = "";
            string str1 = string.Concat("http://", WebHelper.GetHost());
            string str2 = string.Concat(str1, "/images/defaultwxlogo.png");
            SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
            IShopService create = Instance<IShopService>.Create;
            IVShopService vShopService = Instance<IVShopService>.Create;
            CardCreateResultJson cardCreateResultJson = new CardCreateResultJson();
            Card_BaseInfoBase cardBaseInfoBase = new Card_BaseInfoBase()
            {
                logo_url = str2,
                brand_name = siteSettings.SiteName,
                code_type = Card_CodeType.CODE_TYPE_BARCODE,
                title = info.CardTitle,
                sub_title = info.CardSubTitle,
                color = info.CardColor,
                notice = string.Concat("专供", siteSettings.SiteName, "使用")
            };
            Card_BaseInfoBase cardBaseInfoBase1 = cardBaseInfoBase;
            string[] strArrays = new string[5];
            string[] strArrays1 = strArrays;
            if (info.LeastCost > 0)
            {
                int leastCost = info.LeastCost / 100;
                str = string.Concat("满￥", leastCost.ToString("F2"), "使用");
            }
            else
            {
                str = "无门槛使用";
            }
            strArrays1[0] = str;
            strArrays[1] = "，有效期至";
            strArrays[2] = info.BeginTime.ToString("yyyy年MM月dd日");
            strArrays[3] = "-";
            strArrays[4] = info.EndTime.ToString("yyyy年MM月dd日");
            cardBaseInfoBase1.description = string.Concat(strArrays);
            Card_BaseInfo_DateInfo cardBaseInfoDateInfo = new Card_BaseInfo_DateInfo()
            {
                type = Card_DateInfo_Type.DATE_TYPE_FIX_TIME_RANGE.ToString(),
                begin_timestamp = Senparc.Weixin.Helpers.DateTimeHelper.GetWeixinDateTime(info.BeginTime),
                end_timestamp = Senparc.Weixin.Helpers.DateTimeHelper.GetWeixinDateTime(info.EndTime)
            };
            cardBaseInfoBase.date_info = cardBaseInfoDateInfo;
            Card_BaseInfoBase cardBaseInfoBase2 = cardBaseInfoBase;
            Card_BaseInfo_Sku cardBaseInfoSku = new Card_BaseInfo_Sku()
            {
                quantity = (info.Quantity == 0 ? MaxStock : info.Quantity)
            };
            cardBaseInfoBase2.sku = cardBaseInfoSku;
            cardBaseInfoBase.get_limit = (info.GetLimit == 0 ? MaxStock : info.GetLimit);
            cardBaseInfoBase.use_custom_code = false;
            cardBaseInfoBase.bind_openid = false;
            cardBaseInfoBase.can_share = false;
            cardBaseInfoBase.can_give_friend = false;
            cardBaseInfoBase.custom_url_name = "立即使用";
            Card_BaseInfoBase shopName = cardBaseInfoBase;
            Card_GeneralCouponData cardGeneralCouponDatum = new Card_GeneralCouponData()
            {
                base_info = shopName,
                default_detail = info.DefaultDetail
            };
            Card_GeneralCouponData cardGeneralCouponDatum1 = cardGeneralCouponDatum;
            if (info.ShopId > 0)
            {
                long shopId = info.ShopId;
                shopName.custom_url = string.Concat(str1, "/Shop/Home/", shopId.ToString());
            }
            WXShopInfo vShopSetting = vShopService.GetVShopSetting(info.ShopId);
            VShopInfo vShopByShopId = vShopService.GetVShopByShopId(info.ShopId);
            ShopInfo shop = create.GetShop(info.ShopId, false);
            if (vShopSetting != null && shop != null && vShopByShopId != null && !string.IsNullOrWhiteSpace(vShopSetting.AppId) && !string.IsNullOrWhiteSpace(vShopSetting.AppSecret))
            {
                accessToken = GetAccessToken(vShopSetting.AppId, vShopSetting.AppSecret);
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    shopName.brand_name = shop.ShopName;
                    if (!string.IsNullOrWhiteSpace(vShopByShopId.WXLogo))
                    {
                        shopName.logo_url = string.Concat(str1, vShopByShopId.WXLogo);
                    }
                    cardCreateResultJson = CardApi.CreateCard(accessToken, cardGeneralCouponDatum1, 10000);
                    if (cardCreateResultJson.errcode == ReturnCode.请求成功)
                    {
                        info.AppId = vShopSetting.AppId;
                        info.AppSecret = vShopSetting.AppSecret;
                        info.CardId = cardCreateResultJson.card_id;
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                if (!string.IsNullOrWhiteSpace(siteSettings.WeixinAppId) && !string.IsNullOrWhiteSpace(siteSettings.WeixinAppSecret))
                {
                    accessToken = GetAccessToken(siteSettings.WeixinAppId, siteSettings.WeixinAppSecret);
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        shopName.brand_name = shop.ShopName;
                        if (!string.IsNullOrWhiteSpace(siteSettings.WXLogo))
                        {
                            shopName.logo_url = string.Concat(str1, siteSettings.WXLogo);
                        }
                        cardCreateResultJson = CardApi.CreateCard(accessToken, cardGeneralCouponDatum1, 10000);
                        if (cardCreateResultJson.errcode == ReturnCode.请求成功)
                        {
                            info.AppId = siteSettings.WeixinAppId;
                            info.AppSecret = siteSettings.WeixinAppSecret;
                            info.CardId = cardCreateResultJson.card_id;
                            flag = true;
                        }
                    }
                }
                if (info.ShopId < 1)
                {
                    shopName.custom_url = string.Concat(str1, "/");
                }
            }
            if (flag)
            {
                info.AuditStatus = new int?(0);
                context.WXCardLogInfo.Add(info);
                context.SaveChanges();
            }
            return flag;
        }

        public void Consume(string cardid, string code)
        {
            WXCardCodeLogInfo nullable = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => (d.CardId == cardid) && (d.Code == code));
            if (nullable != null)
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == nullable.CardLogId.Value);
                if (wXCardLogInfo != null)
                {
                    string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                    if (!string.IsNullOrWhiteSpace(accessToken) && !string.IsNullOrWhiteSpace(nullable.Code))
                    {
                        WxJsonResult wxJsonResult = CardApi.CardUnavailable(accessToken, nullable.Code, nullable.CardId, 10000);
                        if (wxJsonResult.errcode != ReturnCode.请求成功)
                        {
                            int num = (int)wxJsonResult.errcode;
                            Log.Error("微信同步使用卡券失败", new Exception(string.Concat(num.ToString(), ":", wxJsonResult.errmsg)));
                        }
                    }
                }
                nullable.CodeStatus = 2;
                nullable.UsedTime = new DateTime?(DateTime.Now);
                context.SaveChanges();
            }
        }

        public void Consume(long id)
        {
            WXCardCodeLogInfo wXCardCodeLogInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.Id == id);
            if (wXCardCodeLogInfo != null)
            {
                Consume(wXCardCodeLogInfo.CardId, wXCardCodeLogInfo.Code);
            }
        }

        public void Consume(long couponcodeid, WXCardLogInfo.CouponTypeEnum coupontype)
        {
            WXCardCodeLogInfo wXCardCodeLogInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.CouponCodeId == couponcodeid && (int?)d.CouponType == (int?)coupontype);
            if (wXCardCodeLogInfo != null)
            {
                Consume(wXCardCodeLogInfo.CardId, wXCardCodeLogInfo.Code);
            }
        }

        public void Delete(string cardid)
        {
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
            if (wXCardLogInfo != null)
            {
                Delete(wXCardLogInfo.Id);
            }
        }

        public void Delete(long id)
        {
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == id);
            if (wXCardLogInfo != null)
            {
                string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                List<WXCardCodeLogInfo> list = (
                    from d in context.WXCardCodeLogInfo
                    where d.CardId == wXCardLogInfo.CardId
                    select d).ToList();
                foreach (WXCardCodeLogInfo wXCardCodeLogInfo in list)
                {
                    Consume(wXCardCodeLogInfo.Id);
                }
                context.WXCardLogInfo.Remove(wXCardLogInfo);
                CardDeleteResultJson cardDeleteResultJson = CardApi.CardDelete(accessToken, wXCardLogInfo.CardId, 10000);
                if (cardDeleteResultJson.errcode != ReturnCode.请求成功)
                {
                    int num = (int)cardDeleteResultJson.errcode;
                    Log.Error("微信同步删除卡券失败", new Exception(num.ToString()));
                }
                context.SaveChanges();
            }
        }

        public void EditGetLimit(int? num, string cardid)
        {
            Func<string, WxJsonResult> fun = null;
            if (!num.HasValue)
            {
                num = 0;
            }
            WXCardLogInfo carddata = context.WXCardLogInfo.FirstOrDefault(o => o.CardId == cardid);
            if (carddata != null)
            {
                string accessToken = GetAccessToken(carddata.AppId, carddata.AppSecret);
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    if (fun == null)
                    {
                        fun = delegate (string accessToken2)
                        {
                            string urlFormat = string.Format("https://api.weixin.qq.com/card/update?access_token={0}", accessToken2);
                            var data = new
                            {
                                card_id = carddata.CardId,
                                general_coupon = new { base_info = new { get_limit = num } }
                            };
                            return CommonJsonSend.Send<WxJsonResult>(null, urlFormat, data, CommonJsonSendType.POST, 0x2710, false);
                        };
                    }
                    WxJsonResult result = ApiHandlerWapper.TryCommonApi(fun, accessToken, true);
                    if (result.errcode != ReturnCode.请求成功)
                    {
                        Log.Error("微信同步修改卡券个人限领失败", new Exception(((int)result.errcode).ToString() + ":" + result.errmsg));
                    }
                }
            }
        }


        public void EditGetLimit(int? num, long id)
        {
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == id);
            if (wXCardLogInfo == null)
            {
                throw new HimallException("错误的数据编号");
            }
            EditGetLimit(num, wXCardLogInfo.CardId);
        }

        public void EditStock(int num, string cardid)
        {
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
            if (wXCardLogInfo != null)
            {
                string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    CardDetailGetResultJson cardDetailGetResultJson = CardApi.CardDetailGet(accessToken, wXCardLogInfo.CardId, 10000);
                    if (cardDetailGetResultJson != null)
                    {
                        int num1 = num - cardDetailGetResultJson.card.general_coupon.base_info.sku.quantity;
                        if (num1 != 0)
                        {
                            WxJsonResult wxJsonResult = CardApi.ModifyStock(accessToken, wXCardLogInfo.CardId, (num1 > 0 ? num1 : 0), (num1 < 0 ? Math.Abs(num1) : 0), 10000);
                            if (wxJsonResult.errcode != ReturnCode.请求成功)
                            {
                                int num2 = (int)wxJsonResult.errcode;
                                Log.Error("微信同步修改卡券库存失败", new Exception(num2.ToString()));
                            }
                        }
                    }
                }
            }
        }

        public void EditStock(int num, long id)
        {
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == id);
            if (wXCardLogInfo == null)
            {
                throw new HimallException("错误的数据编号");
            }
            EditStock(num, wXCardLogInfo.CardId);
        }

        public void Event_Audit(string cardid, WXCardLogInfo.AuditStatusEnum auditstatus)
        {
            WXCardLogInfo nullable = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
            if (nullable != null)
            {
                WXCardLogInfo.CouponTypeEnum? couponType = nullable.CouponType;
                WXCardLogInfo.CouponTypeEnum valueOrDefault = couponType.GetValueOrDefault();
                if (couponType.HasValue && valueOrDefault == WXCardLogInfo.CouponTypeEnum.Coupon && nullable.CouponId.HasValue)
                {
                    Instance<ICouponService>.Create.SyncWeixinCardAudit(nullable.CouponId.Value, cardid, auditstatus);
                }
                nullable.AuditStatus = (int)(auditstatus);
                context.SaveChanges();
            }
        }

        public void Event_Send(string cardid, string code, string openid, int outerid)
        {
            if (!string.IsNullOrWhiteSpace(cardid) && !string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(openid))
            {
                long num = outerid;
                WXCardCodeLogInfo nullable = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => (d.CardId == cardid) && d.Id == num);
                if (nullable != null)
                {
                    nullable.Code = code;
                    nullable.CodeStatus = 1;
                    nullable.OpenId = openid;
                    nullable.SendTime = new DateTime?(DateTime.Now);
                }
                context.SaveChanges();
            }
        }

        public void Event_Unavailable(string cardid, string code)
        {
            if (!string.IsNullOrWhiteSpace(cardid) && !string.IsNullOrWhiteSpace(code))
            {
                WXCardCodeLogInfo wXCardCodeLogInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => (d.CardId == cardid) && (d.Code == code));
                if (wXCardCodeLogInfo != null)
                {
                    context.WXCardCodeLogInfo.Remove(wXCardCodeLogInfo);
                    context.SaveChanges();
                }
            }
        }

        public WXCardLogInfo Get(long id)
        {
            return context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == id);
        }

        public WXCardLogInfo Get(string cardid)
        {
            return context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
        }

        public WXCardLogInfo Get(long couponId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            return context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CouponId == couponId && (int?)d.CouponType == (int?)couponType);
        }

        private string GetAccessToken(string appid, string secret)
        {
            wxhelper = new WXHelper();
            return wxhelper.GetAccessToken(appid, secret, false);
        }

        private string GetCardJSApiTicket(string accessToken)
        {
            wxhelper = new WXHelper();
            return wxhelper.GetTicketByToken(accessToken, "wx_card", false);
        }

        public string GetCardReceiveUrl(string cardid, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            string str = "";
            if (!string.IsNullOrWhiteSpace(cardid))
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
                if (wXCardLogInfo != null)
                {
                    string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                    int num = (int)SyncCouponRecordInfo(cardid, couponRecordId, couponType);
                    CreateQRResultJson createQRResultJson = CardApi.CreateQR(accessToken, wXCardLogInfo.CardId, null, null, null, false, null, num, 10000);
                    if (createQRResultJson.errcode != ReturnCode.请求成功)
                    {
                        int num1 = (int)createQRResultJson.errcode;
                        Log.Info(string.Concat("[Coupon]", num1.ToString(), ":", createQRResultJson.errmsg));
                    }
                    else
                    {
                        str = createQRResultJson.url;
                    }
                }
            }
            return str;
        }

        public string GetCardReceiveUrl(long id, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            string cardId = "";
            WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == id);
            if (wXCardLogInfo != null)
            {
                cardId = wXCardLogInfo.CardId;
            }
            return GetCardReceiveUrl(cardId, couponRecordId, couponType);
        }

        public WXCardCodeLogInfo GetCodeInfo(long id)
        {
            return context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.Id == id);
        }

        public WXCardCodeLogInfo GetCodeInfo(string cardid, string code)
        {
            return context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => (d.CardId == cardid) && (d.Code == code));
        }

        public WXCardCodeLogInfo GetCodeInfo(long couponCodeId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            return context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.CouponCodeId == couponCodeId && (int?)d.CouponType == (int?)couponType);
        }

        private string GetJSApiTicket(string appid, string secret)
        {
            wxhelper = new WXHelper();
            return wxhelper.GetTicket(appid, secret, "jsapi");
        }

        private string GetJSApiTicket(string accessToken)
        {
            wxhelper = new WXHelper();
            return wxhelper.GetTicketByToken(accessToken, "jsapi", false);
        }

        public WXJSCardModel GetJSWeiXinCard(long couponid, long couponcodeid, WXCardLogInfo.CouponTypeEnum couponType)
        {
            WXJSCardModel wXJSCardModel = new WXJSCardModel()
            {
                cardId = "0"
            };
            bool flag = false;
            WXCardLogInfo wXCardLogInfo = null;
            WXCardCodeLogInfo codeInfo = null;
            wXCardLogInfo = Get(couponid, couponType);
            if (wXCardLogInfo != null)
            {
                int? auditStatus = wXCardLogInfo.AuditStatus;
                if ((auditStatus.GetValueOrDefault() != 1 ? false : auditStatus.HasValue))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                codeInfo = GetCodeInfo(couponcodeid, couponType);
                if (codeInfo != null && codeInfo.CodeStatus != 0)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                WXSyncJSInfoCardInfo wXSyncJSInfoCardInfo = MakeSyncWXJSInfo(wXCardLogInfo.CardId, couponcodeid, couponType);
                if (wXSyncJSInfoCardInfo != null)
                {
                    wXJSCardModel.cardId = wXSyncJSInfoCardInfo.card_id;
                    wXJSCardModel.cardExt = new WXJSCardExtModel()
                    {
                        signature = wXSyncJSInfoCardInfo.signature,
                        timestamp = wXSyncJSInfoCardInfo.timestamp,
                        nonce_str = wXSyncJSInfoCardInfo.nonce_str,
                        outer_id = wXSyncJSInfoCardInfo.outerid
                    };
                }
            }
            return wXJSCardModel;
        }

        public WXSyncJSInfoByCard GetSyncWeiXin(long couponid, long couponcodeid, WXCardLogInfo.CouponTypeEnum couponType, string url)
        {
            WXSyncJSInfoByCard wXSyncJSInfo = null;
            bool flag = false;
            WXCardLogInfo wXCardLogInfo = null;
            WXCardCodeLogInfo codeInfo = null;
            wXCardLogInfo = Get(couponid, couponType);
            if (wXCardLogInfo != null)
            {
                int? auditStatus = wXCardLogInfo.AuditStatus;
                if ((auditStatus.GetValueOrDefault() != 1 ? false : auditStatus.HasValue))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                codeInfo = GetCodeInfo(couponcodeid, couponType);
                if (codeInfo != null && codeInfo.CodeStatus != 0)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                wXSyncJSInfo = GetWXSyncJSInfo(wXCardLogInfo.CardId, url);
            }
            return wXSyncJSInfo;
        }

        private WXSyncJSInfoByCard GetWXSyncJSInfo(string cardid, string url)
        {
            WXSyncJSInfoByCard wXSyncJSInfoByCard = null;
            if (!string.IsNullOrWhiteSpace(cardid))
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
                if (wXCardLogInfo != null)
                {
                    string jSApiTicket = GetJSApiTicket(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                    if (string.IsNullOrWhiteSpace(jSApiTicket))
                    {
                        Log.Info("[Coupon]票据获取失败");
                    }
                    else
                    {
                        wXSyncJSInfoByCard = new WXSyncJSInfoByCard();
                        JSSDKHelper jSSDKHelper = new JSSDKHelper();
                        wXSyncJSInfoByCard.appid = wXCardLogInfo.AppId;
                        wXSyncJSInfoByCard.apiticket = jSApiTicket;
                        wXSyncJSInfoByCard.timestamp = JSSDKHelper.GetTimestamp();
                        wXSyncJSInfoByCard.nonceStr = JSSDKHelper.GetNoncestr();
                        wXSyncJSInfoByCard.signature = jSSDKHelper.GetSignature(wXSyncJSInfoByCard.apiticket, wXSyncJSInfoByCard.nonceStr, wXSyncJSInfoByCard.timestamp, url);
                    }
                }
            }
            return wXSyncJSInfoByCard;
        }

        private WXSyncJSInfoCardInfo MakeSyncWXJSInfo(string cardid, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            WXSyncJSInfoCardInfo wXSyncJSInfoCardInfo = null;
            if (!string.IsNullOrWhiteSpace(cardid))
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
                if (wXCardLogInfo != null)
                {
                    string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                    string cardJSApiTicket = GetCardJSApiTicket(accessToken);
                    if (string.IsNullOrWhiteSpace(cardJSApiTicket))
                    {
                        Log.Info("[Coupon]票据获取失败");
                    }
                    else
                    {
                        wXSyncJSInfoCardInfo = new WXSyncJSInfoCardInfo();
                        int num = (int)SyncCouponRecordInfo(cardid, couponRecordId, couponType);
                        JSSDKHelper jSSDKHelper = new JSSDKHelper();
                        wXSyncJSInfoCardInfo.card_id = cardid;
                        wXSyncJSInfoCardInfo.timestamp = JSSDKHelper.GetTimestamp();
                        wXSyncJSInfoCardInfo.nonce_str = "";
                        wXSyncJSInfoCardInfo.signature = jSSDKHelper.GetCardSign2015(cardJSApiTicket, wXSyncJSInfoCardInfo.nonce_str, wXSyncJSInfoCardInfo.timestamp, wXSyncJSInfoCardInfo.card_id, "", "");
                        wXSyncJSInfoCardInfo.outerid = num;
                    }
                }
            }
            return wXSyncJSInfoCardInfo;
        }

        private long SyncCouponRecordInfo(string cardid, long couponRecordId, WXCardLogInfo.CouponTypeEnum couponType)
        {
            long id = 0;
            if (!string.IsNullOrWhiteSpace(cardid))
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.CardId == cardid);
                if (wXCardLogInfo != null)
                {
                    WXCardCodeLogInfo wXCardCodeLogInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.CouponCodeId == couponRecordId && (int?)d.CouponType == (int?)couponType);
                    if (wXCardCodeLogInfo == null)
                    {
                        wXCardCodeLogInfo = new WXCardCodeLogInfo()
                        {
                            CardId = cardid,
                            CodeStatus = 0,
                            CouponType = new WXCardLogInfo.CouponTypeEnum?(couponType),
                            CouponCodeId = new long?(couponRecordId),
                            SendTime = new DateTime?(DateTime.Now),
                            CardLogId = new long?(wXCardLogInfo.Id)
                        };
                        context.WXCardCodeLogInfo.Add(wXCardCodeLogInfo);
                        context.SaveChanges();
                        WXCardLogInfo.CouponTypeEnum? nullable = wXCardCodeLogInfo.CouponType;
                        WXCardLogInfo.CouponTypeEnum valueOrDefault = nullable.GetValueOrDefault();
                        if (nullable.HasValue && valueOrDefault == WXCardLogInfo.CouponTypeEnum.Coupon)
                        {
                            CouponRecordInfo couponRecordInfo = context.CouponRecordInfo.FirstOrDefault((CouponRecordInfo d) => d.Id == couponRecordId);
                            if (couponRecordInfo != null)
                            {
                                couponRecordInfo.WXCodeId = new long?(wXCardCodeLogInfo.Id);
                            }
                        }
                        context.SaveChanges();
                    }
                    id = wXCardCodeLogInfo.Id;
                }
            }
            return id;
        }

        public void Unavailable(string cardid, string code)
        {
            WXCardCodeLogInfo nullable = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => (d.CardId == cardid) && (d.Code == code));
            if (nullable != null)
            {
                WXCardLogInfo wXCardLogInfo = context.WXCardLogInfo.FirstOrDefault((WXCardLogInfo d) => d.Id == nullable.CardLogId.Value);
                if (wXCardLogInfo != null)
                {
                    string accessToken = GetAccessToken(wXCardLogInfo.AppId, wXCardLogInfo.AppSecret);
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        WxJsonResult wxJsonResult = CardApi.CardUnavailable(accessToken, nullable.Code, nullable.CardId, 10000);
                        if (wxJsonResult.errcode != ReturnCode.请求成功)
                        {
                            int num = (int)wxJsonResult.errcode;
                            Log.Error("微信同步修改卡券库存失败", new Exception(string.Concat(num.ToString(), ":", wxJsonResult.errmsg)));
                        }
                    }
                }
                nullable.CodeStatus = -1;
                nullable.UsedTime = new DateTime?(DateTime.Now);
                context.SaveChanges();
            }
        }

        public void Unavailable(long id)
        {
            WXCardCodeLogInfo wXCardCodeLogInfo = context.WXCardCodeLogInfo.FirstOrDefault((WXCardCodeLogInfo d) => d.Id == id);
            if (wXCardCodeLogInfo != null)
            {
                Unavailable(wXCardCodeLogInfo.CardId, wXCardCodeLogInfo.Code);
            }
        }
    }
}