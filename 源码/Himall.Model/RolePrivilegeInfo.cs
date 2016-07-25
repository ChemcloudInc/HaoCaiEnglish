using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class RolePrivilegeInfo : BaseModel
	{
		private long _id;

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

		public int Privilege
		{
			get;
			set;
		}

		public long RoleId
		{
			get;
			set;
		}

		public virtual Himall.Model.RoleInfo RoleInfo
		{
			get;
			set;
		}

		public RolePrivilegeInfo()
		{
		}
	}
}