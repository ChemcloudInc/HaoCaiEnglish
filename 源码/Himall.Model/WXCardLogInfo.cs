using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXCardLogInfo : BaseModel
	{
		private long _id;

		public string AppId
		{
			get;
			set;
		}

		public string AppSecret
		{
			get;
			set;
		}

		public int? AuditStatus
		{
			get;
			set;
		}

		public DateTime BeginTime
		{
			get;
			set;
		}

		public string CardColor
		{
			get;
			set;
		}

		public string CardId
		{
			get;
			set;
		}

		public string CardSubTitle
		{
			get;
			set;
		}

		public string CardTitle
		{
			get;
			set;
		}

		public long? CouponId
		{
			get;
			set;
		}

		public WXCardLogInfo.CouponTypeEnum? CouponType
		{
			get;
			set;
		}

		public string DefaultDetail
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public int GetLimit
		{
			get;
			set;
		}

		public virtual ICollection<WXCardCodeLogInfo> Himall_WXCardCodeLog
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

		public int LeastCost
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public int ReduceCost
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

		public static Hashtable WXCardColors
		{
			get
			{
				Hashtable hashtables = new Hashtable()
				{
					{ "Color010", "#63b359" },
					{ "Color020", "#2c9f67" },
					{ "Color030", "#509fc9" },
					{ "Color040", "#5885cf" },
					{ "Color050", "#9062c0" },
					{ "Color060", "#d09a45" },
					{ "Color070", "#e4b138" },
					{ "Color080", "#ee903c" },
					{ "Color081", "#f08500" },
					{ "Color082", "#a9d92d" },
					{ "Color090", "#dd6549" },
					{ "Color100", "#cc463d" },
					{ "Color101", "#cf3e36" },
					{ "Color102", "#5E6671" }
				};
				return hashtables;
			}
		}

		public WXCardLogInfo()
		{
            Himall_WXCardCodeLog = new HashSet<WXCardCodeLogInfo>();
		}

		public enum AuditStatusEnum
		{
			AuditNot = -1,
			Auditin = 0,
			Audited = 1
		}

		public enum CouponTypeEnum
		{
			[Description("优惠券")]
			Coupon = 1,
			[Description("商家红包")]
			Bonus = 2
		}
	}
}