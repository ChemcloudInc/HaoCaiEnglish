using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AccountMetaModel
	{
		public long AccountId
		{
			get;
			set;
		}

		public string DateRange
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string MetaKey
		{
			get;
			set;
		}

		public string MetaValue
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public AccountMetaModel()
		{
		}
	}
}