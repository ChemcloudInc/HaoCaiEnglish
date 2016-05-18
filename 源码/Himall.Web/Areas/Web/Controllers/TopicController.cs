using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class TopicController : BaseController
	{
		public TopicController()
		{
		}

		public ActionResult Detail(string id = "")
		{
			long num = 0;
			long.TryParse(id, out num);
			TopicInfo topicInfo = ServiceHelper.Create<ITopicService>().GetTopicInfo(num);
			return View(topicInfo);
		}

		public ActionResult List()
		{
			TopicQuery topicQuery = new TopicQuery()
			{
				IsRecommend = new bool?(true),
				PlatformType = PlatformType.PC,
				PageNo = 1,
				PageSize = 5
			};
			PageModel<TopicInfo> topics = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery);
			ViewBag.TopicInfo = topics.Models.ToList();
			return View();
		}

		[HttpPost]
		public JsonResult List(int page, int pageSize)
		{
			TopicQuery topicQuery = new TopicQuery()
			{
				IsRecommend = new bool?(true),
				PlatformType = PlatformType.PC,
				PageNo = page,
				PageSize = 5
			};
			PageModel<TopicInfo> topics = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery);
			var list = 
				from item in topics.Models.ToList()
				select new { id = item.Id, name = item.Name, topimage = item.TopImage };
			return Json(new { success = true, data = list });
		}
	}
}