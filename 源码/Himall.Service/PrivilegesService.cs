using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class PrivilegesService : ServiceBase, IPrivilegesService, IService, IDisposable
	{
		public PrivilegesService()
		{
		}

		public void AddPlatformRole(RoleInfo model)
		{
			model.ShopId = 0;
			if (string.IsNullOrEmpty(model.Description))
			{
				model.Description = model.RoleName;
			}
            context.RoleInfo.Add(model);
            context.SaveChanges();
		}

		public void AddSellerRole(RoleInfo model)
		{
			if (string.IsNullOrEmpty(model.Description))
			{
				model.Description = model.RoleName;
			}
            context.RoleInfo.Add(model);
            context.SaveChanges();
		}

		public void DeletePlatformRole(long id)
		{
			RoleInfo roleInfo = (
				from a in context.RoleInfo
				where a.Id == id && a.ShopId == 0
                select a).FirstOrDefault();
            context.RoleInfo.Remove(roleInfo);
            context.SaveChanges();
		}

		public void DeleteSellerRole(long id, long shopId)
		{
			RoleInfo roleInfo = (
				from a in context.RoleInfo
				where a.Id == id && a.ShopId == shopId
				select a).FirstOrDefault();
            context.RoleInfo.Remove(roleInfo);
            context.SaveChanges();
		}

		public RoleInfo GetPlatformRole(long id)
		{
			return (
				from a in context.RoleInfo
				where a.Id == id && a.ShopId == 0
                select a).FirstOrDefault();
		}

		public IQueryable<RoleInfo> GetPlatformRoles()
		{
			return context.RoleInfo.FindBy((RoleInfo item) => item.ShopId == 0);
		}

		public RoleInfo GetSellerRole(long id, long shopid)
		{
			return (
				from a in context.RoleInfo
				where a.Id == id && a.ShopId == shopid
				select a).FirstOrDefault();
		}

		public IQueryable<RoleInfo> GetSellerRoles(long shopId)
		{
			return 
				from item in context.RoleInfo
				where item.ShopId == shopId && item.ShopId != 0
                select item;
		}

		public void UpdatePlatformRole(RoleInfo model)
		{
			RoleInfo roleName = context.RoleInfo.FindBy((RoleInfo a) => a.ShopId == 0 && a.Id == model.Id).FirstOrDefault();
			if (roleName == null)
			{
				throw new HimallException("找不到该权限组");
			}
			roleName.RoleName = model.RoleName;
			roleName.Description = model.Description;
			if (string.IsNullOrEmpty(model.Description))
			{
				roleName.Description = model.RoleName;
			}
            context.RolePrivilegeInfo.RemoveRange(roleName.RolePrivilegeInfo);
			roleName.RolePrivilegeInfo = model.RolePrivilegeInfo;
            context.SaveChanges();
		}

		public void UpdateSellerRole(RoleInfo model)
		{
			RoleInfo roleName = context.RoleInfo.FindBy((RoleInfo a) => a.ShopId == model.ShopId && a.Id == model.Id).FirstOrDefault();
			if (roleName == null)
			{
				throw new HimallException("找不到该权限组");
			}
			roleName.RoleName = model.RoleName;
			roleName.Description = model.Description;
			if (string.IsNullOrEmpty(model.Description))
			{
				roleName.Description = model.RoleName;
			}
            context.RolePrivilegeInfo.RemoveRange(roleName.RolePrivilegeInfo);
			roleName.RolePrivilegeInfo = model.RolePrivilegeInfo;
            context.SaveChanges();
		}
	}
}