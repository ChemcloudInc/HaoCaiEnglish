using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Hishop.Weixin.MP.Api;
using Hishop.Weixin.MP.Domain.Menu;
using Newtonsoft.Json;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class WeixinMenuService : ServiceBase, IWeixinMenuService, IService, IDisposable
	{
		public WeixinMenuService()
		{
		}

		public void AddMenu(MenuInfo model)
		{
			if (model == null)
			{
				throw new ApplicationException("微信自定义菜单的Model不能为空");
			}
			if (model.ParentId < 0)
			{
				throw new HimallException("微信自定义菜单的上级菜单不能为负值");
			}
			if (model.Title.Length == 0 || model.Title.Length > 5 && model.ParentId == 0)
			{
				throw new HimallException("一级菜单的名称不能为空且在5个字符以内");
			}
			if (model.Title.Length == 0 || model.Title.Length > 7 && model.ParentId != 0)
			{
				throw new HimallException("二级菜单的名称不能为空且在5个字符以内");
			}
			if ((
				from item in context.MenuInfo
				where item.ParentId == 0 && item.ShopId == model.ShopId
				select item).Count() >= 3 && model.ParentId == 0 || GetMenuByParentId(model.ParentId).Count() >= 5 && model.ParentId != 0)
			{
				throw new HimallException("微信自定义菜单最多允许三个一级菜单，一级菜单下最多运行5个二级菜单");
			}
			model.Platform = PlatformType.WeiXin;
            context.MenuInfo.Add(model);
            context.SaveChanges();
		}

		private SingleButton BuildMenu(MenuInfo menu)
		{
			SingleViewButton singleViewButton = new SingleViewButton()
			{
				name = menu.Title,
				url = menu.Url
			};
			return singleViewButton;
		}

		public void ConsistentToWeixin(long shopId)
		{
			string empty = string.Empty;
			string weixinAppSecret = string.Empty;
			if (shopId == 0)
			{
				SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
				if (string.IsNullOrEmpty(siteSettings.WeixinAppId) || string.IsNullOrEmpty(siteSettings.WeixinAppSecret))
				{
					throw new HimallException("您的服务号配置存在问题，请您先检查配置！");
				}
				empty = siteSettings.WeixinAppId;
				weixinAppSecret = siteSettings.WeixinAppSecret;
			}
			if (shopId > 0)
			{
				WXShopInfo vShopSetting = Instance<IVShopService>.Create.GetVShopSetting(shopId);
				if (string.IsNullOrEmpty(vShopSetting.AppId) || string.IsNullOrEmpty(vShopSetting.AppSecret))
				{
					throw new HimallException("您的服务号配置存在问题，请您先检查配置！");
				}
				empty = vShopSetting.AppId;
				weixinAppSecret = vShopSetting.AppSecret;
			}
			string str = AccessTokenContainer.TryGetToken(empty, weixinAppSecret, false);
			IQueryable<MenuInfo> allMenu = GetAllMenu(shopId);
			if (allMenu == null)
			{
				throw new HimallException("你还没有添加菜单！");
			}
			List<MenuInfo> list = (
				from item in allMenu
				where item.ParentId == 0
                select item).ToList();
			foreach (MenuInfo menuInfo in list)
			{
				if (GetMenuByParentId(menuInfo.Id).Count() != 0)
				{
					continue;
				}
				MenuInfo.UrlTypes? urlType = menuInfo.UrlType;
				if ((urlType.GetValueOrDefault() != MenuInfo.UrlTypes.Nothing ? true : !urlType.HasValue))
				{
					continue;
				}
				throw new HimallException("你有一级菜单下没有二级菜单并且也没有绑定链接");
			}
			Menu menu = new Menu();
			foreach (MenuInfo menuInfo1 in list)
			{
				if (GetMenuByParentId(menuInfo1.Id).Count() != 0)
				{
					SubMenu subMenu = new SubMenu()
					{
						name = menuInfo1.Title
					};
					foreach (MenuInfo menuByParentId in GetMenuByParentId(menuInfo1.Id))
					{
						subMenu.sub_button.Add(BuildMenu(menuByParentId));
					}
					menu.menu.button.Add(subMenu);
				}
				else
				{
					menu.menu.button.Add(BuildMenu(menuInfo1));
				}
			}
			string str1 = JsonConvert.SerializeObject(menu.menu);
			string str2 = MenuApi.CreateMenus(str, str1);
			Log.Info(string.Concat("微信菜单：", str1));
			if (!str2.Contains("ok"))
			{
				Log.Info(string.Concat("微信菜单同步错误,返回内容：", str2));
				throw new HimallException("服务号配置信息错误或没有微信自定义菜单权限，请检查配置信息以及菜单的长度。");
			}
		}

		public void DeleteMenu(long id)
		{
            context.MenuInfo.OrderBy((MenuInfo a) => a.Id == id || a.ParentId == id);
            context.SaveChanges();
		}

		public IQueryable<MenuInfo> GetAllMenu(long shopId)
		{
			return 
				from a in context.MenuInfo
				where a.ShopId == shopId && (int)a.Platform == 1
				select a;
		}

		public IEnumerable<MenuInfo> GetMainMenu(long shopId)
		{
			return 
				from a in context.MenuInfo
				where a.ParentId == 0 && (int)a.Platform == 1 && a.ShopId == shopId
				select a;
		}

		public MenuInfo GetMenu(long id)
		{
			return (
				from a in context.MenuInfo
				where a.Id == id && (int)a.Platform == 1
				select a).FirstOrDefault();
		}

		public IEnumerable<MenuInfo> GetMenuByParentId(long id)
		{
			return 
				from a in context.MenuInfo
				where a.ParentId == id && (int)a.Platform == 1
				select a;
		}

		public void UpdateMenu(MenuInfo model)
		{
			if (model.Id < 0)
			{
				throw new HimallException("微信自定义菜单的ID有误");
			}
			if (model.ParentId < 0)
			{
				throw new HimallException("微信自定义菜单二级菜单必须指定一个一级菜单");
			}
			if (model.Title.Length == 0 || model.Title.Length > 5 && model.ParentId == 0)
			{
				throw new HimallException("一级菜单的名称不能为空且在5个字符以内");
			}
			if (model.Title.Length == 0 || model.Title.Length > 7 && model.ParentId != 0)
			{
				throw new HimallException("二级菜单的名称不能为空且在5个字符以内");
			}
			MenuInfo parentId = context.MenuInfo.FindById<MenuInfo>(model.Id);
			if (model.ParentId == 0 && GetMenuByParentId(model.Id).Count() > 0)
			{
				MenuInfo.UrlTypes? urlType = model.UrlType;
				if ((urlType.GetValueOrDefault() != MenuInfo.UrlTypes.Nothing ? true : !urlType.HasValue))
				{
					throw new HimallException("一级菜单下有二级菜单，不允许绑定链接");
				}
			}
			parentId.ParentId = model.ParentId;
			parentId.Title = model.Title;
			parentId.Url = model.Url;
			parentId.UrlType = model.UrlType;
			parentId.Platform = PlatformType.WeiXin;
            context.SaveChanges();
		}
	}
}