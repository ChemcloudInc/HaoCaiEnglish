using Himall.PaymentPlugin;
using Himall.PaymentPlugin.Alipay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Himall.Plugin.Payment.Alipay
{
	public class Notify
	{
		private string _partner = "";

		private string _key = "";

		private string _input_charset = "";

		private string _sign_type = "";

		private string Https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&";

		public Notify(string workDirectory)
		{
			Config config = Utility<Config>.GetConfig(workDirectory);
            _partner = config.Partner.Trim();
            _key = config.Key.Trim();
            _input_charset = config.Input_charset.Trim().ToLower();
            _sign_type = config.Sign_type.Trim().ToUpper();
		}

		private string Get_Http(string strUrl, int timeout)
		{
			string str;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(strUrl);
				httpWebRequest.Timeout = timeout;
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
				StringBuilder stringBuilder = new StringBuilder();
				while (-1 != streamReader.Peek())
				{
					stringBuilder.Append(streamReader.ReadLine());
				}
				str = stringBuilder.ToString();
			}
			catch (Exception exception)
			{
				str = string.Concat("错误：", exception.Message);
			}
			return str;
		}

		private string GetResponseTxt(string notify_id)
		{
			string[] httpsVeryfyUrl = new string[] { Https_veryfy_url, "partner=", _partner, "&notify_id=", notify_id };
			return Get_Http(string.Concat(httpsVeryfyUrl), 120000);
		}

		private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			string[] strArrays = new string[] { "sign", "sign_type" };
			string str = UrlHelper.CreateLinkString(UrlHelper.FilterPara(inputPara, strArrays));
			bool flag = false;
			if ((sign == null ? false : sign != ""))
			{
				string _signType = _sign_type;
				if (_signType != null)
				{
					if (_signType == "MD5")
					{
                        flag = Sign.VerifyMD5(str, sign, _key, _input_charset);
                    }
                }
			}
			return flag;
		}

		public bool Verify(SortedDictionary<string, string> inputPara, string notify_id, string sign)
		{
			bool signVeryfy = GetSignVeryfy(inputPara, sign);
			string responseTxt = "true";
			if ((notify_id == null ? false : notify_id != ""))
			{
				responseTxt = GetResponseTxt(notify_id);
			}
			return ((responseTxt != "true" ? true : !signVeryfy) ? false : true);
		}
	}
}