using System;
using System.Collections.Generic;

namespace Himall.Model
{
	public interface ISellerManager : IManager
	{
		List<SellerPrivilege> SellerPrivileges
		{
			get;
			set;
		}

		long VShopId
		{
			get;
			set;
		}
	}
}