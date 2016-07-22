using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Script.Serialization;

namespace Himall.Plugin.OAuth.WeiXin.Assistant
{
	public class HttpHandler
	{
		public const string Boundary = "----WebKitFormBoundary";

		public HttpHandler()
		{
		}

		public static string GetResponseForText(HttpHandler.ClientRequest clientRequest)
		{
			HttpWebRequest httpMethod = (HttpWebRequest)WebRequest.Create(clientRequest.Url);
			httpMethod.Method = clientRequest.HttpMethod;
			HttpWebResponse response = null;
			Stream responseStream = null;
			string end = null;
			if (httpMethod.Method == "GET")
			{
				response = (HttpWebResponse)httpMethod.GetResponse();
				responseStream = response.GetResponseStream();
				if (responseStream.CanRead)
				{
					end = (new StreamReader(responseStream, Encoding.UTF8)).ReadToEnd();
				}
			}
			else if (httpMethod.Method == "POST")
			{
				if (clientRequest.FormData == null)
				{
					response = (HttpWebResponse)httpMethod.GetResponse();
					responseStream = response.GetResponseStream();
					if (responseStream.CanRead)
					{
						end = (new StreamReader(responseStream, Encoding.UTF8)).ReadToEnd();
					}
				}
				else if (clientRequest.FormData.Count != 0)
				{
					httpMethod.ContentType = clientRequest.ContentType;
					StringBuilder stringBuilder = new StringBuilder();
					byte[] bytes = null;
					Stream requestStream = null;
					if (httpMethod.ContentType == "application/x-www-form-urlencoded")
					{
						foreach (KeyValuePair<string, string> keyValuePair in clientRequest.FormData)
						{
							stringBuilder.Append(string.Concat("&", keyValuePair.Key, "=", keyValuePair.Value));
						}
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Remove(0, 1);
						}
						bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
						httpMethod.ContentLength = bytes.Length;
						requestStream = httpMethod.GetRequestStream();
						requestStream.Write(bytes, 0, bytes.Length);
						requestStream.Close();
						response = (HttpWebResponse)httpMethod.GetResponse();
						responseStream = response.GetResponseStream();
						if (responseStream.CanRead)
						{
							end = (new StreamReader(responseStream, Encoding.UTF8)).ReadToEnd();
						}
					}
					else if (httpMethod.ContentType.StartsWith("multipart/form-data"))
					{
						long ticks = DateTime.Now.Ticks;
						string str = string.Concat("----------------------------", ticks.ToString("x"));
						HttpWebRequest keepAlive = (HttpWebRequest)WebRequest.Create(clientRequest.Url);
						keepAlive.ContentType = string.Concat("multipart/form-data; boundary=", str);
						keepAlive.Method = clientRequest.HttpMethod;
						keepAlive.KeepAlive = clientRequest.KeepAlive;
						keepAlive.Credentials = CredentialCache.DefaultCredentials;
						Stream stream = keepAlive.GetRequestStream();
						byte[] numArray = Encoding.UTF8.GetBytes(string.Concat("\r\n--", str, "\r\n"));
						foreach (KeyValuePair<string, string> formDatum in clientRequest.FormData)
						{
							stream.Write(numArray, 0, numArray.Length);
							string str1 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
							bytes = Encoding.UTF8.GetBytes(string.Format(str1, formDatum.Key, formDatum.Value));
							stream.Write(bytes, 0, bytes.Length);
						}
						foreach (KeyValuePair<string, HttpHandler.BinaryData> binaryDatum in clientRequest.BinaryData)
						{
							stream.Write(numArray, 0, numArray.Length);
							string str2 = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n Content-Type: {2}\r\n\r\n";
							string str3 = string.Format(str2, binaryDatum.Key, binaryDatum.Value.FileName, binaryDatum.Value.ContentType);
							byte[] bytes1 = Encoding.UTF8.GetBytes(str3);
							stream.Write(bytes1, 0, bytes1.Length);
							stream.Write(binaryDatum.Value.Binary, 0, binaryDatum.Value.Binary.Length);
						}
						numArray = Encoding.UTF8.GetBytes(string.Concat("\r\n--", str, "--\r\n"));
						stream.Write(numArray, 0, numArray.Length);
						stream.Close();
						response = (HttpWebResponse)keepAlive.GetResponse();
						string end1 = (new StreamReader(response.GetResponseStream())).ReadToEnd();
						response.Close();
						end = end1;
					}
				}
				else
				{
					response = (HttpWebResponse)httpMethod.GetResponse();
					responseStream = response.GetResponseStream();
					if (responseStream.CanRead)
					{
						end = (new StreamReader(responseStream, Encoding.UTF8)).ReadToEnd();
					}
				}
			}
			return end;
		}

		public static T GetResponseResult<T, ET>(HttpHandler.ClientRequest clientRequest, ET err)
		{
			T t = default(T);
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			string responseForText = HttpHandler.GetResponseForText(clientRequest);
			if (!responseForText.Contains("errcode"))
			{
				t = javaScriptSerializer.Deserialize<T>(responseForText);
			}
			else
			{
				err = javaScriptSerializer.Deserialize<ET>(responseForText);
			}
			return t;
		}

		public class BinaryData
		{
			public byte[] Binary
			{
				get;
				set;
			}

			public string ContentType
			{
				get;
				set;
			}

			public string FileName
			{
				get;
				set;
			}

			public BinaryData()
			{
			}
		}

		public class ClientRequest
		{
			private string Accept
			{
				get;
				set;
			}

			public IDictionary<string, HttpHandler.BinaryData> BinaryData
			{
				get;
				set;
			}

			public string Boundary
			{
				get;
				set;
			}

			public string ContentType
			{
				get;
				set;
			}

			public CookieContainer CookieContainer
			{
				get;
				set;
			}

			public string Expect
			{
				get;
				set;
			}

			public IDictionary<string, string> FormData
			{
				get;
				set;
			}

			public string HttpMethod
			{
				get;
				set;
			}

			public DateTime IfModifiedSince
			{
				get;
				set;
			}

			public bool KeepAlive
			{
				get;
				set;
			}

			public Version ProtocolVersion
			{
				get;
				set;
			}

			public int ReadWriteTimeout
			{
				get;
				set;
			}

			public string Referer
			{
				get;
				set;
			}

			public int Timeout
			{
				get;
				set;
			}

			public string Url
			{
				get;
				set;
			}

			private string UserAgent
			{
				get;
				set;
			}

			public ClientRequest(string url)
			{
                Url = url;
			}
		}
	}
}