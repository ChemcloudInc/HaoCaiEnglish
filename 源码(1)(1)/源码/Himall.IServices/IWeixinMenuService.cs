using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IWeixinMenuService : IService, IDisposable
	{
		void AddMenu(MenuInfo model);

		void ConsistentToWeixin(long shopId);

		void DeleteMenu(long id);

		IEnumerable<MenuInfo> GetMainMenu(long shopId);

		MenuInfo GetMenu(long id);

		IEnumerable<MenuInfo> GetMenuByParentId(long id);

		void UpdateMenu(MenuInfo model);
	}
}