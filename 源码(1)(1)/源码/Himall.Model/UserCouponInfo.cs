using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class UserCouponInfo
	{
		public long CouponId
		{
			get;
			set;
		}

		public string CouponName
		{
			get;
			set;
		}

		public DateTime CreateTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public int Num
		{
			get;
			set;
		}

		public decimal OrderAmount
		{
			get;
			set;
		}

		public long? OrderId
		{
			get;
			set;
		}

		public int PerMax
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopLogo
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public CouponRecordInfo.CounponStatuses UseStatus
		{
			get;
			set;
		}

		public DateTime? UseTime
		{
			get;
			set;
		}

		public VShopInfo VShop
		{
			get;
			set;
		}

		public long? VShopId
		{
			get;
			set;
		}

		public UserCouponInfo()
		{
		}
	}
}