using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class TopicController : BaseMobileTemplatesController
	{
		public TopicController()
		{
		}

		public ActionResult Detail(long id)
		{
			return View(ServiceHelper.Create<ITopicService>().GetTopicInfo(id));
		}

		public ActionResult List(int pageNo = 1, int pageSize = 10)
		{
			TopicQuery topicQuery = new TopicQuery()
			{
				ShopId = 0,
				PlatformType = Himall.Core.PlatformType.Mobile,
				PageNo = pageNo,
				PageSize = pageSize
			};
			IQueryable<TopicInfo> models = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery).Models;
			return View(models);
		}

		[HttpPost]
		public JsonResult LoadProducts(long topicId, long moduleId, int page, int pageSize)
		{
			TopicInfo topicInfo = ServiceHelper.Create<ITopicService>().GetTopicInfo(topicId);
			TopicModuleInfo topicModuleInfo = topicInfo.TopicModuleInfo.FirstOrDefault((TopicModuleInfo item) => item.Id == moduleId);
			IEnumerable<long> nums = 
				from item in topicModuleInfo.ModuleProductInfo.Where((ModuleProductInfo item) => {
					if (item.ProductInfo.SaleStatus != ProductInfo.ProductSaleStatus.OnSale)
					{
						return false;
					}
					return item.ProductInfo.AuditStatus == ProductInfo.ProductAuditStatus.Audited;
				}).OrderBy<ModuleProductInfo, long>((ModuleProductInfo item) => item.Id).Skip(pageSize * (page - 1)).Take(pageSize)
				select item.ProductId;
			IQueryable<ProductInfo> productByIds = ServiceHelper.Create<IProductService>().GetProductByIds(nums);
			var array = 
				from item in productByIds.ToArray()
                select new { name = item.ProductName, id = item.Id, image = item.GetImage(ProductInfo.ImageSize.Size_350, 1), price = item.MinSalePrice, marketPrice = item.MarketPrice };
			return Json(array);
		}

		[HttpPost]
		public JsonResult TopicList(int pageNo = 1, int pageSize = 10)
		{
			TopicQuery topicQuery = new TopicQuery()
			{
				ShopId = 0,
				PlatformType = Himall.Core.PlatformType.Mobile,
				PageNo = pageNo,
				PageSize = pageSize
			};
			List<TopicInfo> list = ServiceHelper.Create<ITopicService>().GetTopics(topicQuery).Models.ToList();
			var variable = 
				from item in list
				select new { Id = item.Id, TopImage = item.TopImage, Name = item.Name };
			return Json(variable);
		}
	}
}