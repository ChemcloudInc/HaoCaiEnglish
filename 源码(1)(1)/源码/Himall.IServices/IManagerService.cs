using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IManagerService : IService, IDisposable
	{
		void AddPlatformManager(ManagerInfo model);

		ManagerInfo AddSellerManager(string username, string password, string salt);

		void AddSellerManager(ManagerInfo model, string currentSellerName);

		void BatchDeletePlatformManager(long[] ids);

		void BatchDeleteSellerManager(long[] ids, long shopId);

		void ChangePlatformManagerPassword(long id, string password, long roleId);

		void ChangeSellerManager(ManagerInfo info);

		void ChangeSellerManagerPassword(long id, long shopId, string password, long roleId);

		bool CheckUserNameExist(string userName, bool isPlatFormManager = false);

		void DeletePlatformManager(long id);

		void DeleteSellerManager(long id, long shopId);

		IQueryable<ManagerInfo> GetManagers(string keyWords);

		ManagerInfo GetPlatformManager(long userId);

		IQueryable<ManagerInfo> GetPlatformManagerByRoleId(long roleId);

		PageModel<ManagerInfo> GetPlatformManagers(ManagerQuery query);

		ManagerInfo GetSellerManager(long userId);

		ManagerInfo GetSellerManager(string userName);

		IQueryable<ManagerInfo> GetSellerManagerByRoleId(long roleId, long shopId);

		PageModel<ManagerInfo> GetSellerManagers(ManagerQuery query);

		ManagerInfo Login(string username, string password, bool isPlatFormManager = false);

		void UpdateShopStatus();
	}
}