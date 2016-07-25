using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ShopGradeController : BaseAdminController
	{
		public ShopGradeController()
		{
		}

		[HttpGet]
		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult Add(ShopGradeModel shopG)
		{
			if (!base.ModelState.IsValid)
			{
				return View(shopG);
			}
			ServiceHelper.Create<IShopService>().AddShopGrade(shopG);
			return RedirectToAction("Management");
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteShopGrade(long id)
		{
			string str = "";
			ServiceHelper.Create<IShopService>().DeleteShopGrade(id, out str);
			if (string.IsNullOrWhiteSpace(str))
			{
				return Json(new { Successful = true });
			}
			return Json(new { Successful = false, msg = str });
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult Edit(ShopGradeModel shopG)
		{
			if (!base.ModelState.IsValid)
			{
				return View(shopG);
			}
			ServiceHelper.Create<IShopService>().UpdateShopGrade(shopG);
			return RedirectToAction("Management");
		}

		[HttpGet]
		public ActionResult Edit(long id)
		{
			return View(new ShopGradeModel(ServiceHelper.Create<IShopService>().GetShopGrade(id)));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List()
		{
			IQueryable<ShopGradeInfo> shopGrades = ServiceHelper.Create<IShopService>().GetShopGrades();
			IEnumerable<ShopGradeModel> array = 
				from item in shopGrades.ToArray()
                select new ShopGradeModel()
				{
					Id = item.Id,
					ChargeStandard = item.ChargeStandard,
					ImageLimit = item.ImageLimit,
					ProductLimit = item.ProductLimit,
					Name = item.Name
				};
			DataGridModel<ShopGradeModel> dataGridModel = new DataGridModel<ShopGradeModel>()
			{
				rows = array,
				total = shopGrades.Count()
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}
	}
}