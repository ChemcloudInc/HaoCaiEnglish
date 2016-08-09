using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Market
{
	public class BonusReceiveModel
	{
		public bool IsTransformedDeposit
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public string ReceiveTime
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public BonusReceiveModel()
		{
		}
	}
}