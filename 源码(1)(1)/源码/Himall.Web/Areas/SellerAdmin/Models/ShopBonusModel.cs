using Himall.Core;
using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopBonusModel
	{
		public DateTime BonusDateEnd
		{
			get;
			set;
		}

		public string BonusDateEndStr
		{
			get;
			set;
		}

		public DateTime BonusDateStart
		{
			get;
			set;
		}

		public string BonusDateStartStr
		{
			get;
			set;
		}

		public string CardColor
		{
			get;
			set;
		}

		public string CardSubtitle
		{
			get;
			set;
		}

		public string CardTitle
		{
			get;
			set;
		}

		public int Count
		{
			get;
			set;
		}

		public DateTime DateEnd
		{
			get;
			set;
		}

		public string DateEndStr
		{
			get;
			set;
		}

		public DateTime DateStart
		{
			get;
			set;
		}

		public string DateStartStr
		{
			get;
			set;
		}

		public decimal GrantPrice
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public bool IsInvalid
		{
			get;
			set;
		}

		public bool IsShowSyncWeiXin
		{
			get;
			set;
		}

		public string Name
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

		public long ReceiveId
		{
			get;
			set;
		}

		public string ShareDetail
		{
			get;
			set;
		}

		public string ShareImg
		{
			get;
			set;
		}

		public string ShareTitle
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public bool SynchronizeCard
		{
			get;
			set;
		}

		public ShopBonusInfo.UseStateType UseState
		{
			get;
			set;
		}

		public string UseStateStr
		{
			get;
			set;
		}

		public decimal UsrStatePrice
		{
			get;
			set;
		}

		public WXJSCardModel WXJSCardInfo
		{
			get;
			set;
		}

		public WXSyncJSInfoByCard WXJSInfo
		{
			get;
			set;
		}

		public ShopBonusModel()
		{
		}

		public ShopBonusModel(ShopBonusInfo m)
		{
            Id = m.Id;
            Count = m.Count;
            RandomAmountStart = m.RandomAmountStart;
            RandomAmountEnd = m.RandomAmountEnd;
            UseState = m.UseState;
            UseStateStr = m.UseState.ToDescription();
            UsrStatePrice = m.UsrStatePrice;
            GrantPrice = m.GrantPrice;
            DateStart = m.DateStart;
            DateEnd = m.DateEnd;
            DateStartStr = m.DateStart.ToString("yyyy-MM-dd");
            DateEndStr = m.DateEnd.ToString("yyyy-MM-dd");
            BonusDateStart = m.BonusDateStart;
            BonusDateEnd = m.BonusDateEnd;
            BonusDateStartStr = m.BonusDateStart.ToString("yyyy-MM-dd");
            BonusDateEndStr = m.BonusDateEnd.ToString("yyyy-MM-dd");
            ShareTitle = m.ShareTitle;
            ShareDetail = m.ShareDetail;
            ShareImg = m.ShareImg;
            SynchronizeCard = m.SynchronizeCard;
            CardTitle = m.CardTitle;
            CardColor = m.CardColor;
            CardSubtitle = m.CardSubtitle;
            IsInvalid = m.IsInvalid;
            ReceiveCount = m.ReceiveCount.Value;
            Name = m.Name;
            QRPath = m.QRPath;
            ShopId = m.ShopId;
		}

		public static implicit operator ShopBonusInfo(ShopBonusModel m)
		{
			ShopBonusInfo shopBonusInfo = new ShopBonusInfo()
			{
				Id = m.Id,
				Count = m.Count,
				RandomAmountStart = m.RandomAmountStart,
				RandomAmountEnd = m.RandomAmountEnd,
				UseState = m.UseState,
				UsrStatePrice = m.UsrStatePrice,
				GrantPrice = m.GrantPrice,
				DateStart = m.DateStart,
				DateEnd = m.DateEnd,
				BonusDateStart = m.BonusDateStart,
				BonusDateEnd = m.BonusDateEnd,
				ShareTitle = m.ShareTitle,
				ShareDetail = m.ShareDetail,
				ShareImg = m.ShareImg,
				SynchronizeCard = m.SynchronizeCard,
				CardTitle = m.CardTitle,
				CardColor = m.CardColor,
				CardSubtitle = m.CardSubtitle,
				IsInvalid = m.IsInvalid,
				ReceiveCount = new int?(m.ReceiveCount),
				Name = m.Name,
				QRPath = m.QRPath
			};
			return shopBonusInfo;
		}
	}
}