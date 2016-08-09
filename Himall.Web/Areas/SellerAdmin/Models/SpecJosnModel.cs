using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class SpecJosnModel
	{
		public List<TypeSpecificationModel> json
		{
			get;
			set;
		}

		public tableDataModel tableData
		{
			get;
			set;
		}

		public SpecJosnModel()
		{
		}
	}
}