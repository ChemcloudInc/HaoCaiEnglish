using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MessageLog : BaseModel
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

		public string MessageContent
		{
			get;
			set;
		}

		public DateTime? SendTime
		{
			get;
			set;
		}

		public long? ShopId
		{
			get;
			set;
		}

		public virtual ShopInfo Shops
		{
			get;
			set;
		}

		public string TypeId
		{
			get;
			set;
		}

		public MessageLog()
		{
		}
	}
}