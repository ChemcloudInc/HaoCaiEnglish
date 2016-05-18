using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Himall.WeixinPaymentBase
{
	public static class RequestUtility
	{
		public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}

		public static string GetQueryString(this Dictionary<string, string> formData)
		{
			string str;
			if ((formData == null ? false : formData.Count != 0))
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				foreach (KeyValuePair<string, string> formDatum in formData)
				{
					num++;
					stringBuilder.AppendFormat("{0}={1}", formDatum.Key, formDatum.Value);
					if (num < formData.Count)
					{
						stringBuilder.Append("&");
					}
				}
				str = stringBuilder.ToString();
			}
			else
			{
				str = "";
			}
			return str;
		}

		public static string HtmlDecode(this string html)
		{
			return HttpUtility.HtmlDecode(html);
		}

		public static string HtmlEncode(this string html)
		{
			return HttpUtility.HtmlEncode(html);
		}

		public static string HttpPost(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null)
		{
			string queryString = formData.GetQueryString();
			byte[] numArray = (formData == null ? new byte[0] : Encoding.UTF8.GetBytes(queryString));
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(numArray, 0, numArray.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			string str = RequestUtility.HttpPost(url, cookieContainer, memoryStream, null, null, encoding);
			return str;
		}

		public static string HttpPost(string url, CookieContainer cookieContainer = null, Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null)
		{
			byte[] numArray;
			int num;
			string end;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			if ((fileDictionary == null ? true : fileDictionary.Count <= 0))
			{
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			}
			else
			{
				postStream = new MemoryStream();
				long ticks = DateTime.Now.Ticks;
				string str = string.Concat("----", ticks.ToString("x"));
				string str1 = string.Concat("\r\n--", str, "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n");
				foreach (KeyValuePair<string, string> keyValuePair in fileDictionary)
				{
					try
					{
						string value = keyValuePair.Value;
						FileStream fileStream = FileHelper.GetFileStream(value);
						try
						{
							string str2 = string.Format(str1, keyValuePair.Key, value);
							byte[] bytes = Encoding.ASCII.GetBytes((postStream.Length == 0 ? str2.Substring(2, str2.Length - 2) : str2));
							postStream.Write(bytes, 0, bytes.Length);
							numArray = new byte[1024];
							num = 0;
							while (true)
							{
								int num1 = fileStream.Read(numArray, 0, numArray.Length);
								num = num1;
								if (num1 == 0)
								{
									break;
								}
								postStream.Write(numArray, 0, num);
							}
						}
						finally
						{
							if (fileStream != null)
							{
								((IDisposable)fileStream).Dispose();
							}
						}
					}
					catch (Exception exception)
					{
						throw exception;
					}
				}
				byte[] bytes1 = Encoding.ASCII.GetBytes(string.Concat("\r\n--", str, "--\r\n"));
				postStream.Write(bytes1, 0, bytes1.Length);
				httpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", str);
			}
			httpWebRequest.ContentLength = (postStream != null ? postStream.Length : 0);
			httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			httpWebRequest.KeepAlive = true;
			if (!string.IsNullOrEmpty(refererUrl))
			{
				httpWebRequest.Referer = refererUrl;
			}
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
			if (cookieContainer != null)
			{
				httpWebRequest.CookieContainer = cookieContainer;
			}
			if (postStream != null)
			{
				postStream.Position = 0;
				Stream requestStream = httpWebRequest.GetRequestStream();
				numArray = new byte[1024];
				num = 0;
				while (true)
				{
					int num2 = postStream.Read(numArray, 0, numArray.Length);
					num = num2;
					if (num2 == 0)
					{
						break;
					}
					requestStream.Write(numArray, 0, num);
				}
				postStream.Close();
			}
			HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
			if (cookieContainer != null)
			{
				response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
			}
			Stream responseStream = response.GetResponseStream();
			try
			{
				StreamReader streamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8"));
				try
				{
					end = streamReader.ReadToEnd();
				}
				finally
				{
					if (streamReader != null)
					{
						((IDisposable)streamReader).Dispose();
					}
				}
			}
			finally
			{
				if (responseStream != null)
				{
					((IDisposable)responseStream).Dispose();
				}
			}
			return end;
		}

		public static string HttpPost(string url, string cert, string certpwd, CookieContainer cookieContainer = null, Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null)
		{
			string end;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			X509Certificate x509Certificate = new X509Certificate(cert, certpwd);
			httpWebRequest.ClientCertificates.Add(x509Certificate);
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = 6000;
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.ContentLength = (postStream != null ? postStream.Length : 0);
			if (!string.IsNullOrEmpty(refererUrl))
			{
				httpWebRequest.Referer = refererUrl;
			}
			if (cookieContainer != null)
			{
				httpWebRequest.CookieContainer = cookieContainer;
			}
			if (postStream != null)
			{
				postStream.Position = 0;
				Stream requestStream = httpWebRequest.GetRequestStream();
				byte[] numArray = new byte[1024];
				int num = 0;
				while (true)
				{
					int num1 = postStream.Read(numArray, 0, numArray.Length);
					num = num1;
					if (num1 == 0)
					{
						break;
					}
					requestStream.Write(numArray, 0, num);
				}
				postStream.Close();
			}
			HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
			if (cookieContainer != null)
			{
				response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
			}
			Stream responseStream = response.GetResponseStream();
			try
			{
				StreamReader streamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8"));
				try
				{
					end = streamReader.ReadToEnd();
				}
				finally
				{
					if (streamReader != null)
					{
						((IDisposable)streamReader).Dispose();
					}
				}
			}
			finally
			{
				if (responseStream != null)
				{
					((IDisposable)responseStream).Dispose();
				}
			}
			return end;
		}

		public static bool IsWeixinClientRequest(this HttpContext httpContext)
		{
			return (string.IsNullOrEmpty(httpContext.Request.UserAgent) ? false : httpContext.Request.UserAgent.Contains("MicroMessenger"));
		}

		public static string UrlDecode(this string url)
		{
			return HttpUtility.UrlDecode(url);
		}

		public static string UrlEncode(this string url)
		{
			return HttpUtility.UrlEncode(url);
		}
	}
}