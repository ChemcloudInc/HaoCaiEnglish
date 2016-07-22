using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Payment;
using Himall.PaymentPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml.Linq;

namespace Himall.WeixinPaymentBase
{
	public class ServiceBase : PaymentBase<Config>
	{
		private string _logo;

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

		public ServiceBase()
		{
		}

		public void CheckCanEnable()
		{
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			if (string.IsNullOrWhiteSpace(config.AppId))
			{
				throw new PluginConfigException("未设置AppId");
			}
			if (string.IsNullOrWhiteSpace(config.AppSecret))
			{
				throw new PluginConfigException("未设置AppSecret");
			}
			if (string.IsNullOrWhiteSpace(config.Key))
			{
				throw new PluginConfigException("未设置Key");
			}
			if (string.IsNullOrWhiteSpace(config.MCHID))
			{
				throw new PluginConfigException("未设置MCHID");
			}
			if (string.IsNullOrWhiteSpace(config.pkcs12))
			{
				throw new PluginConfigException("未设置商户证书");
			}
		}

		public string ConfirmPayResult()
		{
			return "SUCCESS";
		}

		public override PaymentInfo EnterprisePay(EnterprisePayPara para)
		{
			RequestHandler requestHandler = new RequestHandler();
			PaymentInfo paymentInfo = new PaymentInfo();
			string empty = string.Empty;
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			if (string.IsNullOrEmpty(config.AppId))
			{
				throw new PluginException("未设置AppId");
			}
			if (string.IsNullOrEmpty(config.MCHID))
			{
				throw new PluginException("未设置MCHID");
			}
			if (string.IsNullOrWhiteSpace(config.pkcs12))
			{
				throw new PluginConfigException("未设置商户证书");
			}
			requestHandler.SetKey(config.Key);
			string noncestr = TenPayUtil.GetNoncestr();
			requestHandler.SetParameter("partner_trade_no", para.out_trade_no);
			int num = Convert.ToInt32(para.amount * new decimal(100));
			requestHandler.SetParameter("amount", num.ToString());
			requestHandler.SetParameter("openid", para.openid);
			requestHandler.SetParameter("mch_appid", config.AppId);
			requestHandler.SetParameter("mchid", config.MCHID);
			requestHandler.SetParameter("nonce_str", noncestr);
			string str = "NO_CHECK";
			if (para.check_name)
			{
				str = "OPTION_CHECK";
			}
			requestHandler.SetParameter("check_name", str);
			requestHandler.SetParameter("desc", para.desc);
			requestHandler.SetParameter("spbill_create_ip", (string.IsNullOrWhiteSpace(para.spbill_create_ip) ? "222.240.184.122" : para.spbill_create_ip));
			string str1 = requestHandler.CreateMd5Sign("key", config.Key);
			requestHandler.SetParameter("sign", str1);
			string str2 = string.Concat(base.WorkDirectory, "\\", config.pkcs12);
			if (!File.Exists(str2))
			{
				throw new PluginException("未找到商户证书文件");
			}
			string str3 = requestHandler.ParseXML();
			string empty1 = string.Empty;
			try
			{
				empty1 = TenPayV3.transfers(str3, str2, config.MCHID);
			}
			catch (Exception exception)
			{
				throw new PluginException(string.Concat("企业付款时出错：", exception.Message));
			}
			XDocument xDocument = XDocument.Parse(empty1);
			if (xDocument == null)
			{
				throw new PluginException(string.Concat("企业付款时出错：", str3));
			}
			XElement xElement = xDocument.Element("xml").Element("return_code");
			XElement xElement1 = xDocument.Element("xml").Element("return_msg");
			if (xElement == null)
			{
				throw new PluginException("企业付款时,返回参数异常");
			}
			if (!(xElement.Value == "SUCCESS"))
			{
				throw new PluginException(string.Concat("企业付款时,接口返回异常:", xElement1.Value));
			}
			xElement1 = xDocument.Element("xml").Element("result_code");
			XElement xElement2 = xDocument.Element("xml").Element("err_code_des");
			if (!(xElement1.Value == "SUCCESS"))
			{
				throw new PluginException(string.Concat("企业付款时,接口返回异常:", xElement2.Value));
			}
			string value = xDocument.Element("xml").Element("payment_no").Value;
			string value1 = xDocument.Element("xml").Element("partner_trade_no").Value;
			string value2 = xDocument.Element("xml").Element("payment_time").Value;
			paymentInfo.OrderIds = new List<long>()
			{
				long.Parse(value1)
			};
			paymentInfo.TradNo = value;
			paymentInfo.TradeTime = new DateTime?(DateTime.Parse(value2));
			return paymentInfo;
		}

		public FormData GetFormData()
		{
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			FormData formDatum = new FormData();
			FormData.FormItem[] formItemArray = new FormData.FormItem[5];
			FormData.FormItem formItem = new FormData.FormItem()
			{
				DisplayName = "AppId",
				Name = "AppId",
				IsRequired = true,
				Type = FormData.FormItemType.text,
				Value = config.AppId
			};
			formItemArray[0] = formItem;
			FormData.FormItem formItem1 = new FormData.FormItem()
			{
				DisplayName = "AppSecret",
				Name = "AppSecret",
				IsRequired = true,
				Type = FormData.FormItemType.text,
				Value = config.AppSecret
			};
			formItemArray[1] = formItem1;
			FormData.FormItem formItem2 = new FormData.FormItem()
			{
				DisplayName = "Key",
				Name = "Key",
				IsRequired = true,
				Type = FormData.FormItemType.text,
				Value = config.Key
			};
			formItemArray[2] = formItem2;
			FormData.FormItem formItem3 = new FormData.FormItem()
			{
				DisplayName = "MCHID",
				Name = "MCHID",
				IsRequired = true,
				Type = FormData.FormItemType.text,
				Value = config.MCHID
			};
			formItemArray[3] = formItem3;
			FormData.FormItem formItem4 = new FormData.FormItem()
			{
				DisplayName = "商户证书",
				Name = "pkcs12",
				IsRequired = true,
				Type = FormData.FormItemType.text,
				Value = config.pkcs12
			};
			formItemArray[4] = formItem4;
			formDatum.Items = formItemArray;
			return formDatum;
		}

		public PaymentInfo ProcessNotify(HttpRequestBase request)
		{
			ResponseHandler responseHandler = new ResponseHandler(request);
			responseHandler.init();
			string parameter = responseHandler.getParameter("out_trade_no");
			string str = responseHandler.getParameter("time_end");
			string parameter1 = responseHandler.getParameter("transaction_id");
			PaymentInfo paymentInfo = new PaymentInfo();
			PaymentInfo paymentInfo1 = paymentInfo;
			char[] chrArray = new char[] { ',' };
			paymentInfo1.OrderIds = 
				from item in parameter.Split(chrArray)
				select long.Parse(item);
			paymentInfo.ResponseContentWhenFinished = "success";
			paymentInfo.TradeTime = new DateTime?(DateTime.ParseExact(str, "yyyyMMddHHmmss", null));
			paymentInfo.TradNo = parameter1;
			return paymentInfo;
		}

		public override PaymentInfo ProcessRefundFee(PaymentPara para)
		{
			RequestHandler requestHandler = new RequestHandler();
			PaymentInfo paymentInfo = new PaymentInfo();
			string empty = string.Empty;
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			if (string.IsNullOrEmpty(config.AppId))
			{
				throw new PluginException("未设置AppId");
			}
			if (string.IsNullOrEmpty(config.MCHID))
			{
				throw new PluginException("未设置MCHID");
			}
			if (string.IsNullOrWhiteSpace(config.pkcs12))
			{
				throw new PluginConfigException("未设置商户证书");
			}
			requestHandler.SetKey(config.Key);
			string noncestr = TenPayUtil.GetNoncestr();
			requestHandler.SetParameter("out_trade_no", para.out_trade_no);
			requestHandler.SetParameter("out_refund_no", para.out_refund_no);
			int num = Convert.ToInt32(para.total_fee * new decimal(100));
			requestHandler.SetParameter("total_fee", num.ToString());
			num = Convert.ToInt32(para.refund_fee * new decimal(100));
			requestHandler.SetParameter("refund_fee", num.ToString());
			requestHandler.SetParameter("op_user_id", config.MCHID);
			requestHandler.SetParameter("appid", config.AppId);
			requestHandler.SetParameter("mch_id", config.MCHID);
			requestHandler.SetParameter("nonce_str", noncestr);
			string str = requestHandler.CreateMd5Sign("key", config.Key);
			requestHandler.SetParameter("sign", str);
			string str1 = string.Concat(base.WorkDirectory, "\\", config.pkcs12);
			if (!File.Exists(str1))
			{
				throw new PluginException("未找到商户证书文件");
			}
			string str2 = requestHandler.ParseXML();
			string empty1 = string.Empty;
			try
			{
				empty1 = TenPayV3.Refund(str2, str1, config.MCHID);
			}
			catch (Exception exception)
			{
				throw new PluginException(string.Concat("原路返回退款时出错：", exception.Message));
			}
			XDocument xDocument = XDocument.Parse(empty1);
			if (xDocument == null)
			{
				throw new PluginException(string.Concat("原路返回退款时出错：", str2));
			}
			XElement xElement = xDocument.Element("xml").Element("return_code");
			XElement xElement1 = xDocument.Element("xml").Element("return_msg");
			if (xElement == null)
			{
				throw new PluginException("原路返回退款时,返回参数异常");
			}
			if (!(xElement.Value == "SUCCESS"))
			{
				throw new PluginException(string.Concat("原路返回退款时,接口返回异常:", xElement1.Value));
			}
			xElement1 = xDocument.Element("xml").Element("result_code");
			XElement xElement2 = xDocument.Element("xml").Element("err_code_des");
			if (!(xElement1.Value == "SUCCESS"))
			{
				throw new PluginException(string.Concat("原路返回退款时,接口返回异常:", xElement2.Value));
			}
			string value = xDocument.Element("xml").Element("refund_id").Value;
			string value1 = xDocument.Element("xml").Element("out_refund_no").Value;
			paymentInfo.OrderIds = new List<long>()
			{
				long.Parse(value1)
			};
			paymentInfo.TradNo = value;
			paymentInfo.TradeTime = new DateTime?(DateTime.Now);
			return paymentInfo;
		}

		public PaymentInfo ProcessReturn(HttpRequestBase request)
		{
			return null;
		}

		public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
		{
			KeyValuePair<string, string> keyValuePair = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "AppId");
			if (string.IsNullOrWhiteSpace(keyValuePair.Value))
			{
				throw new ArgumentNullException("合作者身份AppId不能为空");
			}
			KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "AppSecret");
			if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
			{
				throw new ArgumentNullException("AppSecret不能为空");
			}
			KeyValuePair<string, string> keyValuePair2 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "Key");
			if (string.IsNullOrWhiteSpace(keyValuePair2.Value))
			{
				throw new ArgumentNullException("Key不能为空");
			}
			KeyValuePair<string, string> keyValuePair3 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "MCHID");
			if (string.IsNullOrWhiteSpace(keyValuePair3.Value))
			{
				throw new ArgumentNullException("MCHID不能为空");
			}
			KeyValuePair<string, string> keyValuePair4 = values.FirstOrDefault<KeyValuePair<string, string>>((KeyValuePair<string, string> item) => item.Key == "pkcs12");
			if (string.IsNullOrWhiteSpace(keyValuePair4.Value))
			{
				throw new ArgumentNullException("商户证书不能为空");
			}
			Config config = Utility<Config>.GetConfig(base.WorkDirectory);
			config.AppId = keyValuePair.Value;
			config.Key = keyValuePair2.Value;
			config.AppSecret = keyValuePair1.Value;
			config.MCHID = keyValuePair3.Value;
			config.pkcs12 = keyValuePair4.Value;
			Utility<Config>.SaveConfig(config, base.WorkDirectory);
		}
	}
}