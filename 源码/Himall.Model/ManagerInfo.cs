using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ManagerInfo : BaseModel, ISellerManager, IPaltManager, IManager
	{
		private long _id;

		[NotMapped]
		public List<AdminPrivilege> AdminPrivileges
		{
			get;
			set;
		}

		public DateTime CreateDate
		{
			get;
			set;
		}

		[NotMapped]
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

		public string Password
		{
			get;
			set;
		}

		public string PasswordSalt
		{
			get;
			set;
		}

		public string RealName
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public long RoleId
		{
			get;
			set;
		}

		[NotMapped]
		public string RoleName
		{
			get;
			set;
		}

		[NotMapped]
		public List<SellerPrivilege> SellerPrivileges
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		[NotMapped]
		public long VShopId
		{
			get;
			set;
		}

		public ManagerInfo()
		{
		}
	}
}