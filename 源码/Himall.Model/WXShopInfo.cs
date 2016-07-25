using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WXShopInfo : BaseModel
	{
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

		public string FollowUrl
		{
			get;
			set;
		}

		public long ID
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Token
		{
			get;
			set;
		}

		public WXShopInfo()
		{
		}
	}
}