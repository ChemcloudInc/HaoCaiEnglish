using Himall.PaymentPlugin;
using Himall.PaymentPlugin.Alipay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Himall.Plugin.Payment.Alipay
{
	public class Submit
	{
		private static string _key;

		private static string _input_charset;

		private static string _sign_type;

		static Submit()
		{
			Submit._key = "";
			Submit._input_charset = "utf-8";
			Submit._sign_type = "MD5";
		}

		public Submit()
		{
		}

		private static string BuildRequestMysign(Dictionary<string, string> sPara, Config _config)
		{
			string str;
			string str1 = UrlHelper.CreateLinkString(sPara);
			string str2 = "";
			string _signType = Submit._sign_type;
			if (_signType != null)
			{
				if (_signType != "MD5")
				{
					str2 = "";
					str = str2;
					return str;
				}
				str2 = Sign.MD5(str1, _config.Key, Submit._input_charset);
				str = str2;
				return str;
			}
			str2 = "";
			str = str2;
			return str;
		}

		private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp, Config _config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			string[] strArrays = new string[] { "sign", "sign_type" };
			strs = UrlHelper.FilterPara(sParaTemp, strArrays);
			strs.Add("sign", Submit.BuildRequestMysign(strs, _config));
			strs.Add("sign_type", Submit._sign_type);
			return strs;
		}

		private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, Encoding code, Config _config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Submit.BuildRequestPara(sParaTemp, _config);
			return UrlHelper.CreateLinkStringUrlencode(strs, code);
		}

		public static string BuildRequestUrl(SortedDictionary<string, string> sParaTemp, string workDirectory, Config _config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Submit.BuildRequestPara(sParaTemp, _config);
			StringBuilder stringBuilder = new StringBuilder();
			Config config = Utility<Config>.GetConfig(workDirectory);
			stringBuilder.AppendFormat("{0}", config.GateWay);
			int num = 0;
			foreach (KeyValuePair<string, string> str in strs)
			{
				int num1 = num;
				num = num1 + 1;
				if (num1 > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.AppendFormat("{0}={1}", str.Key, HttpUtility.UrlEncode(str.Value, Encoding.GetEncoding("utf-8")));
			}
			return stringBuilder.ToString();
		}
	}
}