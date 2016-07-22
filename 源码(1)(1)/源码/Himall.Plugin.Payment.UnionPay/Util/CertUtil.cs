using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Himall.Plugin.Payment.UnionPay.Util
{
	public class CertUtil
	{
		public CertUtil()
		{
		}

		public static string GetSignCertId(string signCertpath, string signCertpwd)
		{
			X509Certificate2 x509Certificate2 = new X509Certificate2(signCertpath, signCertpwd);
			return BigInteger.Parse(x509Certificate2.SerialNumber, NumberStyles.HexNumber).ToString();
		}

		public static RSACryptoServiceProvider GetSignProviderFromPfx(string signCertpath, string signCertpwd)
		{
			return (RSACryptoServiceProvider)(new X509Certificate2(signCertpath, signCertpwd)).PrivateKey;
		}

		public static RSACryptoServiceProvider GetValidateProviderFromPath(string certId, string validateCertdir)
		{
			RSACryptoServiceProvider key;
			FileInfo[] files = (new DirectoryInfo(validateCertdir)).GetFiles("*.cer");
			if ((files == null ? false : 0 != files.Length))
			{
				FileInfo[] fileInfoArray = files;
				int num = 0;
				while (num < fileInfoArray.Length)
				{
					FileInfo fileInfo = fileInfoArray[num];
					X509Certificate2 x509Certificate2 = new X509Certificate2(string.Concat(fileInfo.DirectoryName, "\\", fileInfo.Name));
					if (!certId.Equals(BigInteger.Parse(x509Certificate2.SerialNumber, NumberStyles.HexNumber).ToString()))
					{
						num++;
					}
					else
					{
						key = (RSACryptoServiceProvider)x509Certificate2.PublicKey.Key;
						return key;
					}
				}
				key = null;
			}
			else
			{
				key = null;
			}
			return key;
		}
	}
}