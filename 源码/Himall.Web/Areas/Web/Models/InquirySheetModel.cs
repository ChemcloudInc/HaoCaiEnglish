using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class InquirySheetModel
	{
		public string EndDate
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string PublishDate
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public InquirySheetModel()
		{
		}
	}
}