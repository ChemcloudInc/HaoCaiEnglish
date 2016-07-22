using Himall.Core;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface ITopicService : IService, IDisposable
	{
		void AddTopic(TopicInfo topicInfo);

		void DeleteTopic(long id);

		TopicInfo GetTopicInfo(long id);

		PageModel<TopicInfo> GetTopics(int pageNo, int pageSize, PlatformType platformType = 0);

		PageModel<TopicInfo> GetTopics(TopicQuery topicQuery);

		void UpdateTopic(TopicInfo topicInfo);
	}
}