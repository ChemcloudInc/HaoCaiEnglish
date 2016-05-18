using System;
using System.Text;
using System.Web;

namespace Himall.WeixinPaymentBase
{
	public class TenPayUtil
	{
		public TenPayUtil()
		{
		}

		public static string BuildRandomStr(int length)
		{
			string str = (new Random()).Next().ToString();
			if (str.Length > length)
			{
				str = str.Substring(0, length);
			}
			else if (str.Length < length)
			{
				for (int i = length - str.Length; i > 0; i--)
				{
					str.Insert(0, "0");
				}
			}
			return str;
		}

		public static string GetNoncestr()
		{
			int num = (new Random()).Next(1000);
			return MD5Util.GetMD5(num.ToString(), "GBK");
		}

		public static string GetTimestamp()
		{
			TimeSpan utcNow = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(utcNow.TotalSeconds).ToString();
		}

		public static uint UnixStamp()
		{
			TimeSpan now = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			return Convert.ToUInt32(now.TotalSeconds);
		}

		public static string UrlDecode(string instr, string charset)
		{
			string str;
			string str1;
			if ((instr == null ? false : !(instr.Trim() == "")))
			{
				try
				{
					str = HttpUtility.UrlDecode(instr, Encoding.GetEncoding(charset));
				}
				catch (Exception exception)
				{
					str = HttpUtility.UrlDecode(instr, Encoding.GetEncoding("GB2312"));
				}
				str1 = str;
			}
			else
			{
				str1 = "";
			}
			return str1;
		}

		public static string UrlEncode(string instr, string charset)
		{
			string str;
			string str1;
			if ((instr == null ? false : !(instr.Trim() == "")))
			{
				try
				{
					str = HttpUtility.UrlEncode(instr, Encoding.GetEncoding(charset));
				}
				catch (Exception exception)
				{
					str = HttpUtility.UrlEncode(instr, Encoding.GetEncoding("GB2312"));
				}
				str1 = str;
			}
			else
			{
				str1 = "";
			}
			return str1;
		}
	}
}