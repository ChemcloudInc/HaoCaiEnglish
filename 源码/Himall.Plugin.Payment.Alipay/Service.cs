using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using Himall.PaymentPlugin.Alipay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Himall.Plugin.Payment.Alipay
{
    public class Service : PaymentBase<Config>, IPaymentPlugin, IPlugin
    {
        private string _logo;

        public string HelpImage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Logo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_logo))
                {
                    _logo = Utility<Config>.GetConfig(base.WorkDirectory).Logo;
                }
                return _logo;
            }
            set
            {
                _logo = value;
            }
        }

        public string PluginListUrl
        {
            set
            {
                Config.PluginListUrl = value;
            }
        }

        public UrlType RequestUrlType
        {
            get
            {
                return 0;
            }
        }

        public Service()
        {
        }

        public void CheckCanEnable()
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            if (string.IsNullOrWhiteSpace(config.AlipayAccount))
            {
                throw new PluginConfigException("未设置支付宝账号");
            }
            if (string.IsNullOrWhiteSpace(config.GateWay))
            {
                throw new PluginConfigException("未设置支付宝网关");
            }
            if (string.IsNullOrWhiteSpace(config.Input_charset))
            {
                throw new PluginConfigException("未设置编码格式设置");
            }
            if (string.IsNullOrWhiteSpace(config.Key))
            {
                throw new PluginConfigException("未设置安全校验Key");
            }
            if (string.IsNullOrWhiteSpace(config.Partner))
            {
                throw new PluginConfigException("未设置合作者身份ID");
            }
            if (string.IsNullOrWhiteSpace(config.Sign_type))
            {
                throw new PluginConfigException("未设置签名方式");
            }
            if (string.IsNullOrWhiteSpace(config.VeryfyUrl))
            {
                throw new PluginConfigException("未设置支付宝确认地址");
            }
        }

        public string ConfirmPayResult()
        {
            return "success";
        }

        public FormData GetFormData()
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            FormData formDatum = new FormData();
            FormData.FormItem[] formItemArray = new FormData.FormItem[3];
            FormData.FormItem formItem = new FormData.FormItem();
            formItem.DisplayName = "合作者身份ID";
            formItem.Name = "Partner";
            formItem.IsRequired = true;
            formItem.Type = (FormData.FormItemType)1;
            formItem.Value = (config.Partner);
            formItemArray[0] = formItem;
            FormData.FormItem formItem1 = new FormData.FormItem();
            formItem1.DisplayName = ("交易安全检验码");
            formItem1.Name = ("Key");
            formItem1.IsRequired = (true);
            formItem1.Type = (FormData.FormItemType)(1);
            formItem1.Value = (config.Key);
            formItemArray[1] = formItem1;
            FormData.FormItem formItem2 = new FormData.FormItem();
            formItem2.DisplayName = ("收款支付宝帐户");
            formItem2.Name = ("AlipayAccount");
            formItem2.IsRequired = (true);
            formItem2.Type = (FormData.FormItemType)(1);
            formItem2.Value = (config.AlipayAccount);
            formItemArray[2] = formItem2;
            formDatum.Items = (formItemArray);
            return formDatum;
        }

        private NameValueCollection GetQuerystring(HttpRequestBase request)
        {
            NameValueCollection nameValueCollection;
            nameValueCollection = (!(request.HttpMethod == "POST") ? request.QueryString : request.Form);
            return nameValueCollection;
        }

        public string GetRequestUrl(string returnUrl, string notifyUrl, string orderId, decimal totalFee, string productInfo, string openId = null)
        {
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            string str = "1";
            notifyUrl = string.Format(notifyUrl, new object[0]);
            returnUrl = string.Format(returnUrl, new object[0]);
            string alipayAccount = config.AlipayAccount;
            string partner = config.Partner;
            string key = config.Key;
            string str1 = orderId.ToString();
            string str2 = productInfo;
            string str3 = "";
            string str4 = "";
            string str5 = "";
            SortedDictionary<string, string> strs = new SortedDictionary<string, string>()
            {
                { "partner", partner },
                { "return_url", returnUrl },
                { "seller_email", alipayAccount },
                { "out_trade_no", str1 },
                { "_input_charset", config.Input_charset },
                { "service", "create_direct_pay_by_user" },
                { "payment_type", str },
                { "notify_url", notifyUrl },
                { "subject", str2 },
                { "total_fee", totalFee.ToString("F2") },
                { "body", str3 },
                { "anti_phishing_key", str4 },
                { "exter_invoke_ip", str5 }
            };
            return Submit.BuildRequestUrl(strs, base.WorkDirectory, config);
        }


        void Himall.Core.Plugins.Payment.IPaymentPlugin.Disable(PlatformType platformType)
        {
            base.Disable(platformType);
        }

        void Himall.Core.Plugins.Payment.IPaymentPlugin.Enable(PlatformType platformType)
        {
            base.Enable(platformType);
        }


        bool Himall.Core.Plugins.Payment.IPaymentPlugin.IsEnable(PlatformType platformType)
        {
            return base.IsEnable(platformType);
        }

        public PaymentInfo ProcessNotify(HttpRequestBase request)
        {
            NameValueCollection querystring = GetQuerystring(request);
            SortedDictionary<string, string> requestPost = UrlHelper.GetRequestPost(querystring);
            PaymentInfo paymentInfo = null;
            if (querystring.Count <= 0)
            {
                throw new ApplicationException(string.Concat("支付宝支付Notify请求未带参数,QueryString:", querystring.ToString()));
            }
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            Notify notify = new Notify(base.WorkDirectory);
            notify.Verify(requestPost, querystring["notify_id"], config.Sign_type);
            if ((querystring["trade_status"] == "TRADE_FINISHED" ? false : !(querystring["trade_status"] == "TRADE_SUCCESS")))
            {
                throw new ApplicationException(string.Concat("支付宝支付Notify请求验证失败,QueryString:", querystring.ToString()));
            }
            string item = querystring["trade_no"];
            TypeHelper.StringToDateTime(querystring["gmt_payment"]);
            PaymentInfo paymentInfo1 = new PaymentInfo();
            PaymentInfo paymentInfo2 = paymentInfo1;
            string str = querystring["out_trade_no"];
            char[] chrArray = new char[] { ',' };
            paymentInfo2.OrderIds = (
                from t in str.Split(chrArray)
                select long.Parse(t));
            paymentInfo1.TradNo = (querystring["trade_no"]);
            paymentInfo1.TradeTime = (new DateTime?(TypeHelper.StringToDateTime(querystring["gmt_payment"])));
            paymentInfo = paymentInfo1;
            return paymentInfo;
        }

        public PaymentInfo ProcessReturn(HttpRequestBase request)
        {
            NameValueCollection querystring = GetQuerystring(request);
            SortedDictionary<string, string> requestGet = UrlHelper.GetRequestGet(querystring);
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            PaymentInfo paymentInfo = new PaymentInfo();
            if (requestGet.Count <= 0)
            {
                throw new ApplicationException(string.Concat("支付宝支付返回请求未带参数,QueryString:", querystring.ToString()));
            }
            Notify notify = new Notify(base.WorkDirectory);
            notify.Verify(requestGet, querystring["notify_id"], config.Sign_type);
            if ((querystring["trade_status"] == "TRADE_FINISHED" ? false : !(querystring["trade_status"] == "TRADE_SUCCESS")))
            {
                throw new ApplicationException(string.Concat("支付宝支付返回请求验证失败,QueryString:", querystring.ToString()));
            }
            PaymentInfo paymentInfo1 = paymentInfo;
            string item = querystring["out_trade_no"];
            char[] chrArray = new char[] { ',' };
            paymentInfo1.OrderIds = (
                from t in item.Split(chrArray)
                select long.Parse(t));
            paymentInfo.TradNo = (querystring["trade_no"]);
            paymentInfo.TradeTime = (new DateTime?(TypeHelper.StringToDateTime(querystring["notify_time"])));
            return paymentInfo;
        }

        public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
        {
            KeyValuePair<string, string> keyValuePair = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "Partner");
            if (string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                throw new PluginConfigException("合作者身份ID不能为空");
            }
            KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "Key");
            if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
            {
                throw new PluginConfigException("交易安全检验码不能为空");
            }
            KeyValuePair<string, string> keyValuePair2 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "AlipayAccount");
            if (string.IsNullOrWhiteSpace(keyValuePair2.Value))
            {
                throw new PluginConfigException("收款支付宝帐户不能为空");
            }
            Config config = Utility<Config>.GetConfig(base.WorkDirectory);
            config.AlipayAccount = keyValuePair2.Value;
            config.Key = keyValuePair1.Value;
            config.Partner = keyValuePair.Value;
            Utility<Config>.SaveConfig(config, base.WorkDirectory);
        }

     
    }
}