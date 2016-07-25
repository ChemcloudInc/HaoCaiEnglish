using Himall.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class BannerInfo : BaseModel
	{
		private long _id;

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

		public string Name
		{
			get;
			set;
		}

		public PlatformType Platform
		{
			get;
			set;
		}

		public int Position
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public BannerInfo.BannerUrltypes UrlType
		{
			get;
			set;
		}

		public BannerInfo()
		{
		}

		public enum BannerUrltypes
		{
			[Description("链接")]
			Link,
			[Description("全部商品")]
			AllProducts,
			[Description("商品分类")]
			Category,
			[Description("店铺简介")]
			VShopIntroduce
		}
	}
}