using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ProductController : BaseAdminController
	{
		public ProductController()
		{
		}

		[HttpPost]
		[OperationLog(Message="审核商品状态")]
		public JsonResult Audit(long productId, int auditState, string message)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IProductService>().AuditProduct(productId, (ProductInfo.ProductAuditStatus)auditState, message);
				result.success = true;
				result.msg = "审核成功！";
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("审核出错", exception);
				result.msg = "审核出错！";
			}
			return Json(result);
		}

		[HttpPost]
		[OperationLog(Message="批量审核商品状态")]
		public JsonResult BatchAudit(string productIds, int auditState, string message)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in productIds.Split(chrArray)
				select long.Parse(item);
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IProductService>().AuditProducts(nums, (ProductInfo.ProductAuditStatus)auditState, message);
				result.success = true;
				result.msg = "审核成功！";
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("审核出错", exception);
				result.msg = "审核出错！";
			}
			return Json(result);
		}

		[HttpPost]
		public JsonResult GetProductAuditOnOff()
		{
			int produtAuditOnOff = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().ProdutAuditOnOff;
			return Json(new { @value = produtAuditOnOff });
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult List(long? categoryId, string brandName, string productCode, int? auditStatus, string ids, int page, int rows, string keyWords, string shopName, int? saleStatus)
		{
			IEnumerable<long> nums;
			ProductQuery productQuery = new ProductQuery()
			{
				PageSize = rows,
				PageNo = page,
				BrandNameKeyword = brandName,
				KeyWords = keyWords,
				CategoryId = categoryId
			};
			ProductQuery productQuery1 = productQuery;
			if (string.IsNullOrWhiteSpace(ids))
			{
				nums = null;
			}
			else
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
			}
			productQuery1.Ids = nums;
			productQuery.ShopName = shopName;
			productQuery.ProductCode = productCode;
			productQuery.NotIncludedInDraft = true;
			ProductQuery value = productQuery;
			if (auditStatus.HasValue)
			{
				value.AuditStatus = new ProductInfo.ProductAuditStatus[] { (ProductInfo.ProductAuditStatus)auditStatus.Value };
				int? nullable = auditStatus;
				if ((nullable.GetValueOrDefault() != 1 ? false : nullable.HasValue))
				{
					value.SaleStatus = new ProductInfo.ProductSaleStatus?(ProductInfo.ProductSaleStatus.OnSale);
				}
			}
			if (saleStatus.HasValue)
			{
				value.SaleStatus = new ProductInfo.ProductSaleStatus?((ProductInfo.ProductSaleStatus)saleStatus.Value);
			}
			PageModel<ProductInfo> products = ServiceHelper.Create<IProductService>().GetProducts(value);
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			IEnumerable<ProductModel> array = 
				from item in products.Models.ToArray()
				select new ProductModel()
				{
					name = item.ProductName,
					brandName = (item.BrandId == 0 ? "" : (brandService.GetBrand(item.BrandId) == null ? "" : brandService.GetBrand(item.BrandId).Name)),
					categoryName = (categoryService.GetCategory(item.CategoryId) == null ? "" : categoryService.GetCategory(item.CategoryId).Name),
					id = item.Id,
					imgUrl = item.GetImage(ProductInfo.ImageSize.Size_50, 1),
					price = item.MinSalePrice,
					state = item.ShowProductState,
					auditStatus = (int)item.AuditStatus,
					url = "",
					auditReason = (item.ProductDescriptionInfo != null ? item.ProductDescriptionInfo.AuditReason : ""),
					shopName = (shopService.GetShopBasicInfo(item.ShopId) == null ? "" : shopService.GetShopBasicInfo(item.ShopId).ShopName),
					saleStatus = (int)item.SaleStatus,
					productCode = item.ProductCode
				};
			DataGridModel<ProductModel> dataGridModel = new DataGridModel<ProductModel>()
			{
				rows = array,
				total = products.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		public JsonResult SaveProductAuditOnOff(int value)
		{
			ServiceHelper.Create<ISiteSettingService>().SaveSetting("ProdutAuditOnOff", value);
			return Json(new { success = true });
		}
	}
}