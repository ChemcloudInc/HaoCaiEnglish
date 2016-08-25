using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
    public class LimitTimeBuyController : BaseSellerController
    {
        public LimitTimeBuyController()
        {
        }

        [HttpGet]
        public ActionResult AddLimitItem()
        {
            LimitTimeMarketModel limitTimeMarketModel = null;
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            string[] serviceCategories = ServiceHelper.Create<ILimitTimeBuyService>().GetServiceCategories();
            for (int i = 0; i < serviceCategories.Length; i++)
            {
                string str = serviceCategories[i];
                SelectListItem selectListItem = new SelectListItem()
                {
                    Selected = false,
                    Text = str,
                    Value = str
                };
                selectListItems.Add(selectListItem);
            }
            ViewBag.Cate = selectListItems;
            dynamic viewBag = base.ViewBag;
            DateTime dateTime = ServiceHelper.Create<ILimitTimeBuyService>().GetMarketService(base.CurrentSellerManager.ShopId).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo m) => m.EndTime);
            viewBag.EndTime = dateTime.ToString("yyyy-MM-dd");
            return View(limitTimeMarketModel);
        }

        [HttpPost]
        public JsonResult AddLimitItem(string Title, string ProductName, long ProductId, decimal Price, string CategoryName, DateTime StartTime, DateTime EndTime, int MaxSaleCount, int Stock = 0)
        {
            Result result = new Result();
            try
            {
                decimal recentMonthAveragePrice = ServiceHelper.Create<IOrderService>().GetRecentMonthAveragePrice(base.CurrentSellerManager.ShopId, ProductId);
                ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(ProductId);
                LimitTimeMarketInfo limitTimeMarketInfo = new LimitTimeMarketInfo()
                {
                    AuditStatus = LimitTimeMarketInfo.LimitTimeMarketAuditStatus.WaitForAuditing,
                    Title = Title,
                    ProductId = ProductId,
                    ProductName = ProductName,
                    CancelReson = "",
                    CategoryName = CategoryName,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    MaxSaleCount = MaxSaleCount,
                    Price = Price,
                    SaleCount = 0,
                    ShopId = base.CurrentSellerManager.ShopId,
                    ShopName = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false).ShopName,
                    Stock = Stock,
                    AuditTime = StartTime,
                    RecentMonthPrice = recentMonthAveragePrice,
                    ImagePath = product.ImagePath,
                    ProductAd = string.IsNullOrWhiteSpace(product.ShortDescription) ? "" : product.ShortDescription,
                    MinPrice = product.MinSalePrice
                };
                ServiceHelper.Create<ILimitTimeBuyService>().AddLimitTimeItem(limitTimeMarketInfo);
                result.success = true;
                result.msg = "添加限时购成功";
            }
            catch (HimallException himallException)
            {
                result.msg = himallException.Message;
            }
            catch (DbEntityValidationException exception)
            {
                Log.Error("添加限时购出错", exception);
                result.msg = "添加限时购出错！";
            }
            return Json(result);
        }

        public ActionResult BuyService()
        {
            ActiveMarketServiceInfo marketService = ServiceHelper.Create<ILimitTimeBuyService>().GetMarketService(base.CurrentSellerManager.ShopId);
            ((dynamic)base.ViewBag).Market = marketService;
            string str = null;
            if ((marketService != null) && (marketService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(m => m.EndTime) < DateTime.Now))
            {
                str = "您的限时购服务已经过期，您可以续费。";
            }
            else if ((marketService != null) && (marketService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(m => m.EndTime) > DateTime.Now))
            {
                DateTime time = marketService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(m => m.EndTime);
                str = string.Format("{0} 年 {1} 月 {2} 日", time.Year, time.Month, time.Day);
            }
            ViewBag.EndDate = str;
            ViewBag.Price = ServiceHelper.Create<ILimitTimeBuyService>().GetServiceSetting().Price;
            return View();
        }

        [HttpPost]
        [UnAuthorize]
        public JsonResult BuyService(int month)
        {
            Result result = new Result();
            try
            {
                IMarketService marketService = ServiceHelper.Create<IMarketService>();
                marketService.OrderMarketService(month, base.CurrentSellerManager.ShopId, MarketType.LimitTimeBuy);
                result.success = true;
                result.msg = "购买服务成功";
            }
            catch (HimallException himallException)
            {
                result.msg = himallException.Message;
            }
            catch (Exception exception)
            {
                Log.Error("取消出错", exception);
                result.msg = "取消出错！";
            }
            return Json(result);
        }

        public ActionResult EditLimitItem(long id)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            string[] serviceCategories = ServiceHelper.Create<ILimitTimeBuyService>().GetServiceCategories();
            for (int i = 0; i < serviceCategories.Length; i++)
            {
                string str = serviceCategories[i];
                SelectListItem selectListItem = new SelectListItem()
                {
                    Selected = false,
                    Text = str,
                    Value = str
                };
                selectListItems.Add(selectListItem);
            }
            LimitTimeMarketInfo limitTimeMarketItem = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItem(id);
            selectListItems.FirstOrDefault((SelectListItem c) => c.Text.Equals(limitTimeMarketItem.CategoryName)).Selected = true;
            ViewBag.Cate = selectListItems;
            LimitTimeMarketModel limitTimeMarketModel = new LimitTimeMarketModel()
            {
                Title = limitTimeMarketItem.Title,
                CategoryName = limitTimeMarketItem.CategoryName,
                StartTime = limitTimeMarketItem.StartTime.ToString("yyyy-MM-dd HH:mm"),
                EndTime = limitTimeMarketItem.EndTime.ToString("yyyy-MM-dd HH:mm"),
                ProductId = limitTimeMarketItem.ProductId,
                MaxSaleCount = limitTimeMarketItem.MaxSaleCount,
                ProductName = limitTimeMarketItem.ProductName,
                ProductPrice = ServiceHelper.Create<IProductService>().GetProduct(limitTimeMarketItem.ProductId).MinSalePrice,
                AuditStatus = limitTimeMarketItem.AuditStatus.ToDescription(),
                CancelReson = limitTimeMarketItem.CancelReson,
                Price = limitTimeMarketItem.Price,
                Stock = limitTimeMarketItem.Stock
            };
            return View(limitTimeMarketModel);
        }

        [HttpPost]
        public JsonResult EditLimitItem(string Title, string ProductName, long ProductId, decimal Price, string CategoryName, DateTime StartTime, DateTime EndTime, int Stock, int MaxSaleCount, long Id)
        {
            Result result = new Result();
            try
            {
                LimitTimeMarketInfo limitTimeMarketItem = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItem(Id);
                limitTimeMarketItem.Title = Title;
                limitTimeMarketItem.ProductId = ProductId;
                limitTimeMarketItem.ProductName = ProductName;
                limitTimeMarketItem.CategoryName = CategoryName;
                limitTimeMarketItem.StartTime = StartTime;
                limitTimeMarketItem.EndTime = EndTime;
                limitTimeMarketItem.Price = Price;
                limitTimeMarketItem.Stock = Stock;
                limitTimeMarketItem.MaxSaleCount = MaxSaleCount;
                ServiceHelper.Create<ILimitTimeBuyService>().UpdateLimitTimeItem(limitTimeMarketItem);
                result.success = true;
                result.msg = "修改限时购成功";
            }
            catch (HimallException himallException)
            {
                result.msg = himallException.Message;
            }
            catch (Exception exception)
            {
                Log.Error("修改限时购出错", exception);
                result.msg = "修改限时购出错！";
            }
            return Json(result);
        }

        [HttpPost]
        [UnAuthorize]
        public JsonResult GetItemList(int page, int rows)
        {
            ILimitTimeBuyService limitTimeBuyService = ServiceHelper.Create<ILimitTimeBuyService>();
            LimitTimeQuery limitTimeQuery = new LimitTimeQuery()
            {
                ShopId = new long?(base.CurrentSellerManager.ShopId),
                PageSize = rows,
                PageNo = page
            };
            PageModel<LimitTimeMarketInfo> itemList = limitTimeBuyService.GetItemList(limitTimeQuery);
            List<LimitTimeMarketModel> limitTimeMarketModels = new List<LimitTimeMarketModel>();
            foreach (LimitTimeMarketInfo model in itemList.Models)
            {
                if (model.EndTime < DateTime.Now)
                {
                    model.AuditStatus = LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ended;
                }
                LimitTimeMarketModel limitTimeMarketModel = new LimitTimeMarketModel()
                {
                    Id = model.Id,
                    StartTime = model.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    EndTime = model.EndTime.ToString("yyyy-MM-dd HH:mm"),
                    ProductId = model.ProductId,
                    SaleCount = model.SaleCount,
                    ProductName = model.ProductName,
                    AuditStatusNum = (int)model.AuditStatus,
                    AuditStatus = model.AuditStatus.ToDescription(),
                    CancelReson = model.CancelReson,
                    MaxSaleCount = model.MaxSaleCount
                };
                limitTimeMarketModels.Add(limitTimeMarketModel);
            }
            return Json(new { rows = limitTimeMarketModels, total = itemList.Total });
        }

        public ActionResult Management()
        {
            if (ServiceHelper.Create<ILimitTimeBuyService>().GetServiceSetting() == null)
            {
                return View("Nosetting");
            }
            ViewBag.Market = ServiceHelper.Create<ILimitTimeBuyService>().GetMarketService(base.CurrentSellerManager.ShopId);
            return View();
        }
    }
}