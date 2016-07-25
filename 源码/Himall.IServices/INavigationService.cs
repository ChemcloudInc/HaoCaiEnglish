using Himall.Core;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface INavigationService : IService, IDisposable
	{
		void AddPlatformNavigation(BannerInfo model);

		void AddSellerNavigation(BannerInfo model);

		void DeletePlatformNavigation(long id);

		void DeleteSellerformNavigation(long shopId, long id);

		IQueryable<BannerInfo> GetPlatNavigations();

		BannerInfo GetSellerNavigation(long id);

		IQueryable<BannerInfo> GetSellerNavigations(long shopId, PlatformType plat = 0);

		void SwapPlatformDisplaySequence(long id, long id2);

		void SwapSellerDisplaySequence(long shopId, long id, long id2);

		void UpdatePlatformNavigation(BannerInfo model);

		void UpdateSellerNavigation(BannerInfo model);
	}
}