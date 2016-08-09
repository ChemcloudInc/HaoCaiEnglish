using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class LimitTimeBuyController : BaseAdminController
	{
		public LimitTimeBuyController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult AddMarketCategory(string name)
		{
			Result result = new Result();
			try
			{
				ILimitTimeBuyService limitTimeBuyService = ServiceHelper.Create<ILimitTimeBuyService>();
				limitTimeBuyService.AddServiceCategory(name.Replace(",", "").Replace("，", ""));
				result.success = true;
				result.msg = "添加分类成功！";
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("添加分类出错", exception);
				result.msg = "添加分类出错！";
			}
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult AddSlideAd(string pic, string url)
		{
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				ImageUrl = pic,
				Url = url,
				ShopId = 0,
				DisplaySequence = 0,
				TypeId = SlideAdInfo.SlideAdType.PlatformLimitTime
			};
			SlideAdInfo slideAdInfo1 = slideAdInfo;
			if (!string.IsNullOrWhiteSpace(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = "/Storage/Plat/ImageAd/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				slideAdInfo1.ImageUrl = Path.Combine(str1, Path.GetFileName(str));
			}
			ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo1);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult AdjustSlideIndex(long id, int direction)
		{
			ServiceHelper.Create<ISlideAdsService>().AdjustSlidAdIndex(0, id, direction == 1, SlideAdInfo.SlideAdType.PlatformLimitTime);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[OperationLog(Message="审核商品状态")]
		[UnAuthorize]
		public JsonResult AuditItem(string Id, int auditState, string message)
		{
			Result result = new Result();
			try
			{
				LimitTimeMarketInfo.LimitTimeMarketAuditStatus limitTimeMarketAuditStatu = (LimitTimeMarketInfo.LimitTimeMarketAuditStatus)((short)auditState);
				ServiceHelper.Create<ILimitTimeBuyService>().AuditItem(long.Parse(Id), limitTimeMarketAuditStatu, message);
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

		public ActionResult BoughtList()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult CancelItem(long id)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<ILimitTimeBuyService>().AuditItem(id, LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Cancelled, "已被管理员取消");
				result.success = true;
				result.msg = "取消成功！";
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

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteMarketCategory(string name)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<ILimitTimeBuyService>().DeleteServiceCategory(name);
				result.success = true;
				result.msg = "删除分类成功！";
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("删除分类出错", exception);
				result.msg = "删除分类出错！";
			}
			return Json(result);
		}

		[UnAuthorize]
		public JsonResult DeleteSlide(long Id)
		{
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(0, Id);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult EditSlideAd(long id, string pic, string url)
		{
			SlideAdInfo slidAd = ServiceHelper.Create<ISlideAdsService>().GetSlidAd(0, id);
			if (!string.IsNullOrWhiteSpace(pic) && !slidAd.ImageUrl.Equals(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = "/Storage/Plat/ImageAd/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				pic = Path.Combine(str1, Path.GetFileName(str));
			}
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				Id = id,
				ImageUrl = pic,
				Url = url
			};
			slideAdsService.UpdateSlidAd(slideAdInfo);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetBoughtJson(string shopName, int page, int rows)
		{
			MarketBoughtQuery marketBoughtQuery = new MarketBoughtQuery()
			{
				PageSize = rows,
				PageNo = page,
				ShopName = shopName,
				MarketType = new MarketType?(MarketType.LimitTimeBuy)
			};
			PageModel<MarketServiceRecordInfo> boughtShopList = ServiceHelper.Create<IMarketService>().GetBoughtShopList(marketBoughtQuery);
			var array = 
				from item in (
                    from m in boughtShopList.Models
                    orderby m.MarketServiceId descending, m.EndTime descending
                    select m).ToArray()
                select new { Id = item.Id, StartDate = item.StartTime.ToString("yyyy-MM-dd"), EndDate = item.EndTime.ToString("yyyy-MM-dd"), ShopName = item.ActiveMarketServiceInfo.ShopName };
			return Json(new { rows = array, total = boughtShopList.Total });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetMarketCategoryJson()
		{
			var serviceCategories = 
				from i in ServiceHelper.Create<ILimitTimeBuyService>().GetServiceCategories()
				select new { Name = i, Id = 0 };
			return Json(new { rows = serviceCategories, total = serviceCategories.Count() });
		}

		public JsonResult GetSlideJson()
		{
			IQueryable<SlideAdInfo> slidAds = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.PlatformLimitTime);
			IEnumerable<HandSlideModel> array = 
				from item in slidAds.ToArray()
                select new HandSlideModel()
				{
					Id = item.Id,
					Pic = item.ImageUrl,
					URL = item.Url,
					Index = item.DisplaySequence
				};
			DataGridModel<HandSlideModel> dataGridModel = new DataGridModel<HandSlideModel>()
			{
				rows = array,
				total = array.Count()
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int? AuditStatus, int page, int rows, string shopName, string productName)
		{
			LimitTimeMarketInfo.LimitTimeMarketAuditStatus? nullable;
			LimitTimeQuery limitTimeQuery = new LimitTimeQuery();
			LimitTimeQuery limitTimeQuery1 = limitTimeQuery;
			int? auditStatus = AuditStatus;
			if ((auditStatus.GetValueOrDefault() != 0 ? false : auditStatus.HasValue))
			{
				nullable = null;
			}
			else
			{
				int? auditStatus1 = AuditStatus;
				if (auditStatus1.HasValue)
				{
					nullable = new LimitTimeMarketInfo.LimitTimeMarketAuditStatus?((LimitTimeMarketInfo.LimitTimeMarketAuditStatus)((short)auditStatus1.GetValueOrDefault()));
				}
				else
				{
					nullable = null;
				}
			}
			limitTimeQuery1.AuditStatus = nullable;
			limitTimeQuery.PageSize = rows;
			limitTimeQuery.PageNo = page;
			limitTimeQuery.ShopName = shopName;
			limitTimeQuery.ItemName = productName;
			PageModel<LimitTimeMarketInfo> itemList = ServiceHelper.Create<ILimitTimeBuyService>().GetItemList(limitTimeQuery);
			IEnumerable<LimitTimeBuyModel> array = 
				from item in itemList.Models.ToArray()
                select new LimitTimeBuyModel()
				{
					Id = item.Id,
					StartDate = item.StartTime.ToString("yyyy-MM-dd"),
					EndDate = item.EndTime.ToString("yyyy-MM-dd"),
					ShopName = item.ShopName,
					ProductName = item.ProductName,
					ProductId = item.ProductId.ToString(),
					Status = (item.EndTime < DateTime.Now ? LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ended.ToDescription() : item.AuditStatus.ToDescription()),
					Price = item.Price.ToString("f2"),
					RecentMonthPrice = item.RecentMonthPrice.ToString("f2"),
					SaleCount = item.SaleCount.ToString()
				};
			DataGridModel<LimitTimeBuyModel> dataGridModel = new DataGridModel<LimitTimeBuyModel>()
			{
				rows = array,
				total = itemList.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			ViewBag.Status = LimitTimeMarketInfo.LimitTimeMarketAuditStatus.AuditFailed.ToSelectList<LimitTimeMarketInfo.LimitTimeMarketAuditStatus>(true, false);
			return View();
		}

		public ActionResult MarketCategory()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveServiceSetting(decimal Price, int ReviceDays = 0)
		{
			Result result = new Result();
			try
			{
				LimitTimeBuySettingModel limitTimeBuySettingModel = new LimitTimeBuySettingModel()
				{
					Price = Price,
					ReviceDays = ReviceDays
				};
				ServiceHelper.Create<ILimitTimeBuyService>().UpdateServiceSetting(limitTimeBuySettingModel);
				result.success = true;
				result.msg = "保存成功！";
			}
			catch (HimallException himallException)
			{
				result.msg = himallException.Message;
			}
			catch (Exception exception)
			{
				Log.Error("保存出错", exception);
				result.msg = "保存出错！";
			}
			return Json(result);
		}

		public ActionResult ServiceSetting()
		{
			return View(ServiceHelper.Create<ILimitTimeBuyService>().GetServiceSetting());
		}

		public ActionResult SetSlide()
		{
			return View();
		}
	}
}