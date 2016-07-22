using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class HomeCategoryInfo : BaseModel
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

		public int Depth
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

		public int RowNumber
		{
			get;
			set;
		}

		public HomeCategoryInfo()
		{
		}
	}
}