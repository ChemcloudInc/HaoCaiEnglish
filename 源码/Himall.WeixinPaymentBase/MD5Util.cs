using System;
using System.Security.Cryptography;
using System.Text;

namespace Himall.WeixinPaymentBase
{
	public class MD5Util
	{
		public MD5Util()
		{
		}

		public static string GetMD5(string encypStr, string charset)
		{
			byte[] bytes;
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			try
			{
				bytes = Encoding.GetEncoding(charset).GetBytes(encypStr);
			}
			catch (Exception exception)
			{
				bytes = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
			}
			string str = BitConverter.ToString(mD5CryptoServiceProvider.ComputeHash(bytes));
			return str.Replace("-", "").ToUpper();
		}
	}
}