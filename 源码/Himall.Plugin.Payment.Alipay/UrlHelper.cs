using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Himall.PaymentPlugin.Alipay
{
	public class UrlHelper
	{
		public UrlHelper()
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

		public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre, params string[] ignorKeys)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> keyValuePair in dicArrayPre)
			{
				if ((ignorKeys.Contains<string>(keyValuePair.Key.ToLower()) ? false : !string.IsNullOrEmpty(keyValuePair.Value)))
				{
					strs.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return strs;
		}

		public static SortedDictionary<string, string> GetRequestGet(NameValueCollection queryString)
		{
			int i = 0;
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>();
			string[] allKeys = queryString.AllKeys;
			for (i = 0; i < allKeys.Length; i++)
			{
				strs.Add(allKeys[i], queryString[allKeys[i]]);
			}
			return strs;
		}

		public static SortedDictionary<string, string> GetRequestPost(NameValueCollection form)
		{
			int i = 0;
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>();
			string[] allKeys = form.AllKeys;
			for (i = 0; i < allKeys.Length; i++)
			{
				strs.Add(allKeys[i], form[allKeys[i]]);
			}
			return strs;
		}
	}
}