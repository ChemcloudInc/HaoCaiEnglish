using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IPrivilegesService : IService, IDisposable
	{
		void AddPlatformRole(RoleInfo model);

		void AddSellerRole(RoleInfo model);

		void DeletePlatformRole(long id);

		void DeleteSellerRole(long id, long shopId);

		RoleInfo GetPlatformRole(long id);

		IQueryable<RoleInfo> GetPlatformRoles();

		RoleInfo GetSellerRole(long id, long shopId);

		IQueryable<RoleInfo> GetSellerRoles(long shopId);

		void UpdatePlatformRole(RoleInfo model);

		void UpdateSellerRole(RoleInfo model);
	}
}