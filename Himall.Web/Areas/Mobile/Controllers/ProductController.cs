using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Helper;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class ProductController : BaseMobileTemplatesController
	{
		public ProductController()
		{
		}

		[HttpPost]
		public JsonResult AddFavoriteProduct(long pid)
		{
			int num = 0;
			ServiceHelper.Create<IProductService>().AddFavorite(pid, base.CurrentUser.Id, out num);
			Result result = new Result()
			{
				success = true,
				msg = "成功关注"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult DeleteFavoriteProduct(long pid)
		{
			ServiceHelper.Create<IProductService>().DeleteFavorite(pid, base.CurrentUser.Id);
			Result result = new Result()
			{
				success = true,
				msg = "已取消关注"
			};
			return Json(result);
		}

		public ActionResult Detail(string id = "")
		{
			decimal num;
			decimal num1;
			string str = "";
			ProductDetailModelForWeb productDetailModelForWeb = new ProductDetailModelForWeb()
			{
				Product = new ProductInfo(),
				Shop = new ShopInfoModel(),
				Color = new CollectionSKU(),
				Size = new CollectionSKU(),
				Version = new CollectionSKU()
			};
			ProductDetailModelForWeb showMobileDescription = productDetailModelForWeb;
			ProductInfo product = null;
			ShopInfo shop = null;
			long num2 = 0;
			long.TryParse(id, out num2);
			if (num2 == 0)
			{
				return RedirectToAction("Error404", "Error", new { area = "Web" });
			}
			product = ServiceHelper.Create<IProductService>().GetProduct(num2);
			showMobileDescription.ProductDescription = product.ProductDescriptionInfo.ShowMobileDescription;
			if (product == null)
			{
				return RedirectToAction("Error404", "Error", new { area = "Web" });
			}
			LimitTimeMarketInfo limitTimeMarketItemByProductId = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(product.Id);
			if (limitTimeMarketItemByProductId != null)
			{
				return RedirectToAction("Detail", "LimitTimeBuy", new { id = limitTimeMarketItemByProductId.Id });
			}
			shop = ServiceHelper.Create<IShopService>().GetShop(product.ShopId, false);
			ShopServiceMarkModel shopComprehensiveMark = ShopServiceMark.GetShopComprehensiveMark(shop.Id);
			showMobileDescription.Shop.PackMark = shopComprehensiveMark.PackMark;
			showMobileDescription.Shop.ServiceMark = shopComprehensiveMark.ServiceMark;
			showMobileDescription.Shop.ComprehensiveMark = shopComprehensiveMark.ComprehensiveMark;
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(num2);
			showMobileDescription.Shop.Name = shop.ShopName;
			showMobileDescription.Shop.ProductMark = (commentsByProductId == null || commentsByProductId.Count() == 0 ? new decimal(0) : commentsByProductId.Average<ProductCommentInfo>((ProductCommentInfo p) => (decimal)p.ReviewMark));
			showMobileDescription.Shop.Id = product.ShopId;
			showMobileDescription.Shop.FreeFreight = shop.FreeFreight;
			ViewBag.ProductNum = ServiceHelper.Create<IProductService>().GetShopOnsaleProducts(product.ShopId);
			if (base.CurrentUser != null)
			{
				ViewBag.IsFavorite = ServiceHelper.Create<IProductService>().IsFavorite(product.Id, base.CurrentUser.Id);
			}
			else
			{
				ViewBag.IsFavorite = false;
			}
			if (product.SKUInfo != null && product.SKUInfo.Count() > 0)
			{
				long num3 = 0;
				long num4 = 0;
				long num5 = 0;
				foreach (SKUInfo sKUInfo in product.SKUInfo)
				{
					string[] strArrays = sKUInfo.Id.Split(new char[] { '\u005F' });
					if (strArrays.Count() > 0)
					{
						long.TryParse(strArrays[1], out num3);
						if (num3 != 0)
						{
							if (!showMobileDescription.Color.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Color)))
							{
								long num6 = (
									from s in product.SKUInfo
									where s.Color.Equals(sKUInfo.Color)
									select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
								CollectionSKU color = showMobileDescription.Color;
								ProductSKU productSKU = new ProductSKU()
								{
									Name = "选择颜色",
									EnabledClass = (num6 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num3,
									Value = sKUInfo.Color
								};
                                color.Add(productSKU);
							}
						}
					}
					if (strArrays.Count() > 1)
					{
						long.TryParse(strArrays[2], out num4);
						if (num4 != 0)
						{
							if (!showMobileDescription.Size.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Size)))
							{
								long num7 = (
									from s in product.SKUInfo
									where s.Size.Equals(sKUInfo.Size)
									select s).Sum<SKUInfo>((SKUInfo s1) => s1.Stock);
								CollectionSKU size = showMobileDescription.Size;
								ProductSKU productSKU1 = new ProductSKU()
								{
									Name = "选择尺码",
									EnabledClass = (num7 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num4,
									Value = sKUInfo.Size
								};
                                size.Add(productSKU1);
							}
						}
					}
					if (strArrays.Count() <= 2)
					{
						continue;
					}
					long.TryParse(strArrays[3], out num5);
					if (num5 == 0)
					{
						continue;
					}
					if (showMobileDescription.Version.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Version)))
					{
						continue;
					}
					long num8 = (
						from s in product.SKUInfo
						where s.Version.Equals(sKUInfo.Version)
						select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
					CollectionSKU version = showMobileDescription.Version;
					ProductSKU productSKU2 = new ProductSKU()
					{
						Name = "选择版本",
						EnabledClass = (num8 != 0 ? "enabled" : "disabled"),
						SelectedClass = "",
						SKUId = num5,
						Value = sKUInfo.Version
					};
                    version.Add(productSKU2);
				}
				decimal num9 = new decimal(0);
				decimal num10 = new decimal(0);
				num9 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Min<SKUInfo>((SKUInfo s) => s.SalePrice);
				num10 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Max<SKUInfo>((SKUInfo s) => s.SalePrice);
				if (!(num9 == new decimal(0)) || !(num10 == new decimal(0)))
				{
					str = (num10 <= num9 ? string.Format("{0}", num9.ToString("f2")) : string.Format("{0}-{1}", num9.ToString("f2"), num10.ToString("f2")));
				}
				else
				{
					str = product.MinSalePrice.ToString("f2");
				}
			}
			base.ViewBag.Price = (string.IsNullOrWhiteSpace(str) ? product.MinSalePrice.ToString("f2") : str);
			IQueryable<StatisticOrderCommentsInfo> shopStatisticOrderComments = ServiceHelper.Create<IShopService>().GetShopStatisticOrderComments(product.ShopId);
			StatisticOrderCommentsInfo statisticOrderCommentsInfo = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 1
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo1 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 9
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo2 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 5
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo3 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 2
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo4 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 10
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo5 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 6
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo6 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 3
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo7 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 4
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo8 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 11
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo9 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 12
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo10 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 7
				select c).FirstOrDefault();
			StatisticOrderCommentsInfo statisticOrderCommentsInfo11 = (
				from c in shopStatisticOrderComments
				where (int)c.CommentKey == 8
				select c).FirstOrDefault();
			int num11 = 5;
			if (statisticOrderCommentsInfo == null || statisticOrderCommentsInfo3 == null)
			{
				ViewBag.ProductAndDescription = num11;
				ViewBag.ProductAndDescriptionPeer = num11;
				ViewBag.ProductAndDescriptionMin = num11;
				ViewBag.ProductAndDescriptionMax = num11;
			}
			else
			{
				ViewBag.ProductAndDescription = statisticOrderCommentsInfo.CommentValue;
				ViewBag.ProductAndDescriptionPeer = statisticOrderCommentsInfo3.CommentValue;
				ViewBag.ProductAndDescriptionMin = statisticOrderCommentsInfo7.CommentValue;
				ViewBag.ProductAndDescriptionMax = statisticOrderCommentsInfo6.CommentValue;
			}
			if (statisticOrderCommentsInfo1 == null || statisticOrderCommentsInfo4 == null)
			{
				ViewBag.SellerServiceAttitude = num11;
				ViewBag.SellerServiceAttitudePeer = num11;
				ViewBag.SellerServiceAttitudeMax = num11;
				ViewBag.SellerServiceAttitudeMin = num11;
			}
			else
			{
				ViewBag.SellerServiceAttitude = statisticOrderCommentsInfo1.CommentValue;
				ViewBag.SellerServiceAttitudePeer = statisticOrderCommentsInfo4.CommentValue;
				ViewBag.SellerServiceAttitudeMax = statisticOrderCommentsInfo8.CommentValue;
				ViewBag.SellerServiceAttitudeMin = statisticOrderCommentsInfo9.CommentValue;
			}
			if (statisticOrderCommentsInfo5 == null || statisticOrderCommentsInfo2 == null)
			{
				ViewBag.SellerDeliverySpeed = num11;
				ViewBag.SellerDeliverySpeedPeer = num11;
				ViewBag.SellerDeliverySpeedMax = num11;
				ViewBag.sellerDeliverySpeedMin = num11;
			}
			else
			{
				ViewBag.SellerDeliverySpeed = statisticOrderCommentsInfo2.CommentValue;
				ViewBag.SellerDeliverySpeedPeer = statisticOrderCommentsInfo5.CommentValue;
				dynamic viewBag = base.ViewBag;
				num = (statisticOrderCommentsInfo10 != null ? statisticOrderCommentsInfo10.CommentValue : new decimal(0));
				viewBag.SellerDeliverySpeedMax = num;
				dynamic obj = base.ViewBag;
				num1 = (statisticOrderCommentsInfo11 != null ? statisticOrderCommentsInfo11.CommentValue : new decimal(0));
				obj.sellerDeliverySpeedMin = num1;
			}
			showMobileDescription.Product = product;
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			CommentQuery commentQuery = new CommentQuery()
			{
				ProductID = product.Id,
				PageNo = 1,
				PageSize = 10000
			};
			PageModel<ProductCommentInfo> comments = commentService.GetComments(commentQuery);
			ViewBag.CommentCount = comments.Total;
			IQueryable<ProductConsultationInfo> consultations = ServiceHelper.Create<IConsultationService>().GetConsultations(num2);
			double num12 = product.Himall_ProductComments.Count();
			double num13 = product.Himall_ProductComments.Count((ProductCommentInfo item) => item.ReviewMark >= 4);
			ViewBag.NicePercent = num13 / num12 * 100;
			ViewBag.Consultations = consultations.Count();
			if (ServiceHelper.Create<IVShopService>().GetVShopByShopId(shop.Id) != null)
			{
				ViewBag.VShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(shop.Id).Id;
			}
			else
			{
				ViewBag.VShopId = -1;
			}
			IEnumerable<CouponInfo> couponList = GetCouponList(shop.Id);
			if (couponList != null)
			{
				int num14 = couponList.Count();
				ViewBag.CouponCount = num14;
			}
			ShopBonusInfo byShopId = ServiceHelper.Create<IShopBonusService>().GetByShopId(shop.Id);
			if (byShopId != null)
			{
				ViewBag.BonusCount = byShopId.Count;
				ViewBag.BonusGrantPrice = byShopId.GrantPrice;
				ViewBag.BonusRandomAmountStart = byShopId.RandomAmountStart;
				ViewBag.BonusRandomAmountEnd = byShopId.RandomAmountEnd;
			}
			ViewBag.CashDepositsObligation = Instance<ICashDepositsService>.Create.GetCashDepositsObligation(product.Id);
			showMobileDescription.CashDepositsServer = Instance<ICashDepositsService>.Create.GetCashDepositsObligation(product.Id);
			return View(showMobileDescription);
		}

		public JsonResult GetCommentByProduct(long pId)
		{
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(pId);
			if (commentsByProductId == null || commentsByProductId.Count() <= 0)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}
			IEnumerable<ProductCommentInfo> array = (
				from c in commentsByProductId
				orderby c.ReviewMark descending
				select c).ToArray();
			ProductCommentInfo[] productCommentInfoArray = (
				from a in array
				orderby a.ReviewDate descending
				select a).Take(3).ToArray();
			var variable = 
				from c in productCommentInfoArray
                select new { UserName = c.UserName, ReviewContent = c.ReviewContent, ReviewDate = c.ReviewDate.ToString("yyyy-MM-dd HH:mm:ss"), ReplyContent = (string.IsNullOrWhiteSpace(c.ReplyContent) ? "暂无回复" : c.ReplyContent), ReplyDate = (c.ReplyDate.HasValue ? c.ReplyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : " "), ReviewMark = c.ReviewMark, BuyDate = "" };
			return Json(new { successful = true, comments = variable, goodComment = (
				from c in commentsByProductId.ToArray()
                where c.ReviewMark >= 4
				select c).Count(), badComment = (
				from c in commentsByProductId.ToArray()
                where c.ReviewMark == 1
				select c).Count(), comment = ((IEnumerable<ProductCommentInfo>)commentsByProductId.ToArray()).Where((ProductCommentInfo c) => {
				if (c.ReviewMark == 2)
				{
					return true;
				}
				return c.ReviewMark == 3;
			}).Count() }, JsonRequestBehavior.AllowGet);
		}

		private IEnumerable<CouponInfo> GetCouponList(long shopid)
		{
			IQueryable<CouponInfo> couponList = ServiceHelper.Create<ICouponService>().GetCouponList(shopid);
			IQueryable<long> vShopCouponSetting = 
				from item in ServiceHelper.Create<IVShopService>().GetVShopCouponSetting(shopid)
				select item.CouponID;
			if (couponList.Count() <= 0 || vShopCouponSetting.Count() <= 0)
			{
				return null;
			}
			IEnumerable<CouponInfo> array = 
				from item in couponList.ToArray()
                where vShopCouponSetting.Contains(item.Id)
				select item;
			return array;
		}

		public JsonResult GetProductComment(long pId, int pageNo, int commentType = 0, int pageSize = 10)
		{
			IEnumerable<ProductCommentInfo> array;
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(pId);
			switch (commentType)
			{
				case 1:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark >= 4
						select c;
					break;
				}
				case 2:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark == 3
						select c;
					break;
				}
				case 3:
				{
					array = 
						from c in (
                            from c in commentsByProductId
                            orderby c.ReviewMark descending
                            select c).ToArray()
                        where c.ReviewMark <= 2
						select c;
					break;
				}
				default:
				{
					array = (
						from c in commentsByProductId
						orderby c.ReviewMark descending
						select c).ToArray();
					break;
				}
			}
			ProductCommentInfo[] productCommentInfoArray = (
				from a in array
				orderby a.ReviewDate descending
				select a).Skip((pageNo - 1) * pageSize).Take(pageSize).ToArray();
			var collection = 
				from item in productCommentInfoArray.ToArray()
                select new { UserName = item.UserName, ReviewContent = item.ReviewContent, ReviewDate = item.ReviewDate.ToString("yyyy-MM-dd HH:mm:ss"), ReviewMark = item.ReviewMark };
			return Json(collection);
		}

		public JsonResult GetSKUInfo(long pId)
		{
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(pId);
			List<ProductSKUModel> productSKUModels = new List<ProductSKUModel>();
			foreach (SKUInfo sKUInfo in 
				from s in product.SKUInfo
				where s.Stock > 0
                select s)
			{
				ProductSKUModel productSKUModel = new ProductSKUModel()
				{
					Price = sKUInfo.SalePrice,
					SKUId = sKUInfo.Id,
					Stock = (int)sKUInfo.Stock
				};
				productSKUModels.Add(productSKUModel);
			}
			foreach (ProductSKUModel productSKUModel1 in productSKUModels)
			{
				string[] strArrays = productSKUModel1.SKUId.Split(new char[] { '\u005F' });
				productSKUModel1.SKUId = string.Format("{0};{1};{2}", strArrays[1], strArrays[2], strArrays[3]);
			}
			return Json(new { skuArray = productSKUModels }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult HistoryVisite(long userId)
		{
			return View(BrowseHistrory.GetBrowsingProducts(10, userId));
		}

		[HttpPost]
		public JsonResult LogProduct(long pid)
		{
			if (base.CurrentUser == null)
			{
				BrowseHistrory.AddBrowsingProduct(pid, 0);
			}
			else
			{
				BrowseHistrory.AddBrowsingProduct(pid, base.CurrentUser.Id);
			}
			ServiceHelper.Create<IProductService>().LogProductVisti(pid);
			return Json(null);
		}

		public ActionResult ProductComment(long pId, int commentType = 0)
		{
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(pId);
			base.ViewBag.goodComment = commentsByProductId.Count((ProductCommentInfo c) => c.ReviewMark >= 4);
			base.ViewBag.mediumComment = commentsByProductId.Count((ProductCommentInfo c) => c.ReviewMark == 3);
			base.ViewBag.bedComment = commentsByProductId.Count((ProductCommentInfo c) => c.ReviewMark <= 2);
			ViewBag.allComment = commentsByProductId.Count();
			return View();
		}
	}
}