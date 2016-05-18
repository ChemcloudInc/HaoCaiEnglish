using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class MemberQuery : QueryBase
	{
		public int CityId
		{
			get;
			set;
		}

		public int CountyId
		{
			get;
			set;
		}

		public string keyWords
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int ProvinceId
		{
			get;
			set;
		}

		public List<int> RegionIds
		{
			get;
			set;
		}

		public bool? Status
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public MemberQuery()
		{
            RegionIds = new List<int>();
		}
	}
}