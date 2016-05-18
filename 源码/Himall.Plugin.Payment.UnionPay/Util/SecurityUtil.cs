using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Himall.Plugin.Payment.UnionPay.Util
{
	public static class SecurityUtil
	{
		public static string ALGORITHM_SHA1;

		private static ChinaUnionConfig CUConfig;

		static SecurityUtil()
		{
			SecurityUtil.ALGORITHM_SHA1 = "SHA1";
			SecurityUtil.CUConfig = new ChinaUnionConfig();
		}

		public static string DecodeBase64(Encoding encode, string result)
		{
			string str = "";
			byte[] numArray = Convert.FromBase64String(result);
			try
			{
				str = encode.GetString(numArray);
			}
			catch
			{
				str = result;
			}
			return str;
		}

		public static byte[] deflater(byte[] inputByte)
		{
			byte[] numArray = new byte[1024];
			return (new MemoryStream()).ToArray();
		}

		public static string EncodeBase64(Encoding encode, string source)
		{
			string base64String = "";
			byte[] bytes = encode.GetBytes(source);
			try
			{
				base64String = Convert.ToBase64String(bytes);
			}
			catch
			{
				base64String = source;
			}
			return base64String;
		}

		public static string EncryptData(string dataString, string encoding, string EncryptCert)
		{
			string str;
			byte[] numArray = null;
			try
			{
				numArray = SecurityUtil.encryptedData(Encoding.UTF8.GetBytes(dataString), EncryptCert);
				str = SecurityUtil.EncodeBase64(Encoding.Default, Encoding.Default.GetString(numArray));
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				str = "";
			}
			return str;
		}

		public static byte[] encryptedData(byte[] encData, string EncryptCert)
		{
			byte[] numArray;
			try
			{
				X509Certificate2 x509Certificate2 = new X509Certificate2(EncryptCert);
				Console.WriteLine(EncryptCert);
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
				rSACryptoServiceProvider = (RSACryptoServiceProvider)x509Certificate2.PublicKey.Key;
				numArray = rSACryptoServiceProvider.Encrypt(encData, false);
				return numArray;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			numArray = null;
			return numArray;
		}

		public static string EncryptPin(string pin, string card, string encoding, string EncryptCert)
		{
			string str;
			byte[] numArray = SecurityUtil.pin2PinBlockWithCardNO(pin, card);
			byte[] numArray1 = null;
			try
			{
				numArray1 = SecurityUtil.encryptedData(numArray, EncryptCert);
				str = SecurityUtil.EncodeBase64(Encoding.Default, Encoding.Default.GetString(numArray1));
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				str = "";
			}
			return str;
		}

		private static byte[] formatPan(string aPan)
		{
			int length = aPan.Length;
			byte[] num = new byte[8];
			int num1 = length - 13;
			try
			{
				num[0] = 0;
				num[1] = 0;
				for (int i = 2; i < 8; i++)
				{
					string str = aPan.Substring(num1, 2).Trim();
					num[i] = (byte)Convert.ToInt32(str, 16);
					num1 = num1 + 2;
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			return num;
		}

		public static byte[] inflater(byte[] inputByte)
		{
			byte[] numArray = new byte[1024];
			return (new MemoryStream()).ToArray();
		}

		private static byte[] pin2PinBlock(string aPin)
		{
			int i;
			string str;
			int j;
			int num = 1;
			int length = aPin.Length;
			byte[] numArray = new byte[8];
			try
			{
				numArray[0] = (byte)Convert.ToInt32(length.ToString(), 10);
				if (length % 2 != 0)
				{
					for (i = 0; i < length - 1; i = i + 2)
					{
						str = aPin.Substring(i, 2);
						numArray[num] = (byte)Convert.ToInt32(str, 16);
						if (i == length - 3)
						{
							string str1 = string.Concat(aPin.Substring(length - 1), "F");
							numArray[num + 1] = (byte)Convert.ToInt32(str1, 16);
							if (num + 1 < 7)
							{
								for (j = num + 2; j < 8; j++)
								{
									numArray[j] = 255;
								}
							}
						}
						num++;
					}
				}
				else
				{
					for (i = 0; i < length; i = i + 2)
					{
						str = aPin.Substring(i, 2).Trim();
						numArray[num] = (byte)Convert.ToInt32(str, 16);
						if (i == length - 2)
						{
							if (num < 7)
							{
								for (j = num + 1; j < 8; j++)
								{
									numArray[j] = 255;
								}
							}
						}
						num++;
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			return numArray;
		}

		public static byte[] pin2PinBlockWithCardNO(string aPin, string aCardNO)
		{
			byte[] numArray = SecurityUtil.pin2PinBlock(aPin);
			if (aCardNO.Length == 11)
			{
				aCardNO = string.Concat("00", aCardNO);
			}
			else if (aCardNO.Length == 12)
			{
				aCardNO = string.Concat("0", aCardNO);
			}
			byte[] numArray1 = SecurityUtil.formatPan(aCardNO);
			byte[] numArray2 = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				numArray2[i] = (byte)(numArray[i] ^ numArray1[i]);
			}
			return numArray2;
		}

		public static byte[] Sha1X16(string dataStr, Encoding encoding)
		{
			byte[] numArray;
			try
			{
				byte[] bytes = encoding.GetBytes(dataStr);
				SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
				numArray = sHA1CryptoServiceProvider.ComputeHash(bytes, 0, bytes.Length);
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return numArray;
		}

		public static byte[] SignBySoft(RSACryptoServiceProvider provider, byte[] data)
		{
			byte[] numArray;
			byte[] numArray1 = null;
			try
			{
				numArray1 = provider.SignData(data, new SHA1CryptoServiceProvider());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			if (null != numArray1)
			{
				numArray = numArray1;
			}
			else
			{
				numArray = null;
			}
			return numArray;
		}

		public static bool ValidateBySoft(RSACryptoServiceProvider provider, byte[] base64DecodingSignStr, byte[] srcByte)
		{
			return provider.VerifyData(srcByte, new SHA1CryptoServiceProvider(), base64DecodingSignStr);
		}
	}
}