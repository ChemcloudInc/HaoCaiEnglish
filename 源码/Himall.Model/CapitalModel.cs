using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CapitalModel
	{
		public decimal Balance
		{
			get;
			set;
		}

		public decimal ChargeAmount
		{
			get;
			set;
		}

		public decimal FreezeAmount
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string UserCode
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public CapitalModel()
		{
		}
	}
}