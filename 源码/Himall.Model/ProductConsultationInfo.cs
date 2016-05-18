using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductConsultationInfo : BaseModel
	{
		private long _id;

		public string ConsultationContent
		{
			get;
			set;
		}

		public DateTime ConsultationDate
		{
			get;
			set;
		}

		public string Email
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

		public string ReplyContent
		{
			get;
			set;
		}

		public DateTime? ReplyDate
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public ProductConsultationInfo()
		{
		}
	}
}