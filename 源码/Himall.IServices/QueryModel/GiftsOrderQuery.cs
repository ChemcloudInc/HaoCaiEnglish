using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.IServices.QueryModel
{
	public class GiftsOrderQuery : QueryBase
	{
		public long? OrderId
		{
			get;
			set;
		}

		public string skey
		{
			get;
			set;
		}

		public GiftOrderInfo.GiftOrderStatus? status
		{
			get;
			set;
		}

		public long? UserId
		{
			get;
			set;
		}

		public GiftsOrderQuery()
		{
		}
	}
}