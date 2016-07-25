using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Himall.Plugin.Message.SMS
{
	public static class SMSAPiHelper
	{
		public static string BuildSign(Dictionary<string, string> dicArray, string key, string sign_type, string _input_charset)
		{
			string str = string.Concat(SMSAPiHelper.CreateLinkstring(dicArray), key);
			return SMSAPiHelper.Sign(str, sign_type, _input_charset);
		}

		public static string CreateLinkstring(Dictionary<string, string> dicArray)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dicArray)
			{
				stringBuilder.Append(string.Concat(keyValuePair.Key, "=", keyValuePair.Value, "&"));
			}
			int length = stringBuilder.Length;
			stringBuilder.Remove(length - 1, 1);
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> Parameterfilter(SortedDictionary<string, string> dicArrayPre)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> keyValuePair in dicArrayPre)
			{
				if ((!(keyValuePair.Key.ToLower() != "sign") || !(keyValuePair.Key.ToLower() != "sign_type") || !(keyValuePair.Value != "") ? false : keyValuePair.Value != null))
				{
					strs.Add(keyValuePair.Key.ToLower(), keyValuePair.Value);
				}
			}
			return strs;
		}

		public static string PostData(string url, string postData)
		{
			string empty = string.Empty;
			try
			{
				HttpWebRequest length = (HttpWebRequest)WebRequest.Create(new Uri(url));
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				length.Method = "POST";
				length.ContentType = "application/x-www-form-urlencoded";
				length.ContentLength = bytes.Length;
				Stream requestStream = length.GetRequestStream();
				try
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
				finally
				{
					if (requestStream != null)
					{
						((IDisposable)requestStream).Dispose();
					}
				}
				HttpWebResponse response = (HttpWebResponse)length.GetResponse();
				try
				{
					Stream responseStream = response.GetResponseStream();
					try
					{
						Encoding uTF8 = Encoding.UTF8;
						Stream gZipStream = responseStream;
						if (response.ContentEncoding.ToLower() == "gzip")
						{
							gZipStream = new GZipStream(responseStream, CompressionMode.Decompress);
						}
						else if (response.ContentEncoding.ToLower() == "deflate")
						{
							gZipStream = new DeflateStream(responseStream, CompressionMode.Decompress);
						}
						StreamReader streamReader = new StreamReader(gZipStream, uTF8);
						try
						{
							empty = streamReader.ReadToEnd();
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
				}
				finally
				{
					if (response != null)
					{
						((IDisposable)response).Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				empty = string.Format("获取信息错误：{0}", exception.Message);
			}
			return empty;
		}

		public static string Sign(string prestr, string sign_type, string _input_charset)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			if (sign_type.ToUpper() == "MD5")
			{
				MD5 mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] numArray = mD5CryptoServiceProvider.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
				for (int i = 0; i < numArray.Length; i++)
				{
					stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
				}
			}
			return stringBuilder.ToString();
		}
	}
}