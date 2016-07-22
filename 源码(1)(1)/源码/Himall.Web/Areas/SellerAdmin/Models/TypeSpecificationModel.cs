using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class TypeSpecificationModel
	{
		public string Name
		{
			get;
			set;
		}

		public long SpecId
		{
			get;
			set;
		}

		public List<Specification> Values
		{
			get;
			set;
		}

		public TypeSpecificationModel()
		{
		}
	}
}