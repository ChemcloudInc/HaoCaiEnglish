using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class FloorTopicInfo : BaseModel
	{
		private long _id;

		public long FloorId
		{
			get;
			set;
		}

		public virtual Himall.Model.HomeFloorInfo HomeFloorInfo
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

		private string topicImage
		{
			get;
			set;
		}

		public string TopicImage
		{
			get
			{
				return string.Concat(ImageServerUrl, topicImage);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    topicImage = value;
					return;
				}
                topicImage = value.Replace(ImageServerUrl, "");
			}
		}

		public string TopicName
		{
			get;
			set;
		}

		public Position TopicType
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public FloorTopicInfo()
		{
		}
	}
}