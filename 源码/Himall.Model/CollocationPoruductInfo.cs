using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CollocationPoruductInfo : BaseModel
	{
		private long _id;

		public long ColloId
		{
			get;
			set;
		}

		public int? DisplaySequence
		{
			get;
			set;
		}

		public virtual CollocationInfo Himall_Collocation
		{
			get;
			set;
		}

		public virtual ICollection<CollocationSkuInfo> Himall_CollocationSkus
		{
			get;
			set;
		}

		public virtual ProductInfo Himall_Products
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

		public bool IsMain
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public CollocationPoruductInfo()
		{
            Himall_CollocationSkus = new HashSet<CollocationSkuInfo>();
		}
	}
}