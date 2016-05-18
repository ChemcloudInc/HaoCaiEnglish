using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OpenIdsInfo : BaseModel
	{
		private long _id;

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

		public bool IsSubscribe
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public DateTime SubscribeTime
		{
			get;
			set;
		}

		public OpenIdsInfo()
		{
		}
	}
}