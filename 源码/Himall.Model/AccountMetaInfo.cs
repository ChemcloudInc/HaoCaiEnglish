using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AccountMetaInfo : BaseModel
	{
		private long _id;

		public long AccountId
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
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

		public DateTime ServiceEndTime
		{
			get;
			set;
		}

		public DateTime ServiceStartTime
		{
			get;
			set;
		}

		public AccountMetaInfo()
		{
		}
	}
}