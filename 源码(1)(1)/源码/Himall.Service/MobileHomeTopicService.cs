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
	public class MobileHomeTopicService : ServiceBase, IMobileHomeTopicService, IService, IDisposable
	{
		private const int MAX_HOMETOPIC_COUNT = 10;

		public MobileHomeTopicService()
		{
		}

		public void AddMobileHomeTopic(long topicId, long shopId, PlatformType platformType, string frontCoverImage = null)
		{
			if (context.MobileHomeTopicsInfo.Count((MobileHomeTopicsInfo item) => item.TopicId == topicId && item.ShopId == shopId && (int)item.Platform == (int)platformType) > 0)
			{
				throw new HimallException("已经添加过相同的专题");
			}
			if (context.MobileHomeTopicsInfo.Count((MobileHomeTopicsInfo item) => item.ShopId == shopId && (int)item.Platform == (int)platformType) >= 10)
			{
				throw new HimallException(string.Format("最多只能添加{0}个专题", 10));
			}
			MobileHomeTopicsInfo mobileHomeTopicsInfo = new MobileHomeTopicsInfo()
			{
				Platform = platformType,
				ShopId = shopId,
				TopicId = topicId
			};
            context.MobileHomeTopicsInfo.Add(mobileHomeTopicsInfo);
            context.SaveChanges();
		}

		public void Delete(long id)
		{
            context.MobileHomeTopicsInfo.Remove(new object[] { id });
            context.SaveChanges();
		}

		public MobileHomeTopicsInfo GetMobileHomeTopic(long id, long shopId = 0L)
		{
			return context.MobileHomeTopicsInfo.FirstOrDefault((MobileHomeTopicsInfo item) => item.Id == id && item.ShopId == shopId);
		}

		public IQueryable<MobileHomeTopicsInfo> GetMobileHomeTopicInfos(PlatformType platformType, long shopId = 0L)
		{
			return 
				from item in context.MobileHomeTopicsInfo
				where item.ShopId == shopId && (int)item.Platform == (int)platformType
				select item;
		}

		public void SetSequence(long id, int sequence, long shopId = 0L)
		{
            GetMobileHomeTopic(id, shopId).Sequence = sequence;
            context.SaveChanges();
		}
	}
}