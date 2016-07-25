using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Himall.Plugin.Payment.UnionPay.Util
{
	public class SignUtil
	{
		public SignUtil()
		{
		}

		public static string CoverDictionaryToString(Dictionary<string, string> data)
		{
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>(StringComparer.Ordinal);
			foreach (KeyValuePair<string, string> datum in data)
			{
				strs.Add(datum.Key, datum.Value);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> str in strs)
			{
				stringBuilder.Append(string.Concat(str.Key, "=", str.Value, "&"));
			}
			string str1 = stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
			return str1;
		}

		public static Dictionary<string, string> CoverstringToDictionary(string data)
		{
			Dictionary<string, string> strs;
			if ((data == null ? false : 0 != data.Length))
			{
				string[] strArrays = data.Split(new char[] { '&' });
				Dictionary<string, string> strs1 = new Dictionary<string, string>();
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					int num = str.IndexOf("=");
					string str1 = str.Substring(0, num);
					string str2 = str.Substring(num + 1);
					Console.WriteLine(string.Concat(str1, "=", str2));
					strs1.Add(str1, str2);
				}
				strs = strs1;
			}
			else
			{
				strs = null;
			}
			return strs;
		}

		public static string CreateAutoSubmitForm(string url, Dictionary<string, string> data, Encoding encoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html>");
			stringBuilder.AppendLine("<head>");
			stringBuilder.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoder.BodyName);
			stringBuilder.AppendLine("</head>");
			stringBuilder.AppendLine("<body onload=\"OnLoadSubmit();\">");
			stringBuilder.AppendFormat("<form id=\"pay_form\" action=\"{0}\" method=\"post\">", url);
			foreach (KeyValuePair<string, string> datum in data)
			{
				stringBuilder.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", datum.Key, datum.Value);
			}
			stringBuilder.AppendLine("</form>");
			stringBuilder.AppendLine("<script type=\"text/javascript\">");
			stringBuilder.AppendLine("<!--");
			stringBuilder.AppendLine("function OnLoadSubmit()");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("document.getElementById(\"pay_form\").submit();");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("//-->");
			stringBuilder.AppendLine("</script>");
			stringBuilder.AppendLine("</body>");
			stringBuilder.AppendLine("</html>");
			return stringBuilder.ToString();
		}

		public static string encryptData(string data, string encoding, string EncryptCert)
		{
			X509Certificate2 x509Certificate2 = new X509Certificate2(EncryptCert);
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider = (RSACryptoServiceProvider)x509Certificate2.PublicKey.Key;
			byte[] numArray = rSACryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(data), false);
			return Convert.ToBase64String(numArray);
		}

		public static string encryptPin(string card, string pwd, string encoding, string EncryptCert)
		{
			byte[] numArray = SecurityUtil.pin2PinBlockWithCardNO(pwd, card);
			SignUtil.printHexString(numArray);
			X509Certificate2 x509Certificate2 = new X509Certificate2(EncryptCert);
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider = (RSACryptoServiceProvider)x509Certificate2.PublicKey.Key;
			return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(numArray, false));
		}

		public static string PrintDictionaryToString(Dictionary<string, string> data)
		{
			SortedDictionary<string, string> strs = new SortedDictionary<string, string>(StringComparer.Ordinal);
			foreach (KeyValuePair<string, string> datum in data)
			{
				strs.Add(datum.Key, datum.Value);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> str in strs)
			{
				stringBuilder.Append(string.Concat(str.Key, "=", str.Value, "&"));
			}
			string str1 = stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
			return str1;
		}

		public static string printHexString(byte[] b)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < b.Length; i++)
			{
				string str = Convert.ToString(b[i] & 255, 16);
				if (str.Length == 1)
				{
					str = string.Concat('0', str);
				}
				stringBuilder.Append("0x");
				stringBuilder.Append(string.Concat(str, " "));
			}
			stringBuilder.Append("");
			return stringBuilder.ToString();
		}

		public static bool Sign(Dictionary<string, string> data, Encoding encoder, string signCertpath, string signCertpwd)
		{
			data["certId"] = CertUtil.GetSignCertId(signCertpath, signCertpwd);
			string str = SignUtil.CoverDictionaryToString(data);
			byte[] numArray = SecurityUtil.Sha1X16(str, encoder);
			string lower = BitConverter.ToString(numArray).Replace("-", "").ToLower();
			byte[] numArray1 = SecurityUtil.SignBySoft(CertUtil.GetSignProviderFromPfx(signCertpath, signCertpwd), encoder.GetBytes(lower));
			data["signature"] = Convert.ToBase64String(numArray1);
			return true;
		}

		public static bool Validate(Dictionary<string, string> data, Encoding encoder, string validateCertdir)
		{
			bool flag;
			byte[] numArray = Convert.FromBase64String(data["signature"]);
			data.Remove("signature");
			byte[] numArray1 = SecurityUtil.Sha1X16(SignUtil.CoverDictionaryToString(data), encoder);
			string lower = BitConverter.ToString(numArray1).Replace("-", "").ToLower();
			RSACryptoServiceProvider validateProviderFromPath = CertUtil.GetValidateProviderFromPath(data["certId"], validateCertdir);
			flag = (null != validateProviderFromPath ? SecurityUtil.ValidateBySoft(validateProviderFromPath, numArray, encoder.GetBytes(lower)) : false);
			return flag;
		}
	}
}