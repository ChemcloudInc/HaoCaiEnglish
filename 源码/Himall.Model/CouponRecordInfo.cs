using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CouponRecordInfo : BaseModel
	{
		private long _id;

		public string CounponSN
		{
			get;
			set;
		}

		public CouponRecordInfo.CounponStatuses CounponStatus
		{
			get;
			set;
		}

		public DateTime CounponTime
		{
			get;
			set;
		}

		public long CouponId
		{
			get;
			set;
		}

		public virtual CouponInfo Himall_Coupon
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
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

		public long? OrderId
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public DateTime? UsedTime
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

		[NotMapped]
		public WXCardCodeLogInfo WXCardCodeInfo
		{
			get;
			set;
		}

		public long? WXCodeId
		{
			get;
			set;
		}

		public CouponRecordInfo()
		{
		}

		public enum CounponStatuses
		{
			[Description("未使用")]
			Unuse,
			[Description("已使用")]
			Used
		}
	}
}