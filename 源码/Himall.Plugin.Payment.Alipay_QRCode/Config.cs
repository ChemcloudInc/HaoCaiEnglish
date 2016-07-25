using Himall.PaymentPlugin;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.Payment.Alipay_QRCode
{
	public class Config : ConfigBase
	{
		private static string partner;

		private static string input_charset;

		private static string sign_type;

		public string gateWay
		{
			get;
			set;
		}

		public string getCodeService
		{
			get;
			set;
		}

		public string HelpImage
		{
			get;
			set;
		}

		public string Input_charset
		{
			get
			{
				return Config.input_charset;
			}
			set
			{
				Config.input_charset = value;
			}
		}

		public string key
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public string Partner
		{
			get
			{
				return Config.partner;
			}
			set
			{
				Config.partner = value;
			}
		}

		public string Sign_type
		{
			get
			{
				return Config.sign_type;
			}
			set
			{
				Config.sign_type = value;
			}
		}

		public string trade_type
		{
			get;
			set;
		}

		static Config()
		{
			Config.partner = "";
			Config.input_charset = "";
			Config.sign_type = "";
			Config.input_charset = "utf-8";
			Config.sign_type = "MD5";
		}

		public Config()
		{
		}
	}
}