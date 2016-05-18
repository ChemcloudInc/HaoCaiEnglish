using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class InvoiceContextInfo : BaseModel
	{
		private long _id;

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

		public string Name
		{
			get;
			set;
		}

		public InvoiceContextInfo()
		{
		}
	}
}