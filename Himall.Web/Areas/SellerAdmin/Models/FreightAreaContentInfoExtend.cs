using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class FreightAreaContentInfoExtend
	{
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

		public string AreaContentCN
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

		public long Id
		{
			get;
			set;
		}

		public byte? IsDefault
		{
			get;
			set;
		}

		public FreightAreaContentInfoExtend()
		{
		}
	}
}