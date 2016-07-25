using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Market
{
	public class BonusModel
	{
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

		public decimal FixedAmount
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
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

		public BonusInfo.BonusPriceType PriceType
		{
			get;
			set;
		}

		public string QRPath
		{
			get;
			set;
		}

		public decimal RandomAmountEnd
		{
			get;
			set;
		}

		public decimal RandomAmountStart
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

		public string StateStr
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

		public BonusModel()
		{
		}

		public BonusModel(BonusInfo m)
		{
            Id = m.Id;
            Type = m.Type;
            Style = m.Style;
            PriceType = m.PriceType.Value;
            Name = m.Name;
            MerchantsName = m.MerchantsName;
            Remark = m.Remark;
            Blessing = m.Blessing;
            TotalPrice = m.TotalPrice;
            StartTime = m.StartTime;
            EndTime = m.EndTime;
            FixedAmount = m.FixedAmount.Value;
            RandomAmountStart = m.RandomAmountStart.Value;
            RandomAmountEnd = m.RandomAmountEnd.Value;
            ImagePath = m.ImagePath;
            Description = m.Description;
            ReceiveCount = m.ReceiveCount;
            ReceivePrice = m.ReceivePrice;
            ReceiveHref = m.ReceiveHref;
            QRPath = m.QRPath;
            IsInvalid = m.IsInvalid;
		}

		public static implicit operator BonusInfo(BonusModel m)
		{
			BonusInfo bonusInfo = new BonusInfo()
			{
				Id = m.Id,
				Type = m.Type,
				Style = m.Style,
				PriceType = new BonusInfo.BonusPriceType?(m.PriceType),
				Name = m.Name,
				MerchantsName = m.MerchantsName,
				Remark = m.Remark,
				Blessing = m.Blessing,
				TotalPrice = m.TotalPrice,
				StartTime = m.StartTime,
				EndTime = m.EndTime,
				FixedAmount = new decimal?(m.FixedAmount),
				RandomAmountStart = new decimal?(m.RandomAmountStart),
				RandomAmountEnd = new decimal?(m.RandomAmountEnd),
				ImagePath = m.ImagePath,
				Description = m.Description
			};
			return bonusInfo;
		}
	}
}