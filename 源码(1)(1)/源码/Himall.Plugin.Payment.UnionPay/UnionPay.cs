using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using Himall.Plugin.Payment.UnionPay.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace Himall.Plugin.Payment.UnionPay
{
	public class UnionPay : PaymentBase<ChinaUnionConfig>, IPaymentPlugin, IPlugin
	{
		private static ChinaUnionConfig CUConfig;

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
                    _logo = Utility<ChinaUnionConfig>.GetConfig(base.WorkDirectory).Logo;
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
				return UrlType.FormPost;
			}
		}

		public UnionPay()
		{
		}

		public void CheckCanEnable()
		{
			ChinaUnionConfig config = Utility<ChinaUnionConfig>.GetConfig(base.WorkDirectory);
			if (string.IsNullOrWhiteSpace(config.merId))
			{
				throw new PluginException("商户ID不能为空");
			}
			if (string.IsNullOrWhiteSpace(config.signCertpath))
			{
				throw new PluginException("商户私钥证书文件名不能为空");
			}
			if (string.IsNullOrWhiteSpace(config.signCertpwd))
			{
				throw new PluginException("商户私钥证书密码不能为空");
			}
			if (string.IsNullOrWhiteSpace(config.encryptCertpath))
			{
				throw new PluginException("银联公钥证书文件名不能为空");
			}
		}

		public string ConfirmPayResult()
		{
			return "success";
		}

		public FormData GetFormData()
		{
			if (Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig == null)
			{
				Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig = Utility<ChinaUnionConfig>.GetConfig(base.WorkDirectory);
			}
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[4];
			FormData.FormItem formItem = new FormData.FormItem();
			formItem.DisplayName=("商户代码");
			formItem.Name=("merId");
			formItem.IsRequired=(true);
			formItem.Type=(FormData.FormItemType)(1);
			formItem.Value=(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.merId);
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem();
			formItem1.DisplayName=("商户私钥证书文件名");
			formItem1.Name=("signCertpath");
			formItem1.IsRequired=(true);
			formItem1.Type= (FormData.FormItemType)(1);
			formItem1.Value=(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signCertpath);
			formItemArray[1] = formItem1;
			FormData.FormItem formItem2 = new FormData.FormItem();
			formItem2.DisplayName=("商户私钥证书密码");
			formItem2.Name=("signCertpwd");
			formItem2.IsRequired=(true);
			formItem2.Type= (FormData.FormItemType)(1);
			formItem2.Value=(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signCertpwd);
			formItemArray[2] = formItem2;
			FormData.FormItem formItem3 = new FormData.FormItem();
			formItem3.DisplayName=("银联公钥证书文件名");
			formItem3.Name=("encryptCertpath");
			formItem3.IsRequired=(true);
			formItem3.Type= (FormData.FormItemType)(1);
			formItem3.Value=(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.encryptCertpath);
			formItemArray[3] = formItem3;
			formDatum.Items=(formItemArray);
			return formDatum;
		}

		public string GetRequestUrl(string returnUrl, string notifyUrl, string orderId, decimal totalFee, string productInfo, string openId = null)
		{
			string empty = string.Empty;
			if (Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig == null)
			{
				Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig = Utility<ChinaUnionConfig>.GetConfig(base.WorkDirectory);
			}
			if (string.IsNullOrEmpty(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.merId))
			{
				throw new PluginException("未设置商户编码！");
			}
			if (string.IsNullOrEmpty(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signCertpath))
			{
				throw new PluginException("未设置商户私钥证书！");
			}
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "merId", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.merId },
				{ "version", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.version },
				{ "encoding", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.encoding },
				{ "signMethod", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signMethod },
				{ "txnType", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.txnType },
				{ "txnSubType", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.txnSubType },
				{ "bizType", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.bizType },
				{ "channelType", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.channelType },
				{ "accessType", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.accessType },
				{ "currencyCode", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.currencyCode },
				{ "frontUrl", returnUrl },
				{ "backUrl", notifyUrl },
				{ "orderId", orderId },
				{ "txnTime", DateTime.Now.ToString("yyyyMMddHHmmss") }
			};
			int num = (int)(totalFee * new decimal(100));
			strs.Add("txnAmt", num.ToString());
			string str = string.Concat(base.WorkDirectory, "\\Cert\\", Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signCertpath);
			try
			{
				if (!SignUtil.Sign(strs, Encoding.GetEncoding(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.encoding), str, Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.signCertpwd))
				{
					throw new PluginException("签名失败！");
				}
			}
			catch (Exception exception)
			{
				throw new PluginException(exception.Message);
			}
			empty = string.Concat(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.frontTransUrl, "?", SignUtil.CoverDictionaryToString(strs));
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
			PaymentInfo paymentInfo;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			context.Form.AllKeys.ToList().ForEach((string item) => strs.Add(item.ToString(), context.Form[item.ToString()]));
			string str = string.Concat(base.WorkDirectory, "\\Cert\\");
			try
			{
				if (!SignUtil.Validate(strs, Encoding.GetEncoding(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.encoding), str))
				{
					throw new PluginException("验签失败！");
				}
				PaymentInfo paymentInfo1 = new PaymentInfo();
				if (context.Form.AllKeys.Contains<string>("orderId"))
				{
					string str1 = context.Form["orderId"].ToString().Replace("/", ",");
					PaymentInfo paymentInfo2 = paymentInfo1;
					char[] chrArray = new char[] { ',' };
					paymentInfo2.OrderIds=(
						from item in str1.Split(chrArray)
						select long.Parse(item));
				}
				if (context.Form.AllKeys.Contains<string>("queryId"))
				{
					paymentInfo1.TradNo=(context.Form["queryId"].ToString());
				}
				paymentInfo1.TradeTime=(new DateTime?(DateTime.Now));
				paymentInfo1.ResponseContentWhenFinished=(string.Empty);
				paymentInfo = paymentInfo1;
			}
			catch (Exception exception)
			{
				throw new PluginException(string.Concat("后台通知验签异常：", exception.Message));
			}
			return paymentInfo;
		}

		public PaymentInfo ProcessReturn(HttpRequestBase context)
		{
			PaymentInfo paymentInfo;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			context.Form.AllKeys.ToList().ForEach((string item) => strs.Add(item.ToString(), context.Form[item.ToString()]));
			string str = string.Concat(base.WorkDirectory, "\\Cert\\");
			try
			{
				if (!SignUtil.Validate(strs, Encoding.GetEncoding(Himall.Plugin.Payment.UnionPay.UnionPay.CUConfig.encoding), str))
				{
					throw new PluginException("验签失败！");
				}
				PaymentInfo paymentInfo1 = new PaymentInfo();
				if (context.Form.AllKeys.Contains<string>("orderId"))
				{
					string str1 = context.Form["orderId"].ToString().Replace("/", ",");
					PaymentInfo paymentInfo2 = paymentInfo1;
					char[] chrArray = new char[] { ',' };
					paymentInfo2.OrderIds=(
						from item in str1.Split(chrArray)
						select long.Parse(item));
				}
				if (context.Form.AllKeys.Contains<string>("queryId"))
				{
					paymentInfo1.TradNo=(context.Form["queryId"].ToString());
				}
				paymentInfo1.TradeTime=(new DateTime?(DateTime.Now));
				paymentInfo1.ResponseContentWhenFinished=(string.Empty);
				paymentInfo = paymentInfo1;
			}
			catch (Exception exception)
			{
				throw new PluginException(string.Concat("后台通知验签异常：", exception.Message));
			}
			return paymentInfo;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			ChinaUnionConfig config = Utility<ChinaUnionConfig>.GetConfig(base.WorkDirectory);
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "merId");
			if (string.IsNullOrWhiteSpace(keyValuePair.Value))
			{
				throw new PluginException("商户ID不能为空");
			}
			config.merId = keyValuePair.Value;
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "signCertpath");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new PluginException("商户私钥证书文件名不能为空");
			}
			config.signCertpath = keyValuePair1.Value;
			keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "signCertpwd");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new PluginException("商户私钥证书密码不能为空");
			}
			config.signCertpwd = keyValuePair1.Value;
			keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "encryptCertpath");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new PluginException("银联公钥证书文件名不能为空");
			}
			config.encryptCertpath = keyValuePair1.Value;
			Utility<ChinaUnionConfig>.SaveConfig(config, base.WorkDirectory);
		}
	}
}