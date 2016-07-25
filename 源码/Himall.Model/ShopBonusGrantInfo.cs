using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopBonusGrantInfo : BaseModel
	{
		private long _id;

		public string BonusQR
		{
			get;
			set;
		}

		public virtual UserMemberInfo Himall_Members
		{
			get;
			set;
		}

		public virtual ShopBonusInfo Himall_ShopBonus
		{
			get;
			set;
		}

		public virtual ICollection<ShopBonusReceiveInfo> Himall_ShopBonusReceive
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

		public long OrderId
		{
			get;
			set;
		}

		public long ShopBonusId
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public ShopBonusGrantInfo()
		{
            Himall_ShopBonusReceive = new HashSet<ShopBonusReceiveInfo>();
		}
	}
}