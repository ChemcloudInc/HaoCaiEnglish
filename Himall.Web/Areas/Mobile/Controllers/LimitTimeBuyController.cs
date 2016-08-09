using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
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
	public class LimitTimeBuyController : BaseMobileTemplatesController
	{
		public LimitTimeBuyController()
		{
		}

		public ActionResult Detail(string id)
		{
			long valueId;
			ProductInfo[] array;
			int i;
			double num;
			decimal num1;
			decimal num2;
			string str = "";
			ProductDetailModelForWeb productDetailModelForWeb = new ProductDetailModelForWeb()
			{
				HotAttentionProducts = new List<HotProductInfo>(),
				HotSaleProducts = new List<HotProductInfo>(),
				Product = new ProductInfo(),
				Shop = new ShopInfoModel(),
				ShopCategory = new List<CategoryJsonModel>(),
				Color = new CollectionSKU(),
				Size = new CollectionSKU(),
				Version = new CollectionSKU()
			};
			ProductDetailModelForWeb maxSaleCount = productDetailModelForWeb;
			LimitTimeMarketInfo limitTimeMarketItem = null;
			ShopInfo shop = null;
			long productId = 0;
			long num3 = 0;
			long.TryParse(id, out num3);
			if (num3 == 0)
			{
				return RedirectToAction("Error404", "Error", new { area = "Mobile" });
			}
			limitTimeMarketItem = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItem(num3);
			if (limitTimeMarketItem == null || limitTimeMarketItem.AuditStatus != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing)
			{
				limitTimeMarketItem = (limitTimeMarketItem == null ? ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(num3) : limitTimeMarketItem);
				if (limitTimeMarketItem == null || limitTimeMarketItem.AuditStatus != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing)
				{
					return RedirectToAction("Error404", "Error", new { area = "Mobile" });
				}
			}
			if (limitTimeMarketItem != null && (limitTimeMarketItem.AuditStatus != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing || limitTimeMarketItem.EndTime < DateTime.Now))
			{
				return RedirectToAction("Detail", "Product", new { id = limitTimeMarketItem.ProductId });
			}
			maxSaleCount.MaxSaleCount = limitTimeMarketItem.MaxSaleCount;
			maxSaleCount.Title = limitTimeMarketItem.Title;
			shop = ServiceHelper.Create<IShopService>().GetShop(limitTimeMarketItem.ShopId, false);
			if (limitTimeMarketItem == null || limitTimeMarketItem.Id == 0)
			{
				return RedirectToAction("Error404", "Error", new { area = "Web" });
			}
			ProductInfo product = ServiceHelper.Create<IProductService>().GetProduct(limitTimeMarketItem.ProductId);
			productId = limitTimeMarketItem.ProductId;
			product.MarketPrice = limitTimeMarketItem.Price;
			product.SaleCounts = limitTimeMarketItem.SaleCount;
			maxSaleCount.Product = product;
			maxSaleCount.ProductDescription = product.ProductDescriptionInfo.Description;
			if (product.ProductDescriptionInfo.DescriptionPrefixId != 0)
			{
				ProductDescriptionTemplateInfo template = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplate(product.ProductDescriptionInfo.DescriptionPrefixId, product.ShopId);
				maxSaleCount.DescriptionPrefix = (template == null ? "" : template.Content);
			}
			if (product.ProductDescriptionInfo.DescriptiondSuffixId != 0)
			{
				ProductDescriptionTemplateInfo productDescriptionTemplateInfo = ServiceHelper.Create<IProductDescriptionTemplateService>().GetTemplate(product.ProductDescriptionInfo.DescriptiondSuffixId, product.ShopId);
				maxSaleCount.DescriptiondSuffix = (productDescriptionTemplateInfo == null ? "" : productDescriptionTemplateInfo.Content);
			}
			ShopServiceMarkModel shopComprehensiveMark = ShopServiceMark.GetShopComprehensiveMark(shop.Id);
			maxSaleCount.Shop.PackMark = shopComprehensiveMark.PackMark;
			maxSaleCount.Shop.ServiceMark = shopComprehensiveMark.ServiceMark;
			maxSaleCount.Shop.ComprehensiveMark = shopComprehensiveMark.ComprehensiveMark;
			IQueryable<ProductCommentInfo> commentsByProductId = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(productId);
			maxSaleCount.Shop.Name = shop.ShopName;
			maxSaleCount.Shop.ProductMark = (commentsByProductId == null || commentsByProductId.Count() == 0 ? new decimal(0) : commentsByProductId.Average<ProductCommentInfo>((ProductCommentInfo p) => (decimal)p.ReviewMark));
			maxSaleCount.Shop.Id = product.ShopId;
			maxSaleCount.Shop.FreeFreight = shop.FreeFreight;
			ViewBag.ProductNum = ServiceHelper.Create<IProductService>().GetShopOnsaleProducts(product.ShopId);
			if (base.CurrentUser != null)
			{
				ViewBag.IsFavorite = ServiceHelper.Create<IProductService>().IsFavorite(product.Id, base.CurrentUser.Id);
			}
			else
			{
				ViewBag.IsFavorite = false;
			}
			IQueryable<ShopCategoryInfo> shopCategory = ServiceHelper.Create<IShopCategoryService>().GetShopCategory(product.ShopId);
			List<ShopCategoryInfo> list = shopCategory.ToList();
			foreach (ShopCategoryInfo shopCategoryInfo in 
				from s in list
				where s.ParentCategoryId == 0
                select s)
			{
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = shopCategoryInfo.Name
				};
				valueId = shopCategoryInfo.Id;
				categoryJsonModel.Id = valueId.ToString();
				categoryJsonModel.SubCategory = new List<SecondLevelCategory>();
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				foreach (ShopCategoryInfo shopCategoryInfo1 in 
					from s in list
					where s.ParentCategoryId == shopCategoryInfo.Id
					select s)
				{
					SecondLevelCategory secondLevelCategory = new SecondLevelCategory()
					{
						Name = shopCategoryInfo1.Name,
						Id = shopCategoryInfo1.Id.ToString()
					};
					categoryJsonModel1.SubCategory.Add(secondLevelCategory);
				}
				maxSaleCount.ShopCategory.Add(categoryJsonModel1);
			}
			IQueryable<ProductInfo> hotSaleProduct = ServiceHelper.Create<IProductService>().GetHotSaleProduct(shop.Id, 5);
			if (hotSaleProduct != null)
			{
				array = hotSaleProduct.ToArray();
				for (i = 0; i < array.Length; i++)
				{
					ProductInfo productInfo = array[i];
					List<HotProductInfo> hotSaleProducts = maxSaleCount.HotSaleProducts;
					HotProductInfo hotProductInfo = new HotProductInfo()
					{
						ImgPath = productInfo.ImagePath,
						Name = productInfo.ProductName,
						Price = productInfo.MinSalePrice,
						Id = productInfo.Id,
						SaleCount = (int)productInfo.SaleCounts
					};
					hotSaleProducts.Add(hotProductInfo);
				}
			}
			IQueryable<ProductInfo> hotConcernedProduct = ServiceHelper.Create<IProductService>().GetHotConcernedProduct(shop.Id, 5);
			if (hotConcernedProduct != null)
			{
				array = hotConcernedProduct.ToArray();
				for (i = 0; i < array.Length; i++)
				{
					ProductInfo productInfo1 = array[i];
					List<HotProductInfo> hotAttentionProducts = maxSaleCount.HotAttentionProducts;
					HotProductInfo hotProductInfo1 = new HotProductInfo()
					{
						ImgPath = productInfo1.ImagePath,
						Name = productInfo1.ProductName,
						Price = productInfo1.MinSalePrice,
						Id = productInfo1.Id,
						SaleCount = productInfo1.ConcernedCount
					};
					hotAttentionProducts.Add(hotProductInfo1);
				}
			}
			if (product.SKUInfo != null && product.SKUInfo.Count() > 0)
			{
				long num4 = 0;
				long num5 = 0;
				long num6 = 0;
				foreach (SKUInfo sKUInfo in product.SKUInfo)
				{
					string[] strArrays = sKUInfo.Id.Split(new char[] { '\u005F' });
					if (strArrays.Count() > 0)
					{
						long.TryParse(strArrays[1], out num4);
						if (num4 != 0)
						{
							if (!maxSaleCount.Color.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Color)))
							{
								long num7 = (
									from s in product.SKUInfo
									where s.Color.Equals(sKUInfo.Color)
									select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
								CollectionSKU color = maxSaleCount.Color;
								ProductSKU productSKU = new ProductSKU()
								{
									Name = "选择颜色",
									EnabledClass = (num7 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num4,
									Value = sKUInfo.Color
								};
                                color.Add(productSKU);
							}
						}
					}
					if (strArrays.Count() > 1)
					{
						long.TryParse(strArrays[2], out num5);
						if (num5 != 0)
						{
							if (!maxSaleCount.Size.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Size)))
							{
								long num8 = (
									from s in product.SKUInfo
									where s.Size.Equals(sKUInfo.Size)
									select s).Sum<SKUInfo>((SKUInfo s1) => s1.Stock);
								CollectionSKU size = maxSaleCount.Size;
								ProductSKU productSKU1 = new ProductSKU()
								{
									Name = "选择尺码",
									EnabledClass = (num8 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num5,
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
					long.TryParse(strArrays[3], out num6);
					if (num6 == 0)
					{
						continue;
					}
					if (maxSaleCount.Version.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Version)))
					{
						continue;
					}
					long num9 = (
						from s in product.SKUInfo
						where s.Version.Equals(sKUInfo.Version)
						select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
					CollectionSKU version = maxSaleCount.Version;
					ProductSKU productSKU2 = new ProductSKU()
					{
						Name = "选择版本",
						EnabledClass = (num9 != 0 ? "enabled" : "disabled"),
						SelectedClass = "",
						SKUId = num6,
						Value = sKUInfo.Version
					};
                    version.Add(productSKU2);
				}
				decimal num10 = new decimal(0);
				decimal num11 = new decimal(0);
				num10 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Min<SKUInfo>((SKUInfo s) => s.SalePrice);
				num11 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Max<SKUInfo>((SKUInfo s) => s.SalePrice);
				if (!(num10 == new decimal(0)) || !(num11 == new decimal(0)))
				{
					str = (num11 <= num10 ? string.Format("{0}", num10.ToString("f2")) : string.Format("{0}-{1}", num10.ToString("f2"), num11.ToString("f2")));
				}
				else
				{
					str = product.MinSalePrice.ToString("f2");
				}
			}
			base.ViewBag.Price = (string.IsNullOrWhiteSpace(str) ? product.MinSalePrice.ToString("f2") : str);
			List<TypeAttributesModel> typeAttributesModels = new List<TypeAttributesModel>();
			List<ProductAttributeInfo> productAttributeInfos = ServiceHelper.Create<IProductService>().GetProductAttribute(product.Id).ToList();
			foreach (ProductAttributeInfo productAttributeInfo in productAttributeInfos)
			{
				if (typeAttributesModels.Any((TypeAttributesModel p) => p.AttrId == productAttributeInfo.AttributeId))
				{
					TypeAttributesModel typeAttributesModel = typeAttributesModels.FirstOrDefault((TypeAttributesModel p) => p.AttrId == productAttributeInfo.AttributeId);
					if (typeAttributesModel.AttrValues.Any((TypeAttrValue p) => p.Id == productAttributeInfo.ValueId.ToString()))
					{
						continue;
					}
					List<TypeAttrValue> attrValues = typeAttributesModel.AttrValues;
					TypeAttrValue typeAttrValue = new TypeAttrValue();
					valueId = productAttributeInfo.ValueId;
					typeAttrValue.Id = valueId.ToString();
					typeAttrValue.Name = productAttributeInfo.AttributesInfo.AttributeValueInfo.FirstOrDefault((AttributeValueInfo a) => a.Id == productAttributeInfo.ValueId).Value;
					attrValues.Add(typeAttrValue);
				}
				else
				{
					TypeAttributesModel typeAttributesModel1 = new TypeAttributesModel()
					{
						AttrId = productAttributeInfo.AttributeId,
						AttrValues = new List<TypeAttrValue>(),
						Name = productAttributeInfo.AttributesInfo.Name
					};
					TypeAttributesModel typeAttributesModel2 = typeAttributesModel1;
					foreach (AttributeValueInfo attributeValueInfo in productAttributeInfo.AttributesInfo.AttributeValueInfo)
					{
						if (!productAttributeInfos.Any((ProductAttributeInfo p) => p.ValueId == attributeValueInfo.Id))
						{
							continue;
						}
						List<TypeAttrValue> typeAttrValues = typeAttributesModel2.AttrValues;
						TypeAttrValue value = new TypeAttrValue();
						valueId = attributeValueInfo.Id;
						value.Id = valueId.ToString();
						value.Name = attributeValueInfo.Value;
						typeAttrValues.Add(value);
					}
					typeAttributesModels.Add(typeAttributesModel2);
				}
			}
			ViewBag.ProductAttrs = typeAttributesModels;
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			CommentQuery commentQuery = new CommentQuery()
			{
				ProductID = product.Id,
				PageNo = 1,
				PageSize = 10000
			};
			PageModel<ProductCommentInfo> comments = commentService.GetComments(commentQuery);
			ViewBag.CommentCount = comments.Total;
			IQueryable<ProductConsultationInfo> consultations = ServiceHelper.Create<IConsultationService>().GetConsultations(productId);
			ViewBag.Consultations = consultations.Count();
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
			int num14 = 5;
			if (statisticOrderCommentsInfo == null || statisticOrderCommentsInfo3 == null)
			{
				ViewBag.ProductAndDescription = num14;
				ViewBag.ProductAndDescriptionPeer = num14;
				ViewBag.ProductAndDescriptionMin = num14;
				ViewBag.ProductAndDescriptionMax = num14;
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
				ViewBag.SellerServiceAttitude = num14;
				ViewBag.SellerServiceAttitudePeer = num14;
				ViewBag.SellerServiceAttitudeMax = num14;
				ViewBag.SellerServiceAttitudeMin = num14;
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
				ViewBag.SellerDeliverySpeed = num14;
				ViewBag.SellerDeliverySpeedPeer = num14;
				ViewBag.SellerDeliverySpeedMax = num14;
				ViewBag.sellerDeliverySpeedMin = num14;
			}
			else
			{
				ViewBag.SellerDeliverySpeed = statisticOrderCommentsInfo2.CommentValue;
				ViewBag.SellerDeliverySpeedPeer = statisticOrderCommentsInfo5.CommentValue;
				dynamic viewBag = base.ViewBag;
				num1 = (statisticOrderCommentsInfo10 != null ? statisticOrderCommentsInfo10.CommentValue : new decimal(0));
				viewBag.SellerDeliverySpeedMax = num1;
				dynamic obj = base.ViewBag;
				num2 = (statisticOrderCommentsInfo11 != null ? statisticOrderCommentsInfo11.CommentValue : new decimal(0));
				obj.sellerDeliverySpeedMin = num2;
			}
			base.ViewBag.Logined = (base.CurrentUser != null ? 1 : 0);
			base.ViewBag.EnabledBuy = (product.AuditStatus != ProductInfo.ProductAuditStatus.Audited || !(limitTimeMarketItem.StartTime <= DateTime.Now) || !(limitTimeMarketItem.EndTime > DateTime.Now) ? false : product.SaleStatus == ProductInfo.ProductSaleStatus.OnSale);
			DateTime endTime = limitTimeMarketItem.EndTime;
			TimeSpan timeSpan = new TimeSpan(endTime.Ticks);
			endTime = DateTime.Now;
			TimeSpan timeSpan1 = timeSpan.Subtract(new TimeSpan(endTime.Ticks));
			dynamic viewBag1 = base.ViewBag;
			num = (timeSpan1.TotalSeconds < 0 ? 0 : timeSpan1.TotalSeconds);
			viewBag1.Second = num;
			return View(maxSaleCount);
		}
	}
}