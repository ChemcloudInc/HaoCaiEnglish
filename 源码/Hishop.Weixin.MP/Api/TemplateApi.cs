using Hishop.Weixin.MP.Domain;
using Hishop.Weixin.MP.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hishop.Weixin.MP.Api
{
	public class TemplateApi
	{
		public TemplateApi()
		{
		}

		public static void SendMessage(string accessTocken, TemplateMessage templateMessage)
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			stringBuilder.AppendFormat("\"touser\":\"{0}\",", templateMessage.Touser);
			stringBuilder.AppendFormat("\"template_id\":\"{0}\",", templateMessage.TemplateId);
			stringBuilder.AppendFormat("\"url\":\"{0}\",", templateMessage.Url);
			stringBuilder.AppendFormat("\"topcolor\":\"{0}\",", templateMessage.Topcolor);
			stringBuilder.Append("\"data\":{");
			foreach (TemplateMessage.MessagePart datum in templateMessage.Data)
			{
				stringBuilder.AppendFormat("\"{0}\":{{\"value\":\"{1}\",\"color\":\"{2}\"}},", datum.Name, datum.Value, datum.Color);
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("}}");
			WebUtils webUtil = new WebUtils();
			string str = string.Concat("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=", accessTocken);
			webUtil.DoPost(str, stringBuilder.ToString());
		}
	}
}