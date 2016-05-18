using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class MarketBoughtQuery : QueryBase
	{
		public Himall.Model.MarketType? MarketType
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public MarketBoughtQuery()
		{
		}
	}
}