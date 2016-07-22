using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class SensitiveWordQuery : QueryBase
	{
		public string CategoryName
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public string SensitiveWord
		{
			get;
			set;
		}

		public SensitiveWordQuery()
		{
		}
	}
}