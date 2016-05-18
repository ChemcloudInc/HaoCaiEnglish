using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class IntegralRecordQuery : QueryBase
	{
		public DateTime? EndDate
		{
			get;
			set;
		}

		public MemberIntegral.IntegralType? IntegralType
		{
			get;
			set;
		}

		public DateTime? StartDate
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public IntegralRecordQuery()
		{
		}
	}
}