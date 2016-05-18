using Himall.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class MenuInfo : BaseModel
	{
		private long _id;

		public short Depth
		{
			get;
			set;
		}

		public string FullIdPath
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

		public long ParentId
		{
			get;
			set;
		}

		public PlatformType Platform
		{
			get;
			set;
		}

		public short Sequence
		{
			get;
			set;
		}

		public virtual ShopInfo Shop
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public MenuInfo.UrlTypes? UrlType
		{
			get;
			set;
		}

		public MenuInfo()
		{
		}

		public enum UrlTypes
		{
			[Description("")]
			Nothing,
			[Description("首页")]
			ShopHome,
			[Description("微店")]
			WeiStore,
			[Description("分类")]
			ShopCategory,
			[Description("个人中心")]
			MemberCenter,
			[Description("购物车")]
			ShopCart,
			[Description("")]
			Linkage
		}
	}
}