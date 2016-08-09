using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class BrandController : BaseAdminController
	{
		public BrandController()
		{
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult Add(BrandModel brand)
		{
			if (!base.ModelState.IsValid)
			{
				return View(brand);
			}
			BrandInfo brandInfo = new BrandInfo()
			{
				Name = brand.BrandName.Trim(),
				Description = brand.BrandDesc,
				Logo = brand.BrandLogo,
				Meta_Description = brand.MetaDescription,
				Meta_Keywords = brand.MetaKeyWord,
				Meta_Title = brand.MetaTitle,
				IsRecommend = brand.IsRecommend
			};
			BrandInfo brandInfo1 = brandInfo;
			if (!ServiceHelper.Create<IBrandService>().IsExistBrand(brand.BrandName))
			{
				ServiceHelper.Create<IBrandService>().AddBrand(brandInfo1);
			}
			return RedirectToAction("Management");
		}

		public JsonResult ApplyList(int page, int rows, string keyWords)
		{
			keyWords = keyWords.Trim();
			long? nullable = null;
			PageModel<ShopBrandApplysInfo> shopBrandApplys = ServiceHelper.Create<IBrandService>().GetShopBrandApplys(nullable, new int?(0), page, rows, keyWords);
			IEnumerable<BrandApplyModel> array = 
				from item in shopBrandApplys.Models.ToArray()
                select new BrandApplyModel()
				{
					Id = item.Id,
					BrandId = (!item.BrandId.HasValue ? 0 : item.BrandId.Value),
					ShopId = item.ShopId,
					BrandName = item.BrandName,
					BrandLogo = item.Logo,
					BrandDesc = (item.Description == null ? "" : item.Description),
					BrandAuthPic = item.AuthCertificate,
					Remark = item.Remark,
					BrandMode = item.ApplyMode,
					AuditStatus = item.AuditStatus,
					ApplyTime = item.ApplyTime.ToString("yyyy-MM-dd"),
					ShopName = item.Himall_Shops.ShopName
				};
			DataGridModel<BrandApplyModel> dataGridModel = new DataGridModel<BrandApplyModel>()
			{
				rows = array,
				total = shopBrandApplys.Total
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Audit(int id)
		{
			ServiceHelper.Create<IBrandService>().AuditBrand(id, ShopBrandApplysInfo.BrandAuditStatus.Audited);
			Result result = new Result()
			{
				success = true,
				msg = "审核成功！"
			};
			return Json(result);
		}

		[Description("删除品牌")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(int id)
		{
			ServiceHelper.Create<IBrandService>().DeleteBrand(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		[Description("删除品牌申请")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteApply(int id)
		{
			ServiceHelper.Create<IBrandService>().DeleteApply(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		public ActionResult Edit(long id)
		{
			BrandInfo brand = ServiceHelper.Create<IBrandService>().GetBrand(id);
			BrandModel brandModel = new BrandModel()
			{
				ID = brand.Id,
				BrandName = brand.Name,
				BrandDesc = brand.Description,
				BrandLogo = brand.Logo,
				MetaDescription = brand.Meta_Description,
				MetaKeyWord = brand.Meta_Keywords,
				MetaTitle = brand.Meta_Title,
				IsRecommend = brand.IsRecommend
			};
			return View(brandModel);
		}

		[HttpPost]
		[OperationLog(Message="编辑品牌")]
		[UnAuthorize]
		public ActionResult Edit(BrandModel brand)
		{
			if (!base.ModelState.IsValid)
			{
				return View(brand);
			}
			BrandInfo brandInfo = new BrandInfo()
			{
				Id = brand.ID,
				Name = brand.BrandName.Trim(),
				Description = brand.BrandDesc,
				Logo = brand.BrandLogo,
				Meta_Description = brand.MetaDescription,
				Meta_Keywords = brand.MetaKeyWord,
				Meta_Title = brand.MetaTitle,
				IsRecommend = brand.IsRecommend
			};
			ServiceHelper.Create<IBrandService>().UpdateBrand(brandInfo);
			return RedirectToAction("Management");
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetBrands(string keyWords, int? AuditStatus = 2)
		{
			IQueryable<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands(keyWords);
			var variable = 
				from item in brands
				select new { key = item.Id, @value = item.Name, envalue = item.RewriteName };
			return Json(variable);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult IsExist(string name)
		{
			if (!ServiceHelper.Create<IBrandService>().IsExistBrand(name))
			{
				Result result = new Result()
				{
					success = false,
					msg = null
				};
				return Json(result);
			}
			Result result1 = new Result()
			{
				success = true,
				msg = "该品牌已存在，请不要重复添加！"
			};
			return Json(result1);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult IsInUse(long id)
		{
			if (!ServiceHelper.Create<IBrandService>().BrandInUse(id))
			{
				Result result = new Result()
				{
					success = false,
					msg = "该品牌尚未使用！"
				};
				return Json(result);
			}
			Result result1 = new Result()
			{
				success = true,
				msg = "该品牌已使用！"
			};
			return Json(result1);
		}

		[Description("分页获取品牌列表JSON数据")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int page, int rows, string keyWords)
		{
			keyWords = keyWords.Trim();
			PageModel<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands(keyWords, page, rows);
			IEnumerable<BrandModel> array = 
				from item in brands.Models.ToArray()
                select new BrandModel()
				{
					BrandName = item.Name,
					BrandLogo = item.Logo,
					ID = item.Id
				};
			DataGridModel<BrandModel> dataGridModel = new DataGridModel<BrandModel>()
			{
				rows = array,
				total = brands.Total
			};
			return Json(dataGridModel);
		}

		[Description("品牌管理页面")]
		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Refuse(int id)
		{
			ServiceHelper.Create<IBrandService>().AuditBrand(id, ShopBrandApplysInfo.BrandAuditStatus.Refused);
			Result result = new Result()
			{
				success = true,
				msg = "拒绝成功！"
			};
			return Json(result);
		}

		public ActionResult Show(long id)
		{
			return View(ServiceHelper.Create<IBrandService>().GetBrandApply(id));
		}

		public ActionResult Verify()
		{
			return View();
		}
	}
}