using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class ApplyWithDrawQuery : QueryBase
	{
		public long? memberId
		{
			get;
			set;
		}

		public long? withDrawNo
		{
			get;
			set;
		}

		public ApplyWithDrawInfo.ApplyWithDrawStatus? withDrawStatus
		{
			get;
			set;
		}

		public ApplyWithDrawQuery()
		{
		}
	}
}