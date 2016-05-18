using Himall.PaymentPlugin;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.Payment.Alipay
{
	public class Config : ConfigBase
	{
		private static string _input_charset;

		private static string _sign_Type;

		public string AlipayAccount
		{
			get;
			set;
		}

		public string GateWay
		{
			get;
			set;
		}

		[XmlIgnore]
		public string Input_charset
		{
			get
			{
				return Config._input_charset;
			}
		}

		public string Key
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		[Required(ErrorMessage="分类名称必填,且不能多于5个字符")]
		public string Partner
		{
			get;
			set;
		}

		[XmlIgnore]
		public static string PluginListUrl
		{
			get;
			set;
		}

		[XmlIgnore]
		public string Sign_type
		{
			get
			{
				return Config._sign_Type;
			}
		}

		public string VeryfyUrl
		{
			get;
			set;
		}

		static Config()
		{
			Config._input_charset = "utf-8";
			Config._sign_Type = "MD5";
		}

		public Config()
		{
		}
	}
}