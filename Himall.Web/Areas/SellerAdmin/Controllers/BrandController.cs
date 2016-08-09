using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class BrandController : BaseSellerController
	{
		public BrandController()
		{
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[ShopOperationLog(Message="申请品牌")]
		[UnAuthorize]
		public JsonResult Apply(BrandApplyModel brand)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ShopBrandApplysInfo shopBrandApplysInfo = new ShopBrandApplysInfo()
			{
				BrandId = new long?(brand.BrandId),
				ApplyMode = (brand.BrandMode == 1 ? 1 : 2)
			};
			if (brand.BrandMode != 1)
			{
				shopBrandApplysInfo.BrandId = null;
				shopBrandApplysInfo.BrandName = brand.BrandName.Trim();
				shopBrandApplysInfo.Logo = brand.BrandLogo;
				shopBrandApplysInfo.Description = brand.BrandDesc;
			}
			else
			{
				BrandInfo brandInfo = ServiceHelper.Create<IBrandService>().GetBrand(brand.BrandId);
				if (brandInfo != null)
				{
					shopBrandApplysInfo.BrandName = brandInfo.Name;
					shopBrandApplysInfo.Logo = brandInfo.Logo;
					shopBrandApplysInfo.Description = brandInfo.Description;
				}
			}
			shopBrandApplysInfo.Remark = brand.Remark;
			shopBrandApplysInfo.AuthCertificate = brand.BrandAuthPic;
			shopBrandApplysInfo.ShopId = shopId;
			shopBrandApplysInfo.ApplyTime = DateTime.Now;
			if (ServiceHelper.Create<IBrandService>().IsExistApply(shopId, shopBrandApplysInfo.BrandName))
			{
				Result result = new Result()
				{
					success = false,
					msg = "您已经申请了该品牌！"
				};
				return Json(result);
			}
			ServiceHelper.Create<IBrandService>().ApplyBrand(shopBrandApplysInfo);
			Result result1 = new Result()
			{
				success = true,
				msg = "申请成功！"
			};
			return Json(result1);
		}

		[UnAuthorize]
		public JsonResult GetBrandsAjax(long? id)
		{
			IQueryable<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands("");
			List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
			foreach (BrandInfo brand in brands)
			{
				List<BrandViewModel> brandViewModels1 = brandViewModels;
				BrandViewModel brandViewModel = new BrandViewModel()
				{
					id = brand.Id,
					isChecked = (!id.HasValue ? false : id.Equals(brand.Id)),
					@value = brand.Name
				};
				brandViewModels1.Add(brandViewModel);
			}
			return Json(new { data = brandViewModels }, JsonRequestBehavior.AllowGet);
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
					msg = "该品牌不存在！"
				};
				return Json(result);
			}
			Result result1 = new Result()
			{
				success = true,
				msg = "该品牌已存在！"
			};
			return Json(result1);
		}

		[Description("分页获取品牌列表JSON数据")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int page, int rows)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			int? nullable = null;
			PageModel<ShopBrandApplysInfo> shopBrandApplys = ServiceHelper.Create<IBrandService>().GetShopBrandApplys(new long?(shopId), nullable, page, rows, "");
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
					AuditStatus = item.AuditStatus
				};
			DataGridModel<BrandApplyModel> dataGridModel = new DataGridModel<BrandApplyModel>()
			{
				rows = array,
				total = shopBrandApplys.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}

		public ActionResult Show(long id)
		{
			return View(ServiceHelper.Create<IBrandService>().GetBrandApply(id));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateSellerBrand(BrandModel brand)
		{
			BrandInfo brandInfo = new BrandInfo()
			{
				Id = brand.ID,
				Name = brand.BrandName,
				Description = brand.BrandDesc,
				Logo = brand.BrandLogo
			};
			Result result = new Result()
			{
				success = true,
				msg = "更新成功"
			};
			return Json(result);
		}
	}
}