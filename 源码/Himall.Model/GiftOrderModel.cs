using Himall.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class GiftOrderModel
	{
		public UserMemberInfo CurrentUser
		{
			get;
			set;
		}

		public IEnumerable<GiftOrderItemModel> Gifts
		{
			get;
			set;
		}

		public Himall.Core.PlatformType PlatformType
		{
			get;
			set;
		}

		public ShippingAddressInfo ReceiveAddress
		{
			get;
			set;
		}

		public string UserRemark
		{
			get;
			set;
		}

		public GiftOrderModel()
		{
            PlatformType = Himall.Core.PlatformType.PC;
		}
	}
}