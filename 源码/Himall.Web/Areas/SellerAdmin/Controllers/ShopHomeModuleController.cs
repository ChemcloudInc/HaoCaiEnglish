using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ShopHomeModuleController : BaseSellerController
	{
		public ShopHomeModuleController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult AddShopHomeModule(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new InvalidPropertyException("名称不能为空");
			}
			ShopHomeModuleInfo shopHomeModuleInfo = new ShopHomeModuleInfo()
			{
				ShopId = base.CurrentSellerManager.ShopId,
				Name = name.Trim()
			};
			ShopHomeModuleInfo shopHomeModuleInfo1 = shopHomeModuleInfo;
			ServiceHelper.Create<IShopHomeModuleService>().AddShopProductModule(shopHomeModuleInfo1);
			return Json(new { success = true, name = shopHomeModuleInfo1.Name, id = shopHomeModuleInfo1.Id });
		}

		[HttpPost]
		[ShopOperationLog(Message="删除商品模块")]
		[UnAuthorize]
		public JsonResult Delelte(long id)
		{
			if (id <= 0)
			{
				throw new InvalidPropertyException("商品模块id必须大于0");
			}
			ServiceHelper.Create<IShopHomeModuleService>().Delete(base.CurrentSellerManager.ShopId, id);
			return Json(new { success = true });
		}

		[UnAuthorize]
		public ActionResult Management()
		{
			ShopHomeModuleInfo[] array = ServiceHelper.Create<IShopHomeModuleService>().GetAllShopHomeModuleInfos(base.CurrentSellerManager.ShopId).ToArray();
			IEnumerable<ShopHomeModuleBasicModel> shopHomeModuleBasicModel = 
				from item in array
                select new ShopHomeModuleBasicModel()
				{
					Id = item.Id,
					Name = item.Name,
					ProductIds = 
						from t in item.ShopHomeModuleProductInfo
						select t.ProductId
				};
			return View(shopHomeModuleBasicModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveName(long id, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new InvalidPropertyException("名称不能为空");
			}
			if (id <= 0)
			{
				throw new InvalidPropertyException("商品模块id必须大于0");
			}
			name = name.Trim();
			ServiceHelper.Create<IShopHomeModuleService>().UpdateShopProductModuleName(base.CurrentSellerManager.ShopId, id, name);
			return Json(new { success = true });
		}

		[HttpPost]
		[ShopOperationLog(Message="添加商品模块")]
		[UnAuthorize]
		public JsonResult SaveShopModuleProducts(long id, string productIds)
		{
			IEnumerable<long> nums;
			if (id <= 0)
			{
				throw new InvalidPropertyException("商品模块id必须大于0");
			}
			try
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in productIds.Split(chrArray)
					select long.Parse(item);
			}
			catch (FormatException formatException)
			{
				throw new InvalidPropertyException("商品编号不合法，请使用半角逗号连接各商品id");
			}
			ServiceHelper.Create<IShopHomeModuleService>().UpdateShopProductModuleProducts(base.CurrentSellerManager.ShopId, id, nums);
			return Json(new { success = true });
		}
	}
}