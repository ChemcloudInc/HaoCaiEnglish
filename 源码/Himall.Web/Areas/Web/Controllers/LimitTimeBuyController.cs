using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class LimitTimeBuyController : BaseWebController
	{
		public LimitTimeBuyController()
		{
		}

		[HttpPost]
		public JsonResult CheckLimitTimeBuy(string skuIds, string counts)
		{
			string[] strArrays = skuIds.Split(new char[] { ',' });
			string str = counts.TrimEnd(new char[] { ',' });
			char[] chrArray = new char[] { ',' };
			IEnumerable<int> nums1 = 
				from t in str.Split(chrArray)
				select int.Parse(t);
			IProductService productService = ServiceHelper.Create<IProductService>();
			int num3 = 0;
			CartItemModel cartItemModel = strArrays.Select<string, CartItemModel>((string item) => {
				SKUInfo sku = productService.GetSku(item);
				IEnumerable<int> nums = nums1;
				int num = num3;
				int num1 = num;
				num3 = num + 1;
				int num2 = nums.ElementAt<int>(num1);
				return new CartItemModel()
				{
					id = sku.ProductInfo.Id,
					count = num2
				};
			}).ToList().FirstOrDefault();
			int marketSaleCountForUserId = ServiceHelper.Create<ILimitTimeBuyService>().GetMarketSaleCountForUserId(cartItemModel.id, base.CurrentUser.Id);
			int maxSaleCount = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(cartItemModel.id).MaxSaleCount;
			return Json(new { success = maxSaleCount >= marketSaleCountForUserId + cartItemModel.count, maxSaleCount = maxSaleCount, remain = maxSaleCount - marketSaleCountForUserId });
		}

		public ActionResult Detail(string id)
		{
			long valueId;
			ProductInfo[] array;
			int i;
			double num;
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
			long num1 = 0;
			long.TryParse(id, out num1);
			if (num1 == 0)
			{
				return RedirectToAction("Error404", "Error", new { area = "Web" });
			}
			limitTimeMarketItem = ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItem(num1);
			if (limitTimeMarketItem == null || limitTimeMarketItem.AuditStatus != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing)
			{
				limitTimeMarketItem = (limitTimeMarketItem == null ? ServiceHelper.Create<ILimitTimeBuyService>().GetLimitTimeMarketItemByProductId(num1) : limitTimeMarketItem);
				if (limitTimeMarketItem == null || limitTimeMarketItem.AuditStatus != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing)
				{
					return RedirectToAction("Error404", "Error", new { area = "Web" });
				}
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
				long num2 = 0;
				long num3 = 0;
				long num4 = 0;
				foreach (SKUInfo sKUInfo in product.SKUInfo)
				{
					string[] strArrays = sKUInfo.Id.Split(new char[] { '\u005F' });
					if (strArrays.Count() > 0)
					{
						long.TryParse(strArrays[1], out num2);
						if (num2 != 0)
						{
							if (!maxSaleCount.Color.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Color)))
							{
								long num5 = (
									from s in product.SKUInfo
									where s.Color.Equals(sKUInfo.Color)
									select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
								CollectionSKU color = maxSaleCount.Color;
								ProductSKU productSKU = new ProductSKU()
								{
									Name = "选择颜色",
									EnabledClass = (num5 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num2,
									Value = sKUInfo.Color
								};
                                color.Add(productSKU);
							}
						}
					}
					if (strArrays.Count() > 1)
					{
						long.TryParse(strArrays[2], out num3);
						if (num3 != 0)
						{
							if (!maxSaleCount.Size.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Size)))
							{
								long num6 = (
									from s in product.SKUInfo
									where s.Size.Equals(sKUInfo.Size)
									select s).Sum<SKUInfo>((SKUInfo s1) => s1.Stock);
								CollectionSKU size = maxSaleCount.Size;
								ProductSKU productSKU1 = new ProductSKU()
								{
									Name = "选择尺码",
									EnabledClass = (num6 != 0 ? "enabled" : "disabled"),
									SelectedClass = "",
									SKUId = num3,
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
					long.TryParse(strArrays[3], out num4);
					if (num4 == 0)
					{
						continue;
					}
					if (maxSaleCount.Version.Any((ProductSKU v) => v.Value.Equals(sKUInfo.Version)))
					{
						continue;
					}
					long num7 = (
						from s in product.SKUInfo
						where s.Version.Equals(sKUInfo.Version)
						select s).Sum<SKUInfo>((SKUInfo s) => s.Stock);
					CollectionSKU version = maxSaleCount.Version;
					ProductSKU productSKU2 = new ProductSKU()
					{
						Name = "选择版本",
						EnabledClass = (num7 != 0 ? "enabled" : "disabled"),
						SelectedClass = "",
						SKUId = num4,
						Value = sKUInfo.Version
					};
                    version.Add(productSKU2);
				}
				decimal num8 = new decimal(0);
				decimal num9 = new decimal(0);
				num8 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Min<SKUInfo>((SKUInfo s) => s.SalePrice);
				num9 = (
					from s in product.SKUInfo
					where s.Stock >= 0
                    select s).Max<SKUInfo>((SKUInfo s) => s.SalePrice);
				if (!(num8 == new decimal(0)) || !(num9 == new decimal(0)))
				{
					str = (num9 <= num8 ? string.Format("{0}", num8.ToString("f2")) : string.Format("{0}-{1}", num8.ToString("f2"), num9.ToString("f2")));
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
			if (base.CurrentUser == null)
			{
				BrowseHistrory.AddBrowsingProduct(product.Id, 0);
			}
			else
			{
				BrowseHistrory.AddBrowsingProduct(product.Id, base.CurrentUser.Id);
			}
			ServiceHelper.Create<IProductService>().LogProductVisti(productId);
			base.ViewBag.Logined = (base.CurrentUser != null ? 1 : 0);
			base.ViewBag.EnabledBuy = (product.AuditStatus != ProductInfo.ProductAuditStatus.Audited || !(limitTimeMarketItem.StartTime <= DateTime.Now) || !(limitTimeMarketItem.EndTime > DateTime.Now) ? false : product.SaleStatus == ProductInfo.ProductSaleStatus.OnSale);
			DateTime endTime = limitTimeMarketItem.EndTime;
			TimeSpan timeSpan = new TimeSpan(endTime.Ticks);
			endTime = DateTime.Now;
			TimeSpan timeSpan1 = timeSpan.Subtract(new TimeSpan(endTime.Ticks));
			dynamic viewBag = base.ViewBag;
			num = (timeSpan1.TotalSeconds < 0 ? 0 : timeSpan1.TotalSeconds);
			viewBag.Second = num;
			return View(maxSaleCount);
		}

		public ActionResult Home(string keywords = "", string catename = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 60)
		{
			LimitTimeQuery limitTimeQuery = new LimitTimeQuery()
			{
				ItemName = keywords,
				OrderKey = orderKey,
				OrderType = orderType,
				CategoryName = catename,
				PageNo = pageNo,
				PageSize = pageSize,
				AuditStatus = new LimitTimeMarketInfo.LimitTimeMarketAuditStatus?(LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing),
				CheckProductStatus = true
			};
			LimitTimeQuery limitTimeQuery1 = limitTimeQuery;
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
			if (!string.IsNullOrWhiteSpace(catename))
			{
				SelectListItem selectListItem1 = selectListItems.FirstOrDefault((SelectListItem c) => c.Text.Equals(catename));
				if (selectListItem1 != null)
				{
					selectListItem1.Selected = true;
				}
			}
			ViewBag.Cate = selectListItems;
			ViewBag.keywords = keywords;
			ViewBag.orderKey = orderKey;
			ViewBag.orderType = orderType;
			base.ViewBag.Logined = (base.CurrentUser != null ? 1 : 0);
			ViewBag.Slide = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.PlatformLimitTime);
			PageModel<LimitTimeMarketInfo> itemList = ServiceHelper.Create<ILimitTimeBuyService>().GetItemList(limitTimeQuery1);
			int total = itemList.Total;
			LimitTimeMarketInfo[] array = itemList.Models.ToArray();
			if (itemList.Total == 0)
			{
				ViewBag.keywords = keywords;
				return View();
			}
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = limitTimeQuery1.PageNo,
				ItemsPerPage = pageSize,
				TotalItems = total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(array ?? new LimitTimeMarketInfo[0]);
		}
	}
}