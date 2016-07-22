using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class StatisticOrderCommentsInfo : BaseModel
	{
		private long _id;

		public StatisticOrderCommentsInfo.EnumCommentKey CommentKey
		{
			get;
			set;
		}

		public decimal CommentValue
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

		public long ShopId
		{
			get;
			set;
		}

		public StatisticOrderCommentsInfo()
		{
		}

		public enum EnumCommentKey
		{
			[Description("宝贝与描述相符 商家得分")]
			ProductAndDescription = 1,
			[Description("宝贝与描述相符 同行业平均分")]
			ProductAndDescriptionPeer = 2,
			[Description("宝贝与描述相符 同行业商家最高得分")]
			ProductAndDescriptionMax = 3,
			[Description("宝贝与描述相符 同行业商家最低得分")]
			ProductAndDescriptionMin = 4,
			[Description("卖家发货速度 商家得分")]
			SellerDeliverySpeed = 5,
			[Description("卖家发货速度 同行业平均分")]
			SellerDeliverySpeedPeer = 6,
			[Description("卖家发货速度 同行业商家最高得分")]
			SellerDeliverySpeedMax = 7,
			[Description("卖家发货速度 同行业商家最低得分")]
			SellerDeliverySpeedMin = 8,
			[Description("卖家服务态度 商家得分")]
			SellerServiceAttitude = 9,
			[Description("卖家服务态度 同行业平均分")]
			SellerServiceAttitudePeer = 10,
			[Description("卖家服务态度 同行业商家最高得分")]
			SellerServiceAttitudeMax = 11,
			[Description("卖家服务态度 同行业商家最低得分")]
			SellerServiceAttitudeMin = 12
		}
	}
}