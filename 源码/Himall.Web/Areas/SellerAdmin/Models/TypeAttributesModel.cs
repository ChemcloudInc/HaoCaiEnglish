using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class TypeAttributesModel
	{
		public long AttrId
		{
			get;
			set;
		}

		public List<TypeAttrValue> AttrValues
		{
			get;
			set;
		}

		public bool IsMulti
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Selected
		{
			get;
			set;
		}

		public TypeAttributesModel()
		{
		}
	}
}