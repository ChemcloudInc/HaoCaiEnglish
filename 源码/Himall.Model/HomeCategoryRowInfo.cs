using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class HomeCategoryRowInfo : BaseModel
	{
		private long _id;

		public new long Id
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

		private string image1
		{
			get;
			set;
		}

		public string Image1
		{
			get
			{
				return string.Concat(ImageServerUrl, image1);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    image1 = value;
					return;
				}
                image1 = value.Replace(ImageServerUrl, "");
			}
		}

		private string image2
		{
			get;
			set;
		}

		public string Image2
		{
			get
			{
				return string.Concat(ImageServerUrl, image2);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    image2 = value;
					return;
				}
                image2 = value.Replace(ImageServerUrl, "");
			}
		}

		public int RowId
		{
			get;
			set;
		}

		public string Url1
		{
			get;
			set;
		}

		public string Url2
		{
			get;
			set;
		}

		public HomeCategoryRowInfo()
		{
		}
	}
}