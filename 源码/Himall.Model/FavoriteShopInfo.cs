using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FavoriteShopInfo : BaseModel
	{
		private long _id;

		public DateTime Date
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

		public virtual UserMemberInfo MemberInfo
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Tags
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public FavoriteShopInfo()
		{
		}
	}
}