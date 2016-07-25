using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface ISlideAdsService : IService, IDisposable
	{
		void AddHandSlidAd(HandSlideAdInfo model);

		void AddSlidAd(SlideAdInfo model);

		void AdjustHandSlidAdIndex(long id, bool direction);

		void AdjustSlidAdIndex(long shopId, long id, bool direction, SlideAdInfo.SlideAdType type);

		void DeleteHandSlidAd(long id);

		void DeleteSlidAd(long shopId, long id);

		HandSlideAdInfo GetHandSlidAd(long id);

		IQueryable<HandSlideAdInfo> GetHandSlidAds();

		ImageAdInfo GetImageAd(long shopId, long id);

		IEnumerable<ImageAdInfo> GetImageAds(long shopId);

		SlideAdInfo GetSlidAd(long shopId, long id);

		IQueryable<SlideAdInfo> GetSlidAds(long shopId, SlideAdInfo.SlideAdType type);

		void UpdateHandSlidAd(HandSlideAdInfo models);

		void UpdateImageAd(ImageAdInfo model);

		void UpdateSlidAd(SlideAdInfo models);

		void UpdateWeixinSlideSequence(long shopId, long sourceSequence, long destiSequence, SlideAdInfo.SlideAdType type);
	}
}