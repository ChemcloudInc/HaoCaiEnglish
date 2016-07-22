using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CouponInfo : BaseModel
	{
		private long _id;

		[NotMapped]
		public bool CanVshopIndex
		{
			get;
			set;
		}

		public long? CardLogId
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

		public DateTime? EndIntegralExchange
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		[NotMapped]
		public bool FormIsSyncWeiXin
		{
			get;
			set;
		}

		[NotMapped]
		public string FormWXColor
		{
			get;
			set;
		}

		[NotMapped]
		public string FormWXCSubTit
		{
			get;
			set;
		}

		[NotMapped]
		public string FormWXCTit
		{
			get;
			set;
		}

		public virtual ICollection<CouponRecordInfo> Himall_CouponRecord
		{
			get;
			set;
		}

		public virtual ICollection<CouponSettingInfo> Himall_CouponSetting
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

		public string IntegralCover
		{
			get;
			set;
		}

		public int IsSyncWeiXin
		{
			get;
			set;
		}

		public int NeedIntegral
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

		public CouponInfo.CouponReceiveType ReceiveType
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

		public string ShowIntegralCover
		{
			get
			{
				string integralCover = "";
				if (this != null)
				{
					integralCover = IntegralCover;
					if (string.IsNullOrWhiteSpace(integralCover) && Himall_Shops != null)
					{
						integralCover = Himall_Shops.Logo;
					}
				}
				return integralCover;
			}
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public int WXAuditStatus
		{
			get;
			set;
		}

		[NotMapped]
		public WXCardLogInfo WXCardInfo
		{
			get;
			set;
		}

		public CouponInfo()
		{
            Himall_CouponRecord = new HashSet<CouponRecordInfo>();
            Himall_CouponSetting = new HashSet<CouponSettingInfo>();
		}

		public enum CouponReceiveStatus
		{
			Success = 1,
			HasExpired = 2,
			HasLimitOver = 3,
			HasReceiveOver = 4,
			IntegralLess = 5
		}

		public enum CouponReceiveType
		{
			[Description("店铺首页")]
			ShopIndex,
			[Description("积分兑换")]
			IntegralExchange,
			[Description("主动发放")]
			DirectHair
		}
	}
}