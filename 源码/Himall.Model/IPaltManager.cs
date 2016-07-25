using System;
using System.Collections.Generic;

namespace Himall.Model
{
	public interface IPaltManager : IManager
	{
		List<AdminPrivilege> AdminPrivileges
		{
			get;
			set;
		}
	}
}