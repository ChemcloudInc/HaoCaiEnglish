using System;
using System.IO;
using System.Text;

namespace Himall.WeixinPaymentBase
{
	public static class TenPayV3
	{
		public static string Refund(string data, string cert, string password)
		{
			string str = "https://api.mch.weixin.qq.com/secapi/pay/refund";
			byte[] numArray = (data == null ? new byte[0] : Encoding.UTF8.GetBytes(data));
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(numArray, 0, numArray.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			string str1 = RequestUtility.HttpPost(str, cert, password, null, memoryStream, null, null, null);
			return str1;
		}

		public static string transfers(string data, string cert, string password)
		{
			string str = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
			byte[] numArray = (data == null ? new byte[0] : Encoding.UTF8.GetBytes(data));
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(numArray, 0, numArray.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			string str1 = RequestUtility.HttpPost(str, cert, password, null, memoryStream, null, null, null);
			return str1;
		}

		public static string Unifiedorder(string data)
		{
			string str = "https://api.mch.weixin.qq.com/pay/unifiedorder";
			byte[] numArray = (data == null ? new byte[0] : Encoding.UTF8.GetBytes(data));
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(numArray, 0, numArray.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			string str1 = RequestUtility.HttpPost(str, null, memoryStream, null, null, null);
			return str1;
		}
	}
}