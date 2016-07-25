using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FreightAreaContentInfo : BaseModel
	{
		private long _id;

		public int? AccumulationUnit
		{
			get;
			set;
		}

		public float? AccumulationUnitMoney
		{
			get;
			set;
		}

		public string AreaContent
		{
			get;
			set;
		}

		public int? FirstUnit
		{
			get;
			set;
		}

		public float? FirstUnitMonry
		{
			get;
			set;
		}

		public long FreightTemplateId
		{
			get;
			set;
		}

		public virtual FreightTemplateInfo Himall_FreightTemplate
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

        public Nullable<byte> IsDefault
		{
			get;
			set;
		}

		public FreightAreaContentInfo()
		{
		}
	}
}