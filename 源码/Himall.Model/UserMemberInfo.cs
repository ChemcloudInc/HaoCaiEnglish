using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class UserMemberInfo : BaseModel
	{
		private int _availableIntegrals;

		private int _historyIntegral;

		private long _id;

		public string Address
		{
			get;
			set;
		}

		public int AvailableIntegrals
		{
			get
			{
				return _availableIntegrals;
			}
		}

		public string CellPhone
		{
			get;
			set;
		}

		public DateTime CreateDate
		{
			get;
			set;
		}

		public bool Disabled
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public decimal Expenditure
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.FavoriteInfo> FavoriteInfo
		{
			get;
			set;
		}

		public virtual ICollection<BonusReceiveInfo> Himall_BonusReceive
		{
			get;
			set;
		}

		public virtual ICollection<BrowsingHistoryInfo> Himall_BrowsingHistory
		{
			get;
			set;
		}

		public virtual ICollection<FavoriteShopInfo> Himall_FavoriteShops
		{
			get;
			set;
		}

		public virtual ICollection<MemberIntegral> Himall_MemberIntegral
		{
			get;
			set;
		}

		public virtual ICollection<MemberIntegralRecord> Himall_MemberIntegralRecord
		{
			get;
			set;
		}

		public virtual ICollection<ProductCommentInfo> Himall_ProductComments
		{
			get;
			set;
		}

		public virtual ICollection<ShopBonusGrantInfo> Himall_ShopBonusGrant
		{
			get;
			set;
		}

		public virtual ICollection<ShopBonusReceiveInfo> Himall_ShopBonusReceive
		{
			get;
			set;
		}

		public int HistoryIntegral
		{
			get
			{
				return _historyIntegral;
			}
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

		public virtual ICollection<InviteRecordInfo> InviteMemberRecord
		{
			get;
			set;
		}

		public long? InviteUserId
		{
			get;
			set;
		}

		public DateTime LastLoginDate
		{
			get;
			set;
		}

		public long MemberGradeId
		{
			get;
			set;
		}

		public string MemberGradeName
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.MemberOpenIdInfo> MemberOpenIdInfo
		{
			get;
			set;
		}

		public string Nick
		{
			get;
			set;
		}

		public int OrderNumber
		{
			get;
			set;
		}

		public long ParentSellerId
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string PasswordSalt
		{
			get;
			set;
		}

		public string PayPwd
		{
			get;
			set;
		}

		public string PayPwdSalt
		{
			get;
			set;
		}

		internal string photo
		{
			get;
			set;
		}

		[NotMapped]
		public string Photo
		{
			get
			{
				return string.Concat(ImageServerUrl, photo);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    photo = value;
					return;
				}
                photo = value.Replace(ImageServerUrl, "");
			}
		}

		public int Points
		{
			get;
			set;
		}

		public string QQ
		{
			get;
			set;
		}

		public string RealName
		{
			get;
			set;
		}

		public int RegionId
		{
			get;
			set;
		}

		public virtual ICollection<InviteRecordInfo> RegMemberRecord
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.ShippingAddressInfo> ShippingAddressInfo
		{
			get;
			set;
		}

		internal virtual ICollection<ShoppingCartItemInfo> ShoppingCartInfo
		{
			get;
			set;
		}

		public int TopRegionId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public UserMemberInfo()
		{
            FavoriteInfo = new HashSet<Himall.Model.FavoriteInfo>();
            MemberOpenIdInfo = new HashSet<Himall.Model.MemberOpenIdInfo>();
            ShippingAddressInfo = new HashSet<Himall.Model.ShippingAddressInfo>();
            ShoppingCartInfo = new HashSet<ShoppingCartItemInfo>();
            Himall_FavoriteShops = new HashSet<FavoriteShopInfo>();
            Himall_BrowsingHistory = new HashSet<BrowsingHistoryInfo>();
            Himall_ProductComments = new HashSet<ProductCommentInfo>();
            Himall_MemberIntegralRecord = new HashSet<MemberIntegralRecord>();
            Himall_MemberIntegral = new HashSet<MemberIntegral>();
            InviteMemberRecord = new HashSet<InviteRecordInfo>();
            RegMemberRecord = new HashSet<InviteRecordInfo>();
            Himall_BonusReceive = new HashSet<BonusReceiveInfo>();
            Himall_ShopBonusReceive = new HashSet<ShopBonusReceiveInfo>();
            Himall_ShopBonusGrant = new HashSet<ShopBonusGrantInfo>();
		}

		public void InitUserIntegralInfo()
		{
            _availableIntegrals = 0;
            _historyIntegral = 0;
			if (this != null)
			{
				MemberIntegral memberIntegral = Himall_MemberIntegral.FirstOrDefault();
				if (memberIntegral != null)
				{
                    _availableIntegrals = memberIntegral.AvailableIntegrals;
                    _historyIntegral = memberIntegral.HistoryIntegrals;
				}
			}
		}
	}
}