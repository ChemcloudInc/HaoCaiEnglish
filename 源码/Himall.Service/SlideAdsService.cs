using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class SlideAdsService : ServiceBase, ISlideAdsService, IService, IDisposable
	{
		public SlideAdsService()
		{
		}

		public void AddHandSlidAd(HandSlideAdInfo model)
		{
			DbSet<HandSlideAdInfo> handSlideAdInfo = context.HandSlideAdInfo;
			model.DisplaySequence = (handSlideAdInfo.Count() == 0 ? 0 : handSlideAdInfo.Max<HandSlideAdInfo, long>((HandSlideAdInfo s) => s.DisplaySequence)) + 1;
            context.HandSlideAdInfo.Add(model);
            context.SaveChanges();
		}

		public void AddSlidAd(SlideAdInfo model)
		{
			string empty = string.Empty;
			IQueryable<SlideAdInfo> slideAdInfo = 
				from s in context.SlideAdInfo
				where s.ShopId == model.ShopId
				select s;
			long num = (slideAdInfo.Count() == 0 ? 0 : slideAdInfo.Max<SlideAdInfo, long>((SlideAdInfo s) => s.DisplaySequence));
			if (model.TypeId == SlideAdInfo.SlideAdType.VShopHome || model.TypeId == SlideAdInfo.SlideAdType.WeixinHome)
			{
				if ((
					from item in context.SlideAdInfo
					where item.ShopId == model.ShopId && (int)item.TypeId == (int)model.TypeId
					select item).Count() + 1 > 5)
				{
					throw new HimallException("最多只能添加5张轮播图");
				}
			}
			model.DisplaySequence = num + 1;
            context.SlideAdInfo.Add(model);
            context.SaveChanges();
			empty = model.ImageUrl;
			empty = MoveImages(ref empty, model.TypeId, model.ShopId);
			model.ImageUrl = empty;
            context.SaveChanges();
		}

		public void AdjustHandSlidAdIndex(long id, bool direction)
		{
			HandSlideAdInfo displaySequence = context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.Id == id);
			if (!direction)
			{
				HandSlideAdInfo handSlideAdInfo = context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.DisplaySequence == displaySequence.DisplaySequence + 1);
				displaySequence.DisplaySequence = displaySequence.DisplaySequence + 1;
				handSlideAdInfo.DisplaySequence = handSlideAdInfo.DisplaySequence - 1;
			}
			else
			{
				HandSlideAdInfo displaySequence1 = context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.DisplaySequence == displaySequence.DisplaySequence - 1);
				displaySequence.DisplaySequence = displaySequence.DisplaySequence - 1;
				displaySequence1.DisplaySequence = displaySequence1.DisplaySequence + 1;
			}
            context.SaveChanges();
		}

		public void AdjustSlidAdIndex(long shopId, long id, bool direction, SlideAdInfo.SlideAdType type)
		{
			SlideAdInfo displaySequence = context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.Id == id && (int)s.TypeId == (int)type);
			if (!direction)
			{
				SlideAdInfo slideAdInfo = context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.DisplaySequence == displaySequence.DisplaySequence + 1 && (int)s.TypeId == (int)type);
				if (slideAdInfo != null)
				{
					displaySequence.DisplaySequence = displaySequence.DisplaySequence + 1;
					slideAdInfo.DisplaySequence = slideAdInfo.DisplaySequence - 1;
				}
			}
			else
			{
				SlideAdInfo displaySequence1 = context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.DisplaySequence == displaySequence.DisplaySequence - 1 && (int)s.TypeId == (int)type);
				if (displaySequence1 != null)
				{
					displaySequence.DisplaySequence = displaySequence.DisplaySequence - 1;
					displaySequence1.DisplaySequence = displaySequence1.DisplaySequence + 1;
				}
			}
            context.SaveChanges();
		}

		public void DeleteHandSlidAd(long id)
		{
			long displaySequence = context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.Id == id).DisplaySequence;
            context.HandSlideAdInfo.Remove(context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.Id == id));
            ResetHandSlideAdIndexFrom(displaySequence);
            context.SaveChanges();
		}

		public void DeleteSlidAd(long shopId, long id)
		{
			long displaySequence = context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.Id == id).DisplaySequence;
            context.SlideAdInfo.Remove(context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.Id == id));
            ResetSlideAdIndexFrom(shopId, displaySequence);
            context.SaveChanges();
		}

		public HandSlideAdInfo GetHandSlidAd(long id)
		{
			return context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.Id == id);
		}

		public IQueryable<HandSlideAdInfo> GetHandSlidAds()
		{
			return 
				from s in context.HandSlideAdInfo.FindAll<HandSlideAdInfo>()
				orderby s.DisplaySequence
				select s;
		}

		public ImageAdInfo GetImageAd(long shopId, long id)
		{
			return context.ImageAdInfo.FirstOrDefault((ImageAdInfo item) => item.Id == id && item.ShopId == shopId);
		}

		public IEnumerable<ImageAdInfo> GetImageAds(long shopId)
		{
			IEnumerable<ImageAdInfo> imageAdInfos = 
				from i in context.ImageAdInfo.FindBy((ImageAdInfo i) => i.ShopId == shopId)
				orderby i.Id
				select i;
			if (imageAdInfos.Count() == 0)
			{
				ImageAdInfo[] imageAdInfoArray = new ImageAdInfo[4];
				for (int num = 0; num < 4; num++)
				{
					ImageAdInfo imageAdInfo = new ImageAdInfo()
					{
						ImageUrl = "",
						ShopId = shopId,
						Url = ""
					};
					imageAdInfoArray[num] = imageAdInfo;
				}
                context.ImageAdInfo.AddRange(imageAdInfoArray);
                context.SaveChanges();
				imageAdInfos = imageAdInfoArray;
			}
			return imageAdInfos;
		}

		public SlideAdInfo GetSlidAd(long shopId, long id)
		{
			return context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == shopId && s.Id == id);
		}

		public IQueryable<SlideAdInfo> GetSlidAds(long shopId, SlideAdInfo.SlideAdType type)
		{
			return 
				from s in context.SlideAdInfo
				where s.ShopId == shopId && (int)s.TypeId == (int)type
				select s into t
				orderby t.DisplaySequence
				select t;
		}

		private string MoveImages(ref string backGroundImage, SlideAdInfo.SlideAdType type, long shopId)
		{
			string empty = string.Empty;
			string str = string.Empty;
			if (type == SlideAdInfo.SlideAdType.WeixinHome)
			{
				empty = IOHelper.GetMapPath("/Storage/Plat/Weixin/SlidAd/");
				str = "/Storage/Plat/Weixin/SlidAd/";
			}
			if (type == SlideAdInfo.SlideAdType.VShopHome)
			{
				empty = IOHelper.GetMapPath(string.Concat("/Storage/Shop/", shopId, "/VShop/"));
				str = string.Concat("/Storage/Shop/", shopId, "/VShop/");
			}
			if (type == SlideAdInfo.SlideAdType.IOSShopHome)
			{
				empty = IOHelper.GetMapPath("/Storage/Plat/APP/SlidAd/");
				str = "/Storage/Plat/APP/SlidAd/";
			}
			if (!string.IsNullOrWhiteSpace(empty) && !Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (!string.IsNullOrWhiteSpace(empty) && backGroundImage.Replace("\\", "/").Contains("/temp/"))
			{
				string mapPath = IOHelper.GetMapPath(backGroundImage);
				IOHelper.CopyFile(mapPath, empty, false, "");
				backGroundImage = string.Concat(str, (new FileInfo(mapPath)).Name);
			}
			return backGroundImage;
		}

		private void ResetHandSlideAdIndexFrom(long index)
		{
			List<HandSlideAdInfo> list = (
				from s in context.HandSlideAdInfo.FindBy<HandSlideAdInfo>((HandSlideAdInfo s) => s.DisplaySequence > index)
				orderby s.DisplaySequence descending
				select s).ToList();
			for (int i = 0; i < list.Count(); i++)
			{
				if (i != list.Count() - 1)
				{
					list[i].DisplaySequence = list[i + 1].DisplaySequence;
				}
				else
				{
					list[i].DisplaySequence = index;
				}
			}
		}

		private void ResetSlideAdIndexFrom(long shopId, long index)
		{
			List<SlideAdInfo> list = (
				from s in context.SlideAdInfo.FindBy<SlideAdInfo>((SlideAdInfo s) => s.DisplaySequence > index && s.ShopId == shopId)
				orderby s.DisplaySequence descending
				select s).ToList();
			for (int i = 0; i < list.Count(); i++)
			{
				if (i != list.Count() - 1)
				{
					list[i].DisplaySequence = list[i + 1].DisplaySequence;
				}
				else
				{
					list[i].DisplaySequence = index;
				}
			}
		}

		public void UpdateHandSlidAd(HandSlideAdInfo models)
		{
			HandSlideAdInfo imageUrl = context.HandSlideAdInfo.FirstOrDefault((HandSlideAdInfo s) => s.Id == models.Id);
			imageUrl.ImageUrl = models.ImageUrl;
			imageUrl.Url = models.Url;
            context.SaveChanges();
		}

		public void UpdateImageAd(ImageAdInfo model)
		{
			ImageAdInfo imageUrl = context.ImageAdInfo.FirstOrDefault((ImageAdInfo i) => i.ShopId == model.ShopId && i.Id == model.Id);
			if (imageUrl != null && imageUrl.Id == model.Id)
			{
				imageUrl.ImageUrl = model.ImageUrl;
				imageUrl.Url = model.Url;
			}
            context.SaveChanges();
		}

		public void UpdateSlidAd(SlideAdInfo models)
		{
			string empty = string.Empty;
			SlideAdInfo description = context.SlideAdInfo.FirstOrDefault((SlideAdInfo s) => s.ShopId == models.ShopId && s.Id == models.Id);
			description.Description = models.Description;
			description.ImageUrl = models.ImageUrl;
			description.Url = models.Url;
            context.SaveChanges();
			empty = models.ImageUrl;
			empty = MoveImages(ref empty, models.TypeId, models.ShopId);
			description.ImageUrl = empty;
            context.SaveChanges();
		}

		public void UpdateWeixinSlideSequence(long shopId, long sourceSequence, long destiSequence, SlideAdInfo.SlideAdType type)
		{
			SlideAdInfo slideAdInfo = context.SlideAdInfo.FirstOrDefault((SlideAdInfo item) => item.DisplaySequence == sourceSequence && item.ShopId == shopId && (int)item.TypeId == (int)type);
			SlideAdInfo slideAdInfo1 = context.SlideAdInfo.FirstOrDefault((SlideAdInfo item) => item.DisplaySequence == destiSequence && item.ShopId == shopId && (int)item.TypeId == (int)type);
			slideAdInfo.DisplaySequence = destiSequence;
			slideAdInfo1.DisplaySequence = sourceSequence;
            context.SaveChanges();
		}
	}
}