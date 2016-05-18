using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using Himall.WeixinPaymentBase;
using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;

namespace Himall.Plugin.Payment.WeiXinPay
{
	public class Service : ServiceBase, IPaymentPlugin, IPlugin
	{
		public string HelpImage
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public UrlType RequestUrlType
		{
			get
			{
				return UrlType.Page;
			}
		}

		public Service()
		{
		}

		public string GetRequestUrl(string returnUrl, string notifyUrl, string orderId, decimal totalFee, string productInfo, string openId = null)
		{
			string timestamp = "";
			string noncestr = "";
			string str = "";
			string str1 = orderId;
			DateTime.Now.ToString("yyyyMMdd");
			RequestHandler requestHandler = new RequestHandler();
			requestHandler.Init();
			timestamp = TenPayUtil.GetTimestamp();
			noncestr = TenPayUtil.GetNoncestr();
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			requestHandler.SetParameter("appid", config.AppId);
			requestHandler.SetParameter("mch_id", config.MCHID);
			requestHandler.SetParameter("nonce_str", noncestr);
			requestHandler.SetParameter("body", productInfo);
			requestHandler.SetParameter("out_trade_no", str1);
			int num = (int)(totalFee * new decimal(100));
			requestHandler.SetParameter("total_fee", num.ToString());
			requestHandler.SetParameter("spbill_create_ip", "222.240.184.122");
			requestHandler.SetParameter("notify_url", notifyUrl);
			requestHandler.SetParameter("trade_type", "JSAPI");
			requestHandler.SetParameter("openid", openId);
			string str2 = requestHandler.CreateMd5Sign("key", config.Key);
			requestHandler.SetParameter("sign", str2);
			string str3 = requestHandler.ParseXML();
			XDocument xDocument = XDocument.Parse(TenPayV3.Unifiedorder(str3));
			if (xDocument == null)
			{
				throw new ApplicationException(string.Concat("调用统一支付出错：请求内容：", str3));
			}
			if (xDocument.Element("xml").Element("return_code").Value == "FAIL")
			{
				throw new ApplicationException(string.Concat("预支付失败：", xDocument.Element("xml").Element("return_msg").Value));
			}
			if (xDocument.Element("xml").Element("result_code").Value == "FAIL")
			{
				throw new ApplicationException(string.Concat("预支付失败：", xDocument.Element("xml").Element("err_code_des").Value));
			}
			string value = xDocument.Element("xml").Element("prepay_id").Value;
			RequestHandler requestHandler1 = new RequestHandler();
			requestHandler1.SetParameter("appId", config.AppId);
			requestHandler1.SetParameter("timeStamp", timestamp);
			requestHandler1.SetParameter("nonceStr", noncestr);
			requestHandler1.SetParameter("package", string.Format("prepay_id={0}", value));
			requestHandler1.SetParameter("signType", "MD5");
			str = requestHandler1.CreateMd5Sign("key", config.Key);
			string str4 = string.Concat("WeixinJSBridge.invoke('getBrandWCPayRequest', {{'appId': '{0}','timeStamp': '{1}','nonceStr': '{2}','package': '{3}','signType': 'MD5','paySign': '{4}'}}, function (res) {{if (res.err_msg == 'brand_wcpay_request:ok') {{location.href='", returnUrl, "'}}else alert('支付失败！')}});");
			object[] appId = new object[] { config.AppId, timestamp, noncestr, string.Format("prepay_id={0}", value), str };
			string str5 = string.Format(str4, appId);
			return string.Format("javascript:{0}", str5);
		}

		void Himall.Core.Plugins.IPlugin.CheckCanEnable()
		{
			base.CheckCanEnable();
		}

	
		string Himall.Core.Plugins.Payment.IPaymentPlugin.ConfirmPayResult()
		{
			return base.ConfirmPayResult();
		}

		void Himall.Core.Plugins.Payment.IPaymentPlugin.Disable(PlatformType platformType)
		{
			base.Disable(platformType);
		}

		void Himall.Core.Plugins.Payment.IPaymentPlugin.Enable(PlatformType platformType)
		{
			base.Enable(platformType);
		}

	

		FormData Himall.Core.Plugins.Payment.IPaymentPlugin.GetFormData()
		{
			return base.GetFormData();
		}

		bool Himall.Core.Plugins.Payment.IPaymentPlugin.IsEnable(PlatformType platformType)
		{
			return base.IsEnable(platformType);
		}

		PaymentInfo Himall.Core.Plugins.Payment.IPaymentPlugin.ProcessNotify(HttpRequestBase httpRequestBase)
		{
			return base.ProcessNotify(httpRequestBase);
		}

		PaymentInfo Himall.Core.Plugins.Payment.IPaymentPlugin.ProcessReturn(HttpRequestBase httpRequestBase)
		{
			return base.ProcessReturn(httpRequestBase);
		}


		void Himall.Core.Plugins.Payment.IPaymentPlugin.SetFormValues(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
		{
			base.SetFormValues(keyValuePairs);
		}
	}
}