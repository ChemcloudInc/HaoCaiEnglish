using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class CollocationController : BaseSellerController
	{
		private IMarketService service;

		private MarketSettingInfo settings;

		public CollocationController()
		{
            service = ServiceHelper.Create<IMarketService>();
            settings = service.GetServiceSetting(MarketType.Collocation);
		}

		public ActionResult Add()
		{
			dynamic viewBag = base.ViewBag;
			DateTime dateTime = ServiceHelper.Create<IMarketService>().GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Collocation).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
			viewBag.EndTime = dateTime.ToString("yyyy-MM-dd");
			return View();
		}

		[HttpPost]
		public ActionResult AddCollocation(string collocationjson)
		{
			CollocationDataModel collocationDataModel = JsonConvert.DeserializeObject<CollocationDataModel>(collocationjson);
			if (collocationDataModel == null)
			{
				throw new HimallException("添加组合购参数错误！");
			}
			CollocationInfo collocationInfo = new CollocationInfo()
			{
				CreateTime = new DateTime?(DateTime.Now),
				EndTime = collocationDataModel.EndTime,
				ShopId = base.CurrentSellerManager.ShopId,
				ShortDesc = collocationDataModel.ShortDesc,
				Title = collocationDataModel.Title,
				StartTime = collocationDataModel.StartTime,
				Id = collocationDataModel.Id,
				Himall_CollocationPoruducts = (
					from a in collocationDataModel.CollocationPoruducts
					select new CollocationPoruductInfo()
					{
						Id = a.Id,
						ColloId = a.ColloId,
						DisplaySequence = new int?(a.DisplaySequence),
						IsMain = a.IsMain,
						ProductId = a.ProductId,
						Himall_CollocationSkus = (
							from b in a.CollocationSkus
							select new CollocationSkuInfo()
							{
								Id = b.Id,
								Price = b.Price,
								SkuID = b.SkuID,
								SkuPirce = new decimal?(b.SkuPirce),
								ColloProductId = b.ColloProductId,
								ProductId = b.ProductId
							}).ToArray()
					}).ToArray()
			};
			ServiceHelper.Create<ICollocationService>().AddCollocation(collocationInfo);
			Result result = new Result()
			{
				success = true,
				msg = "添加成功！"
			};
			return Json(result);
		}

		public ActionResult BuyService()
		{
			if (settings == null)
			{
				return View("Nosetting");
			}
            SetExpire();
			return View(settings);
		}

		[HttpPost]
		public JsonResult BuyService(int month)
		{
			Result result = new Result();
			IMarketService marketService = ServiceHelper.Create<IMarketService>();
			marketService.OrderMarketService(month, base.CurrentSellerManager.ShopId, MarketType.Collocation);
			result.success = true;
			result.msg = "购买服务成功";
			return Json(result);
		}

		[HttpPost]
		public JsonResult Cancel(long Id)
		{
			long shopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<ICollocationService>().CancelCollocation(Id, shopId);
			Result result = new Result()
			{
				success = true,
				msg = "操作成功！"
			};
			return Json(result);
		}

		public ActionResult Edit(long id)
		{
			CollocationInfo collocation = ServiceHelper.Create<ICollocationService>().GetCollocation(id);
			if (collocation.ShopId != base.CurrentSellerManager.ShopId)
			{
				RedirectToAction("Management");
			}
			CollocationDataModel collocationDataModel = new CollocationDataModel()
			{
				CreateTime = collocation.CreateTime.Value,
				EndTime = collocation.EndTime,
				ShopId = collocation.ShopId,
				ShortDesc = collocation.ShortDesc,
				Title = collocation.Title,
				StartTime = collocation.StartTime,
				Id = collocation.Id,
				CollocationPoruducts = collocation.Himall_CollocationPoruducts.Select<CollocationPoruductInfo, CollocationPoruductModel>((CollocationPoruductInfo a) => {
					List<SKUInfo> list = a.Himall_Products.SKUInfo.ToList();
					return new CollocationPoruductModel()
					{
						Id = a.Id,
						ColloId = a.ColloId,
						DisplaySequence = a.DisplaySequence.Value,
						IsMain = a.IsMain,
						ProductId = a.ProductId,
						ProductName = a.Himall_Products.ProductName,
						ImagePath = a.Himall_Products.ImagePath,
						CollocationSkus = a.Himall_CollocationSkus.Select<CollocationSkuInfo, CollocationSkus>((CollocationSkuInfo b) => {
							SKUInfo sKUInfo = list.FirstOrDefault((SKUInfo t) => t.Id == b.SkuID);
							return new CollocationSkus()
							{
								Id = b.Id,
								Price = b.Price,
								SkuID = b.SkuID,
								SKUName = string.Concat(new string[] { sKUInfo.Color, " ", sKUInfo.Size, " ", sKUInfo.Version }),
								SkuPirce = b.SkuPirce.Value,
								ColloProductId = b.ColloProductId,
								ProductId = b.ProductId
							};
						}).ToList()
					};
				}).OrderBy<CollocationPoruductModel, int>((CollocationPoruductModel a) => a.DisplaySequence).ToList()
			};
			dynamic viewBag = base.ViewBag;
			DateTime dateTime = ServiceHelper.Create<IMarketService>().GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Collocation).MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
			viewBag.EndTime = dateTime.ToString("yyyy-MM-dd");
			return View(collocationDataModel);
		}

		[HttpPost]
		public ActionResult EditCollocation(string collocationjson)
		{
			CollocationDataModel collocationDataModel = JsonConvert.DeserializeObject<CollocationDataModel>(collocationjson);
			if (collocationDataModel == null)
			{
				throw new HimallException("组合购参数错误！");
			}
			CollocationInfo collocationInfo = new CollocationInfo()
			{
				EndTime = collocationDataModel.EndTime,
				ShopId = base.CurrentSellerManager.ShopId,
				ShortDesc = collocationDataModel.ShortDesc,
				Title = collocationDataModel.Title,
				StartTime = collocationDataModel.StartTime,
				Id = collocationDataModel.Id,
				Himall_CollocationPoruducts = (
					from a in collocationDataModel.CollocationPoruducts
					select new CollocationPoruductInfo()
					{
						Id = a.Id,
						ColloId = a.ColloId,
						DisplaySequence = new int?(a.DisplaySequence),
						IsMain = a.IsMain,
						ProductId = a.ProductId,
						Himall_CollocationSkus = (
							from b in a.CollocationSkus
							select new CollocationSkuInfo()
							{
								Id = b.Id,
								Price = b.Price,
								SkuID = b.SkuID,
								SkuPirce = new decimal?(b.SkuPirce),
								ColloProductId = b.ColloProductId,
								ProductId = b.ProductId
							}).ToArray()
					}).ToArray()
			};
			base.UpdateModel<CollocationInfo>(collocationInfo);
			ServiceHelper.Create<ICollocationService>().EditCollocation(collocationInfo);
			Result result = new Result()
			{
				success = true,
				msg = "修改成功！"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult GetProductSKU(long productId)
		{
			List<SKUInfo> list = ServiceHelper.Create<IProductService>().GetSKUs(productId).ToList();
			ProductSkuModel productSkuModel = new ProductSkuModel()
			{
				productId = productId,
				SKUs = (
					from a in list
					select new SKUModel()
					{
						Id = a.Id,
						SalePrice = a.SalePrice,
						Size = a.Size,
						Stock = a.Stock,
						Version = a.Version,
						Color = a.Color,
						Sku = a.Sku,
						AutoId = a.AutoId,
						ProductId = a.ProductId
					}).ToList()
			};
			return Json(productSkuModel);
		}

		public JsonResult GetProductsSku(string productIds)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from a in productIds.Split(chrArray)
				select long.Parse(a);
			IQueryable<SKUInfo> sKUs = ServiceHelper.Create<IProductService>().GetSKUs(nums);
			IQueryable<IGrouping<long, SKUInfo>> productId = 
				from a in sKUs
				group a by a.ProductId;
			List<ProductSkuModel> productSkuModels = new List<ProductSkuModel>();
			foreach (IGrouping<long, SKUInfo> nums1 in productId)
			{
				ProductSkuModel productSkuModel = new ProductSkuModel()
				{
					productId = nums1.Key,
					SKUs = (
						from a in nums1
						select new SKUModel()
						{
							Id = a.Id,
							SalePrice = a.SalePrice,
							Size = a.Size,
							Stock = a.Stock,
							Version = a.Version,
							Color = a.Color,
							Sku = a.Sku,
							AutoId = a.AutoId,
							ProductId = a.ProductId
						}).ToList()
				};
				productSkuModels.Add(productSkuModel);
			}
			return Json(productSkuModels);
		}

		[HttpPost]
		public JsonResult List(int page, int rows, string collName)
		{
			ICollocationService collocationService = ServiceHelper.Create<ICollocationService>();
			CollocationQuery collocationQuery = new CollocationQuery()
			{
				Title = collName,
				ShopId = new long?(base.CurrentSellerManager.ShopId),
				PageSize = rows,
				PageNo = page
			};
			PageModel<CollocationInfo> collocationList = collocationService.GetCollocationList(collocationQuery);
			var list = 
				from item in collocationList.Models.ToList()
				select new { Id = item.Id, StartTime = item.StartTime.ToString("yyyy/MM/dd"), EndTime = item.EndTime.ToString("yyyy/MM/dd"), Title = item.Title, ShopName = item.ShopName, ProductId = item.ProductId, Status = item.Status };
			return Json(new { rows = list, total = collocationList.Total });
		}

		public ActionResult Management()
		{
			if (settings == null)
			{
				return View("Nosetting");
			}
            SetExpire();
			return View();
		}

		private void SetExpire()
		{
			DateTime date = DateTime.Now.Date;
			ActiveMarketServiceInfo marketService = service.GetMarketService(base.CurrentSellerManager.ShopId, MarketType.Collocation);
			bool flag = false;
			if (marketService != null)
			{
				ViewBag.IsBuy = true;
				DateTime dateTime = marketService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
				if (dateTime <= date)
				{
					flag = true;
                    ViewBag.EndDateInfo = "Your coupon service has expired, you can renew.";
				}
				else
				{
					ViewBag.EndDateInfo = dateTime.ToString("yyyy.MM.dd");
				}
			}
			else
			{
				ViewBag.IsBuy = false;
				flag = true;
			}
			ViewBag.Expire = flag;
		}
	}
}