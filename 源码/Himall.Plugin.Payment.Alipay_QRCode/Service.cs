using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Xml;

namespace Himall.Plugin.Payment.Alipay_QRCode
{
	public class Service : PaymentBase<Config>, IPaymentPlugin, IPlugin
	{
		private static Config _config;

		private string _HelpImage = string.Empty;

		private string _logo;

		public string HelpImage
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_HelpImage))
				{
                    _HelpImage = Utility<Config>.GetConfig(base.WorkDirectory).HelpImage;
				}
				return _HelpImage;
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
				throw new NotImplementedException();
			}
		}

		public UrlType RequestUrlType
		{
			get
			{
				return UrlType.QRCode;
			}
		}

		static Service()
		{
			Service._config = null;
		}

		public Service()
		{
		}

		public void CheckCanEnable()
		{
			if (Service._config == null)
			{
				Service._config = Utility<Config>.GetConfig(base.WorkDirectory);
			}
			if (string.IsNullOrEmpty(Service._config.Partner))
			{
				throw new PluginConfigException("未设置合作者身份ID!");
			}
			if (string.IsNullOrEmpty(Service._config.key))
			{
				throw new PluginConfigException("未设置Key!");
			}
		}

		public string ConfirmPayResult()
		{
			return "success";
		}

		public FormData GetFormData()
		{
			if (Service._config == null)
			{
				Service._config = Utility<Config>.GetConfig(base.WorkDirectory);
			}
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[2];
			FormData.FormItem formItem = new FormData.FormItem();
			formItem.DisplayName=("合作者身份ID");
			formItem.IsRequired=(true);
			formItem.Name=("partner");
			formItem.Type= (FormData.FormItemType)(1);
			formItem.Value=(Service._config.Partner);
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem();
			formItem1.DisplayName=("Key");
			formItem1.IsRequired=(true);
			formItem1.Name=("key");
			formItem1.Type= (FormData.FormItemType)(1);
			formItem1.Value=(Service._config.key);
			formItemArray[1] = formItem1;
			formDatum.Items=(formItemArray);
			return formDatum;
		}

		public string GetRequestUrl(string returnUrl, string notifyUrl, string orderId, decimal totalFee, string productInfo, string openId = null)
		{
			string innerText;
			if (string.IsNullOrEmpty(productInfo))
			{
				throw new PluginConfigException("商品信息不能为空!");
			}
			if (string.IsNullOrEmpty(orderId))
			{
				throw new PluginConfigException("订单号不能为空!");
			}
			if ((!string.IsNullOrWhiteSpace(returnUrl) ? false : string.IsNullOrWhiteSpace(notifyUrl)))
			{
				throw new PluginConfigException("返回URL不能为空!");
			}
			if (Service._config == null)
			{
				Service._config = Utility<Config>.GetConfig(base.WorkDirectory);
			}
			string empty = string.Empty;
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "service", Service._config.getCodeService },
				{ "partner", Service._config.Partner },
				{ "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
				{ "sign_type", Service._config.Sign_type },
				{ "_input_charset", Service._config.Input_charset },
				{ "method", "add" },
				{ "biz_type", "10" }
			};
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(string.Concat("\"trade_type\":\"", Service._config.trade_type, "\""));
			stringBuilder.Append(",\"need_address\":\"F\"");
			stringBuilder.Append(",\"goods_info\":{");
			stringBuilder.Append(string.Concat("\"id\":\"", orderId));
			stringBuilder.Append(string.Concat("\",\"name\":\"", productInfo.Substring(0, (productInfo.Length > 32 ? 32 : productInfo.Length))));
			stringBuilder.Append(string.Concat("\",\"desc\":\"", productInfo));
			stringBuilder.Append(string.Concat("\",\"price\":\"", totalFee.ToString("F2")));
			stringBuilder.Append("\"}");
			stringBuilder.Append(string.Concat(",\"notify_url\":\"", notifyUrl));
			stringBuilder.Append(string.Concat(",\"return_url\":\"", notifyUrl));
			stringBuilder.Append("\"}");
			strs.Add("biz_data", stringBuilder.ToString());
			string str = Submit.BuildRequest(Service._config.gateWay, strs, Service._config);
			if (!string.IsNullOrWhiteSpace(str))
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.LoadXml(str);
					string innerText1 = xmlDocument.SelectSingleNode("/alipay/is_success").InnerText;
					if (innerText1 == "F")
					{
						innerText = xmlDocument.SelectSingleNode("/alipay/error").InnerText;
						throw new PluginConfigException(string.Concat("生成二维码出现异常:", innerText));
					}
					if (innerText1 == "T")
					{
						XmlNode xmlNodes = xmlDocument.SelectSingleNode("/alipay/response/alipay/result_code");
						if ((xmlNodes == null ? true : !(xmlNodes.InnerText == "SUCCESS")))
						{
							innerText = xmlDocument.SelectSingleNode("/alipay/response/alipay/error_message").InnerText;
							throw new PluginConfigException(string.Concat("生成二维码出现异常:", innerText));
						}
						empty = xmlDocument.SelectSingleNode("/alipay/response/alipay/qrcode").InnerText;
					}
				}
				catch (Exception exception)
				{
					throw new PluginConfigException(string.Concat("生成二维码出现异常:", exception.Message));
				}
			}
			return empty;
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

		public PaymentInfo ProcessNotify(HttpRequestBase context)
		{
			NameValueCollection form = context.Form;
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>();
			string[] allKeys = form.AllKeys;
			for (int i = 0; i < allKeys.Length; i++)
			{
				string str = allKeys[i];
				strs.Add(str, form[str]);
			}
			if (Service._config == null)
			{
				Service._config = Utility<Config>.GetConfig(base.WorkDirectory);
			}
			Notify notify = new Notify(Service._config);
			PaymentInfo paymentInfo = new PaymentInfo();
			if (notify.Verify(strs, form["sign"]))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(form["notify_data"]);
				string innerText = xmlDocument.SelectSingleNode("/notify/out_trade_no").InnerText;
				string innerText1 = xmlDocument.SelectSingleNode("/notify/trade_no").InnerText;
				string str1 = xmlDocument.SelectSingleNode("/notify/trade_status").InnerText;
				string innerText2 = xmlDocument.SelectSingleNode("/notify/gmt_create").InnerText;
				if ((str1 == "TRADE_FINISHED" || str1 == "TRADE_SUCCESS" ? true : str1 == "WAIT_SELLER_SEND_GOODS"))
				{
					PaymentInfo paymentInfo1 = paymentInfo;
					char[] chrArray = new char[] { ',' };
					paymentInfo1.OrderIds=(
						from item in innerText.Split(chrArray)
						select long.Parse(item));
					paymentInfo.TradNo=(innerText1);
					paymentInfo.TradeTime=(new DateTime?(DateTime.Parse(innerText2)));
					paymentInfo.ResponseContentWhenFinished=("success");
				}
			}
			return paymentInfo;
		}

		public PaymentInfo ProcessReturn(HttpRequestBase context)
		{
			NameValueCollection queryString = context.QueryString;
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>();
			string[] allKeys = queryString.AllKeys;
			for (int i = 0; i < allKeys.Length; i++)
			{
				string str = allKeys[i];
				strs.Add(str, queryString[str]);
			}
			if (Service._config == null)
			{
				Service._config = Utility<Config>.GetConfig(base.WorkDirectory);
			}
			Notify notify = new Notify(Service._config);
			bool flag = notify.Verify(strs, queryString["sign"]);
			PaymentInfo paymentInfo = new PaymentInfo();
			if (flag)
			{
				PaymentInfo paymentInfo1 = paymentInfo;
				string str1 = queryString["out_trade_no"];
				char[] chrArray = new char[] { ',' };
				paymentInfo1.OrderIds=(
					from item in str1.Split(chrArray)
					select long.Parse(item));
				paymentInfo.TradNo=(queryString["trade_no"]);
			}
			return paymentInfo;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "partner");
			if (string.IsNullOrEmpty(keyValuePair.Value))
			{
				throw new PluginConfigException("未设置合作者身份ID!");
			}
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "key");
			if (string.IsNullOrEmpty(keyValuePair1.Value))
			{
				throw new PluginConfigException("未设置Key!");
			}
			Service._config.Partner = keyValuePair.Value;
			Service._config.key = keyValuePair1.Value;
			Utility<Config>.SaveConfig(Service._config, base.WorkDirectory);
		}
	}
}