using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShopGradeInfo : BaseModel
	{
		private long _id;

		public decimal ChargeStandard
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

		public int ImageLimit
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int ProductLimit
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.ShopInfo> ShopInfo
		{
			get;
			set;
		}

		public int TemplateLimit
		{
			get;
			set;
		}

		public ShopGradeInfo()
		{
            ShopInfo = new HashSet<Himall.Model.ShopInfo>();
		}
	}
}