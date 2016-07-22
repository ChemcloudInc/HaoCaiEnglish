using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class IntegralMallPageModel
	{
		public List<CouponInfo> CouponList
		{
			get;
			set;
		}

		public int CouponMaxPage
		{
			get;
			set;
		}

		public int CouponPageSize
		{
			get;
			set;
		}

		public int CouponTotal
		{
			get;
			set;
		}

		public List<GiftModel> GiftList
		{
			get;
			set;
		}

		public int GiftMaxPage
		{
			get;
			set;
		}

		public int GiftPageSize
		{
			get;
			set;
		}

		public int GiftTotal
		{
			get;
			set;
		}

		public IntegralMallPageModel()
		{
		}
	}
}