using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopBonusInfo : BaseModel
	{
		private long _id;

		public DateTime BonusDateEnd
		{
			get;
			set;
		}

		public DateTime BonusDateStart
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

		public DateTime DateStart
		{
			get;
			set;
		}

		public decimal GrantPrice
		{
			get;
			set;
		}

		public virtual ICollection<ShopBonusGrantInfo> Himall_ShopBonusGrant
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

		public bool IsInvalid
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

		public int? ReceiveCount
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

		public decimal UsrStatePrice
		{
			get;
			set;
		}

		public int WXCardState
		{
			get;
			set;
		}

		public ShopBonusInfo()
		{
            Himall_ShopBonusGrant = new HashSet<ShopBonusGrantInfo>();
		}

		public enum UseStateType
		{
			[Description("没有限制")]
			None = 1,
			[Description("满X元使用")]
			FilledSend = 2
		}
	}
}