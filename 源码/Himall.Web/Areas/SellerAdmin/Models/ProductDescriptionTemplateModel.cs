using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ProductDescriptionTemplateModel
	{
		public string HtmlContent
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int Position
		{
			get;
			set;
		}

		public string PositionText
		{
			get;
			set;
		}

		public ProductDescriptionTemplateModel()
		{
		}
	}
}