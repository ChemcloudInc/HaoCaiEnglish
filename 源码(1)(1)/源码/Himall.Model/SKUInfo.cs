using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SKUInfo : BaseModel
	{
		private string _id;

		public long AutoId
		{
			get;
			set;
		}

		public string Color
		{
			get;
			set;
		}

		public decimal CostPrice
		{
			get;
			set;
		}

		public new string Id
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

		public long ProductId
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductInfo ProductInfo
		{
			get;
			set;
		}

		public decimal SalePrice
		{
			get;
			set;
		}

		public string Size
		{
			get;
			set;
		}

		public string Sku
		{
			get;
			set;
		}

		public long Stock
		{
			get;
			set;
		}

		public string Version
		{
			get;
			set;
		}

		public SKUInfo()
		{
		}
	}
}