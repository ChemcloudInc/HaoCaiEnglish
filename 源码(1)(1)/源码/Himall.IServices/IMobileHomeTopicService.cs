using Himall.Core;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IMobileHomeTopicService : IService, IDisposable
	{
		void AddMobileHomeTopic(long topicId, long shopId, PlatformType platformType, string frontCoverImage = null);

		void Delete(long id);

		MobileHomeTopicsInfo GetMobileHomeTopic(long id, long shopId = 0L);

		IQueryable<MobileHomeTopicsInfo> GetMobileHomeTopicInfos(PlatformType platformType, long shopId = 0L);

		void SetSequence(long id, int sequence, long shopId = 0L);
	}
}