using System;
using System.Collections.Generic;
using System.Text;

namespace Himall.Plugin.Payment.UnionPay.Util
{
	public class BigNum
	{
		private const ulong base32 = 4294967296L;

		private const ulong base32_10 = 1000000000L;

		private const int wLen = 8;

		private readonly static ulong[] baseMod;

		private readonly static char[] trimZero;

		static BigNum()
		{
			BigNum.baseMod = new ulong[] { 1L, 294967296L, 709551616L, 543950336L, 768211456L, 932542976L, 34512896L, 610249216L, 129639936L, 375533056L, 86936576L, 746218496L, 990306816L, 725889536L, 628614656L, 306290176L };
			BigNum.trimZero = new char[] { '0' };
		}

		public BigNum()
		{
		}

		public static ulong[] ConvertFromHex(string s)
		{
			int length = s.Length;
			int num = length - 8;
			int num1 = (length - 1) / 8 + 1;
			ulong[] numArray = new ulong[num1];
			int num2 = num1 - 1;
			while (num >= 0)
			{
				string str = s.Substring(num, 8);
				numArray[num2] = Convert.ToUInt64(str, 16);
				num2--;
				num = num - 8;
			}
			if (num > -8)
			{
				numArray[0] = Convert.ToUInt64(s.Substring(0, num + 8), 16);
			}
			return numArray;
		}

		private static ulong[] Div(ulong[] ll)
		{
			List<ulong> nums = new List<ulong>();
			int upperBound = ll.GetUpperBound(0);
			int num = 0;
			ulong num1 = 0;
			ulong num2 = 0;
			ulong num3 = 0;
			bool flag = true;
			while (num <= upperBound)
			{
				num1 = ll[num] + num2;
				num2 = num1 % 1000000000;
				num3 = num1 / 1000000000;
				if ((!flag ? true : num3 != 0))
				{
					nums.Add(num3);
					flag = false;
				}
				num2 = num2 * 4294967296L;
				num++;
			}
			return nums.ToArray();
		}

		private static ulong GetBaseMod(int pow)
		{
			return BigNum.baseMod[pow];
		}

		private static ulong Mod(ulong[] ll)
		{
			int upperBound = ll.GetUpperBound(0);
			int num = 0;
			ulong baseMod = 0;
			ulong num1 = ll[upperBound] % 1000000000;
			while (num < upperBound)
			{
				baseMod = ll[num] % 1000000000;
				baseMod = baseMod * BigNum.GetBaseMod(upperBound - num);
				num1 = num1 + baseMod % 1000000000;
				num++;
			}
			return num1 % 1000000000;
		}

		public static string ToDecimalStr(ulong[] ll)
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder();
			List<string> strs = new List<string>();
			while (true)
			{
				if (ll.Length <= 0 ? true : ll[0] == 0)
				{
					break;
				}
				ulong num = BigNum.Mod(ll);
				ll = BigNum.Div(ll);
				strs.Add(num.ToString("D9"));
			}
			for (int i = strs.Count - 1; i >= 0; i--)
			{
				stringBuilder.Append(strs[i]);
			}
			str = (stringBuilder.Length != 0 ? stringBuilder.ToString().TrimStart(BigNum.trimZero) : "0");
			return str;
		}
	}
}