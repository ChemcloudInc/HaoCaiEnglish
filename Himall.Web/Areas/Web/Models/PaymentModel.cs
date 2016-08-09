using Himall.Core.Plugins.Payment;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class PaymentModel
	{
		public string Id
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public string RequestUrl
		{
			get;
			set;
		}

		public Himall.Core.Plugins.Payment.UrlType UrlType
		{
			get;
			set;
		}

		public PaymentModel()
		{
		}
	}
}