using System;
using System.Security.Cryptography;
using System.Text;

namespace Himall.WeixinPaymentBase
{
	internal class SHA1Util
	{
		public SHA1Util()
		{
		}

		public static string getSha1(string str)
		{
			SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			byte[] numArray = sHA1CryptoServiceProvider.ComputeHash((new ASCIIEncoding()).GetBytes(str));
			return BitConverter.ToString(numArray).Replace("-", "");
		}
	}
}