using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ProductTypeController : BaseAdminController
	{
		public ProductTypeController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DataGridJson(string searchKeyWord, int page, int rows)
		{
			PageModel<ProductTypeInfo> types = ServiceHelper.Create<ITypeService>().GetTypes(searchKeyWord, page, rows);
			IEnumerable<ProductTypeInfo> list = 
				from item in types.Models.ToList()
				select new ProductTypeInfo()
				{
					Id = item.Id,
					Name = item.Name
				};
			DataGridModel<ProductTypeInfo> dataGridModel = new DataGridModel<ProductTypeInfo>()
			{
				rows = list,
				total = types.Total
			};
			return Json(dataGridModel, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteType(long Id)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<ITypeService>().DeleteType(Id);
				IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
				LogInfo logInfo = new LogInfo()
				{
					Date = DateTime.Now,
					Description = string.Concat("删除平台类目，Id=", Id),
					IPAddress = base.Request.UserHostAddress,
					PageUrl = "/ProductType/DeleteTyp",
					UserName = base.CurrentManager.UserName,
					ShopId = 0
                };
				operationLogService.AddPlatformOperationLog(logInfo);
				result.success = true;
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("删除平台类目失败", exception);
				result.msg = "删除平台类目失败";
			}
			return Json(result);
		}

		[UnAuthorize]
		public ActionResult Edit(long id = 0L)
		{
			IQueryable<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands("");
			ViewBag.Brands = brands.ToList();
			if (id == 0)
			{
				return View(new ProductTypeInfo(true));
			}
			ProductTypeInfo type = ServiceHelper.Create<ITypeService>().GetType(id);
            TransformAttrs(type);
            TransformSpec(type);
			return View(type);
		}

		[UnAuthorize]
		public JsonResult GetBrandsAjax(long id)
		{
			ProductTypeInfo type;
			IQueryable<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands("");
			if (id == 0)
			{
				type = null;
			}
			else
			{
				type = ServiceHelper.Create<ITypeService>().GetType(id);
			}
			ProductTypeInfo productTypeInfo = type;
			List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
			foreach (BrandInfo brand in brands)
			{
				List<BrandViewModel> brandViewModels1 = brandViewModels;
				BrandViewModel brandViewModel = new BrandViewModel()
				{
					id = brand.Id,
					isChecked = (productTypeInfo == null ? false : productTypeInfo.TypeBrandInfo.Any((TypeBrandInfo b) => b.BrandId.Equals(brand.Id))),
					@value = brand.Name
				};
				brandViewModels1.Add(brandViewModel);
			}
			return Json(new { data = brandViewModels }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult SaveModel(TypeInfoModel type)
		{
			if (0 != type.Id)
			{
				ServiceHelper.Create<ITypeService>().UpdateType(type);
			}
			else if (0 == type.Id)
			{
				ServiceHelper.Create<ITypeService>().AddType(type);
			}
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("修改平台类目，Id=", type.Id),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/ProductType/SaveModel",
				UserName = base.CurrentManager.UserName,
				ShopId = 0
            };
			operationLogService.AddPlatformOperationLog(logInfo);
			return RedirectToAction("Management");
		}

		private void TransformAttrs(ProductTypeInfo model)
		{
			foreach (AttributeInfo attributeInfo in model.AttributeInfo)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string str in 
					from c in attributeInfo.AttributeValueInfo
					select c.Value)
				{
					stringBuilder.Append(str);
					stringBuilder.Append(',');
				}
				string str1 = stringBuilder.ToString();
				char[] chrArray = new char[] { ',' };
				attributeInfo.AttrValue = str1.TrimEnd(chrArray);
			}
		}

		private void TransformSpec(ProductTypeInfo model)
		{
			if (model.IsSupportColor)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SpecificationValueInfo specificationValueInfo in model.SpecificationValueInfo.Where((SpecificationValueInfo s) => {
					if (s.Specification != SpecificationType.Color)
					{
						return false;
					}
					return s.TypeId.Equals(model.Id);
				}))
				{
					stringBuilder.Append(specificationValueInfo.Value);
					stringBuilder.Append(',');
				}
				ProductTypeInfo productTypeInfo = model;
				string str = stringBuilder.ToString();
				char[] chrArray = new char[] { ',' };
				productTypeInfo.ColorValue = str.TrimEnd(chrArray);
			}
			if (model.IsSupportSize)
			{
				StringBuilder stringBuilder1 = new StringBuilder();
				foreach (SpecificationValueInfo specificationValueInfo1 in model.SpecificationValueInfo.Where((SpecificationValueInfo s) => {
					if (s.Specification != SpecificationType.Size)
					{
						return false;
					}
					return s.TypeId.Equals(model.Id);
				}))
				{
					stringBuilder1.Append(specificationValueInfo1.Value);
					stringBuilder1.Append(',');
				}
				ProductTypeInfo productTypeInfo1 = model;
				string str1 = stringBuilder1.ToString();
				char[] chrArray1 = new char[] { ',' };
				productTypeInfo1.SizeValue = str1.TrimEnd(chrArray1);
			}
			if (model.IsSupportVersion)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (SpecificationValueInfo specificationValueInfo2 in model.SpecificationValueInfo.Where((SpecificationValueInfo s) => {
					if (s.Specification != SpecificationType.Version)
					{
						return false;
					}
					return s.TypeId.Equals(model.Id);
				}))
				{
					stringBuilder2.Append(specificationValueInfo2.Value);
					stringBuilder2.Append(',');
				}
				ProductTypeInfo productTypeInfo2 = model;
				string str2 = stringBuilder2.ToString();
				char[] chrArray2 = new char[] { ',' };
				productTypeInfo2.VersionValue = str2.TrimEnd(chrArray2);
			}
		}
	}
}