using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class FreightTemplateInfoExtend
	{
		public virtual IEnumerable<FreightAreaContentInfoExtend> FreightAreaContent
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public FreightTemplateInfo.FreightTemplateType IsFree
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string SendTime
		{
			get;
			set;
		}

		public int? ShippingMethod
		{
			get;
			set;
		}

		public long ShopID
		{
			get;
			set;
		}

		public int? SourceAddress
		{
			get;
			set;
		}

		public string SourceAddressStr
		{
			get;
			set;
		}

		public FreightTemplateInfo.ValuationMethodType ValuationMethod
		{
			get;
			set;
		}

		public FreightTemplateInfoExtend()
		{
            FreightAreaContent = new HashSet<FreightAreaContentInfoExtend>();
		}
	}
}