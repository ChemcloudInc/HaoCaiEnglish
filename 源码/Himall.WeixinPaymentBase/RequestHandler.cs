using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Himall.WeixinPaymentBase
{
	public class RequestHandler
	{
		private string Key;

		protected Hashtable Parameters;

		private string DebugInfo;

		public RequestHandler()
		{
            Parameters = new Hashtable();
		}

		public virtual string CreateMd5Sign(string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ArrayList arrayLists = new ArrayList(Parameters.Keys);
			arrayLists.Sort();
			foreach (string arrayList in arrayLists)
			{
				string item = (string)Parameters[arrayList];
				if ((item == null || "".CompareTo(item) == 0 || "sign".CompareTo(arrayList) == 0 ? false : "key".CompareTo(arrayList) != 0))
				{
					stringBuilder.Append(string.Concat(arrayList, "=", item, "&"));
				}
			}
			stringBuilder.Append(string.Concat(key, "=", value));
			string upper = MD5Util.GetMD5(stringBuilder.ToString(), GetCharset()).ToUpper();
			return upper;
		}

		public Hashtable GetAllParameters()
		{
			return Parameters;
		}

		protected virtual string GetCharset()
		{
			return "UTF-8";
		}

		public string GetDebugInfo()
		{
			return DebugInfo;
		}

		public string GetKey()
		{
			return Key;
		}

		public virtual void Init()
		{
		}

		public string ParseXML()
		{
			string[] strArrays;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<xml>");
			foreach (string key in Parameters.Keys)
			{
				string item = (string)Parameters[key];
				if (item == null)
				{
					item = string.Empty;
				}
				if (!Regex.IsMatch(item, "^[0-9.]$"))
				{
					strArrays = new string[] { "<", key, "><![CDATA[", item, "]]></", key, ">" };
					stringBuilder.Append(string.Concat(strArrays));
				}
				else
				{
					strArrays = new string[] { "<", key, ">", item, "</", key, ">" };
					stringBuilder.Append(string.Concat(strArrays));
				}
			}
			stringBuilder.Append("</xml>");
			return stringBuilder.ToString();
		}

		public void SetDebugInfo(string debugInfo)
		{
            DebugInfo = debugInfo;
		}

		public void SetKey(string key)
		{
            Key = key;
		}

		public void SetParameter(string parameter, string parameterValue)
		{
			if ((parameter == null ? false : parameter != ""))
			{
				if (Parameters.Contains(parameter))
				{
                    Parameters.Remove(parameter);
				}
                Parameters.Add(parameter, parameterValue);
			}
		}
	}
}