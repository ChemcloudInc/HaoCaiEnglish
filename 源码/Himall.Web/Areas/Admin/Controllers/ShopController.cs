using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class ShopController : BaseAdminController
	{
		public ShopController()
		{
		}

		[Description("开店审核页面")]
		[HttpGet]
		[UnAuthorize]
		public ActionResult Auditing(long id)
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(id, true);
			ViewBag.ShopId = id;
			ViewBag.Status = shop.ShopStatus;
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
			return View(shopModel);
		}

		[Description("开店审核页面(POST)")]
		[HttpPost]
		[UnAuthorize]
		public JsonResult Auditing(long shopId, int status, string comment = "")
		{
			ServiceHelper.Create<IShopService>().UpdateShopStatus(shopId, (ShopInfo.ShopAuditStatus)status, comment);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Format("开店审核页面，店铺Id={0},状态为：{1}, 说明是：{2}", shopId, (ShopInfo.ShopAuditStatus)status, comment),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = string.Concat("/Shop/Auditing/", shopId),
				UserName = base.CurrentManager.UserName,
				ShopId = 0
            };
			operationLogService.AddPlatformOperationLog(logInfo);
			return Json(new { Successful = true });
		}

		[Description("类目经营页面")]
		public ActionResult BusinessCategory(long id)
		{
			List<BusinessCategoryInfo> list = ServiceHelper.Create<IShopService>().GetBusinessCategory(id).ToList();
			ViewBag.ShopId = id;
			return View(list);
		}

		[HttpPost]
		public bool CheckShopGrade(long shopId, long newId)
		{
			IShopService shopService = ServiceHelper.Create<IShopService>();
			int shopAllProducts = ServiceHelper.Create<IProductService>().GetShopAllProducts(shopId);
			long shopSpaceUsage = shopService.GetShopSpaceUsage(shopId);
			long num = (shopSpaceUsage > 0 ? shopSpaceUsage : 0);
			if (newId <= 0)
			{
				return true;
			}
			ShopGradeInfo shopGrade = ServiceHelper.Create<IShopService>().GetShopGrade(newId);
			if (shopGrade == null)
			{
				return true;
			}
			int productLimit = shopGrade.ProductLimit;
			if (productLimit >= shopAllProducts && shopGrade.ImageLimit >= num)
			{
				return true;
			}
			return false;
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteShop(long Id)
		{
			ServiceHelper.Create<IShopService>().DeleteShop(Id);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("删除店铺，Id=", Id),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = string.Concat("/Shop/Edit/", Id),
				UserName = base.CurrentManager.UserName,
				ShopId = 0
            };
			operationLogService.AddPlatformOperationLog(logInfo);
			return Json(new { Successful = true });
		}

		public ActionResult Details(long id)
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(id, true);
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
			ShopInfo shopInfo = ServiceHelper.Create<IShopService>().GetShop(id, false);
			ViewBag.PassStr = shopInfo.ShopStatus.ToDescription();
			return View(shopModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Edit(ShopModel shop)
		{
			IShopService shopService = ServiceHelper.Create<IShopService>();
			ShopInfo companyName = shopService.GetShop(shop.Id, false);
			if (!CheckShopGrade(shop.Id, Convert.ToInt64(shop.ShopGrade)))
			{
				throw new HimallException("该店铺已使用空间数或已添加商品数大于该套餐");
			}
			if (shopService.ExistShop(companyName.ShopName, shop.Id))
			{
				throw new HimallException("该店铺已存在");
			}
			if (!base.ModelState.IsValid)
			{
				List<string> strs = new List<string>();
				foreach (string list in base.ModelState.Keys.ToList())
				{
					foreach (ModelError modelError in base.ModelState[list].Errors.ToList())
					{
						strs.Add(modelError.ErrorMessage);
					}
				}
				Result result = new Result()
				{
					success = false,
					msg = strs[0].ToString()
				};
				return Json(result);
			}
			companyName.CompanyName = shop.CompanyName;
			companyName.CompanyAddress = shop.CompanyAddress;
			companyName.CompanyPhone = shop.CompanyPhone;
			companyName.CompanyEmployeeCount = shop.CompanyEmployeeCount;
			companyName.CompanyRegisteredCapital = shop.CompanyRegisteredCapital;
			companyName.ContactsName = shop.ContactsName;
			companyName.ContactsEmail = shop.ContactsEmail;
			companyName.ContactsPhone = shop.ContactsPhone;
			companyName.BankAccountName = shop.BankAccountName;
			companyName.BankAccountNumber = shop.BankAccountNumber;
			companyName.BankCode = shop.BankCode;
			companyName.BankName = shop.BankName;
			companyName.BankPhoto = shop.BankPhoto;
			companyName.ShopName = shop.Name;
			companyName.GradeId = Convert.ToInt64(shop.ShopGrade);
			companyName.EndDate = new DateTime?(Convert.ToDateTime(shop.EndDate));
			companyName.CompanyRegionId = shop.NewCompanyRegionId;
			companyName.BankRegionId = shop.NewBankRegionId;
			companyName.ShopStatus = (ShopInfo.ShopAuditStatus)Convert.ToInt32(shop.Status);
			ServiceHelper.Create<IShopService>().UpdateShop(companyName);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("修改店铺信息，Id=", shop.Id),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = string.Concat("/Shop/Edit/", shop.Id),
				UserName = base.CurrentManager.UserName,
				ShopId = 0
            };
			operationLogService.AddPlatformOperationLog(logInfo);
			Result result1 = new Result()
			{
				success = true,
				msg = "保存成功！"
			};
			return Json(result1);
		}

		[HttpGet]
		[UnAuthorize]
		public ActionResult Edit(long id = 0L)
		{
			if (id == 0)
			{
				return View(new ShopModel());
			}
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(id, false);
			IQueryable<ShopGradeInfo> shopGrades = ServiceHelper.Create<IShopService>().GetShopGrades();
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			foreach (ShopGradeInfo shopGrade in shopGrades)
			{
				SelectListItem selectListItem = new SelectListItem()
				{
					Selected = shopGrade.Id == shop.GradeId,
					Text = shopGrade.Name,
					Value = shopGrade.Id.ToString()
				};
				selectListItems.Add(selectListItem);
			}
			ViewBag.ShopGrade = selectListItems;
			ViewBag.Status = shop.ShopStatus;
			ViewBag.BankRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.BankRegionId);
			ViewBag.CompanyRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.CompanyRegionId);
			return View(new ShopModel(shop));
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult GetCategoryCommisRate(long id)
		{
			decimal commisRate = ServiceHelper.Create<ICategoryService>().GetCategory(id).CommisRate;
			return Json(new { successful = true, CommisRate = commisRate }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(long? shopGradeId, int? shopStatus, int page, int rows, string shopName, string shopAccount, string type = "")
		{
			ShopInfo.ShopAuditStatus? nullable;
			ShopQuery shopQuery = new ShopQuery();
			ShopQuery shopQuery1 = shopQuery;
			int? nullable1 = shopStatus;
			if (nullable1.HasValue)
			{
				nullable = new ShopInfo.ShopAuditStatus?((ShopInfo.ShopAuditStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			shopQuery1.Status = nullable;
			shopQuery.PageSize = rows;
			shopQuery.PageNo = page;
			shopQuery.ShopAccount = shopAccount;
			shopQuery.ShopName = shopName;
			shopQuery.ShopGradeId = shopGradeId;
			PageModel<ShopInfo> shops = ServiceHelper.Create<IShopService>().GetShops(shopQuery);
			ServiceHelper.Create<ICategoryService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			IEnumerable<ShopModel> array = 
				from item in shops.Models.ToArray()
				select new ShopModel()
				{
					Id = item.Id,
					Account = item.ShopAccount,
					EndDate = (type == "Auditing" ? "--" : (item.EndDate.HasValue ? item.EndDate.Value.ToString("yyyy-MM-dd") : "")),
					Name = item.ShopName,
					ShopGrade = (shopService.GetShopGrade(item.GradeId) == null ? "" : shopService.GetShopGrade(item.GradeId).Name),
					Status = item.ShowShopAuditStatus.ToDescription(),
					IsSelf = item.IsSelf
				};
			DataGridModel<ShopModel> dataGridModel = new DataGridModel<ShopModel>()
			{
				rows = array,
				total = shops.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management(string type = "")
		{
			IEnumerable<SelectListItem> selectListItems;
			SelectList selectList = ShopInfo.ShopAuditStatus.Open.ToSelectList<ShopInfo.ShopAuditStatus>(true, false);
			dynamic viewBag = base.ViewBag;
            if (type == "Auditing")
            {
                selectListItems = (type == "Auditing" ? selectList.Where((SelectListItem c) =>
                {
                    if (c.Value == "2" || c.Value == "3")
                    {
                        return true;
                    }
                    return c.Value == "5";
                }) :
                    from c in selectList
                    where c.Value != "6"
                    select c);
            }
            else
            {

                selectListItems = (type == "Paying" ? selectList.Where((SelectListItem c) =>
                {
                    return c.Value == "5";
                }) :
                    from c in selectList
                    where c.Value != "6"
                    select c);
            }
			viewBag.Status = selectListItems;
			IQueryable<ShopGradeInfo> shopGrades = ServiceHelper.Create<IShopService>().GetShopGrades();
			List<SelectListItem> selectListItems1 = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = true,
				Value = 0.ToString(),
				Text = "请选择..."
			};
			selectListItems1.Add(selectListItem);
			List<SelectListItem> selectListItems2 = selectListItems1;
			foreach (ShopGradeInfo shopGrade in shopGrades)
			{
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Selected = false,
					Value = shopGrade.Id.ToString(),
					Text = shopGrade.Name
				};
				selectListItems2.Add(selectListItem1);
			}
			ViewBag.Type = type;
			ViewBag.Grade = selectListItems2;
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveBusinessCategory(long shopId = 0L, string bcategory = "")
		{
			Dictionary<long, decimal> nums = new Dictionary<long, decimal>();
			string[] strArrays = bcategory.Split(new char[] { ',' });
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrWhiteSpace(str))
				{
					char[] chrArray = new char[] { '|' };
					if (!nums.ContainsKey(int.Parse(str.Split(chrArray)[0])))
					{
						char[] chrArray1 = new char[] { '|' };
						long num = int.Parse(str.Split(chrArray1)[0]);
						char[] chrArray2 = new char[] { '|' };
						nums.Add(num, decimal.Parse(str.Split(chrArray2)[1]));
					}
				}
			}
			ServiceHelper.Create<IShopService>().SaveBusinessCategory(shopId, nums);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("修改店铺经营类目，店铺Id=", shopId),
				IPAddress = base.Request.UserHostAddress
			};
			object[] objArray = new object[] { "/Shop/SaveBusinessCategory?shopId=", shopId, "&bcategory=", bcategory };
			logInfo.PageUrl = string.Concat(objArray);
			logInfo.UserName = base.CurrentManager.UserName;
			logInfo.ShopId = 0;
			operationLogService.AddPlatformOperationLog(logInfo);
			return Json(new { Successful = true });
		}

		[HttpPost]
		public JsonResult UpdateShopCommisRate(long businessCategoryId, decimal commisRate)
		{
			ServiceHelper.Create<IShopService>().SaveBusinessCategory(businessCategoryId, commisRate);
			return Json(new { success = true });
		}
	}
}