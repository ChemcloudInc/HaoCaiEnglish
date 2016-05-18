using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class FreightTemplateController : BaseSellerController
	{
		public FreightTemplateController()
		{
		}

		public ActionResult Add(long? id)
		{
			long num = (id.HasValue ? id.Value : 0);
			FreightTemplateInfoExtend freightTemplateInfoExtend = new FreightTemplateInfoExtend()
			{
				ShopID = base.CurrentSellerManager.ShopId
			};
			ViewBag.IsUsed = 0;
			if (id.HasValue)
			{
				FreightTemplateInfo freightTemplate = ServiceHelper.Create<IFreightTemplateService>().GetFreightTemplate(num);
				if (freightTemplate.ShopID != base.CurrentSellerManager.ShopId)
				{
					throw new HimallException(string.Concat("该运费模板不存在", id));
				}
				IRegionService regionService = ServiceHelper.Create<IRegionService>();
				if (freightTemplate.SourceAddress.HasValue)
				{
					IRegionService regionService1 = regionService;
					int? sourceAddress = freightTemplate.SourceAddress;
					freightTemplateInfoExtend.SourceAddressStr = regionService1.GetRegionIdPath(sourceAddress.Value);
				}
				freightTemplateInfoExtend.Id = freightTemplate.Id;
				freightTemplateInfoExtend.IsFree = freightTemplate.IsFree;
				freightTemplateInfoExtend.Name = freightTemplate.Name;
				freightTemplateInfoExtend.ShopID = freightTemplate.ShopID;
				freightTemplateInfoExtend.SendTime = freightTemplate.SendTime;
				freightTemplateInfoExtend.SourceAddress = freightTemplate.SourceAddress;
				freightTemplateInfoExtend.ValuationMethod = freightTemplate.ValuationMethod;
				freightTemplateInfoExtend.FreightAreaContent = 
					from e in freightTemplate.Himall_FreightAreaContent
					select new FreightAreaContentInfoExtend()
					{
						AreaContent = e.AreaContent,
						AreaContentCN = regionService.GetRegionName(e.AreaContent, ","),
						AccumulationUnit = e.AccumulationUnit,
						AccumulationUnitMoney = e.AccumulationUnitMoney,
						FirstUnit = e.FirstUnit,
						FirstUnitMonry = e.FirstUnitMonry,
						IsDefault = e.IsDefault,
						FreightTemplateId = e.FreightTemplateId,
						Id = e.Id
					};
				if (ServiceHelper.Create<IFreightTemplateService>().GetProductUseFreightTemp(num).Count() > 0)
				{
					ViewBag.IsUsed = 1;
				}
			}
			return View(freightTemplateInfoExtend);
		}

		public JsonResult DeleteTemplate(long id)
		{
			if (ServiceHelper.Create<IFreightTemplateService>().GetProductUseFreightTemp(id).Count() > 0)
			{
				return Json(new { successful = false, msg = "此运费模板已使用，不能删除!" });
			}
			if (ServiceHelper.Create<IFreightTemplateService>().GetFreightTemplate(id).ShopID != base.CurrentSellerManager.ShopId)
			{
				return Json(new { successful = false, msg = "此运费模板不存在!" });
			}
			ServiceHelper.Create<IFreightTemplateService>().DeleteFreightTemplate(id);
			return Json(new { successful = true, msg = "删除成功" });
		}

		public JsonResult GetFreightTemplateInfo(long templateid)
		{
			FreightTemplateInfo freightTemplate = ServiceHelper.Create<IFreightTemplateService>().GetFreightTemplate(templateid);
			return Json(new { model = freightTemplate, success = true });
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int page, int rows)
		{
			IQueryable<FreightTemplateInfoModel> shopFreightTemplate = 
				from e in ServiceHelper.Create<IFreightTemplateService>().GetShopFreightTemplate(base.CurrentSellerManager.ShopId)
				select new FreightTemplateInfoModel()
				{
					Id = e.Id,
					Name = e.Name,
					ShopID = e.ShopID
				};
			DataGridModel<FreightTemplateInfoModel> dataGridModel = new DataGridModel<FreightTemplateInfoModel>()
			{
				rows = shopFreightTemplate.ToArray(),
				total = shopFreightTemplate.Count()
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult SaveTemplate(string templateinfo)
		{
			FreightTemplateInfo freightTemplateInfo = JsonConvert.DeserializeObject<FreightTemplateInfo>(templateinfo);
			ServiceHelper.Create<IFreightTemplateService>().UpdateFreightTemplate(freightTemplateInfo);
			return Json(new { successful = true });
		}
	}
}