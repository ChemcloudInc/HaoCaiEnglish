using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Himall.Plugin.Payment.Alipay_QRCode
{
	public class Core
	{
		public Core()
		{
		}

		public static string CreateLinkString(Dictionary<string, string> dicArray)
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

		public static string CreateLinkStringUrlencode(Dictionary<string, string> dicArray, Encoding code)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dicArray)
			{
				stringBuilder.Append(string.Concat(keyValuePair.Key, "=", HttpUtility.UrlEncode(keyValuePair.Value, code), "&"));
			}
			int length = stringBuilder.Length;
			stringBuilder.Remove(length - 1, 1);
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> keyValuePair in dicArrayPre)
			{
				if ((!(keyValuePair.Key.ToLower() != "sign") || !(keyValuePair.Key.ToLower() != "sign_type") || !(keyValuePair.Value != "") ? false : keyValuePair.Value != null))
				{
					strs.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return strs;
		}

		public static string GetAbstractToMD5(Stream sFile)
		{
			byte[] numArray = (new MD5CryptoServiceProvider()).ComputeHash(sFile);
			StringBuilder stringBuilder = new StringBuilder(32);
			for (int i = 0; i < numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
		}

		public static string GetAbstractToMD5(byte[] dataFile)
		{
			byte[] numArray = (new MD5CryptoServiceProvider()).ComputeHash(dataFile);
			StringBuilder stringBuilder = new StringBuilder(32);
			for (int i = 0; i < numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
		}

		public static void LogResult(string sWord)
		{
			string str = HttpContext.Current.Server.MapPath("log");
			DateTime now = DateTime.Now;
			str = string.Concat(str, "\\", now.ToString().Replace(":", ""), ".txt");
			StreamWriter streamWriter = new StreamWriter(str, false, Encoding.Default);
			streamWriter.Write(sWord);
			streamWriter.Close();
		}
	}
}