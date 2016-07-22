using Himall.Core;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class TopicQuery : QueryBase
	{
		public bool? IsRecommend
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public Himall.Core.PlatformType PlatformType
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

		public TopicQuery()
		{
            PlatformType = Himall.Core.PlatformType.PC;
		}
	}
}