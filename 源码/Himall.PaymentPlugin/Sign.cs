using System;
using System.Security.Cryptography;
using System.Text;

namespace Himall.PaymentPlugin
{
	public class Sign
	{
		public Sign()
		{
		}

		public static string MD5(string prestr, string key, string _input_charset)
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

		public static string SHA1(string str)
		{
			SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			byte[] numArray = sHA1CryptoServiceProvider.ComputeHash((new ASCIIEncoding()).GetBytes(str));
			return BitConverter.ToString(numArray).Replace("-", "");
		}

		public static bool VerifyMD5(string prestr, string sign, string key, string _input_charset)
		{
			bool flag;
			flag = (!(Sign.MD5(prestr, key, _input_charset) == sign) ? false : true);
			return flag;
		}
	}
}