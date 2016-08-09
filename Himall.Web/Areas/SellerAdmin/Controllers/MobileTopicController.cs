using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class MobileTopicController : BaseSellerController
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
			TopicInfo shopId = JsonConvert.DeserializeObject<TopicInfo>(topicJson, jsonSerializerSetting);
			TopicInfo topicInfo = ServiceHelper.Create<ITopicService>().GetTopicInfo(shopId.Id);
			shopId.PlatForm = PlatformType.Mobile;
			shopId.ShopId = base.CurrentSellerManager.ShopId;
			shopId.BackgroundImage = (topicInfo == null ? string.Empty : topicInfo.BackgroundImage);
			shopId.FrontCoverImage = (topicInfo == null ? string.Empty : topicInfo.FrontCoverImage);
			if (shopId.Id <= 0)
			{
				ServiceHelper.Create<ITopicService>().AddTopic(shopId);
			}
			else
			{
				ServiceHelper.Create<ITopicService>().UpdateTopic(shopId);
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
				ShopId = base.CurrentSellerManager.ShopId,
				Sort = "id"
			};
			PageModel<TopicInfo> topics = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery);
			var variable = new { rows = 
				from item in topics.Models.ToArray()
				select new { id = item.Id, url = string.Concat(new object[] { "http://", base.HttpContext.Request.Url.Authority, "/m-wap/topic/detail/", item.Id }), name = item.Name, imgUrl = item.FrontCoverImage, tags = (string.IsNullOrWhiteSpace(item.Tags) ? "" : item.Tags.Replace(",", " ")) }, total = topics.Total };
			return Json(variable);
		}

		public ActionResult Management()
		{
			return View();
		}

		public ActionResult Save(long id = 0L)
		{
			TopicInfo topicInfo;
			if (id <= 0)
			{
				topicInfo = new TopicInfo();
			}
			else
			{
				topicInfo = ServiceHelper.Create<ITopicService>().GetTopicInfo(id);
				if (topicInfo.ShopId != base.CurrentSellerManager.ShopId)
				{
					throw new HimallException(string.Concat("不存在该专题或者删除！", id));
				}
			}
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