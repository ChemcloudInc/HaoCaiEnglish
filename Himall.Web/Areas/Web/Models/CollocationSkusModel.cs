using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class CollocationSkusModel
	{
		public long ColloProductId
		{
			get;
			set;
		}

		public CollectionSKU Color
		{
			get;
			set;
		}

		public string ImagePath
		{
			get;
			set;
		}

		public string MeasureUnit
		{
			get;
			set;
		}

		public decimal MinPrice
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public CollectionSKU Size
		{
			get;
			set;
		}

		public long Stock
		{
			get;
			set;
		}

		public CollectionSKU Version
		{
			get;
			set;
		}

		public CollocationSkusModel()
		{
            Color = new CollectionSKU();
            Size = new CollectionSKU();
            Version = new CollectionSKU();
		}
	}
}