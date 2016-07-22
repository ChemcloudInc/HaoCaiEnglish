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
	public class NavigationService : ServiceBase, INavigationService, IService, IDisposable
	{
		public NavigationService()
		{
		}

		private void AddNavigation(BannerInfo model)
		{
			model.Position = 0;
			long num = (
                from item in context.BannerInfo
                where item.ShopId == model.ShopId
                select item).Count();
			if (num > 0)
			{
				num = (
					from item in context.BannerInfo
					where item.ShopId == model.ShopId
					select item).Max<BannerInfo, long>((BannerInfo item) => item.DisplaySequence);
			}
			model.DisplaySequence = num + 1;
            context.BannerInfo.Add(model);
            context.SaveChanges();
		}

		public void AddPlatformNavigation(BannerInfo model)
		{
			model.ShopId = 0;
            AddNavigation(model);
		}

		public void AddSellerNavigation(BannerInfo model)
		{
			if (model.ShopId == 0)
			{
				throw new HimallException("店铺id必须大于0");
			}
			if (model.Platform == PlatformType.WeiXin)
			{
				if ((
					from item in context.BannerInfo
					where item.ShopId == model.ShopId && (int)item.Platform == 1
					select item).Count() >= 5)
				{
					throw new HimallException("导航最多只能添加5个");
				}
			}
            AddNavigation(model);
		}

		private void DeleteNavigation(long shopId, long id)
		{
			BannerInfo bannerInfo = context.BannerInfo.FindBy((BannerInfo item) => item.Id == id && item.ShopId == shopId).FirstOrDefault();
			if (bannerInfo == null)
			{
				throw new HimallException("该导航不存在，或者已被删除!");
			}
            context.BannerInfo.Remove(bannerInfo);
            context.SaveChanges();
		}

		public void DeletePlatformNavigation(long id)
		{
            DeleteNavigation(0, id);
		}

		public void DeleteSellerformNavigation(long shopId, long id)
		{
            DeleteNavigation(shopId, id);
		}

		public IQueryable<BannerInfo> GetPlatNavigations()
		{
			return 
				from item in context.BannerInfo.FindBy((BannerInfo item) => item.ShopId == 0)
				orderby item.DisplaySequence
				select item;
		}

		public BannerInfo GetSellerNavigation(long id)
		{
			return context.BannerInfo.FindById<BannerInfo>(id);
		}

		public IQueryable<BannerInfo> GetSellerNavigations(long shopId, PlatformType plat = 0)
		{
			return 
				from item in context.BannerInfo.FindBy((BannerInfo item) => item.ShopId != 0 && item.ShopId == shopId && (int)item.Platform == (int)plat)
				orderby item.DisplaySequence
				select item;
		}

		private void SwapDisplaySequence(long shopId, long id, long id2)
		{
			BannerInfo displaySequence = context.BannerInfo.FirstOrDefault((BannerInfo item) => item.ShopId == shopId && item.Id == id);
			if (displaySequence == null)
			{
				throw new HimallException(string.Concat("id为", id, "的导航不存在，或者已被删除!"));
			}
			BannerInfo bannerInfo = context.BannerInfo.FirstOrDefault((BannerInfo item) => item.ShopId == shopId && item.Id == id2);
			if (bannerInfo == null)
			{
				throw new HimallException(string.Concat("id为", id2, "的导航不存在，或者已被删除!"));
			}
			if (displaySequence != null && bannerInfo != null)
			{
				long num = displaySequence.DisplaySequence;
				displaySequence.DisplaySequence = bannerInfo.DisplaySequence;
				bannerInfo.DisplaySequence = num;
                context.SaveChanges();
			}
		}

		public void SwapPlatformDisplaySequence(long id, long id2)
		{
            SwapDisplaySequence(0, id, id2);
		}

		public void SwapSellerDisplaySequence(long shopId, long id, long id2)
		{
            SwapDisplaySequence(shopId, id, id2);
		}

		private void UpdateNavigation(BannerInfo model)
		{
			BannerInfo name = context.BannerInfo.FindBy((BannerInfo item) => item.Id == model.Id && item.ShopId == model.ShopId).FirstOrDefault();
			if (name == null)
			{
				throw new HimallException("该导航不存在，或者已被删除!");
			}
			name.Name = model.Name;
			name.Url = model.Url;
			name.UrlType = model.UrlType;
            context.SaveChanges();
		}

		public void UpdatePlatformNavigation(BannerInfo model)
		{
			model.ShopId = 0;
            UpdateNavigation(model);
		}

		public void UpdateSellerNavigation(BannerInfo model)
		{
			if (model.ShopId == 0)
			{
				throw new HimallException("店铺id必须大于0");
			}
            UpdateNavigation(model);
		}
	}
}