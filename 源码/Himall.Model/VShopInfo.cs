using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class VShopInfo : BaseModel
	{
		private long _id;

		private string backgroundImage
		{
			get;
			set;
		}

		public string BackgroundImage
		{
			get
			{
				return string.Concat(ImageServerUrl, backgroundImage);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    backgroundImage = value;
					return;
				}
                backgroundImage = value.Replace(ImageServerUrl, "");
			}
		}

		public int buyNum
		{
			get;
			set;
		}

		public DateTime CreateTime
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
		{
			get;
			set;
		}

		public string HomePageTitle
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

		private string logo
		{
			get;
			set;
		}

		public string Logo
		{
			get
			{
				return string.Concat(ImageServerUrl, logo);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    logo = value;
					return;
				}
                logo = value.Replace(ImageServerUrl, "");
			}
		}

		public string Name
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public VShopInfo.VshopStates State
		{
			get;
			set;
		}

		public string Tags
		{
			get;
			set;
		}

		public int VisitNum
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.VShopExtendInfo> VShopExtendInfo
		{
			get;
			set;
		}

		public string WXLogo
		{
			get;
			set;
		}

		public VShopInfo()
		{
            VShopExtendInfo = new HashSet<Himall.Model.VShopExtendInfo>();
		}

		public enum VshopStates
		{
			[Description("未审核")]
			NotAudit = 1,
			[Description("审核通过")]
			Normal = 2,
			[Description("审核拒绝")]
			Refused = 3,
			[Description("开启微店第一步")]
			Step1 = 4,
			[Description("开启微店第二步")]
			Step2 = 5,
			[Description("开启微店第三步")]
			Step3 = 6,
			[Description("已关闭")]
			Close = 99
		}
	}
}