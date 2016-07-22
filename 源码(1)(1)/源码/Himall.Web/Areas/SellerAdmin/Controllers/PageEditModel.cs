using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class PageEditModel
	{
		public string comment
		{
			get;
			set;
		}

		public string description
		{
			get;
			set;
		}

		public string icon_url
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public long page_id
		{
			get;
			set;
		}

		public string page_url
		{
			get;
			set;
		}

		public string title
		{
			get;
			set;
		}

		public PageEditModel()
		{
		}
	}
}