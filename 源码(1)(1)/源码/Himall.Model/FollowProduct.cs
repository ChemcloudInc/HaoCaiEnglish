using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FollowProduct
	{
		private string _imagePath;

		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
                _imagePath = value;
			}
		}

		public decimal Price
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

		public FollowProduct()
		{
		}
	}
}