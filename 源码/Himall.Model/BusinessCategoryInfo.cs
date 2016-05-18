using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class BusinessCategoryInfo : BaseModel
	{
		private long _id;

		public long CategoryId
		{
			get;
			set;
		}

		public virtual Himall.Model.CategoryInfo CategoryInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string CategoryName
		{
			get;
			set;
		}

		public decimal CommisRate
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

		public long ShopId
		{
			get;
			set;
		}

		public BusinessCategoryInfo()
		{
		}
	}
}