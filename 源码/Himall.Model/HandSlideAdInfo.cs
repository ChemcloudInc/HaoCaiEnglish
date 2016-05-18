using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class HandSlideAdInfo : BaseModel
	{
		private long _id;

		public long DisplaySequence
		{
			get;
			set;
		}

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

		private string imageUrl
		{
			get;
			set;
		}

		public string ImageUrl
		{
			get
			{
				return string.Concat(ImageServerUrl, imageUrl);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    imageUrl = value;
					return;
				}
                imageUrl = value.Replace(ImageServerUrl, "");
			}
		}

		public string Url
		{
			get;
			set;
		}

		public HandSlideAdInfo()
		{
		}
	}
}