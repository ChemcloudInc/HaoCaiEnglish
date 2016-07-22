using Himall.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class TopicInfo : BaseModel
	{
		private long _id;

		private string backgroundImage
		{
			get;
			set;
		}

		public string BackgroundImage
		{
			get
			{
				return string.Concat(ImageServerUrl, backgroundImage);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    backgroundImage = value;
					return;
				}
                backgroundImage = value.Replace(ImageServerUrl, "");
			}
		}

		public string frontCoverImage
		{
			get;
			set;
		}

		public string FrontCoverImage
		{
			get
			{
				return string.Concat(ImageServerUrl, frontCoverImage);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    frontCoverImage = value;
					return;
				}
                frontCoverImage = value.Replace(ImageServerUrl, "");
			}
		}

		public virtual ICollection<MobileHomeTopicsInfo> Himall_MobileHomeTopics
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

		public bool IsRecommend
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public PlatformType PlatForm
		{
			get;
			set;
		}

		public string SelfDefineText
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string Tags
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.TopicModuleInfo> TopicModuleInfo
		{
			get;
			set;
		}

		private string topImage
		{
			get;
			set;
		}

		public string TopImage
		{
			get
			{
				return string.Concat(ImageServerUrl, topImage);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    topImage = value;
					return;
				}
                topImage = value.Replace(ImageServerUrl, "");
			}
		}

		public TopicInfo()
		{
            TopicModuleInfo = new HashSet<Himall.Model.TopicModuleInfo>();
            Himall_MobileHomeTopics = new HashSet<MobileHomeTopicsInfo>();
		}
	}
}