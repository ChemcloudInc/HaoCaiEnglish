using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class HotProductInfo
	{
		public long Id
		{
			get;
			set;
		}

		public string ImgPath
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public decimal Price
		{
			get;
			set;
		}

		public int SaleCount
		{
			get;
			set;
		}

		public HotProductInfo()
		{
		}
	}
}