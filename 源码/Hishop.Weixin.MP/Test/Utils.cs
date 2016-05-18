using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Handler;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Response;
using Hishop.Weixin.MP.Util;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Test
{
	internal class Utils
	{
		private const string xml = "<xml><ToUserName><![CDATA[gh_ef4e2090afe3]]></ToUserName><FromUserName><![CDATA[opUMDj9jbOmTtbZuE2hM6wnv27B0]]></FromUserName><CreateTime>1385887183</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[s]]></Content><MsgId>5952340126940580233</MsgId></xml>";

		public Utils()
		{
		}

		public AbstractRequest ConvertRequest<T>(Stream inputStream)
		where T : AbstractRequest
		{
			AbstractRequest abstractRequest;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(inputStream);
			if (!(xmlDocument.SelectSingleNode("xml/MsgType").InnerText.ToLower() != "text"))
			{
				TextRequest textRequest = new TextRequest()
				{
					Content = xmlDocument.SelectSingleNode("xml/Content").InnerText,
					FromUserName = xmlDocument.SelectSingleNode("xml/FromUserName").InnerText,
					MsgId = Convert.ToInt32(xmlDocument.SelectSingleNode("xml/FromUserName").InnerText)
                };
				abstractRequest = textRequest;
			}
			else
			{
				abstractRequest = default(T);
			}
			return abstractRequest;
		}

		public string MethodName()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml("<xml><ToUserName><![CDATA[gh_ef4e2090afe3]]></ToUserName><FromUserName><![CDATA[opUMDj9jbOmTtbZuE2hM6wnv27B0]]></FromUserName><CreateTime>1385887183</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[s]]></Content><MsgId>5952340126940580233</MsgId></xml>");
			return xmlDocument.SelectSingleNode("xml/ToUserName").InnerText;
		}

		public void Test02()
		{
			XDocument xDocument = XDocument.Parse("<xml><ToUserName><![CDATA[gh_ef4e2090afe3]]></ToUserName><FromUserName><![CDATA[opUMDj9jbOmTtbZuE2hM6wnv27B0]]></FromUserName><CreateTime>1385887183</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[s]]></Content><MsgId>5952340126940580233</MsgId></xml>");
			EntityHelper.FillEntityWithXml<TextRequest>(new TextRequest(), xDocument);
		}

		public string Test03()
		{
			TextResponse textResponse = new TextResponse()
			{
				Content = "hah",
				FromUserName = "123",
				ToUserName = "456"
			};
			return EntityHelper.ConvertEntityToXml<TextResponse>(textResponse).ToString();
		}

		public void Test04()
		{
			A a = new A("<xml><ToUserName><![CDATA[gh_ef4e2090afe3]]></ToUserName><FromUserName><![CDATA[opUMDj9jbOmTtbZuE2hM6wnv27B0]]></FromUserName><CreateTime>1385887183</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[s]]></Content><MsgId>5952340126940580233</MsgId></xml>");
			object requestDocument = a.RequestDocument;
		}
	}
}