using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopBrandApplysInfo : BaseModel
	{
		private int _id;

		public int ApplyMode
		{
			get;
			set;
		}

		public DateTime ApplyTime
		{
			get;
			set;
		}

		public int AuditStatus
		{
			get;
			set;
		}

		public string AuthCertificate
		{
			get;
			set;
		}

		public long? BrandId
		{
			get;
			set;
		}

		public string BrandName
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public virtual BrandInfo Himall_Brands
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
		{
			get;
			set;
		}

		public new int Id
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

		public string Logo
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public ShopBrandApplysInfo()
		{
		}

		public enum BrandApplyMode
		{
			[Description("平台已有品牌")]
			Exist = 1,
			[Description("新品牌")]
			New = 2
		}

		public enum BrandAuditStatus
		{
			[Description("未审核")]
			UnAudit,
			[Description("通过审核")]
			Audited,
			[Description("拒绝通过")]
			Refused
		}
	}
}