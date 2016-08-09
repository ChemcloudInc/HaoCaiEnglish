using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ProductController : BaseSellerController
	{
		private long shopId = 2;

		public ProductController()
		{
			if (base.CurrentSellerManager != null)
			{
                shopId = base.CurrentSellerManager.ShopId;
			}
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchOnSale(string ids)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			ServiceHelper.Create<IProductService>().OnSale(nums, base.CurrentSellerManager.ShopId);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("商家商品批量上架，Ids=", ids),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/Product/BatchOnSale",
				UserName = base.CurrentSellerManager.UserName,
				ShopId = base.CurrentSellerManager.ShopId
			};
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BatchSaleOff(string ids)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			ServiceHelper.Create<IProductService>().SaleOff(nums, base.CurrentSellerManager.ShopId);
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("商家商品批量下架，Ids=", ids),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/Product/BatchSaleOff",
				UserName = base.CurrentSellerManager.UserName,
				ShopId = base.CurrentSellerManager.ShopId
			};
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult BindTemplates(string ids, long? topTemplateId, long? bottomTemplateId)
		{
			char[] chrArray = new char[] { ',' };
			IEnumerable<long> nums = 
				from item in ids.Split(chrArray)
				select long.Parse(item);
			ServiceHelper.Create<IProductService>().BindTemplate(topTemplateId, bottomTemplateId, nums);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Browse(long? categoryId, int? auditStatus, string ids, int page, int rows, string keyWords, int? saleStatus, bool? isShopCategory, bool isLimitTimeBuy = false, bool showSku = false)
		{
			long? nullable;
			long? nullable1;
			IEnumerable<long> nums;
			Func<SKUInfo, SKUModel> func = null;
			ProductQuery productQuery = new ProductQuery()
			{
				PageSize = rows,
				PageNo = page,
				KeyWords = keyWords
			};
			ProductQuery productQuery1 = productQuery;
			if (isShopCategory.GetValueOrDefault())
			{
				nullable = null;
			}
			else
			{
				nullable = categoryId;
			}
			productQuery1.CategoryId = nullable;
			ProductQuery productQuery2 = productQuery;
			if (isShopCategory.GetValueOrDefault())
			{
				nullable1 = categoryId;
			}
			else
			{
				nullable1 = null;
			}
			productQuery2.ShopCategoryId = nullable1;
			ProductQuery productQuery3 = productQuery;
			if (string.IsNullOrWhiteSpace(ids))
			{
				nums = null;
			}
			else
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
			}
			productQuery3.Ids = nums;
			productQuery.ShopId = new long?(base.CurrentSellerManager.ShopId);
			productQuery.IsLimitTimeBuy = isLimitTimeBuy;
			ProductQuery value = productQuery;
			if (auditStatus.HasValue)
			{
				value.AuditStatus = new ProductInfo.ProductAuditStatus[] { (ProductInfo.ProductAuditStatus)auditStatus.Value };
			}
			if (saleStatus.HasValue)
			{
				value.SaleStatus = new ProductInfo.ProductSaleStatus?((ProductInfo.ProductSaleStatus)saleStatus.Value);
			}
			PageModel<ProductInfo> products = ServiceHelper.Create<IProductService>().GetProducts(value);
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			var collection = products.Models.ToArray().Select((ProductInfo item) => {
				string str;
				string str1;
				IEnumerable<SKUModel> sKUModels;
				BrandInfo brand = brandService.GetBrand(item.BrandId);
				CategoryInfo category = categoryService.GetCategory(item.CategoryId);
				string productName = item.ProductName;
				str = (item.BrandId == 0 || brand == null ? "" : brand.Name);
				str1 = (brand == null ? "" : category.Name);
				long id = item.Id;
				string image = item.GetImage(ProductInfo.ImageSize.Size_50, 1);
				decimal minSalePrice = item.MinSalePrice;
				if (!showSku)
				{
					sKUModels = null;
				}
				else
				{
					ICollection<SKUInfo> sKUInfo = item.SKUInfo;
					if (func == null)
					{
						func = (SKUInfo a) => new SKUModel()
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
						};
					}
					sKUModels = sKUInfo.Select<SKUInfo, SKUModel>(func);
				}
				return new { name = productName, brandName = str, categoryName = str1, id = id, imgUrl = image, price = minSalePrice, skus = sKUModels };
			});
			return Json(new { rows = collection, total = products.Total });
		}

		private SKUSpecModel DeepClone(SKUSpecModel obj)
		{
			SKUSpecModel sKUSpecModel;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				IFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Seek(0, SeekOrigin.Begin);
				sKUSpecModel = binaryFormatter.Deserialize(memoryStream) as SKUSpecModel;
			}
			return sKUSpecModel;
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(string ids)
		{
			JsonResult jsonResult;
			try
			{
				char[] chrArray = new char[] { ',' };
				IEnumerable<long> nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
				ServiceHelper.Create<IProductService>().DeleteProduct(nums, base.CurrentSellerManager.ShopId);
				foreach (long num in nums)
				{
					string str = Server.MapPath(string.Format("/Storage/Shop/{0}/Products/{1}", base.CurrentSellerManager.ShopId, num));
                    DeleteDirectory(str);
				}
				IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
				LogInfo logInfo = new LogInfo()
				{
					Date = DateTime.Now,
					Description = string.Concat("商家删除商品，Ids=", ids),
					IPAddress = base.Request.UserHostAddress,
					PageUrl = "/Product/Delete",
					UserName = base.CurrentSellerManager.UserName,
					ShopId = base.CurrentSellerManager.ShopId
				};
				operationLogService.AddSellerOperationLog(logInfo);
				jsonResult = Json(new { success = true });
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				jsonResult = Json(new { success = false, msg = exception.Message });
			}
			return jsonResult;
		}

		private void DeleteDirectory(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo.Exists)
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					directories[i].Delete(true);
				}
				directoryInfo.Delete(true);
			}
		}

		[UnAuthorize]
		public JsonResult GetAttributes(long categoryId, long productId = 0L, long isCategoryId = 0L)
		{
			Dictionary<long, string> nums = new Dictionary<long, string>();
			if (isCategoryId == 1)
			{
				List<TypeAttributesModel> plateformAttr = GetPlateformAttr(categoryId);
				return Json(new { json = plateformAttr }, JsonRequestBehavior.AllowGet);
			}
			List<TypeAttributesModel> typeAttributesModels = new List<TypeAttributesModel>();
			IQueryable<ProductAttributeInfo> productAttribute = ServiceHelper.Create<IProductService>().GetProductAttribute(productId);
			if (productAttribute == null || productAttribute.Count() == 0)
			{
				typeAttributesModels = GetPlateformAttr(categoryId);
				return Json(new { json = typeAttributesModels }, JsonRequestBehavior.AllowGet);
			}
			ProductAttributeInfo[] array = productAttribute.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				ProductAttributeInfo productAttributeInfo = array[i];
				if (nums.ContainsKey(productAttributeInfo.AttributeId))
				{
					long attributeId = productAttributeInfo.AttributeId;
					string item = nums[productAttributeInfo.AttributeId];
					long valueId = productAttributeInfo.ValueId;
					nums[attributeId] = string.Concat(item, ",", valueId.ToString());
				}
				else
				{
					nums.Add(productAttributeInfo.AttributeId, productAttributeInfo.ValueId.ToString());
					AttributeInfo attributesInfo = productAttributeInfo.AttributesInfo;
					ICollection<AttributeValueInfo> attributeValueInfo = ServiceHelper.Create<ITypeService>().GetType(attributesInfo.TypeId).AttributeInfo.FirstOrDefault((AttributeInfo a) => a.Id == attributesInfo.Id).AttributeValueInfo;
					TypeAttributesModel typeAttributesModel = new TypeAttributesModel()
					{
						Name = attributesInfo.Name,
						AttrId = productAttributeInfo.AttributeId,
						Selected = "",
						IsMulti = attributesInfo.IsMulti,
						AttrValues = new List<TypeAttrValue>()
					};
					TypeAttributesModel typeAttributesModel1 = typeAttributesModel;
					AttributeValueInfo[] attributeValueInfoArray = attributeValueInfo.ToArray();
					for (int j = 0; j < attributeValueInfoArray.Length; j++)
					{
						AttributeValueInfo attributeValueInfo1 = attributeValueInfoArray[j];
						List<TypeAttrValue> attrValues = typeAttributesModel1.AttrValues;
						TypeAttrValue typeAttrValue = new TypeAttrValue()
						{
							Id = attributeValueInfo1.Id.ToString(),
							Name = attributeValueInfo1.Value
						};
						attrValues.Add(typeAttrValue);
					}
					categoryId = ServiceHelper.Create<IProductService>().GetProduct(productId).CategoryId;
					typeAttributesModels.Add(typeAttributesModel1);
				}
			}
			List<TypeAttributesModel> plateformAttr1 = GetPlateformAttr(categoryId);
			foreach (TypeAttributesModel item1 in typeAttributesModels)
			{
				item1.Selected = nums[item1.AttrId];
				plateformAttr1.Remove(plateformAttr1.FirstOrDefault((TypeAttributesModel a) => a.AttrId == item1.AttrId));
			}
			typeAttributesModels.AddRange(plateformAttr1);
			return Json(new { json = typeAttributesModels }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetAuthorizationCategory()
		{
			List<long> nums = new List<long>();
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IQueryable<CategoryInfo> businessCategory = ServiceHelper.Create<IShopCategoryService>().GetBusinessCategory(shopId);
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			foreach (CategoryInfo categoryInfo in businessCategory)
			{
				string[] strArrays = categoryInfo.Path.Split(new char[] { '|' });
				long num = long.Parse(strArrays.Length > 0 ? strArrays[0] : "0");
				long num1 = long.Parse(strArrays.Length > 1 ? strArrays[1] : "0");
				CategoryInfo category = categoryService.GetCategory(num);
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = category.Name,
					Id = category.Id.ToString(),
					SubCategory = new List<SecondLevelCategory>()
				};
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				SecondLevelCategory secondLevelCategory = null;
				ThirdLevelCategoty thirdLevelCategoty = null;
				CategoryInfo category1 = categoryService.GetCategory(num1);
				if (category1 != null)
				{
					SecondLevelCategory secondLevelCategory1 = new SecondLevelCategory()
					{
						Name = category1.Name,
						Id = category1.Id.ToString(),
						SubCategory = new List<ThirdLevelCategoty>()
					};
					secondLevelCategory = secondLevelCategory1;
					if (strArrays.Length >= 3)
					{
						ThirdLevelCategoty thirdLevelCategoty1 = new ThirdLevelCategoty()
						{
							Id = categoryInfo.Id.ToString(),
							Name = categoryInfo.Name
						};
						thirdLevelCategoty = thirdLevelCategoty1;
					}
					if (thirdLevelCategoty != null)
					{
						secondLevelCategory.SubCategory.Add(thirdLevelCategoty);
					}
				}
				CategoryJsonModel categoryJsonModel2 = categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString());
				if (categoryJsonModel2 != null && category1 != null && categoryJsonModel2.SubCategory.Any((SecondLevelCategory j) => j.Id == category1.Id.ToString()))
				{
					categoryJsonModel2.SubCategory.FirstOrDefault((SecondLevelCategory j) => j.Id == category1.Id.ToString()).SubCategory.Add(thirdLevelCategoty);
				}
				else if (categoryJsonModel2 != null && secondLevelCategory != null)
				{
					categoryJsonModel2.SubCategory.Add(secondLevelCategory);
				}
				else if (secondLevelCategory != null)
				{
					categoryJsonModel1.SubCategory.Add(secondLevelCategory);
				}
				if (categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString()) != null)
				{
					continue;
				}
				categoryJsonModels.Add(categoryJsonModel1);
			}
			return Json(new { json = categoryJsonModels }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult GetFreightTemplate()
		{
			IQueryable<FreightTemplateInfo> shopFreightTemplate = ServiceHelper.Create<IFreightTemplateService>().GetShopFreightTemplate(base.CurrentSellerManager.ShopId);
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = false,
				Text = "请选择运费模板...",
				Value = "0"
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			foreach (FreightTemplateInfo freightTemplateInfo in shopFreightTemplate)
			{
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Text = string.Concat(freightTemplateInfo.Name, "【", freightTemplateInfo.ValuationMethod.ToDescription(), "】"),
					Value = freightTemplateInfo.Id.ToString()
				};
				selectListItems1.Add(selectListItem1);
			}
			return Json(new { success = true, model = selectListItems1 });
		}

		private List<TypeAttributesModel> GetPlateformAttr(long categoryId)
		{
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(categoryId);
			ProductTypeInfo type = ServiceHelper.Create<ITypeService>().GetType(category.TypeId);
			List<TypeAttributesModel> typeAttributesModels = new List<TypeAttributesModel>();
			foreach (AttributeInfo attributeInfo in type.AttributeInfo)
			{
				TypeAttributesModel typeAttributesModel = new TypeAttributesModel()
				{
					Name = attributeInfo.Name,
					AttrId = attributeInfo.Id,
					Selected = "",
					IsMulti = attributeInfo.IsMulti,
					AttrValues = new List<TypeAttrValue>()
				};
				TypeAttributesModel typeAttributesModel1 = typeAttributesModel;
				foreach (AttributeValueInfo attributeValueInfo in attributeInfo.AttributeValueInfo)
				{
					List<TypeAttrValue> attrValues = typeAttributesModel1.AttrValues;
					TypeAttrValue typeAttrValue = new TypeAttrValue()
					{
						Id = attributeValueInfo.Id.ToString(),
						Name = attributeValueInfo.Value
					};
					attrValues.Add(typeAttrValue);
				}
				typeAttributesModels.Add(typeAttributesModel1);
			}
			return typeAttributesModels;
		}

		private SpecJosnModel GetPlatformSpec(long categoryId, long productId = 0L)
		{
			SpecJosnModel specJosnModel = new SpecJosnModel()
			{
				json = new List<TypeSpecificationModel>()
			};
			tableDataModel _tableDataModel = new tableDataModel()
			{
				cost = new List<SKUSpecModel>(),
				mallPrice = new List<SKUSpecModel>(),
				productId = productId,
				sku = new List<SKUSpecModel>(),
				stock = new List<SKUSpecModel>()
			};
			specJosnModel.tableData = _tableDataModel;
			SpecJosnModel specJosnModel1 = specJosnModel;
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(categoryId);
			ProductTypeInfo type = ServiceHelper.Create<ITypeService>().GetType(category.TypeId);
			foreach (object value in Enum.GetValues(typeof(SpecificationType)))
			{
				TypeSpecificationModel typeSpecificationModel = new TypeSpecificationModel()
				{
					Name = Enum.GetNames(typeof(SpecificationType))[(int)value - 1],
					Values = new List<Specification>(),
					SpecId = (int)value
                };
				TypeSpecificationModel typeSpecificationModel1 = typeSpecificationModel;
				foreach (SpecificationValueInfo specificationValueInfo in 
					from s in type.SpecificationValueInfo
					where (int)s.Specification == (int)value
					orderby s.Value
					select s)
				{
					List<Specification> values = typeSpecificationModel1.Values;
					Specification specification = new Specification()
					{
						Id = specificationValueInfo.Id.ToString(),
						Name = specificationValueInfo.Value,
						isPlatform = true,
						Selected = false
					};
					values.Add(specification);
				}
				specJosnModel1.json.Add(typeSpecificationModel1);
			}
			IOrderedQueryable<SKUInfo> sKUs = 
				from s in ServiceHelper.Create<IProductService>().GetSKUs(productId)
				orderby s.Color, s.Size, s.Version
				select s;
            InitialTableData(sKUs, specJosnModel1);
			return specJosnModel1;
		}

		private List<SKUInfo> GetProducrSpec(IQueryable<SKUInfo> skuList)
		{
			List<SKUInfo> sKUInfos = new List<SKUInfo>();
			foreach (SKUInfo sKUInfo in skuList)
			{
				string[] strArrays = (string.IsNullOrWhiteSpace(sKUInfo.Id) ? new string[] { "" } : sKUInfo.Id.Split(new char[] { '\u005F' }));
				List<SKUInfo> sKUInfos1 = sKUInfos;
				SKUInfo sKUInfo1 = new SKUInfo()
				{
					Color = strArrays.Length >= 2 ? strArrays[1] : "",
					Size = strArrays.Length >= 3 ? strArrays[2] : "",
					Version = strArrays.Length >= 4 ? strArrays[3] : "",
					Id = sKUInfo.Id
				};
				sKUInfos1.Add(sKUInfo1);
			}
			return sKUInfos;
		}

		public string GetQrCodeImagePath(long productId)
		{
			object[] authority = new object[] { "http://", base.HttpContext.Request.Url.Authority, "/m-wap/product/detail/", productId };
			Bitmap bitmap = QRCodeHelper.Create(string.Concat(authority));
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Gif);
			string str = string.Concat("data:image/gif;base64,", Convert.ToBase64String(memoryStream.ToArray()));
			return str;
		}

		[UnAuthorize]
		private List<CategoryJsonModel> GetShopCategoryJson(long shopId)
		{
			ShopCategoryInfo[] array = ServiceHelper.Create<IShopCategoryService>().GetShopCategory(shopId).ToArray();
			List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();
			foreach (ShopCategoryInfo shopCategoryInfo in 
				from s in array
                where s.ParentCategoryId == 0
                select s)
			{
				CategoryJsonModel categoryJsonModel = new CategoryJsonModel()
				{
					Name = shopCategoryInfo.Name,
					Id = shopCategoryInfo.Id.ToString(),
					SubCategory = new List<SecondLevelCategory>()
				};
				CategoryJsonModel categoryJsonModel1 = categoryJsonModel;
				foreach (ShopCategoryInfo shopCategoryInfo1 in 
					from s in array
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
				categoryJsonModels.Add(categoryJsonModel1);
			}
			return categoryJsonModels;
		}

		[UnAuthorize]
		public JsonResult GetShopProductCategory(long productId = 0L)
		{
			ShopProductCategoryModel shopProductCategoryModel = new ShopProductCategoryModel()
			{
				SelectedCategory = new List<SelectedCategory>(),
				Data = GetShopCategoryJson(shopId)
			};
			if (0 != productId)
			{
				IQueryable<ProductShopCategoryInfo> productShopCategories = ServiceHelper.Create<IProductService>().GetProductShopCategories(productId);
				foreach (CategoryJsonModel datum in shopProductCategoryModel.Data)
				{
					long num = long.Parse(datum.Id);
					if (productShopCategories.Any((ProductShopCategoryInfo c) => c.ShopCategoryId == num))
					{
						List<SelectedCategory> selectedCategory = shopProductCategoryModel.SelectedCategory;
						SelectedCategory selectedCategory1 = new SelectedCategory()
						{
							Id = datum.Id,
							Level = "1"
						};
						selectedCategory.Add(selectedCategory1);
					}
					foreach (SecondLevelCategory subCategory in datum.SubCategory)
					{
						num = long.Parse(subCategory.Id);
						if (!productShopCategories.Any((ProductShopCategoryInfo c) => c.ShopCategoryId == num))
						{
							continue;
						}
						List<SelectedCategory> selectedCategories = shopProductCategoryModel.SelectedCategory;
						SelectedCategory selectedCategory2 = new SelectedCategory()
						{
							Id = subCategory.Id,
							Level = "2"
						};
						selectedCategories.Add(selectedCategory2);
					}
				}
			}
			return Json(new { json = shopProductCategoryModel }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetSpecifications(long categoryId, long productId = 0L, long isCategoryId = 0L)
		{
			Dictionary<long, string> nums = new Dictionary<long, string>();
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(categoryId);
			SpecJosnModel platformSpec = GetPlatformSpec(categoryId, productId);
			IQueryable<SellerSpecificationValueInfo> sellerSpecifications = ServiceHelper.Create<IProductService>().GetSellerSpecifications(shopId, category.TypeId);
			IQueryable<SKUInfo> sKUs = ServiceHelper.Create<IProductService>().GetSKUs(productId);
			List<SKUInfo> producrSpec = GetProducrSpec(sKUs);
			foreach (TypeSpecificationModel typeSpecificationModel in platformSpec.json)
			{
				IQueryable<SellerSpecificationValueInfo> specification = 
					from s in sellerSpecifications
					where (int)s.Specification == typeSpecificationModel.SpecId
					select s;
				Specification value = null;
				foreach (SellerSpecificationValueInfo sellerSpecificationValueInfo in specification)
				{
					value = typeSpecificationModel.Values.FirstOrDefault((Specification s) => s.Id == sellerSpecificationValueInfo.ValueId.ToString());
					if (value != null && value.Id == sellerSpecificationValueInfo.ValueId.ToString())
					{
						value.Name = sellerSpecificationValueInfo.Value;
						value.isPlatform = false;
					}
					if (!sKUs.Any((SKUInfo s) => s.Color.Equals(sellerSpecificationValueInfo.Value)))
					{
						if (!sKUs.Any((SKUInfo s) => s.Size.Equals(sellerSpecificationValueInfo.Value)))
						{
							if (!sKUs.Any((SKUInfo s) => s.Version.Equals(sellerSpecificationValueInfo.Value)))
							{
								continue;
							}
						}
					}
					value.Selected = true;
				}
				foreach (Specification color in typeSpecificationModel.Values)
				{
					if (typeSpecificationModel.Name == "Color")
					{
						if (producrSpec.Any((SKUInfo s) => color.Id == s.Color))
						{
							SKUInfo sKUInfo = producrSpec.FirstOrDefault((SKUInfo s) => color.Id == s.Color);
							color.Name = sKUs.FirstOrDefault((SKUInfo p) => p.Id == sKUInfo.Id).Color;
							color.isPlatform = false;
							color.Selected = true;
						}
					}
					if (typeSpecificationModel.Name == "Size")
					{
						if (producrSpec.Any((SKUInfo s) => color.Id == s.Size))
						{
							SKUInfo sKUInfo1 = producrSpec.FirstOrDefault((SKUInfo s) => color.Id == s.Size);
							color.Name = sKUs.FirstOrDefault((SKUInfo p) => p.Id == sKUInfo1.Id).Size;
							color.isPlatform = false;
							color.Selected = true;
						}
					}
					if (typeSpecificationModel.Name != "Version")
					{
						continue;
					}
					if (!producrSpec.Any((SKUInfo s) => color.Id == s.Version))
					{
						continue;
					}
					SKUInfo sKUInfo2 = producrSpec.FirstOrDefault((SKUInfo s) => color.Id == s.Version);
					color.Name = sKUs.FirstOrDefault((SKUInfo p) => p.Id == sKUInfo2.Id).Version;
					color.isPlatform = false;
					color.Selected = true;
				}
			}
			return Json(new { data = platformSpec }, JsonRequestBehavior.AllowGet);
		}

		private string HTMLProcess(string content, string path)
		{
			string str = Path.Combine(path, "Details").Replace("\\", "/");
			string mapPath = IOHelper.GetMapPath(str);
			string str1 = Path.Combine(path, "Temp").Replace("\\", "/");
			string mapPath1 = IOHelper.GetMapPath(str1);
			try
			{
				string str2 = str;
				string mapPath2 = IOHelper.GetMapPath(str2);
				content = HtmlContentHelper.TransferToLocalImage(content, IOHelper.GetMapPath("/"), mapPath2, string.Concat(str2, "/"));
				content = HtmlContentHelper.RemoveScriptsAndStyles(content);
				if (Directory.Exists(mapPath1))
				{
					Directory.Delete(mapPath1, true);
				}
			}
			catch
			{
				if (Directory.Exists(mapPath1))
				{
					string[] files = Directory.GetFiles(mapPath1);
					for (int i = 0; i < files.Length; i++)
					{
						string str3 = files[i];
                        System.IO.File.Copy(str3, Path.Combine(mapPath, Path.GetFileName(str3)), true);
					}
					Directory.Delete(mapPath1, true);
				}
			}
			return content;
		}

		private void InitialTableData(IQueryable<SKUInfo> skus, SpecJosnModel data)
		{
			if (skus.Count() == 0)
			{
				return;
			}
			int num = 0;
			string version = "";
			SKUInfo[] array = skus.ToArray();
			if (!string.IsNullOrWhiteSpace(array[0].Version))
			{
				num = 2;
				version = array[0].Version;
			}
			if (!string.IsNullOrWhiteSpace(array[0].Size))
			{
				num = 1;
				version = array[0].Size;
			}
			if (!string.IsNullOrWhiteSpace(array[0].Color))
			{
				num = 0;
				version = array[0].Color;
			}
			if (string.IsNullOrWhiteSpace(version))
			{
				return;
			}
			SKUSpecModel sKUSpecModel = new SKUSpecModel()
			{
				ValueSet = new List<string>()
			};
			SKUSpecModel sKUSpecModel1 = new SKUSpecModel()
			{
				ValueSet = new List<string>()
			};
			SKUSpecModel sKUSpecModel2 = new SKUSpecModel()
			{
				ValueSet = new List<string>()
			};
			SKUSpecModel sKUSpecModel3 = new SKUSpecModel()
			{
				ValueSet = new List<string>()
			};
			foreach (SKUInfo sku in skus)
			{
				string color = "";
				switch (num)
				{
					case 0:
					{
						color = sku.Color;
						break;
					}
					case 1:
					{
						color = sku.Size;
						break;
					}
					case 2:
					{
						color = sku.Version;
						break;
					}
				}
				if (!color.Equals(version))
				{
					data.tableData.cost.Add(DeepClone(sKUSpecModel));
					data.tableData.stock.Add(DeepClone(sKUSpecModel1));
					data.tableData.sku.Add(DeepClone(sKUSpecModel2));
					data.tableData.mallPrice.Add(DeepClone(sKUSpecModel3));
					sKUSpecModel = new SKUSpecModel()
					{
						ValueSet = new List<string>()
					};
					sKUSpecModel1 = new SKUSpecModel()
					{
						ValueSet = new List<string>()
					};
					sKUSpecModel2 = new SKUSpecModel()
					{
						ValueSet = new List<string>()
					};
					sKUSpecModel3 = new SKUSpecModel()
					{
						ValueSet = new List<string>()
					};
					sKUSpecModel.ValueSet.Add((sku.CostPrice == new decimal(0) ? "" : sku.CostPrice.ToString("f2")));
					sKUSpecModel.index = color;
					sKUSpecModel1.ValueSet.Add((sku.Stock == 0 ? "" : sku.Stock.ToString("f2")));
					sKUSpecModel1.index = color;
					sKUSpecModel2.ValueSet.Add(sku.Sku);
					sKUSpecModel2.index = color;
					sKUSpecModel3.ValueSet.Add((sku.SalePrice == new decimal(0) ? "" : sku.SalePrice.ToString("f2")));
					sKUSpecModel3.index = color;
					version = color;
				}
				else
				{
					sKUSpecModel.ValueSet.Add((sku.CostPrice == new decimal(0) ? "" : sku.CostPrice.ToString("f2")));
					sKUSpecModel.index = color;
					sKUSpecModel1.ValueSet.Add((sku.Stock == 0 ? "" : sku.Stock.ToString("f2")));
					sKUSpecModel1.index = color;
					sKUSpecModel2.ValueSet.Add(sku.Sku);
					sKUSpecModel2.index = color;
					sKUSpecModel3.ValueSet.Add((sku.SalePrice == new decimal(0) ? "" : sku.SalePrice.ToString("f2")));
					sKUSpecModel3.index = color;
				}
			}
			data.tableData.cost.Add(DeepClone(sKUSpecModel));
			data.tableData.stock.Add(DeepClone(sKUSpecModel1));
			data.tableData.sku.Add(DeepClone(sKUSpecModel2));
			data.tableData.mallPrice.Add(DeepClone(sKUSpecModel3));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(long? categoryId, string productCode, string brandName, int? auditStatus, string auditStatuses, int? saleStatus, string ids, int page, int rows, string keyWords, DateTime? startDate, DateTime? endDate)
		{
			ProductInfo.ProductSaleStatus? nullable;
			IEnumerable<long> nums;
			ProductQuery productQuery = new ProductQuery();
			ProductQuery productQuery1 = productQuery;
			int? nullable1 = saleStatus;
			if (nullable1.HasValue)
			{
				nullable = new ProductInfo.ProductSaleStatus?((ProductInfo.ProductSaleStatus)nullable1.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			productQuery1.SaleStatus = nullable;
			productQuery.PageSize = rows;
			productQuery.PageNo = page;
			productQuery.BrandNameKeyword = brandName;
			productQuery.KeyWords = keyWords;
			productQuery.ShopCategoryId = categoryId;
			ProductQuery productQuery2 = productQuery;
			if (string.IsNullOrWhiteSpace(ids))
			{
				nums = null;
			}
			else
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in ids.Split(chrArray)
					select long.Parse(item);
			}
			productQuery2.Ids = nums;
			productQuery.ShopId = new long?(base.CurrentSellerManager.ShopId);
			productQuery.StartDate = startDate;
			productQuery.EndDate = endDate;
			productQuery.ProductCode = productCode;
			ProductQuery value = productQuery;
			if (!string.IsNullOrWhiteSpace(auditStatuses))
			{
				ProductQuery productQuery3 = value;
				char[] chrArray1 = new char[] { ',' };
				productQuery3.AuditStatus = 
					from item in auditStatuses.Split(chrArray1)
					select (ProductInfo.ProductAuditStatus)((int)long.Parse(item));
				if (auditStatuses == "1,3" || auditStatuses == "1")
				{
					value.SaleStatus = new ProductInfo.ProductSaleStatus?(ProductInfo.ProductSaleStatus.OnSale);
				}
			}
			if (saleStatus.HasValue)
			{
				int? nullable2 = saleStatus;
				if ((nullable2.GetValueOrDefault() != 2 ? false : nullable2.HasValue))
				{
					int? nullable3 = auditStatus;
					if ((nullable3.GetValueOrDefault() != 4 ? true : !nullable3.HasValue))
					{
						value.AuditStatus = new ProductInfo.ProductAuditStatus[] { ProductInfo.ProductAuditStatus.Audited, ProductInfo.ProductAuditStatus.UnAudit, ProductInfo.ProductAuditStatus.WaitForAuditing };
					}
				}
			}
			if (auditStatus.HasValue)
			{
				value.AuditStatus = new ProductInfo.ProductAuditStatus[] { (ProductInfo.ProductAuditStatus)auditStatus.Value };
			}
			PageModel<ProductInfo> products = ServiceHelper.Create<IProductService>().GetProducts(value);
			IShopCategoryService shopCategoryService = ServiceHelper.Create<IShopCategoryService>();
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			IEnumerable<ProductModel> list = (
				from item in products.Models.ToArray()
				select new ProductModel()
				{
					Name = item.ProductName,
					Id = item.Id,
					Image = item.GetImage(ProductInfo.ImageSize.Size_50, 1),
					Price = item.MinSalePrice,
					Url = "",
					PublishTime = item.AddedDate.ToString("yyyy-MM-dd HH:mm"),
					SaleState = (int)item.SaleStatus,
					CategoryId = item.CategoryId,
					BrandId = item.BrandId,
					AuditState = (int)item.AuditStatus,
					AuditReason = (item.ProductDescriptionInfo != null ? item.ProductDescriptionInfo.AuditReason : ""),
					ProductCode = item.ProductCode,
					QrCode = GetQrCodeImagePath(item.Id),
					SaleCount = item.SaleCounts,
					Unit = item.MeasureUnit,
					Uid = base.CurrentSellerManager.Id
				}).ToList();
			IEnumerable<ProductModel> productModels = list.Select<ProductModel, ProductModel>((ProductModel item) => {
				BrandInfo brand = brandService.GetBrand(item.BrandId);
				ShopCategoryInfo categoryByProductId = shopCategoryService.GetCategoryByProductId(item.Id);
				return new ProductModel()
				{
					Name = item.Name,
					Id = item.Id,
					Image = item.Image,
					Price = item.Price,
					Url = "",
					PublishTime = item.PublishTime,
					SaleState = item.SaleState,
					AuditState = item.AuditState,
					AuditReason = (item.AuditReason != null ? item.AuditReason : ""),
					ProductCode = item.ProductCode,
					QrCode = GetQrCodeImagePath(item.Id),
					SaleCount = item.SaleCount,
					Unit = item.Unit,
					Uid = CurrentSellerManager.Id,
					CategoryName = (categoryByProductId == null ? "" : categoryByProductId.Name),
					BrandName = (item.BrandId == 0 || brand == null ? "" : brand.Name)
				};
			}).ToList();
			DataGridModel<ProductModel> dataGridModel = new DataGridModel<ProductModel>()
			{
				rows = productModels,
				total = products.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			int produtAuditOnOff = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().ProdutAuditOnOff;
			ViewBag.AuditOnOff = produtAuditOnOff;
			return View();
		}

		private void ProcessSKU(ProductDetailModel m, ProductInfo result)
		{
			if (m.specificationsValue == null || m.specificationsValue.Count == 0)
			{
				m.specificationsValue = new List<SpecificationsValue>();
				List<SpecificationsValue> specificationsValues = m.specificationsValue;
				SpecificationsValue specificationsValue = new SpecificationsValue()
				{
					colorId = "",
					colorName = "",
					sizeId = "",
					sizeName = "",
					versionId = "",
					versionName = "",
					cost = m.cost,
					mallPrice = m.mallPrce,
					sku = "",
					stock = m.stock.ToString()
				};
				specificationsValues.Add(specificationsValue);
			}
			foreach (SpecificationsValue specificationsValue1 in m.specificationsValue)
			{
				decimal num = new decimal(0);
				decimal num1 = new decimal(0);
				decimal num2 = new decimal(0);
				decimal.TryParse(specificationsValue1.cost, out num);
				decimal.TryParse(specificationsValue1.stock, out num1);
				decimal.TryParse(specificationsValue1.mallPrice, out num2);
				ICollection<SKUInfo> sKUInfo = result.SKUInfo;
				SKUInfo sKUInfo1 = new SKUInfo()
				{
					Color = specificationsValue1.colorName,
					Size = specificationsValue1.sizeName,
					Version = specificationsValue1.versionName,
					CostPrice = num,
					ProductId = result.Id,
					SalePrice = num2,
					Sku = specificationsValue1.sku.ToString(),
					Stock = (long)num1
				};
				SKUInfo sKUInfo2 = sKUInfo1;
				object[] id = new object[] { result.Id, null, null, null };
				id[1] = (string.IsNullOrWhiteSpace(specificationsValue1.colorId) ? "0" : specificationsValue1.colorId);
				id[2] = (string.IsNullOrWhiteSpace(specificationsValue1.sizeId) ? "0" : specificationsValue1.sizeId);
				id[3] = (string.IsNullOrWhiteSpace(specificationsValue1.versionId) ? "0" : specificationsValue1.versionId);
				sKUInfo2.Id = string.Format("{0}_{1}_{2}_{3}", id);
				sKUInfo.Add(sKUInfo1);
			}
		}

		public ActionResult PublicStepOne()
		{
			return View();
		}

		[UnAuthorize]
		public ActionResult PublicStepTwo(string categoryNames = "", long categoryId = 0L, long productId = 0L)
		{
			long num;
			long num1;
			string str;
			string str1;
			IProductService productService = ServiceHelper.Create<IProductService>();
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			string str2 = "0";
			string str3 = "0";
			string str4 = "0";
			ProductInfo productInfo = new ProductInfo();
           // productId = productInfo.Id;
			if (productId != 0)
			{
				productInfo = productService.GetProduct(productId);
				if (productInfo == null || productInfo.ShopId != base.CurrentSellerManager.ShopId)
				{
					throw new HimallException(string.Concat(productId, ",该商品已删除或者不属于该店铺"));
				}
				if (productInfo.SKUInfo.Count() > 0)
				{
					IEnumerable<SKUInfo> sKUInfo = 
						from s in productInfo.SKUInfo
						where s.SalePrice > new decimal(0)
						select s;
					IEnumerable<SKUInfo> sKUInfos = 
						from s in productInfo.SKUInfo
						where s.CostPrice > new decimal(0)
						select s;
					if (sKUInfo.Count() == 0)
					{
						str = productInfo.MinSalePrice.ToString("f3");
					}
					else
					{
						decimal num2 = sKUInfo.Min<SKUInfo>((SKUInfo s) => s.SalePrice);
						str = num2.ToString();
					}
					str2 = str;
					long num3 = productInfo.SKUInfo.Sum<SKUInfo>((SKUInfo s) => s.Stock);
					str3 = num3.ToString();
					if (sKUInfos.Count() == 0)
					{
						str1 = str4;
					}
					else
					{
						decimal num4 = sKUInfos.Min<SKUInfo>((SKUInfo s) => s.CostPrice);
						str1 = num4.ToString();
					}
					str4 = str1;
				}
				if (string.IsNullOrWhiteSpace(categoryNames))
				{
					string[] strArrays = productInfo.CategoryPath.Split(new char[] { '|' });
					for (int i = 0; i < strArrays.Length; i++)
					{
						if (!string.IsNullOrWhiteSpace(strArrays[i]))
						{
							CategoryInfo category = categoryService.GetCategory(long.Parse(strArrays[i]));
							categoryNames = string.Concat(categoryNames, string.Format("{0} {1} ", (category == null ? "" : category.Name), (i == strArrays.Length - 1 ? "" : " > ")));
						}
					}
				}
				if (categoryId == 0)
				{
					categoryId = productInfo.CategoryId;
				}
			}
			ServiceHelper.Create<ITypeService>().GetType(categoryId);
			IBrandService brandService = ServiceHelper.Create<IBrandService>();
			long num5 = shopId;
			long[] numArray = new long[] { categoryId };
			IEnumerable<BrandInfo> brandsByCategoryIds = brandService.GetBrandsByCategoryIds(num5, numArray);
			List<SelectListItem> selectListItems = new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Selected = false,
				Text = "请选择品牌...",
				Value = "0"
			};
			selectListItems.Add(selectListItem);
			List<SelectListItem> selectListItems1 = selectListItems;
			foreach (BrandInfo brandsByCategoryId in brandsByCategoryIds)
			{
				List<SelectListItem> selectListItems2 = selectListItems1;
				SelectListItem selectListItem1 = new SelectListItem()
				{
					Selected = (productId == 0 || productInfo == null ? false : productInfo.BrandId == brandsByCategoryId.Id),
					Text = brandsByCategoryId.Name,
					Value = brandsByCategoryId.Id.ToString()
				};
				selectListItems2.Add(selectListItem1);
			}
			IQueryable<FreightTemplateInfo> shopFreightTemplate = ServiceHelper.Create<IFreightTemplateService>().GetShopFreightTemplate(base.CurrentSellerManager.ShopId);
			List<SelectListItem> selectListItems3 = new List<SelectListItem>();
			SelectListItem selectListItem2 = new SelectListItem()
			{
				Selected = false,
				Text = "请选择运费模板...",
				Value = "0"
			};
			selectListItems3.Add(selectListItem2);
			List<SelectListItem> selectListItems4 = selectListItems3;
			foreach (FreightTemplateInfo freightTemplateInfo in shopFreightTemplate)
			{
				List<SelectListItem> selectListItems5 = selectListItems4;
				SelectListItem selectListItem3 = new SelectListItem()
				{
					Selected = (productId == 0 || productInfo == null ? false : productInfo.FreightTemplateId == freightTemplateInfo.Id),
					Text = string.Concat(freightTemplateInfo.Name, "【", freightTemplateInfo.ValuationMethod.ToDescription(), "】"),
					Value = freightTemplateInfo.Id.ToString()
				};
				selectListItems5.Add(selectListItem3);
			}
			ViewBag.FreightTemplates = selectListItems4;
			ViewBag.BrandDrop = selectListItems1;
			dynamic viewBag = base.ViewBag;
			num = 0 == productInfo.Id ? 0 : productInfo.ProductDescriptionInfo.DescriptionPrefixId;
			viewBag.TopId = num;
			dynamic obj = base.ViewBag;
			num1 = 0 == productInfo.Id ? 0 : productInfo.ProductDescriptionInfo.DescriptiondSuffixId;
			obj.BottomId = num1;
			ViewBag.CategoryNames = categoryNames;
			ViewBag.CategoryId = categoryId;
			ViewBag.ProductId = productId;
			base.ViewBag.IsCategory = (productId == 0 ? 1 : 0);
			ViewBag.ShopId = shopId;
			ViewBag.SalePrice = str2;
			ViewBag.Stock = str3;
			ViewBag.CostPrice = str4;
			return View(productInfo);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult SaveProductDetail(string productDetail)
		{
         //   productDetail = "{\"saleStatus\":1,\"productId\":0,\"specificationsValue\":[],\"specifications\":[],\"categoryId\":\"48\",\"brandId\":\"0\",\"goodsName\":\"大风车\",\"adWord\":\"\",\"mallPrce\":\"12\",\"marketPrice\":\"13\",\"cost\":\"0.00\",\"rebate\":\"92.31\",\"stock\":\"555\",\"productCode\":\"\",\"goodsCategory\":[334],\"attrSelectData\":[{\"isPlatform\":false,\"AttrId\":10,\"valueId\":\"\"},{\"isPlatform\":false,\"AttrId\":11,\"valueId\":\"\"},{\"isPlatform\":false,\"AttrId\":12,\"valueId\":\"\"},{\"isPlatform\":false,\"AttrId\":70,\"valueId\":\"\"}],\"pic\":[\"/temp/201607271334267704210.jpg\",\"/2.png\",\"/3.png\",\"/4.png\",\"/5.png\"],\"des\":\"<p>22<br/></p>\",\"mdes\":\"\",\"styleTemplateId\":[0,0],\"seoTitle\":\" \",\"seoKey\":\" \",\"seoDes\":\" \",\"MeasureUnit\":\"个\",\"Volume\":\"0\",\"Weight\":\"0\",\"FreightTemplateId\":\"148\",\"DisableBuy\":\"False\"}";
			IProductService productService = ServiceHelper.Create<IProductService>();
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			ProductDetailModel productDetailModel = JsonConvert.DeserializeObject<ProductDetailModel>(productDetail);
			long num = productDetailModel.productId;
			ProductInfo productInfo = ProductDetailModel.GetProductInfo(productDetailModel, num);
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			int shopAllProducts = productService.GetShopAllProducts(base.CurrentSellerManager.ShopId);
			ProductInfo.ProductEditStatus editStatus = productService.GetEditStatus(num, productInfo);
			foreach (string str in productDetailModel.pic)
			{
				if (str.IndexOf("Storage") >= 0)
				{
					continue;
				}
				editStatus = (editStatus <= ProductInfo.ProductEditStatus.EditedAndPending ? ProductInfo.ProductEditStatus.EditedAndPending : ProductInfo.ProductEditStatus.CompelPendingHasEdited);
			}
			productInfo.EditStatus = (short)editStatus;
			if (productDetailModel.productId != 0)
			{
				productInfo.ShopId = shopId;
				productInfo.ImagePath = string.Format("/Storage/Shop/{0}/Products/{1}", shopId, productInfo.Id);
				productInfo.CategoryPath = categoryService.GetCategory(productInfo.CategoryId).Path;
				productService.UpdateProduct(productInfo);
			}
			else
			{
				productInfo.ShopId = base.CurrentSellerManager.ShopId;
				if (productDetailModel.specificationsValue.Count > 0)
				{
					productInfo.MinSalePrice = decimal.Parse(productDetailModel.specificationsValue.Min<SpecificationsValue, string>((SpecificationsValue p) => p.mallPrice));
				}
				productInfo.ImagePath = string.Format("/Storage/Shop/{0}/Products/{1}", shopId, productInfo.Id);
				productInfo.CategoryPath = categoryService.GetCategory(productInfo.CategoryId).Path;
				if (ServiceHelper.Create<IShopService>().GetShopSpaceUsage(base.CurrentSellerManager.ShopId) == -1)
				{
					return Json(new { successful = false, msg = "数据提交失败。 原因：店铺存储图片空间不足,不能发布商品!" });
				}
				ShopGradeInfo shopGrade = ServiceHelper.Create<IShopService>().GetShopGrade(shop.GradeId);
				if (shopGrade != null && shopAllProducts >= shopGrade.ProductLimit)
				{
					return Json(new { successful = false, msg = string.Concat("数据提交失败。 原因：此店铺等级最多只能发布", shopGrade.ProductLimit, "件商品") });
				}
				productService.AddProduct(productInfo);
				string str1 = string.Format("/Storage/Shop/{0}/Products/{1}", shopId, productInfo.Id);
				productService.UpdateProductImagePath(productInfo.Id, str1);
			}
            ProcessSKU(productDetailModel, productInfo);
			productService.AddSKU(productInfo);
			productService.GetSellerSpecifications(shopId, categoryService.GetCategory(productDetailModel.categoryId).TypeId);
			CategoryInfo category = categoryService.GetCategory(productDetailModel.categoryId);
			List<SellerSpecificationValueInfo> sellerSpecificationValueInfos = new List<SellerSpecificationValueInfo>();
			foreach (Specifications specification in productDetailModel.specifications)
			{
				if (!specification.selected)
				{
					continue;
				}
				SellerSpecificationValueInfo sellerSpecificationValueInfo = new SellerSpecificationValueInfo()
				{
					ShopId = shopId,
					ValueId = specification.Id,
					Value = specification.newValue,
					TypeId = category.TypeId
				};
				sellerSpecificationValueInfos.Add(sellerSpecificationValueInfo);
			}
			productService.SaveSellerSpecifications(sellerSpecificationValueInfos);
			int num1 = 1;
			foreach (string str2 in productDetailModel.pic)
			{
				string str3 = Server.MapPath(str2);//图片位置
				string str4 = Server.MapPath(string.Format("/Storage/Shop/{0}/Products/{1}", shopId, productInfo.Id));
				if (string.IsNullOrWhiteSpace(str2))
				{
					string str5 = string.Format("{0}\\{1}.png", str4, num1);
					if (System.IO.File.Exists(str5))
					{
                        System.IO.File.Delete(str5);
					}
					IEnumerable<int> dictionary = 
						from t in EnumHelper.ToDictionary<ProductInfo.ImageSize>()
						select t.Key;
					foreach (int num2 in dictionary)
					{
						string str6 = string.Format("{0}/{1}_{2}.png", str4, num1, num2);
						if (!System.IO.File.Exists(str6))
						{
							continue;
						}
                        System.IO.File.Delete(str6);
					}
					num1++;
				}
				else
				{
					try
					{
						if (!Directory.Exists(str4))
						{
							Directory.CreateDirectory(str4);
						}
						string str7 = string.Format("{0}\\{1}.png", str4, num1);//生成图片的位置和名字
						if (str3 != str7)
						{
                            using (Image image = Image.FromFile(str3))//从temp位置把图片eg:/temp/201607201045475671970.jpg 生成到/Storage/Shop/shopid/Products/productId/ 文件夹下
							{
								image.Save(str7, ImageFormat.Png);
								IEnumerable<int> nums = 
									from t in EnumHelper.ToDictionary<ProductInfo.ImageSize>()
									select t.Key;
                                foreach (int num3 in nums)//根据ProductInfo.ImageSize的枚举值生成五张小图
								{
									string str8 = string.Format("{0}/{1}_{2}.png", str4, num1, num3);
									ImageHelper.CreateThumbnail(str7, str8, num3, num3);
								}
							}
							num1++;
						}
						else
						{
							num1++;
						}
					}
					catch (FileNotFoundException fileNotFoundException1)
					{
						FileNotFoundException fileNotFoundException = fileNotFoundException1;
						num1++;
						Log.Error("发布商品时候，没有找到文件", fileNotFoundException);
					}
					catch (ExternalException externalException1)
					{
						ExternalException externalException = externalException1;
						num1++;
						Log.Error("发布商品时候，ExternalException异常", externalException);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						num1++;
						Log.Error("发布商品时候，Exception异常", exception);
					}
				}
			}
			IOperationLogService operationLogService = ServiceHelper.Create<IOperationLogService>();
			LogInfo logInfo = new LogInfo()
			{
				Date = DateTime.Now,
				Description = string.Concat("商家发布商品，名称=", productInfo.ProductName),
				IPAddress = base.Request.UserHostAddress,
				PageUrl = "/Product/SaveProductDetail",
				UserName = base.CurrentSellerManager.UserName,
				ShopId = base.CurrentSellerManager.ShopId
			};
            
			operationLogService.AddSellerOperationLog(logInfo);
			return Json(new { successful = true });
		}
	}
}