using System;
using System.Web.Security;

namespace Hishop.Weixin.MP.Util
{
	public class CheckSignature
	{
		public readonly static string Token;

		static CheckSignature()
		{
			CheckSignature.Token = "weixin_test";
		}

		public CheckSignature()
		{
		}

		public static bool Check(string signature, string timestamp, string nonce, string token)
		{
			token = token ?? CheckSignature.Token;
			string[] strArrays = new string[] { timestamp, nonce, token };
			Array.Sort<string>(strArrays);
			string str = string.Join("", strArrays);
			str = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
			return signature == str.ToLower();
		}
	}
}