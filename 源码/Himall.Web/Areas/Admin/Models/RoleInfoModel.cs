using Himall.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class RoleInfoModel
	{
		public long ID
		{
			get;
			set;
		}

		[Required(ErrorMessage="角色名称必填")]
		[StringLength(15, ErrorMessage="角色名称在15个字符以内")]
		public string RoleName
		{
			get;
			set;
		}

		public IEnumerable<Himall.Model.RolePrivilegeInfo> RolePrivilegeInfo
		{
			get;
			set;
		}

		public RoleInfoModel()
		{
		}
	}
}