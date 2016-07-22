using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Mobile.Models;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class SearchController : BaseMobileTemplatesController
	{
		public SearchController()
		{
		}

		public ActionResult Index(string keywords = "", string exp_keywords = "", long cid = 0L, long b_id = 0L, string a_id = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 10, long vshopId = 0L)
		{
			long valueId;
			if (!string.IsNullOrWhiteSpace(keywords))
			{
				keywords = keywords.Trim();
			}
			long shopId = 0;
			if (vshopId > 0)
			{
				VShopInfo vShop = ServiceHelper.Create<IVShopService>().GetVShop(vshopId);
				if (vShop != null)
				{
					shopId = vShop.ShopId;
				}
			}
			ProductSearch productSearch = new ProductSearch()
			{
				shopId = shopId,
				BrandId = b_id,
				CategoryId = cid,
				Ex_Keyword = exp_keywords,
				Keyword = keywords,
				OrderKey = orderKey,
				OrderType = orderType == 1,
				AttrIds = new List<string>(),
				PageNumber = pageNo,
				PageSize = pageSize
			};
			ProductSearch productSearch1 = productSearch;
			string str = a_id.Replace("%40", "@");
			char[] chrArray = new char[] { '@' };
			string[] strArrays = str.Split(chrArray);
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str1 = strArrays[i];
				if (!string.IsNullOrWhiteSpace(str1))
				{
					productSearch1.AttrIds.Add(str1);
				}
			}
			PageModel<ProductInfo> pageModel = ServiceHelper.Create<IProductService>().SearchProduct(productSearch1);
			ProductInfo[] array = pageModel.Models.ToArray();
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(cid);
			if (category != null && category.Depth == 3)
			{
				array = (
					from p in array
                    where p.CategoryId == cid
					select p).ToArray();
			}
			int total = pageModel.Total;
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			IEnumerable<ProductItem> productItem = 
				from item in array
				select new ProductItem()
				{
					Id = item.Id,
					ImageUrl = item.GetImage(ProductInfo.ImageSize.Size_350, 1),
					SalePrice = item.MinSalePrice,
					Name = item.ProductName,
					CommentsCount = commentService.GetCommentsByProductId(item.Id).Count()
				};
			var categoryId = 
				from prod in array
                orderby prod.CategoryId
				group prod by prod.CategoryId into G
				select new { Key = G.Key, Path = G.FirstOrDefault().CategoryPath };
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			foreach (var variable in categoryId)
			{
				string path = variable.Path;
				char[] chrArray1 = new char[] { '|' };
				if (path.Split(chrArray1).Length != 2)
				{
					string path1 = variable.Path;
					char[] chrArray2 = new char[] { '|' };
					if (path1.Split(chrArray2).Length != 3)
					{
						continue;
					}
					string path2 = variable.Path;
					char[] chrArray3 = new char[] { '|' };
					long num = long.Parse(path2.Split(chrArray3)[1]);
					string str2 = variable.Path;
					char[] chrArray4 = new char[] { '|' };
                    InitialCategory(categoryJsonModels, num, long.Parse(str2.Split(chrArray4)[2]));
				}
				else
				{
					string path3 = variable.Path;
					char[] chrArray5 = new char[] { '|' };
					long num1 = long.Parse(path3.Split(chrArray5)[0]);
					string str3 = variable.Path;
					char[] chrArray6 = new char[] { '|' };
                    InitialCategory(categoryJsonModels, num1, long.Parse(str3.Split(chrArray6)[1]));
				}
			}
			List<BrandInfo> brandInfos = new List<BrandInfo>();
			var brandId = 
				from prod in array
                orderby prod.BrandId
				where prod.BrandId != 0
                group prod by prod.BrandId into G
				select new { Key = G.Key };
			foreach (var variable1 in brandId)
			{
				BrandInfo brand = ServiceHelper.Create<IBrandService>().GetBrand(variable1.Key);
				BrandInfo brandInfo = new BrandInfo()
				{
					Id = brand.Id,
					Name = brand.Name
				};
				brandInfos.Add(brandInfo);
			}
			List<TypeAttributesModel> typeAttributesModels = new List<TypeAttributesModel>();
			var collection = 
				from p in array
                group p by p.CategoryId into G
				select new { Key = G.Key, Count = G.Count() } into pp
				orderby pp.Count descending
				select pp;
			long num2 = (collection.ToList().Count > 0 ? collection.ToList()[0].Key : 0);
			array = (
				from p in array
				orderby p.CategoryId.Equals(num2) descending
				select p).ToArray();
			if (collection.Count() <= 20)
			{
				List<ProductInfo> list = (
					from p in array
                    where p.CategoryId.Equals(num2)
					select p).ToList();
				foreach (ProductInfo productInfo in list)
				{
					List<ProductAttributeInfo> productAttributeInfos = ServiceHelper.Create<IProductService>().GetProductAttribute(productInfo.Id).ToList();
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
				}
			}
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (string attrId in productSearch1.AttrIds)
			{
				long num3 = 0;
				chrArray = new char[] { '\u005F' };
				long.TryParse(attrId.Split(chrArray)[0], out num3);
				long num4 = 0;
				chrArray = new char[] { '\u005F' };
				long.TryParse(attrId.Split(chrArray)[1], out num4);
				AttributeInfo attributeInfo = ServiceHelper.Create<IProductService>().GetAttributeInfo(num3);
				AttributeValueInfo attributeValueInfo1 = attributeInfo.AttributeValueInfo.FirstOrDefault((AttributeValueInfo v) => v.Id == num4);
				string str4 = string.Concat(attributeInfo.Name, ':', attributeValueInfo1.Value);
				string str5 = "";
				foreach (string attrId1 in productSearch1.AttrIds)
				{
					if (attrId1.Equals(attrId))
					{
						if (productSearch1.AttrIds.Count() != 1)
						{
							continue;
						}
						str5 = attrId1;
					}
					else
					{
						str5 = string.Concat(str5, attrId1, '@');
					}
				}
				chrArray = new char[] { '@' };
				strs.Add(str4, str5.TrimEnd(chrArray));
			}
			ViewBag.Attrs = typeAttributesModels.ToArray();
			ViewBag.Brands = brandInfos.ToArray();
			ViewBag.Category = categoryJsonModels.ToArray();
			ViewBag.AttrDic = strs;
			ViewBag.cid = cid;
			ViewBag.b_id = b_id;
			ViewBag.a_id = a_id;
			ViewBag.Total = total;
			ViewBag.Keywords = keywords;
			return View(productItem);
		}

		[HttpPost]
		public JsonResult Index(string keywords = "", string exp_keywords = "", long cid = 0L, long b_id = 0L, string a_id = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 10, string t = "")
		{
			int num;
			if (!string.IsNullOrWhiteSpace(keywords))
			{
				keywords = keywords.Trim();
			}
			ProductInfo[] array = searchProducts(out num, keywords, exp_keywords, cid, b_id, a_id, orderKey, orderType, pageNo, pageSize).ToArray();
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(cid);
			if (category != null && category.Depth == 3)
			{
				array = (
					from p in array
                    where p.CategoryId == cid
					select p).ToArray();
			}
			ICommentService commentService = ServiceHelper.Create<ICommentService>();
			var variable = 
				from item in array
				select new { id = item.Id, name = item.ProductName, price = item.MinSalePrice.ToString("F2"), commentsCount = commentService.GetCommentsByProductId(item.Id).Count(), img = item.GetImage(ProductInfo.ImageSize.Size_350, 1) };
			return Json(variable);
		}

		private void InitialCategory(List<CategoryJsonModel> model, long f_cateId, long sub_cateId)
		{
			string name = ServiceHelper.Create<ICategoryService>().GetCategory(f_cateId).Name;
			string str = ServiceHelper.Create<ICategoryService>().GetCategory(sub_cateId).Name;
			if (model.Any((CategoryJsonModel c) => c.Id == f_cateId.ToString()))
			{
				List<SecondLevelCategory> subCategory = model.FirstOrDefault((CategoryJsonModel c) => c.Id == f_cateId.ToString()).SubCategory;
				SecondLevelCategory secondLevelCategory = new SecondLevelCategory()
				{
					Name = str,
					Id = sub_cateId.ToString()
				};
				subCategory.Add(secondLevelCategory);
				return;
			}
			CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
			{
				Name = name,
				Id = f_cateId.ToString()
			};
			List<SecondLevelCategory> secondLevelCategories = new List<SecondLevelCategory>();
			SecondLevelCategory secondLevelCategory1 = new SecondLevelCategory()
			{
				Id = sub_cateId.ToString(),
				Name = str
			};
			secondLevelCategories.Add(secondLevelCategory1);
			categoryJsonModel.SubCategory = secondLevelCategories;
			model.Add(categoryJsonModel);
		}

		private IQueryable<ProductInfo> searchProducts(out int total, string keywords = "", string exp_keywords = "", long cid = 0L, long b_id = 0L, string a_id = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 6)
		{
			ProductSearch productSearch = new ProductSearch()
			{
				shopId = 0,
				BrandId = b_id,
				CategoryId = cid,
				Ex_Keyword = exp_keywords,
				Keyword = keywords,
				OrderKey = orderKey,
				OrderType = orderType == 1,
				AttrIds = new List<string>(),
				PageNumber = pageNo,
				PageSize = pageSize
			};
			ProductSearch productSearch1 = productSearch;
			string[] strArrays = a_id.Replace("%40", "@").Split(new char[] { '@' });
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrWhiteSpace(str))
				{
					productSearch1.AttrIds.Add(str);
				}
			}
			PageModel<ProductInfo> pageModel = ServiceHelper.Create<IProductService>().SearchProduct(productSearch1);
			total = pageModel.Total;
			return pageModel.Models;
		}
	}
}