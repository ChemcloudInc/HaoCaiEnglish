using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXCardCodeLogInfo : BaseModel
	{
		private long _id;

		public string CardId
		{
			get;
			set;
		}

		public long? CardLogId
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public int CodeStatus
		{
			get;
			set;
		}

		public long? CouponCodeId
		{
			get;
			set;
		}

		public WXCardLogInfo.CouponTypeEnum? CouponType
		{
			get;
			set;
		}

		public virtual WXCardLogInfo Himall_WXCardLog
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

		public string OpenId
		{
			get;
			set;
		}

		public DateTime? SendTime
		{
			get;
			set;
		}

		public DateTime? UsedTime
		{
			get;
			set;
		}

		public WXCardCodeLogInfo()
		{
		}

		public enum CodeStatusEnum
		{
			HasFailed = -1,
			WaitReceive = 0,
			Normal = 1,
			HasConsume = 2,
			HasDelete = 3
		}
	}
}