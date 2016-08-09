using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class CartItemModel
	{
		public string color
		{
			get;
			set;
		}

		public int count
		{
			get;
			set;
		}

		public long id
		{
			get;
			set;
		}

		public string imgUrl
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public decimal price
		{
			get;
			set;
		}

		public string productCode
		{
			get;
			set;
		}

		public long shopId
		{
			get;
			set;
		}

		public string shopName
		{
			get;
			set;
		}

		public string size
		{
			get;
			set;
		}

		public string skuId
		{
			get;
			set;
		}

		public string version
		{
			get;
			set;
		}

		public CartItemModel()
		{
		}
	}
}