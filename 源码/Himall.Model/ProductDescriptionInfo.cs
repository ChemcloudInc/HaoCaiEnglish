using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductDescriptionInfo : BaseModel
	{
		private long _id;

		public string AuditReason
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public long DescriptiondSuffixId
		{
			get;
			set;
		}

		public long DescriptionPrefixId
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

		public string Meta_Description
		{
			get;
			set;
		}

		public string Meta_Keywords
		{
			get;
			set;
		}

		public string Meta_Title
		{
			get;
			set;
		}

		public string MobileDescription
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductInfo ProductInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string ShowMobileDescription
		{
			get
			{
				string mobileDescription = "";
				if (this != null)
				{
					mobileDescription = MobileDescription;
					if (string.IsNullOrWhiteSpace(mobileDescription))
					{
						mobileDescription = Description;
					}
				}
				return mobileDescription;
			}
		}

		public ProductDescriptionInfo()
		{
		}
	}
}