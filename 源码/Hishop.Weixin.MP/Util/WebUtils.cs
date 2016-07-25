using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Hishop.Weixin.MP.Util
{
	public sealed class WebUtils
	{
		public WebUtils()
		{
		}

		public string BuildGetUrl(string url, IDictionary<string, string> parameters)
		{
			if ((parameters == null ? false : parameters.Count > 0))
			{
				if (!url.Contains("?"))
				{
					url = string.Concat(url, "?", WebUtils.BuildQuery(parameters));
				}
				else
				{
					url = string.Concat(url, "&", WebUtils.BuildQuery(parameters));
				}
			}
			return url;
		}

		public static string BuildQuery(IDictionary<string, string> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			IEnumerator<KeyValuePair<string, string>> enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				string key = current.Key;
				current = enumerator.Current;
				string value = current.Value;
				if ((string.IsNullOrEmpty(key) ? false : !string.IsNullOrEmpty(value)))
				{
					if (flag)
					{
						stringBuilder.Append("&");
					}
					stringBuilder.Append(key);
					stringBuilder.Append("=");
					stringBuilder.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
					flag = true;
				}
			}
			return stringBuilder.ToString();
		}

		public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}

		public string DoGet(string url, IDictionary<string, string> parameters)
		{
			if ((parameters == null ? false : parameters.Count > 0))
			{
				if (!url.Contains("?"))
				{
					url = string.Concat(url, "?", WebUtils.BuildQuery(parameters));
				}
				else
				{
					url = string.Concat(url, "&", WebUtils.BuildQuery(parameters));
				}
			}
			HttpWebRequest webRequest = GetWebRequest(url, "GET");
			webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
			HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
			return GetResponseAsString(response, Encoding.UTF8);
		}

		public string DoPost(string url, IDictionary<string, string> parameters)
		{
			HttpWebRequest webRequest = GetWebRequest(url, "POST");
			webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
			byte[] bytes = Encoding.UTF8.GetBytes(WebUtils.BuildQuery(parameters));
			Stream requestStream = webRequest.GetRequestStream();
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Close();
			HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
			return GetResponseAsString(response, Encoding.UTF8);
		}

		public string DoPost(string url, string value)
		{
			HttpWebRequest webRequest = GetWebRequest(url, "POST");
			webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			Stream requestStream = webRequest.GetRequestStream();
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Close();
			HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
			return GetResponseAsString(response, Encoding.UTF8);
		}

		public string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
		{
			string end;
			Stream responseStream = null;
			StreamReader streamReader = null;
			try
			{
				responseStream = rsp.GetResponseStream();
				streamReader = new StreamReader(responseStream, encoding);
				end = streamReader.ReadToEnd();
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
				if (responseStream != null)
				{
					responseStream.Close();
				}
				if (rsp != null)
				{
					rsp.Close();
				}
			}
			return end;
		}

		public HttpWebRequest GetWebRequest(string url, string method)
		{
			HttpWebRequest httpWebRequest = null;
			if (!url.Contains("https"))
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			}
			else
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
				httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
			}
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = method;
			httpWebRequest.KeepAlive = true;
			httpWebRequest.UserAgent = "Hishop";
			return httpWebRequest;
		}
	}
}