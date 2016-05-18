using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FreightTemplateInfo : BaseModel
	{
		private long _id;

		[NotMapped]
		public FreightTemplateInfo.SendTimeEnum? GetSendTime
		{
			get
			{
				FreightTemplateInfo.SendTimeEnum? nullable = null;
				if (!string.IsNullOrWhiteSpace(SendTime))
				{
					int num = 0;
					if (int.TryParse(SendTime, out num) && Enum.IsDefined(typeof(FreightTemplateInfo.SendTimeEnum), num))
					{
						nullable = new FreightTemplateInfo.SendTimeEnum?((FreightTemplateInfo.SendTimeEnum)num);
					}
				}
				return nullable;
			}
		}

		public virtual ICollection<FreightAreaContentInfo> Himall_FreightAreaContent
		{
			get;
			set;
		}

		public virtual ICollection<ProductInfo> Himall_Products
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

		public FreightTemplateInfo.FreightTemplateType IsFree
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string SendTime
		{
			get;
			set;
		}

		public int? ShippingMethod
		{
			get;
			set;
		}

		public long ShopID
		{
			get;
			set;
		}

		public int? SourceAddress
		{
			get;
			set;
		}

		public FreightTemplateInfo.ValuationMethodType ValuationMethod
		{
			get;
			set;
		}

		public FreightTemplateInfo()
		{
            Himall_FreightAreaContent = new HashSet<FreightAreaContentInfo>();
            Himall_Products = new HashSet<ProductInfo>();
		}

		public enum FreightTemplateType
		{
			[Description("自定义模板")]
			SelfDefine,
			[Description("卖家承担运费")]
			Free
		}

		public enum SendTimeEnum
		{
			[Description("4小时")]
			FourHours = 4,
			[Description("8小时")]
			EightHours = 8,
			[Description("12小时")]
			TwelveHours = 12,
			[Description("1天内")]
			OneDay = 24,
			[Description("2天内")]
			TwoDay = 48,
			[Description("3天内")]
			ThreeDay = 72,
			[Description("5天内")]
			FiveDay = 120,
			[Description("8天内")]
			EightDay = 192,
			[Description("10天内")]
			TenDay = 240,
			[Description("15天内")]
			FifteenDay = 360,
			[Description("17天内")]
			SeventeenDay = 408,
			[Description("20天内")]
			TwentyDay = 480,
			[Description("25天内")]
			TwentyFiveDay = 600,
			[Description("30天内")]
			ThirtyDay = 720
		}

		public enum ValuationMethodType
		{
			[Description("按件数")]
			Piece,
			[Description("按重量")]
			Weight,
			[Description("按体积")]
			Bulk
		}
	}
}