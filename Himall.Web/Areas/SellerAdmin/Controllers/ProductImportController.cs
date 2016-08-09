using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
    public class ProductImportController : BaseSellerController
    {
        private long _shopid;

        private long _userid;

        private long shopId = 2;
        public ProductImportController()
        {
            _shopid = base.CurrentSellerManager.ShopId;
            _userid = base.CurrentSellerManager.Id;
            shopId = base.CurrentSellerManager.ShopId;
        }

        public JsonResult GetImportCount()
        {
            long num = 0;
            long num1 = 0;
            int num2 = 0;
            GetImportCountFromCache(out num, out num1);
            if (num1 == num && num1 > 0)
            {
                num2 = 1;
            }
            return Json(new { Count = num, Total = num1, Success = num2 }, JsonRequestBehavior.AllowGet);
        }

        private void GetImportCountFromCache(out long count, out long total)
        {
            object obj = Cache.Get(CacheKeyCollection.UserImportProductCount(_userid));
            object obj1 = Cache.Get(CacheKeyCollection.UserImportProductTotal(_userid));
            count = (obj == null ? 0 : long.Parse(obj.ToString()));
            total = (obj1 == null ? 0 : long.Parse(obj1.ToString()));
            if (count == total && total > 0)
            {
                Cache.Remove(CacheKeyCollection.UserImportProductCount(_userid));
                Cache.Remove(CacheKeyCollection.UserImportProductTotal(_userid));
            }
        }

        public JsonResult GetImportOpCount()
        {
            long num = 0;
            object obj = Cache.Get("Cache-UserImportOpCount");
            if (obj != null)
            {
                num = (string.IsNullOrEmpty(obj.ToString()) ? 0 : long.Parse(obj.ToString()));
            }
            return Json(new { Count = num }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [UnAuthorize]
        public JsonResult GetPlatFormCategory(long? key = null, int? level = -1)
        {
            int? nullable = level;
            if ((nullable.GetValueOrDefault() != -1 ? false : nullable.HasValue))
            {
                key = new long?(0);
            }
            if (!key.HasValue)
            {
                return Json(new object[0]);
            }
            IEnumerable<CategoryInfo> categoryByParentId = ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(key.Value);
            long? nullable1 = key;
            if ((nullable1.GetValueOrDefault() != 0 ? false : nullable1.HasValue))
            {
                IShopService shopService = ServiceHelper.Create<IShopService>();
                if (!(shopService.GetShop(base.CurrentSellerManager.ShopId, false) ?? new ShopInfo()).IsSelf)
                {
                    IQueryable<long> businessCategory =
                        from e in shopService.GetBusinessCategory(base.CurrentSellerManager.ShopId)
                        select e.CategoryId;
                    categoryByParentId = ServiceHelper.Create<ICategoryService>().GetTopLevelCategories(businessCategory);
                }
            }
            IEnumerable<KeyValuePair<long, string>> keyValuePair =
                from item in categoryByParentId
                select new KeyValuePair<long, string>(item.Id, item.Name);
            return Json(keyValuePair);
        }


        [HttpPost]
        [UnAuthorize]
        public JsonResult GetShopBrand(long categoryId)
        {
            IBrandService brandService = ServiceHelper.Create<IBrandService>();
            long shopId = base.CurrentSellerManager.ShopId;
            long[] numArray = new long[] { categoryId };
            IEnumerable<KeyValuePair<long, string>> brandsByCategoryIds =
                from item in brandService.GetBrandsByCategoryIds(shopId, numArray)
                select new KeyValuePair<long, string>(item.Id, item.Name);
            return Json(brandsByCategoryIds);
        }

        public ActionResult ImportManage()
        {
            long num = 0;
            long num1 = 0;
            int num2 = 0;
            GetImportCountFromCache(out num, out num1);
            if (num1 == num && num1 > 0)
            {
                num2 = 1;
            }
            ViewBag.Count = num;
            ViewBag.Total = num1;
            ViewBag.Success = num2;
            ViewBag.shopid = _shopid;
            ViewBag.userid = _userid;
            return View();
        }
        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }
        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
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
        [HttpPost]
        public JsonResult ImportProductsFromExcel(long shopCategoyId, string filename)
        {
            
            /*供应商产品批量导入*/
            if (!string.IsNullOrWhiteSpace(filename))
            {
                string serverFilepath = Server.MapPath("/temp/");
                string fullFilePath = serverFilepath + filename;
                FileInfo file = new FileInfo(fullFilePath);
                /*计数统计*/
                int ErrorCount = 0, SuccessCount = 0;
                if (file != null && file.Length > 0)
                {
                    //_importManager.ImportProductsFromXlsx(file.InputStream);
                    // ok, we can run the real code of the sample now
                    using (var xlPackage = new ExcelPackage(file))
                    {
                        // get the first worksheet in the workbook
                        var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = 6 });//文件为空
                        }                
                        //the columns
                        var properties = new[]
                        {
                           "CategoryId",
                           "ProductName",	
                           "ProductCode",	
                           "ShortDescription",	
                           "Pictures",	
                           "MarketPrice",
                           "MinSalePrice",	
                           "FreightTemplateId",	
                           "Weight",	
                           "Volume",	
                           "Quantity",	
                           "MeasureUnit",	
                           "Description",	
                           "Meta_Title",
                           "Meta_Description",	
                           "Meta_Keywords"
                        };
                        int iRow = 3;//从Excel第三行开始读取
                        while (true)
                        {
                            int errorTemp=0;
                            bool allColumnsAreEmpty = true;
                            for (var i = 1; i <= properties.Length; i++)
                                if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                                {
                                    allColumnsAreEmpty = false;
                                    break;
                                }
                            if (allColumnsAreEmpty)
                                break;

                            long categoryId = Convert.ToInt64(worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryId")].Value);
                            string productName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductName")].Value);
                            string productCode = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductCode")].Value);
                            string shortDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ShortDescription")].Value);
                            string pictures = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "pictures")].Value);
                            decimal marketPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MarketPrice")].Value);
                            decimal minSalePrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MinSalePrice")].Value);
                            long freightTemplateId = Convert.ToInt64(worksheet.Cells[iRow, GetColumnIndex(properties, "FreightTemplateId")].Value);
                            decimal weight = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Weight")].Value);
                            decimal volume = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Volume")].Value);
                            int quantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "Quantity")].Value);
                            string measureUnit = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MeasureUnit")].Value);
                            string description = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Description")].Value);
                            string meta_Title = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Meta_Title")].Value);
                            string meta_Description = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Meta_Description")].Value);
                            string meta_Keywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Meta_Keywords")].Value);

                            ProductDetailModel productDetailModel = new ProductDetailModel();
                            productDetailModel.adWord = shortDescription;
                            productDetailModel.saleStatus = 1;
                            productDetailModel.brandId = 0;
                            productDetailModel.productId = 0;
                            productDetailModel.categoryId = categoryId;
                            if (!string.IsNullOrWhiteSpace(description))
                            {
                                productDetailModel.des ="<p>"+ description+"</p>";
                            }
                            productDetailModel.FreightTemplateId = freightTemplateId;
                            productDetailModel.goodsName = productName;
                            productDetailModel.mallPrce = minSalePrice.ToString();
                            productDetailModel.marketPrice = marketPrice.ToString();
                            productDetailModel.MeasureUnit = measureUnit;
                            productDetailModel.productCode = productCode;
                            productDetailModel.seoDes = meta_Description;
                            productDetailModel.seoKey = meta_Keywords;
                            productDetailModel.seoTitle = meta_Title;
                            productDetailModel.stock = quantity;
                            productDetailModel.Weight = weight;
                            productDetailModel.Volume = volume;
                            productDetailModel.styleTemplateId = new List<long>{0,0};
                            productDetailModel.goodsCategory = new List<long> { shopCategoyId };
                            if (pictures != null && pictures.Length > 4)
                            {
                                string[] picArray = pictures.Split(new char[] { ',' });
                                productDetailModel.pic = new List<string>(picArray);
                            }
                            else
                            {
                                productDetailModel.pic = new List<string>();
                            }
                            List<AttrSelectData> attrSelectDataList = new List<AttrSelectData> { 
                                new AttrSelectData{attrId=10,valueId=""},
                                new AttrSelectData{attrId=11, valueId=""}
                            };
                            productDetailModel.attrSelectData = attrSelectDataList;
                            productDetailModel.specifications = new List<Specifications>();
                            productDetailModel.specificationsValue = new List<SpecificationsValue>();
                            IProductService productService = ServiceHelper.Create<IProductService>();
                            ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();

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
                            #region 生成图片
                            foreach (string str2 in productDetailModel.pic)
                            {
                                //string str3 = Server.MapPath(string.Concat(@"/temp/", str2));//图片位置
                                string str3 = Server.MapPath(string.Format("/Storage/Original/Shop/{0}/{1}", shopId, str2));
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
                                        if (System.IO.File.Exists(str3))
                                        {
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
                                    }
                                    catch (FileNotFoundException fileNotFoundException1)
                                    {
                                        
                                        FileNotFoundException fileNotFoundException = fileNotFoundException1;
                                        num1++;
                                        ErrorCount++;
                                        errorTemp++;
                                        Log.Error("发布商品时候，没有找到文件", fileNotFoundException);
                                    }
                                    catch (ExternalException externalException1)
                                    {
                                        ExternalException externalException = externalException1;
                                        num1++;
                                        ErrorCount++;
                                        errorTemp++;
                                        Log.Error("发布商品时候，ExternalException异常", externalException);
                                    }
                                    catch (Exception exception1)
                                    {
                                        Exception exception = exception1;
                                        num1++;
                                        ErrorCount++;
                                        errorTemp++;
                                        Log.Error("发布商品时候，Exception异常", exception);
                                    }
                                }
                            }
                            #endregion
                            //next product
                            iRow++;
                            if(errorTemp==0)
                            {
                                SuccessCount++;
                            }
                        }
                        
                    }

                    return Json(new
                    {
                        success = true,
                        message = 1,
                        ErrorCount = ErrorCount,
                        SuccessCount = SuccessCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = 6 });//"文件为空"
                }
            }
            else
            {
                return Json(new { success = false, message = 6 });//"没有找到文件！"
            }
        }
    }
}