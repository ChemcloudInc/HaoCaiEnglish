using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SlideAdInfo : BaseModel
	{
		private long _id;

		public string Description
		{
			get;
			set;
		}

		public long DisplaySequence
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

		private string imageUrl
		{
			get;
			set;
		}

		public string ImageUrl
		{
			get
			{
				return string.Concat(ImageServerUrl, imageUrl);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    imageUrl = value;
					return;
				}
                imageUrl = value.Replace(ImageServerUrl, "");
			}
		}

		public long ShopId
		{
			get;
			set;
		}

		public SlideAdInfo.SlideAdType TypeId
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public SlideAdInfo()
		{
		}

		public enum SlideAdType
		{
			[Description("平台首页轮播图")]
			PlatformHome = 1,
			[Description("平台限时购轮播图")]
			PlatformLimitTime = 2,
			[Description("店铺首页轮播图")]
			ShopHome = 3,
			[Description("微店轮播图")]
			VShopHome = 4,
			[Description("微信首页轮播图")]
			WeixinHome = 5,
			[Description("触屏版首页轮播图")]
			WapHome = 6,
			[Description("触屏版微店首页轮播图")]
			WapShopHome = 7,
			[Description("APP首页轮播图")]
			IOSShopHome = 8
		}
	}
}