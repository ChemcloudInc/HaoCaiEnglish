using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Product
{
	public class TypeAttribute
	{
		public long Id
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

		public long TypeId
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public TypeAttribute()
		{
		}
	}
}