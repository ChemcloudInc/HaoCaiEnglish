using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Himall.WeixinPaymentBase
{
	public class ResponseHandler
	{
		private string key;

		private string appkey;

		private Hashtable xmlMap;

		protected Hashtable parameters;

		private string debugInfo;

		protected string content;

		private string charset = "gb2312";

		private static string SignField;

		protected HttpRequestBase request;

		static ResponseHandler()
		{
			ResponseHandler.SignField = "appid,appkey,timestamp,openid,noncestr,issubscribe";
		}

		public ResponseHandler(HttpRequestBase request)
		{
			NameValueCollection form;
            parameters = new Hashtable();
            xmlMap = new Hashtable();
			this.request = request;
			if (this.request.HttpMethod == "POST")
			{
				form = request.Form;
				foreach (string str in form)
				{
                    setParameter(str, form[str]);
				}
			}
			form = this.request.QueryString;
			foreach (string str1 in form)
			{
                setParameter(str1, form[str1]);
			}
			if (this.request.InputStream.Length > 0)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(this.request.InputStream);
				foreach (XmlNode childNode in xmlDocument.SelectSingleNode("xml").ChildNodes)
				{
                    setParameter(childNode.Name, childNode.InnerText);
                    xmlMap.Add(childNode.Name, childNode.InnerText);
				}
			}
		}

		protected virtual string getCharset()
		{
			return request.ContentEncoding.BodyName;
		}

		public string getKey()
		{
			return key;
		}

		public string getParameter(string parameter)
		{
			string item = (string)parameters[parameter];
			return (item == null ? "" : item);
		}

		public virtual void init()
		{
		}

		public virtual bool isTenpaySign()
		{
			StringBuilder stringBuilder = new StringBuilder();
			ArrayList arrayLists = new ArrayList(parameters.Keys);
			arrayLists.Sort();
			foreach (string arrayList in arrayLists)
			{
				string item = (string)parameters[arrayList];
				if ((item == null || "".CompareTo(item) == 0 || "sign".CompareTo(arrayList) == 0 ? false : "key".CompareTo(arrayList) != 0))
				{
					stringBuilder.Append(string.Concat(arrayList, "=", item, "&"));
				}
			}
			stringBuilder.Append(string.Concat("key=", getKey()));
			string lower = MD5Util.GetMD5(stringBuilder.ToString(), getCharset()).ToLower();
            setDebugInfo(string.Concat(stringBuilder.ToString(), " => sign:", lower));
			bool flag = getParameter("sign").ToLower().Equals(lower);
			return flag;
		}

		public virtual bool isWXsign()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Hashtable hashtables = new Hashtable();
			foreach (string key in xmlMap.Keys)
			{
				if ((key == "SignMethod" ? false : key != "AppSignature"))
				{
					hashtables.Add(key.ToLower(), xmlMap[key]);
				}
			}
			hashtables.Add("appkey", appkey);
			ArrayList arrayLists = new ArrayList(hashtables.Keys);
			arrayLists.Sort();
			foreach (string arrayList in arrayLists)
			{
				string item = (string)hashtables[arrayList];
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(string.Concat("&", arrayList, "=", item));
				}
				else
				{
					stringBuilder.Append(string.Concat(arrayList, "=", item));
				}
			}
			string lower = SHA1Util.getSha1(stringBuilder.ToString()).ToString().ToLower();
            setDebugInfo(string.Concat(stringBuilder.ToString(), " => SHA1 sign:", lower));
			return lower.Equals(xmlMap["AppSignature"]);
		}

		protected void setDebugInfo(string debugInfo)
		{
			this.debugInfo = debugInfo;
		}

		public void setKey(string key, string appkey)
		{
			this.key = key;
			this.appkey = appkey;
		}

		public void setParameter(string parameter, string parameterValue)
		{
			if ((parameter == null ? false : parameter != ""))
			{
				if (parameters.Contains(parameter))
				{
                    parameters.Remove(parameter);
				}
                parameters.Add(parameter, parameterValue);
			}
		}
	}
}