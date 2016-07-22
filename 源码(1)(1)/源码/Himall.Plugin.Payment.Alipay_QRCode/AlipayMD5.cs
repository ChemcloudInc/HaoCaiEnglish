using System;
using System.Security.Cryptography;
using System.Text;

namespace Himall.Plugin.Payment.Alipay_QRCode
{
	public sealed class AlipayMD5
	{
		public AlipayMD5()
		{
		}

		public static string Sign(string prestr, string key, string _input_charset)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			prestr = string.Concat(prestr, key);
			MD5 mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] numArray = mD5CryptoServiceProvider.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
			for (int i = 0; i < numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
		}

		public static bool Verify(string prestr, string sign, string key, string _input_charset)
		{
			bool flag;
			flag = (!(AlipayMD5.Sign(prestr, key, _input_charset) == sign) ? false : true);
			return flag;
		}
	}
}