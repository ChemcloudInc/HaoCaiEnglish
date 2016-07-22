using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class RoleInfo : BaseModel
	{
		private long _id;

		public string Description
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public string RoleName
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.RolePrivilegeInfo> RolePrivilegeInfo
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public RoleInfo()
		{
            RolePrivilegeInfo = new HashSet<Himall.Model.RolePrivilegeInfo>();
		}
	}
}