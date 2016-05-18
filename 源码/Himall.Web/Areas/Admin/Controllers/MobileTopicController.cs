using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class MobileTopicController : BaseAdminController
	{
		public MobileTopicController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Add(string topicJson)
		{
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
			TopicInfo topicInfo = JsonConvert.DeserializeObject<TopicInfo>(topicJson, jsonSerializerSetting);
			TopicInfo topicInfo1 = ServiceHelper.Create<ITopicService>().GetTopicInfo(topicInfo.Id);
			topicInfo.PlatForm = PlatformType.Mobile;
			topicInfo.BackgroundImage = (topicInfo1 == null ? string.Empty : topicInfo1.BackgroundImage);
			topicInfo.FrontCoverImage = (topicInfo1 == null ? string.Empty : topicInfo1.FrontCoverImage);
			if (topicInfo.Id <= 0)
			{
				ServiceHelper.Create<ITopicService>().AddTopic(topicInfo);
			}
			else
			{
				ServiceHelper.Create<ITopicService>().UpdateTopic(topicInfo);
			}
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			Result result = new Result();
			ServiceHelper.Create<ITopicService>().DeleteTopic(id);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int page, int rows, string titleKeyword, string tagsKeyword)
		{
			TopicQuery topicQuery = new TopicQuery()
			{
				IsAsc = false,
				PageSize = rows,
				PageNo = page,
				Name = titleKeyword,
				Tags = tagsKeyword,
				PlatformType = PlatformType.Mobile,
				ShopId = base.CurrentManager.ShopId,
				Sort = "id"
			};
			PageModel<TopicInfo> topics = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery);
			var variable = new { rows = 
				from item in topics.Models.ToArray()
				select new { id = item.Id, name = item.Name, imgUrl = item.FrontCoverImage, url = string.Concat(new object[] { "http://", base.HttpContext.Request.Url.Authority, "/m-wap/topic/detail/", item.Id }), tags = (string.IsNullOrWhiteSpace(item.Tags) ? "" : item.Tags.Replace(",", " ")) }, total = topics.Total };
			return Json(variable);
		}

		public ActionResult Management()
		{
			return View();
		}

		public ActionResult Save(long id = 0L)
		{
			TopicInfo topicInfo;
			topicInfo = (id <= 0 ? new TopicInfo() : ServiceHelper.Create<ITopicService>().GetTopicInfo(id));
			TopicModel topicModel = new TopicModel()
			{
				Id = topicInfo.Id,
				Name = topicInfo.Name,
				TopImage = topicInfo.TopImage,
				TopicModuleInfo = topicInfo.TopicModuleInfo,
				Tags = topicInfo.Tags
			};
			return View(topicModel);
		}
	}
}