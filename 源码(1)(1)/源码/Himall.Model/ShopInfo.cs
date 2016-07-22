using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopInfo : BaseModel
	{
		private long _id;

		public string BankAccountName
		{
			get;
			set;
		}

		public string BankAccountNumber
		{
			get;
			set;
		}

		public string BankCode
		{
			get;
			set;
		}

		public string BankName
		{
			get;
			set;
		}

		public string BankPhoto
		{
			get;
			set;
		}

		public int BankRegionId
		{
			get;
			set;
		}

		[NotMapped]
		public Dictionary<long, decimal> BusinessCategory
		{
			get;
			set;
		}

		public DateTime? BusinessLicenceEnd
		{
			get;
			set;
		}

		public string BusinessLicenceNumber
		{
			get;
			set;
		}

		public string BusinessLicenceNumberPhoto
		{
			get;
			set;
		}

		public int BusinessLicenceRegionId
		{
			get;
			set;
		}

		public DateTime? BusinessLicenceStart
		{
			get;
			set;
		}

		public string BusinessLicenseCert
		{
			get;
			set;
		}

		public string BusinessSphere
		{
			get;
			set;
		}

		public string CompanyAddress
		{
			get;
			set;
		}

		public Himall.Model.CompanyEmployeeCount CompanyEmployeeCount
		{
			get;
			set;
		}

		public DateTime? CompanyFoundingDate
		{
			get;
			set;
		}

		public string CompanyName
		{
			get;
			set;
		}

		public string CompanyPhone
		{
			get;
			set;
		}

		[NotMapped]
		public string CompanyRegionAddress
		{
			get;
			set;
		}

		public int CompanyRegionId
		{
			get;
			set;
		}

		public decimal CompanyRegisteredCapital
		{
			get;
			set;
		}

		public string ContactsEmail
		{
			get;
			set;
		}

		public string ContactsName
		{
			get;
			set;
		}

		public string ContactsPhone
		{
			get;
			set;
		}

		public DateTime CreateDate
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public decimal FreeFreight
		{
			get;
			set;
		}

		public decimal Freight
		{
			get;
			set;
		}

		public string GeneralTaxpayerPhot
		{
			get;
			set;
		}

		public long GradeId
		{
			get;
			set;
		}

		public virtual ICollection<CashDepositInfo> Himall_CashDeposit
		{
			get;
			set;
		}

		public virtual ICollection<CouponInfo> Himall_Coupon
		{
			get;
			set;
		}

		public virtual ICollection<CouponRecordInfo> Himall_CouponRecord
		{
			get;
			set;
		}

		public virtual ICollection<FavoriteShopInfo> Himall_FavoriteShops
		{
			get;
			set;
		}

		public virtual ICollection<MenuInfo> Himall_Menus
		{
			get;
			set;
		}

		public virtual ICollection<MessageLog> Himall_MessageLog
		{
			get;
			set;
		}

		public virtual ICollection<ProductCommentInfo> Himall_ProductComments
		{
			get;
			set;
		}

		public virtual ICollection<ProductInfo> Himall_Products
		{
			get;
			set;
		}

		public virtual ICollection<ShopBonusInfo> Himall_ShopBonus
		{
			get;
			set;
		}

		public virtual ICollection<ShopBrandApplysInfo> Himall_ShopBrandApplys
		{
			get;
			set;
		}

		public virtual ICollection<ShopBrandsInfo> Himall_ShopBrands
		{
			get;
			set;
		}

		public virtual ICollection<StatisticOrderCommentsInfo> Himall_StatisticOrderComments
		{
			get;
			set;
		}

		public virtual ICollection<VShopInfo> Himall_VShop
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

		public bool IsSelf
		{
			get;
			set;
		}

		public string legalPerson
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public string OrganizationCode
		{
			get;
			set;
		}

		public string OrganizationCodePhoto
		{
			get;
			set;
		}

		public string OtherCert
		{
			get;
			set;
		}

		public string PayPhoto
		{
			get;
			set;
		}

		public string PayRemark
		{
			get;
			set;
		}

		[NotMapped]
		public string ProductAndDescription
		{
			get;
			set;
		}

		public string ProductCert
		{
			get;
			set;
		}

		public string RefuseReason
		{
			get;
			set;
		}

		[NotMapped]
		public int Sales
		{
			get;
			set;
		}

		[NotMapped]
		public string SellerDeliverySpeed
		{
			get;
			set;
		}

		[NotMapped]
		public string SellerServiceAttitude
		{
			get;
			set;
		}

		public string SenderAddress
		{
			get;
			set;
		}

		public string SenderName
		{
			get;
			set;
		}

		public string SenderPhone
		{
			get;
			set;
		}

		public int? SenderRegionId
		{
			get;
			set;
		}

		[NotMapped]
		public string ShopAccount
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public ShopInfo.ShopAuditStatus ShopStatus
		{
			get;
			set;
		}

		[NotMapped]
		public ShopInfo.ShopAuditStatus ShowShopAuditStatus
		{
			get
			{
				ShopInfo.ShopAuditStatus shopStatus = ShopInfo.ShopAuditStatus.Unusable;
				if (this != null)
				{
					shopStatus = ShopStatus;
					if (EndDate.HasValue && ShopStatus == ShopInfo.ShopAuditStatus.Open && (EndDate.Value.Date.AddDays(1).AddSeconds(-1) - DateTime.Now).TotalSeconds < 0)
					{
						shopStatus = ShopInfo.ShopAuditStatus.HasExpired;
					}
				}
				return shopStatus;
			}
		}

		public ShopInfo.ShopStage? Stage
		{
			get;
			set;
		}

		public string SubDomains
		{
			get;
			set;
		}

		public string TaxpayerId
		{
			get;
			set;
		}

		public string TaxRegistrationCertificate
		{
			get;
			set;
		}

		public string TaxRegistrationCertificatePhoto
		{
			get;
			set;
		}

		public string Theme
		{
			get;
			set;
		}

		public ShopInfo()
		{
            Himall_FavoriteShops = new HashSet<FavoriteShopInfo>();
            Himall_Menus = new HashSet<MenuInfo>();
            Himall_MessageLog = new HashSet<MessageLog>();
            Himall_CashDeposit = new HashSet<CashDepositInfo>();
            Himall_ShopBrandApplys = new HashSet<ShopBrandApplysInfo>();
            Himall_ShopBrands = new HashSet<ShopBrandsInfo>();
            Himall_StatisticOrderComments = new HashSet<StatisticOrderCommentsInfo>();
            Himall_ProductComments = new HashSet<ProductCommentInfo>();
            Himall_Coupon = new HashSet<CouponInfo>();
            Himall_CouponRecord = new HashSet<CouponRecordInfo>();
            Himall_VShop = new HashSet<VShopInfo>();
            Himall_Products = new HashSet<ProductInfo>();
            Himall_ShopBonus = new HashSet<ShopBonusInfo>();
		}

		public enum ShopAuditStatus
		{
			[Description("己过期")]
			HasExpired = -1,
			[Description("不可用")]
			Unusable = 1,
			[Description("待审核")]
			WaitAudit = 2,
			[Description("待付款")]
			WaitPay = 3,
			[Description("被拒绝")]
			Refuse = 4,
			[Description("待确认")]
			WaitConfirm = 5,
			[Description("冻结")]
			Freeze = 6,
			[Description("开启")]
			Open = 7
		}

		public enum ShopStage
		{
			[Description("许可协议")]
			Agreement,
			[Description("公司信息")]
			CompanyInfo,
			[Description("财务信息")]
			FinancialInfo,
			[Description("店铺信息")]
			ShopInfo,
			[Description("上传支付凭证")]
			UploadPayOrder,
			[Description("完成")]
			Finish
		}

		public class ShopVistis
		{
			public decimal OrderCounts
			{
				get;
				set;
			}

			public decimal SaleAmounts
			{
				get;
				set;
			}

			public decimal SaleCounts
			{
				get;
				set;
			}

			public decimal VistiCounts
			{
				get;
				set;
			}

			public ShopVistis()
			{
			}
		}
	}
}