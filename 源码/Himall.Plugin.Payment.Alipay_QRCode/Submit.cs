using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Himall.Plugin.Payment.Alipay_QRCode
{
	public class Submit
	{
		private static string GATEWAY_NEW;

		private static string _key;

		private static string _input_charset;

		private static string _sign_type;

		static Submit()
		{
			Submit.GATEWAY_NEW = "https://mapi.alipay.com/gateway.do?";
			Submit._key = "";
			Submit._input_charset = "";
			Submit._sign_type = "";
		}

		public Submit()
		{
		}

		public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue, Config config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Submit.BuildRequestPara(sParaTemp, config);
			StringBuilder stringBuilder = new StringBuilder();
			string[] gATEWAYNEW = new string[] { "<form id='alipaysubmit' name='alipaysubmit' action='", Submit.GATEWAY_NEW, "_input_charset=", Submit._input_charset, "' method='", strMethod.ToLower().Trim(), "'>" };
			stringBuilder.Append(string.Concat(gATEWAYNEW));
			foreach (KeyValuePair<string, string> str in strs)
			{
				gATEWAYNEW = new string[] { "<input type='hidden' name='", str.Key, "' value='", str.Value, "'/>" };
				stringBuilder.Append(string.Concat(gATEWAYNEW));
			}
			stringBuilder.Append(string.Concat("<input type='submit' value='", strButtonValue, "' style='display:none;'></form>"));
			stringBuilder.Append("<script>document.forms['alipaysubmit'].submit();</script>");
			return stringBuilder.ToString();
		}

		public static string BuildRequest(SortedDictionary<string, string> sParaTemp, Config config)
		{
			Encoding encoding = Encoding.GetEncoding(Submit._input_charset);
			string str = Submit.BuildRequestParaToString(sParaTemp, encoding, config);
			byte[] bytes = encoding.GetBytes(str);
			string str1 = string.Concat(Submit.GATEWAY_NEW, "_input_charset=", Submit._input_charset);
			string str2 = "";
			try
			{
				HttpWebRequest length = (HttpWebRequest)WebRequest.Create(str1);
				length.Method = "post";
				length.ContentType = "application/x-www-form-urlencoded";
				length.ContentLength = (int)bytes.Length;
				Stream requestStream = length.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				Stream responseStream = ((HttpWebResponse)length.GetResponse()).GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, encoding);
				StringBuilder stringBuilder = new StringBuilder();
				while (true)
				{
					string str3 = streamReader.ReadLine();
					string str4 = str3;
					if (str3 == null)
					{
						break;
					}
					stringBuilder.Append(str4);
				}
				responseStream.Close();
				str2 = stringBuilder.ToString();
			}
			catch (Exception exception)
			{
				str2 = string.Concat("报错：", exception.Message);
			}
			return str2;
		}

		public static string BuildRequest(string GATEWAY_NEW, Dictionary<string, string> sParaTemp, Config config)
		{
			Submit._sign_type = config.Sign_type;
			Submit._input_charset = config.Input_charset;
			Submit._key = config.key;
			Encoding encoding = Encoding.GetEncoding(config.Input_charset);
			string str = Submit.BuildRequestParaToString(new SortedDictionary<string, string>(sParaTemp), encoding, config);
			byte[] bytes = encoding.GetBytes(str);
			string str1 = string.Concat(GATEWAY_NEW, "?_input_charset=", Submit._input_charset);
			string str2 = "";
			try
			{
				HttpWebRequest length = (HttpWebRequest)WebRequest.Create(str1);
				length.Method = "post";
				length.ContentType = "application/x-www-form-urlencoded";
				length.ContentLength = (int)bytes.Length;
				Stream requestStream = length.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				Stream responseStream = ((HttpWebResponse)length.GetResponse()).GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, encoding);
				StringBuilder stringBuilder = new StringBuilder();
				while (true)
				{
					string str3 = streamReader.ReadLine();
					string str4 = str3;
					if (str3 == null)
					{
						break;
					}
					stringBuilder.Append(str4);
				}
				responseStream.Close();
				str2 = stringBuilder.ToString();
			}
			catch (Exception exception)
			{
				str2 = string.Concat("报错：", exception.Message);
			}
			return str2;
		}

		public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string fileName, byte[] data, string contentType, int lengthFile, Config config)
		{
			Stream responseStream;
			string str;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Submit.BuildRequestPara(sParaTemp, config);
			string str1 = string.Concat(Submit.GATEWAY_NEW, "_input_charset=", Submit._input_charset);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str1);
			httpWebRequest.Method = strMethod;
			string str2 = DateTime.Now.Ticks.ToString("x");
			string str3 = string.Concat("--", str2);
			httpWebRequest.ContentType = string.Concat("\r\nmultipart/form-data; boundary=", str2);
			httpWebRequest.KeepAlive = true;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in strs)
			{
				string[] key = new string[] { str3, "\r\nContent-Disposition: form-data; name=\"", keyValuePair.Key, "\"\r\n\r\n", keyValuePair.Value, "\r\n" };
				stringBuilder.Append(string.Concat(key));
			}
			stringBuilder.Append(string.Concat(str3, "\r\nContent-Disposition: form-data; name=\"withhold_file\"; filename=\""));
			stringBuilder.Append(fileName);
			stringBuilder.Append(string.Concat("\"\r\nContent-Type: ", contentType, "\r\n\r\n"));
			string str4 = stringBuilder.ToString();
			Encoding encoding = Encoding.GetEncoding(Submit._input_charset);
			byte[] bytes = encoding.GetBytes(str4);
			byte[] numArray = Encoding.ASCII.GetBytes(string.Concat("\r\n", str3, "--\r\n"));
			long length = (int)bytes.Length + lengthFile + (int)numArray.Length;
			httpWebRequest.ContentLength = length;
			Stream requestStream = httpWebRequest.GetRequestStream();
			try
			{
				try
				{
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Write(data, 0, lengthFile);
					requestStream.Write(numArray, 0, numArray.Length);
					responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
				}
				catch (WebException webException)
				{
					str = webException.ToString();
					return str;
				}
			}
			finally
			{
				if (requestStream != null)
				{
					requestStream.Close();
				}
			}
			StreamReader streamReader = new StreamReader(responseStream, encoding);
			StringBuilder stringBuilder1 = new StringBuilder();
			while (true)
			{
				string str5 = streamReader.ReadLine();
				string str6 = str5;
				if (str5 == null)
				{
					break;
				}
				stringBuilder1.Append(str6);
			}
			responseStream.Close();
			str = stringBuilder1.ToString();
			return str;
		}

		private static string BuildRequestMysign(Dictionary<string, string> sPara)
		{
			string str;
			string str1 = Core.CreateLinkString(sPara);
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
				str2 = AlipayMD5.Sign(str1, Submit._key, Submit._input_charset);
				str = str2;
				return str;
			}
			str2 = "";
			str = str2;
			return str;
		}

		private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp, Config config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Core.FilterPara(sParaTemp);
			strs.Add("sign", Submit.BuildRequestMysign(strs));
			strs.Add("sign_type", Submit._sign_type);
			return strs;
		}

		private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, Encoding code, Config config)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs = Submit.BuildRequestPara(sParaTemp, config);
			return Core.CreateLinkStringUrlencode(strs, code);
		}

		public static string Query_timestamp(Config config)
		{
			string[] gATEWAYNEW = new string[] { Submit.GATEWAY_NEW, "service=query_timestamp&partner=", config.Partner, "&_input_charset=", config.Input_charset };
			string str = string.Concat(gATEWAYNEW);
			XmlTextReader xmlTextReader = new XmlTextReader(str);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlTextReader);
			return xmlDocument.SelectSingleNode("/alipay/response/timestamp/encrypt_key").InnerText;
		}
	}
}