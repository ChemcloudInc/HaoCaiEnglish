using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ConcernShopInfo
	{
		public int ConcernCount
		{
			get;
			set;
		}

		public DateTime ConcernTime
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Logo
		{
			get;
			set;
		}

		public decimal SericeMark
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public ConcernShopInfo()
		{
		}
	}
}