using Himall.PaymentPlugin;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Plugin.Payment.UnionPay.Util
{
	public class ChinaUnionConfig : ConfigBase
	{
		public string accessType
		{
			get;
			set;
		}

		public string backTransUrl
		{
			get;
			set;
		}

		public string bizType
		{
			get;
			set;
		}

		public string channelType
		{
			get;
			set;
		}

		public string currencyCode
		{
			get;
			set;
		}

		public string encoding
		{
			get;
			set;
		}

		public string encryptCertpath
		{
			get;
			set;
		}

		public string frontTransUrl
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public string merId
		{
			get;
			set;
		}

		public string signCertpath
		{
			get;
			set;
		}

		public string signCertpwd
		{
			get;
			set;
		}

		public string signCerttype
		{
			get;
			set;
		}

		public string signMethod
		{
			get;
			set;
		}

		public string singleQueryUrl
		{
			get;
			set;
		}

		public string txnSubType
		{
			get;
			set;
		}

		public string txnType
		{
			get;
			set;
		}

		public string version
		{
			get;
			set;
		}

		public ChinaUnionConfig()
		{
		}
	}
}