using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class BonusInfo : BaseModel
	{
		private long _id;

		public string Blessing
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public string EndTimeStr
		{
			get;
			set;
		}

		public decimal? FixedAmount
		{
			get;
			set;
		}

		public virtual ICollection<BonusReceiveInfo> Himall_BonusReceive
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

		public string ImagePath
		{
			get;
			set;
		}

		public bool IsInvalid
		{
			get;
			set;
		}

		public string MerchantsName
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public BonusInfo.BonusPriceType? PriceType
		{
			get;
			set;
		}

		public string QRPath
		{
			get;
			set;
		}

		public decimal? RandomAmountEnd
		{
			get;
			set;
		}

		public decimal? RandomAmountStart
		{
			get;
			set;
		}

		public int ReceiveCount
		{
			get;
			set;
		}

		public string ReceiveHref
		{
			get;
			set;
		}

		public decimal ReceivePrice
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public string StartTimeStr
		{
			get;
			set;
		}

		public BonusInfo.BonusStyle Style
		{
			get;
			set;
		}

		public decimal TotalPrice
		{
			get;
			set;
		}

		public BonusInfo.BonusType Type
		{
			get;
			set;
		}

		public string TypeStr
		{
			get;
			set;
		}

		public BonusInfo()
		{
            Himall_BonusReceive = new HashSet<BonusReceiveInfo>();
		}

		public enum BonusPriceType
		{
			[Description("固定")]
			Fixed = 1,
			[Description("随机")]
			Random = 2
		}

		public enum BonusStyle
		{
			[Description("模板1")]
			TempletOne = 1,
			[Description("模板2")]
			TempletTwo = 2
		}

		public enum BonusType
		{
			[Description("活动红包")]
			Activity = 1,
			[Description("关注红包")]
			Attention = 2
		}
	}
}