using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class ShopQuery : QueryBase
	{
		public long BrandId
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public int OrderBy
		{
			get;
			set;
		}

		public string ShopAccount
		{
			get;
			set;
		}

		public long? ShopGradeId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public ShopInfo.ShopAuditStatus? Status
		{
			get;
			set;
		}

		public ShopQuery()
		{
		}
	}
}