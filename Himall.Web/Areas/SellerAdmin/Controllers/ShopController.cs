using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ShopController : BaseSellerController
	{
		public ShopController()
		{
		}

		[HttpPost]
		public JsonResult EditProfile1()
		{
			string item = base.Request.Params["CompanyName"] ?? "";
			string str = base.Request.Params["CompanyAddress"] ?? "";
			string item1 = base.Request.Params["CompanyRegionId"] ?? "";
			item1 = (string.IsNullOrWhiteSpace(item1) ? base.Request.Params["NewCompanyRegionId"] : item1);
			int num = 0;
			if (!int.TryParse(item1, out num))
			{
				num = 0;
			}
			string str1 = base.Request.Params["CompanyRegionAddress"] ?? "";
			string item2 = base.Request.Params["CompanyPhone"] ?? "";
			string str2 = base.Request.Params["CompanyEmployeeCount"] ?? "";
			string item3 = base.Request.Params["CompanyRegisteredCapital"] ?? "";
			string str3 = base.Request.Params["ContactsName"] ?? "";
			string item4 = base.Request.Params["ContactsPhone"] ?? "";
			string str4 = base.Request.Params["ContactsEmail"] ?? "";
			string item5 = base.Request.Params["BusinessLicenseCert"] ?? "";
			string str5 = base.Request.Params["ProductCert"] ?? "";
			string item6 = base.Request.Params["OtherCert"] ?? "";
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = base.CurrentSellerManager.ShopId,
				CompanyName = item,
				CompanyAddress = str,
				CompanyRegionId = num,
				CompanyRegionAddress = str1,
				CompanyPhone = item2,
				CompanyEmployeeCount = (CompanyEmployeeCount)int.Parse(str2),
				CompanyRegisteredCapital = decimal.Parse(item3),
				ContactsName = str3,
				ContactsPhone = item4,
				ContactsEmail = str4,
				BusinessLicenseCert = item5,
				ProductCert = str5,
				OtherCert = item6
			};
			ServiceHelper.Create<IShopService>().UpdateShop(shopInfo);
			return Json(new { success = true });
		}

		public ActionResult FreightSetting()
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			ShopFreightModel shopFreightModel = new ShopFreightModel()
			{
				FreeFreight = shop.FreeFreight,
				Freight = shop.Freight
			};
			return View(shopFreightModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveFreightSetting(ShopFreightModel shopFreight)
		{
			ServiceHelper.Create<IShopService>().UpdateShopFreight(base.CurrentSellerManager.ShopId, shopFreight.Freight, shopFreight.FreeFreight);
			return Json(new { success = true });
		}

		public ActionResult ShopDetail()
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(shopId, true);
			ShopModel shopModel = new ShopModel(shop)
			{
				BusinessCategory = new List<CategoryKeyVal>()
			};
			foreach (long key in shop.BusinessCategory.Keys)
			{
				List<CategoryKeyVal> businessCategory = shopModel.BusinessCategory;
				CategoryKeyVal categoryKeyVal = new CategoryKeyVal()
				{
					CommisRate = shop.BusinessCategory[key],
					Name = ServiceHelper.Create<ICategoryService>().GetCategory(key).Name
				};
				businessCategory.Add(categoryKeyVal);
			}
			ViewBag.CompanyRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.CompanyRegionId);
			ViewBag.BusinessLicenseCert = shop.BusinessLicenseCert;
			string[] strArrays = new string[3];
			string[] strArrays1 = new string[3];
			string[] strArrays2 = new string[3];
			for (int i = 0; i < 3; i++)
			{
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.BusinessLicenseCert, string.Format("{0}.png", i + 1)))))
				{
					strArrays[i] = string.Concat(shop.BusinessLicenseCert, string.Format("{0}.png", i + 1));
				}
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.ProductCert, string.Format("{0}.png", i + 1)))))
				{
					strArrays1[i] = string.Concat(shop.ProductCert, string.Format("{0}.png", i + 1));
				}
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.OtherCert, string.Format("{0}.png", i + 1)))))
				{
					strArrays2[i] = string.Concat(shop.OtherCert, string.Format("{0}.png", i + 1));
				}
			}
			ViewBag.BusinessLicenseCerts = strArrays;
			ViewBag.ProductCerts = strArrays1;
			ViewBag.OtherCerts = strArrays2;
			return View(shopModel);
		}
	}
}