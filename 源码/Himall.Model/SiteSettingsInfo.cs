using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SiteSettingsInfo : BaseModel
	{
		private long _id;

		[NotMapped]
		public decimal AdvancePaymentLimit
		{
			get;
			set;
		}

		[NotMapped]
		public decimal AdvancePaymentPercent
		{
			get;
			set;
		}

		[NotMapped]
		public string CustomerTel
		{
			get;
			set;
		}

		[NotMapped]
		public string FlowScript
		{
			get;
			set;
		}

		[NotMapped]
		public string Hotkeywords
		{
			get;
			set;
		}

		[NotMapped]
		public string ICPNubmer
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

		internal string Key
		{
			get;
			set;
		}

		[NotMapped]
		public string Keyword
		{
			get;
			set;
		}

		[NotMapped]
		public string Logo
		{
			get;
			set;
		}

		[NotMapped]
		public string MemberLogo
		{
			get;
			set;
		}

		[NotMapped]
		public bool MobileVerifOpen
		{
			get;
			set;
		}

		public int NoReceivingTimeout
		{
			get;
			set;
		}

		[NotMapped]
		public string PageFoot
		{
			get;
			set;
		}

		public int ProdutAuditOnOff
		{
			get;
			set;
		}

		[NotMapped]
		public string QRCode
		{
			get;
			set;
		}

		public int SalesReturnTimeout
		{
			get;
			set;
		}

		public string SellerAdminAgreement
		{
			get;
			set;
		}

		[NotMapped]
		public string Site_SEODescription
		{
			get;
			set;
		}

		[NotMapped]
		public string Site_SEOKeywords
		{
			get;
			set;
		}

		[NotMapped]
		public string Site_SEOTitle
		{
			get;
			set;
		}

		[NotMapped]
		public bool SiteIsClose
		{
			get;
			set;
		}

		[NotMapped]
		public string SiteName
		{
			get;
			set;
		}

		public int UnpaidTimeout
		{
			get;
			set;
		}

		[NotMapped]
		public string UserCookieKey
		{
			get;
			set;
		}

		internal string Value
		{
			get;
			set;
		}

		[NotMapped]
		public int WeekSettlement
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinAppId
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinAppSecret
		{
			get;
			set;
		}

		[NotMapped]
		public bool WeixinIsValidationService
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinLoginUrl
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinPartnerID
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinPartnerKey
		{
			get;
			set;
		}

		[NotMapped]
		public string WeixinToken
		{
			get;
			set;
		}

		[NotMapped]
		public string WX_MSGGetCouponTemplateId
		{
			get;
			set;
		}

		[NotMapped]
		public string WXLogo
		{
			get;
			set;
		}
        [NotMapped]
        public string ProductDetailsPictureSize  //≤˙∆∑œÍœ∏“≥Õº∆¨≥ﬂ¥Á
        {
            get;
            set;
        }
        [NotMapped]
        public string CategoryProductPictureSize  //∑÷¿‡“≥Õº∆¨≥ﬂ¥Á
        {
            get;
            set;
        }
        [NotMapped]
        public string MobilePictureSize  //Mobile∂ÀÕº∆¨≥ﬂ¥Á
        {
            get;
            set;
        }
        [NotMapped]
        public string OrderPictureSize  //∂©µ•¡–±ÌÕº∆¨≥ﬂ¥Á
        {
            get;
            set;
        }
        [NotMapped]
        public string CartPictureSize  //π∫ŒÔ≥µÕº∆¨≥ﬂ¥Á
        {
            get;
            set;
        }
		public SiteSettingsInfo()
		{
		}
	}
}