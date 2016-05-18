using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service
{
	public class ManagerService : ServiceBase, IManagerService, IService, IDisposable
	{
		public ManagerService()
		{
		}

		public void AddPlatformManager(ManagerInfo model)
		{
			if (model.RoleId == 0)
			{
				throw new HimallException("权限组选择不正确!");
			}
			if (CheckUserNameExist(model.UserName, true))
			{
				throw new HimallException("该用户名已存在！");
			}
			model.ShopId = 0;
			model.PasswordSalt = Guid.NewGuid().ToString();
			model.CreateDate = DateTime.Now;
			string str = SecureHelper.MD5(model.Password);
			model.Password = SecureHelper.MD5(string.Concat(str, model.PasswordSalt));
            context.ManagerInfo.Add(model);
            context.SaveChanges();
		}

		public void AddSellerManager(ManagerInfo model, string currentSellerName)
		{
			if (model.RoleId == 0)
			{
				throw new HimallException("权限组选择不正确!");
			}
			if (CheckUserNameExist(model.UserName, false))
			{
				throw new HimallException("该用户名已存在！");
			}
			if (model.ShopId == 0)
			{
				throw new HimallException("没有权限进行该操作！");
			}
			model.PasswordSalt = Guid.NewGuid().ToString();
			model.CreateDate = DateTime.Now;
			string str = SecureHelper.MD5(model.Password);
			model.Password = SecureHelper.MD5(string.Concat(str, model.PasswordSalt));
            context.ManagerInfo.Add(model);
            context.SaveChanges();
		}

		public ManagerInfo AddSellerManager(string username, string password, string salt = "")
		{
			ManagerInfo managerInfo;
			ManagerInfo managerInfo1 = context.ManagerInfo.FirstOrDefault((ManagerInfo p) => p.UserName == username);
			if (managerInfo1 != null)
			{
				return new ManagerInfo()
				{
					Id = managerInfo1.Id
				};
			}
			if (string.IsNullOrEmpty(salt))
			{
				Guid guid = Guid.NewGuid();
				salt = SecureHelper.MD5(guid.ToString("N"));
			}
			using (TransactionScope transactionScope = new TransactionScope())
			{
				ShopInfo shopInfo = Instance<IShopService>.Create.CreateEmptyShop();
				ManagerInfo managerInfo2 = new ManagerInfo()
				{
					CreateDate = DateTime.Now,
					UserName = username,
					Password = password,
					PasswordSalt = salt,
					ShopId = shopInfo.Id,
					SellerPrivileges = new List<SellerPrivilege>()
					{
						0
					},
					AdminPrivileges = new List<AdminPrivilege>(),
					RoleId = 0
                };
				managerInfo = managerInfo2;
                context.ManagerInfo.Add(managerInfo);
                context.SaveChanges();
				transactionScope.Complete();
			}
			return managerInfo;
		}

		public void BatchDeletePlatformManager(long[] ids)
		{
			IQueryable<ManagerInfo> managerInfos = context.ManagerInfo.FindBy((ManagerInfo item) => item.ShopId == 0 && item.RoleId != 0 && ids.Contains(item.Id));
            context.ManagerInfo.RemoveRange(managerInfos);
            context.SaveChanges();
		}

		public void BatchDeleteSellerManager(long[] ids, long shopId)
		{
			IQueryable<ManagerInfo> managerInfos = context.ManagerInfo.FindBy((ManagerInfo item) => item.ShopId == shopId && item.RoleId != 0 && ids.Contains(item.Id));
            context.ManagerInfo.RemoveRange(managerInfos);
            context.SaveChanges();
		}

		public void ChangePlatformManagerPassword(long id, string password, long roleId)
		{
			ManagerInfo managerInfo = context.ManagerInfo.FindBy((ManagerInfo item) => item.Id == id && item.ShopId == 0).FirstOrDefault();
			if (managerInfo == null)
			{
				throw new HimallException("该管理员不存在，或者已被删除!");
			}
			if (roleId != 0 && managerInfo.RoleId != 0)
			{
				managerInfo.RoleId = roleId;
			}
			if (!string.IsNullOrWhiteSpace(password))
			{
				string str = SecureHelper.MD5(password);
				managerInfo.Password = SecureHelper.MD5(string.Concat(str, managerInfo.PasswordSalt));
			}
            context.SaveChanges();
			Cache.Remove(CacheKeyCollection.Manager(id));
		}

		public void ChangeSellerManager(ManagerInfo info)
		{
			ManagerInfo roleId = context.ManagerInfo.FindBy((ManagerInfo item) => item.Id == info.Id && item.ShopId == info.ShopId).FirstOrDefault();
			if (roleId == null)
			{
				throw new HimallException("该管理员不存在，或者已被删除!");
			}
			if (info.RoleId != 0 && roleId.RoleId != 0)
			{
				roleId.RoleId = info.RoleId;
			}
			if (!string.IsNullOrWhiteSpace(info.Password))
			{
				string str = SecureHelper.MD5(info.Password);
				roleId.Password = SecureHelper.MD5(string.Concat(str, roleId.PasswordSalt));
			}
			roleId.RealName = info.RealName;
			roleId.Remark = info.Remark;
            context.SaveChanges();
			Cache.Remove(CacheKeyCollection.Seller(info.Id));
		}

		public void ChangeSellerManagerPassword(long id, long shopId, string password, long roleId)
		{
			ManagerInfo managerInfo = context.ManagerInfo.FindBy((ManagerInfo item) => item.Id == id && item.ShopId == shopId).FirstOrDefault();
			if (managerInfo == null)
			{
				throw new HimallException("该管理员不存在，或者已被删除!");
			}
			if (roleId != 0 && managerInfo.RoleId != 0)
			{
				managerInfo.RoleId = roleId;
			}
			if (!string.IsNullOrWhiteSpace(password))
			{
				string str = SecureHelper.MD5(password);
				managerInfo.Password = SecureHelper.MD5(string.Concat(str, managerInfo.PasswordSalt));
			}
            context.SaveChanges();
			Cache.Remove(CacheKeyCollection.Seller(id));
		}

		public bool CheckUserNameExist(string username, bool isPlatFormManager = false)
		{
			if (!isPlatFormManager)
			{
				return context.UserMemberInfo.Any((UserMemberInfo item) => item.UserName.ToLower() == username.ToLower());
			}
			return context.ManagerInfo.Any((ManagerInfo item) => (item.UserName.ToLower() == username.ToLower()) && item.ShopId == 0);
		}

		public void DeletePlatformManager(long id)
		{
			ManagerInfo managerInfo = context.ManagerInfo.FindBy((ManagerInfo item) => item.Id == id && item.ShopId == 0 && item.RoleId != 0).FirstOrDefault();
            context.ManagerInfo.Remove(managerInfo);
            context.SaveChanges();
		}

		public void DeleteSellerManager(long id, long shopId)
		{
			ManagerInfo managerInfo = context.ManagerInfo.FindBy((ManagerInfo item) => item.Id == id && item.ShopId == shopId && item.RoleId != 0).FirstOrDefault();
            context.ManagerInfo.Remove(managerInfo);
            context.SaveChanges();
		}

		public IQueryable<ManagerInfo> GetManagers(string keyWords)
		{
			return context.ManagerInfo.FindBy((ManagerInfo item) => keyWords == null || (keyWords == "") || item.UserName.Contains(keyWords));
		}

		private string GetPasswrodWithTwiceEncode(string password, string salt)
		{
			string str = SecureHelper.MD5(password);
			return SecureHelper.MD5(string.Concat(str, salt));
		}

		public ManagerInfo GetPlatformManager(long userId)
		{
			ManagerInfo roleName = null;
			string str = CacheKeyCollection.Manager(userId);
			if (Cache.Get(str) == null)
			{
				roleName = context.ManagerInfo.FirstOrDefault((ManagerInfo item) => item.Id == userId && item.ShopId == 0);
				if (roleName == null)
				{
					return null;
				}
				if (roleName.RoleId != 0)
				{
					RoleInfo roleInfo = context.RoleInfo.FindById<RoleInfo>(roleName.RoleId);
					if (roleInfo != null)
					{
						List<AdminPrivilege> adminPrivileges = new List<AdminPrivilege>();
						roleInfo.RolePrivilegeInfo.ToList().ForEach((RolePrivilegeInfo a) => adminPrivileges.Add((AdminPrivilege)a.Privilege));
						roleName.RoleName = roleInfo.RoleName;
						roleName.AdminPrivileges = adminPrivileges;
						roleName.Description = roleInfo.Description;
					}
				}
				else
				{
					List<AdminPrivilege> adminPrivileges1 = new List<AdminPrivilege>()
					{
						0
					};
					roleName.RoleName = "系统管理员";
					roleName.AdminPrivileges = adminPrivileges1;
					roleName.Description = "系统管理员";
				}
				Cache.Insert(str, roleName);
			}
			else
			{
				roleName = (ManagerInfo)Cache.Get(str);
			}
			return roleName;
		}

		public IQueryable<ManagerInfo> GetPlatformManagerByRoleId(long roleId)
		{
			return context.ManagerInfo.FindBy((ManagerInfo item) => item.ShopId == 0 && item.RoleId == roleId);
		}

		public PageModel<ManagerInfo> GetPlatformManagers(ManagerQuery query)
		{
			int num = 0;
			IQueryable<ManagerInfo> managerInfos = context.ManagerInfo.FindBy<ManagerInfo, long>((ManagerInfo item) => item.ShopId == 0, query.PageNo, query.PageSize, out num, (ManagerInfo item) => item.RoleId, true);
			return new PageModel<ManagerInfo>()
			{
				Models = managerInfos,
				Total = num
			};
		}

		public ManagerInfo GetSellerManager(long userId)
		{
			ManagerInfo roleName = null;
			string str = CacheKeyCollection.Seller(userId);
			if (Cache.Get(str) == null)
			{
				roleName = (
					from item in context.ManagerInfo
					where item.Id == userId && item.ShopId != 0
                    select item).FirstOrDefault();
				if (roleName == null)
				{
					return null;
				}
				if (roleName.RoleId != 0)
				{
					RoleInfo roleInfo = context.RoleInfo.FindById<RoleInfo>(roleName.RoleId);
					if (roleInfo != null)
					{
						List<SellerPrivilege> sellerPrivileges = new List<SellerPrivilege>();
						roleInfo.RolePrivilegeInfo.ToList().ForEach((RolePrivilegeInfo a) => sellerPrivileges.Add((SellerPrivilege)a.Privilege));
						roleName.RoleName = roleInfo.RoleName;
						roleName.SellerPrivileges = sellerPrivileges;
						roleName.Description = roleInfo.Description;
					}
				}
				else
				{
					List<SellerPrivilege> sellerPrivileges1 = new List<SellerPrivilege>()
					{
						0
					};
					roleName.RoleName = "系统管理员";
					roleName.SellerPrivileges = sellerPrivileges1;
					roleName.Description = "系统管理员";
				}
				Cache.Insert(str, roleName);
			}
			else
			{
				roleName = (ManagerInfo)Cache.Get(str);
			}
			if (roleName != null)
			{
				VShopInfo vShopInfo = context.VShopInfo.FirstOrDefault((VShopInfo item) => item.ShopId == roleName.ShopId);
				roleName.VShopId = -1;
				if (vShopInfo != null)
				{
					roleName.VShopId = vShopInfo.Id;
				}
			}
			return roleName;
		}

		public ManagerInfo GetSellerManager(string userName)
		{
			ManagerInfo managerInfo = (
				from item in context.ManagerInfo
				where (item.UserName == userName) && item.ShopId != 0
                select item).FirstOrDefault();
			return managerInfo;
		}

		public IQueryable<ManagerInfo> GetSellerManagerByRoleId(long roleId, long shopId)
		{
			return context.ManagerInfo.FindBy((ManagerInfo item) => item.ShopId == shopId && item.RoleId == roleId);
		}

		public PageModel<ManagerInfo> GetSellerManagers(ManagerQuery query)
		{
			int num = 0;
			IQueryable<ManagerInfo> managerInfos = context.ManagerInfo.FindBy((ManagerInfo item) => item.ShopId == query.ShopID && item.RoleId != 0 && item.Id != query.userID, query.PageNo, query.PageSize, out num);
			return new PageModel<ManagerInfo>()
			{
				Models = managerInfos,
				Total = num
			};
		}

		public ManagerInfo Login(string username, string password, bool isPlatFormManager = false)
		{
			ManagerInfo managerInfo;
			managerInfo = (!isPlatFormManager ? context.ManagerInfo.FindBy((ManagerInfo item) => (item.UserName == username) && item.ShopId != 0).FirstOrDefault() : context.ManagerInfo.FindBy((ManagerInfo item) => (item.UserName == username) && item.ShopId == 0).FirstOrDefault());
			if (managerInfo != null)
			{
				if (GetPasswrodWithTwiceEncode(password, managerInfo.PasswordSalt).ToLower() != managerInfo.Password)
				{
					managerInfo = null;
				}
				else if (managerInfo.ShopId > 0)
				{
					ShopInfo shop = Instance<IShopService>.Create.GetShop(managerInfo.ShopId, false);
					if (shop == null)
					{
						throw new HimallException("未找到帐户对应的店铺");
					}
					if (!shop.IsSelf && shop.ShopStatus == ShopInfo.ShopAuditStatus.Freeze)
					{
						throw new HimallException("帐户所在店铺已被冻结");
					}
				}
			}
			return managerInfo;
		}

		public void UpdateShopStatus()
		{
			List<ShopInfo> list = (
				from s in context.ShopInfo
				where s.EndDate < DateTime.Now
                select s).ToList();
			foreach (ShopInfo nullable in list)
			{
				if (!nullable.IsSelf)
				{
					nullable.ShopStatus = ShopInfo.ShopAuditStatus.Unusable;
				}
				else
				{
					DateTime now = DateTime.Now;
					nullable.EndDate = new DateTime?(now.AddYears(10));
					nullable.ShopStatus = ShopInfo.ShopAuditStatus.Open;
				}
			}
            context.SaveChanges();
		}
	}
}