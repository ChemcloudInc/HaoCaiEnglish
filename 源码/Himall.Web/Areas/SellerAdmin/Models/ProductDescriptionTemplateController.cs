using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ProductDescriptionTemplateController : BaseSellerController
	{
		public ProductDescriptionTemplateController()
		{
		}

		public ActionResult Add(long id)
		{
			ProductDescriptionTemplateInfo productDescriptionTemplateInfo = new ProductDescriptionTemplateInfo();
			if (id > 0)
			{
				productDescriptionTemplateInfo = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplate(id, base.CurrentSellerManager.ShopId);
			}
			ProductDescriptionTemplateModel productDescriptionTemplateModel = new ProductDescriptionTemplateModel()
			{
				Id = productDescriptionTemplateInfo.Id,
				Position = (int)productDescriptionTemplateInfo.Position,
				Name = productDescriptionTemplateInfo.Name,
				HtmlContent = productDescriptionTemplateInfo.Content
			};
			return View(productDescriptionTemplateModel);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Add(ProductDescriptionTemplateModel model)
		{
			ProductDescriptionTemplateInfo productDescriptionTemplateInfo = new ProductDescriptionTemplateInfo()
			{
				Id = model.Id,
				Content = model.HtmlContent,
				Name = model.Name,
				Position = (ProductDescriptionTemplateInfo.TemplatePosition)model.Position,
				ShopId = base.CurrentSellerManager.ShopId
			};
			ProductDescriptionTemplateInfo productDescriptionTemplateInfo1 = productDescriptionTemplateInfo;
			IProductDescriptionTemplateService productDescriptionTemplateService = ServiceHelper.Create<IProductDescriptionTemplateService>();
			if (model.Id <= 0)
			{
				productDescriptionTemplateService.AddTemplate(productDescriptionTemplateInfo1);
			}
			else
			{
				productDescriptionTemplateService.UpdateTemplate(productDescriptionTemplateInfo1);
			}
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<IProductDescriptionTemplateService>().DeleteTemplate(base.CurrentSellerManager.ShopId, new long[] { id });
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetAll()
		{
			ProductDescriptionTemplateInfo[] array = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplates(base.CurrentSellerManager.ShopId).ToArray();
			IEnumerable<ProductDescriptionTemplateModel> productDescriptionTemplateModel = 
				from item in array
                select new ProductDescriptionTemplateModel()
				{
					HtmlContent = item.Content,
					Id = item.Id,
					Name = item.Name,
					Position = (int)item.Position,
					PositionText = item.Position.ToDescription()
				};
			return Json(productDescriptionTemplateModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetTemplateForSelect()
		{
			ProductDescriptionTemplateInfo[] array = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplates(base.CurrentSellerManager.ShopId).ToArray();
			var variable = 
				from item in array
                select new { Id = item.Id, Name = item.Name, Position = (int)item.Position };
			return Json(variable);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int page, int rows, string name, int? position)
		{
			ProductDescriptionTemplateInfo.TemplatePosition? nullable;
			int? nullable1 = position;
			if (nullable1.HasValue)
			{
				nullable = new ProductDescriptionTemplateInfo.TemplatePosition?((ProductDescriptionTemplateInfo.TemplatePosition)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			ProductDescriptionTemplateInfo.TemplatePosition? nullable2 = nullable;
			PageModel<ProductDescriptionTemplateInfo> templates = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplates(base.CurrentSellerManager.ShopId, page, rows, name, nullable2);
			DataGridModel<ProductDescriptionTemplateModel> dataGridModel = new DataGridModel<ProductDescriptionTemplateModel>()
			{
				rows = 
					from item in templates.Models.ToArray()
                    select new ProductDescriptionTemplateModel()
					{
						HtmlContent = item.Content,
						Id = item.Id,
						Name = item.Name,
						Position = (int)item.Position,
						PositionText = item.Position.ToDescription()
					},
				total = templates.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}
	}
}