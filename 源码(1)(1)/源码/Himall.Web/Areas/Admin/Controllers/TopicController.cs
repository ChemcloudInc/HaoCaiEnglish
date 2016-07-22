using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class TopicController : BaseAdminController
	{
		public TopicController()
		{
		}

		public ActionResult Add(long? id)
		{
			if (!id.HasValue)
			{
				return View(new TopicModel());
			}
			TopicInfo topicInfo = ServiceHelper.Create<ITopicService>().GetTopicInfo(id.Value);
			TopicModel topicModel = new TopicModel()
			{
				BackgroundImage = topicInfo.BackgroundImage,
				Id = topicInfo.Id,
				Name = topicInfo.Name,
				TopImage = topicInfo.TopImage,
				TopicModuleInfo = topicInfo.TopicModuleInfo,
				IsRecommend = topicInfo.IsRecommend,
				SelfDefineText = topicInfo.SelfDefineText
			};
			return View(topicModel);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Add(string topicJson)
		{
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
			TopicInfo topicInfo = JsonConvert.DeserializeObject<TopicInfo>(topicJson, jsonSerializerSetting);
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

		public ActionResult Index()
		{
			return View();
		}

        [UnAuthorize, HttpPost]
        public JsonResult List(int page, int rows)
        {
            PageModel<TopicInfo> model = ServiceHelper.Create<ITopicService>().GetTopics(page, rows, PlatformType.PC);
            var data = new
            {
                rows = from item in model.Models select new { id = item.Id, name = item.Name, url = ("http://" + HttpContext.Request.Url.Authority) + "/topic/detail/" + item.Id, IsRecommend = item.IsRecommend },
                total = model.Total
            };
            return Json(data);
        }


        public ActionResult Management()
		{
			return View();
		}
	}
}