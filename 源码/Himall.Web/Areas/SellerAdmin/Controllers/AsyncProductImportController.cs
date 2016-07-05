using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class AsyncProductImportController : BaseAsyncController
	{
		public AsyncProductImportController()
		{
		}

		public void ImportProductAsync(long paraCategory, long paraShopCategory, long? paraBrand, int paraSaleStatus, long _shopid, long _userid, string file)
		{
			base.AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() => {
				string str = Server.MapPath(string.Concat("/temp/", file));
				string str1 = string.Format("/Storage/Shop/{0}/Products", _shopid);
				string str2 = Server.MapPath(str1);
				long value = 0;
				if (paraBrand.HasValue)
				{
					value = paraBrand.Value;
				}
				JsonResult jsonResult = new JsonResult();
				if (!System.IO.File.Exists(str))
				{
                    AsyncManager.Parameters["success"] = false;
                    AsyncManager.Parameters["message"] = "上传文件不存在";
				}
				else
				{
					ZipHelper.ZipInfo zipInfo = ZipHelper.UnZipFile(str);
					if (!zipInfo.Success)
					{
						Log.Error(string.Concat("解压文件异常：", zipInfo.InfoMessage));
                        AsyncManager.Parameters["success"] = false;
                        AsyncManager.Parameters["message"] = "解压出现异常,请检查压缩文件格式";
					}
					else
					{
						try
						{
							int num = ProcessProduct(paraCategory, paraShopCategory, value, paraSaleStatus, _shopid, _userid, zipInfo.UnZipPath, str1, str2);
							if (num <= 0)
							{
								Cache.Remove(CacheKeyCollection.UserImportProductCount(_userid));
								Cache.Remove(CacheKeyCollection.UserImportProductTotal(_userid));
                                AsyncManager.Parameters["success"] = false;
                                AsyncManager.Parameters["message"] = "导入【0】件商品，请检查数据包格式，或是否重复导入";
							}
							else
							{
                                AsyncManager.Parameters["success"] = true;
                                AsyncManager.Parameters["message"] = string.Concat("成功导入【", num.ToString(), "】件商品");
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							Log.Error(string.Concat("导入商品异常：", exception.Message));
							Cache.Remove(CacheKeyCollection.UserImportProductCount(_userid));
							Cache.Remove(CacheKeyCollection.UserImportProductTotal(_userid));
                            AsyncManager.Parameters["success"] = false;
                            AsyncManager.Parameters["message"] = string.Concat("导入商品异常:", exception.Message);
						}
					}
				}
                AsyncManager.OutstandingOperations.Decrement();
				object obj = Cache.Get("Cache-UserImportOpCount");
				if (obj != null)
				{
					Cache.Insert("Cache-UserImportOpCount", int.Parse(obj.ToString()) - 1);
				}
			});
		}

		public JsonResult ImportProductCompleted(bool success, string message)
		{
			return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
		}

		private void ImportProductImg(long pid, long _shopid, string path, string filenames)
		{
			Func<KeyValuePair<int, string>, int> key = null;
			string str1 = path;
			str1 = str1.Replace(Path.GetExtension(str1), string.Empty);
			filenames = filenames.Replace("\"", string.Empty);
			string str2 = Server.MapPath(string.Format("/Storage/Shop/{0}/Products/{1}", _shopid, pid));
			string[] files = new string[0];
			string empty = string.Empty;
			int length = 0;
			char[] chrArray = new char[] { ';' };
			filenames.Split(chrArray).ToList().ForEach((string item) => {
				if (item != string.Empty)
				{
					string[] strArrays = item.Split(new char[] { ':' });
					if (strArrays.Length > 0)
					{
						files = Directory.GetFiles(str1, string.Concat(strArrays[0], ".*"), SearchOption.AllDirectories);
						if (!Directory.Exists(str2))
						{
							Directory.CreateDirectory(str2);
						}
						else
						{
							length = Directory.GetFiles(str2, "*.png").Length;
						}
						for (int i = 0; i < files.Length; i++)
						{
							try
							{
								string str = string.Format("{0}\\{1}.png", str2, i + 1);
								using (Image image = Image.FromFile(files[i]))
								{
									image.Save(str, ImageFormat.Png);
									Dictionary<int, string> dictionary = EnumHelper.ToDictionary<ProductInfo.ImageSize>();
									if (key == null)
									{
										key = (KeyValuePair<int, string> t) => t.Key;
									}
									foreach (int num in dictionary.Select<KeyValuePair<int, string>, int>(key))
									{
										ImageHelper.CreateThumbnail(str, string.Format("{0}/{1}_{2}.png", str2, i + 1, num), num, num);
									}
								}
							}
							catch (FileNotFoundException fileNotFoundException)
							{
								Log.Error("导入商品处理图片时，没有找到文件", fileNotFoundException);
							}
							catch (ExternalException externalException)
							{
								Log.Error("导入商品处理图片时，ExternalException异常", externalException);
							}
							catch (Exception exception)
							{
								Log.Error("导入商品处理图片时，Exception异常", exception);
							}
						}
					}
				}
			});
		}

		[HttpGet]
		[UnAuthorize]
		public JsonResult ImportProductJson(long paraCategory, long paraShopCategory, long? paraBrand, int paraSaleStatus, long _shopid, long _userid, string file)
		{
			string str = Server.MapPath(string.Concat("/temp/", file));
			string str1 = string.Format("/Storage/Shop/{0}/Products", _shopid);
			string str2 = Server.MapPath(str1);
			long value = 0;
			if (paraBrand.HasValue)
			{
				value = paraBrand.Value;
			}
			JsonResult jsonResult = new JsonResult();
			if (!System.IO.File.Exists(str))
			{
				jsonResult = Json(new { success = false, message = "上传文件不存在" }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				ZipHelper.ZipInfo zipInfo = ZipHelper.UnZipFile(str);
				if (!zipInfo.Success)
				{
					Log.Error(string.Concat("解压文件异常：", zipInfo.InfoMessage));
					jsonResult = Json(new { success = false, message = "解压出现异常,请检查压缩文件格式" }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					try
					{
						int num = ProcessProduct(paraCategory, paraShopCategory, value, paraSaleStatus, _shopid, _userid, zipInfo.UnZipPath, str1, str2);
						jsonResult = (num <= 0 ? Json(new { success = false, message = "导入【0】件商品，请检查数据包，是否是重复导入" }, JsonRequestBehavior.AllowGet) : Json(new { success = true, message = string.Concat("成功导入【", num.ToString(), "】件商品") }, JsonRequestBehavior.AllowGet));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						Log.Error(string.Concat("导入商品异常：", exception.Message));
						Cache.Remove(CacheKeyCollection.UserImportProductCount(_userid));
						Cache.Remove(CacheKeyCollection.UserImportProductTotal(_userid));
						jsonResult = Json(new { success = false, message = string.Concat("导入商品异常:", exception.Message) }, JsonRequestBehavior.AllowGet);
					}
				}
			}
			object obj = Cache.Get("Cache-UserImportOpCount");
			if (obj != null)
			{
				Cache.Insert("Cache-UserImportOpCount", int.Parse(obj.ToString()) - 1);
			}
			return jsonResult;
		}

		private int ProcessProduct(long paraCategory, long paraShopCategory, long paraBrand, int paraSaleStatus, long _shopid, long _userid, string mainpath, string imgpath1, string imgpath2)
		{
			string str;
			ProductQuery productQuery;
			IProductService productService;
			long nextProductId;
			long num;
			decimal num1;
            decimal num1_min;
			ProductInfo productInfo;
			int num2 = 0;
			string str1 = mainpath;
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(paraCategory);
			if (Directory.Exists(str1))
			{
				string[] files = Directory.GetFiles(str1, "*.csv", SearchOption.AllDirectories);
				string empty = string.Empty;
				string[] strArrays = new string[0];
				for (int i = 0; i < files.Length; i++)
				{
					using (StreamReader streamReader = System.IO.File.OpenText(files[i]))
					{
						int num3 = 0;
						List<string> strs = new List<string>();
						while (true)
						{
							string str2 = streamReader.ReadLine();
							empty = str2;
							if (str2 == null || string.IsNullOrEmpty(empty))
							{
								break;
							}
							num3++;
							if (num3 >= 4)
							{
								strs.Add(empty);
							}
						}
						Cache.Insert(CacheKeyCollection.UserImportProductTotal(_userid), strs.Count);
						foreach (string str3 in strs)
						{
							string[] strArrays1 = new string[] { "\t" };
							strArrays = str3.Split(strArrays1, StringSplitOptions.None);
							int length = strArrays.Length;
							if (length == 58)            //淘宝5.6版本
							{
								str = strArrays[0].Replace("\"", "");
								productQuery = new ProductQuery()
								{
									CategoryId = new long?(category.Id),
									ShopId = new long?(_shopid),
									KeyWords = str
								};
								productService = ServiceHelper.Create<IProductService>();
								if (productService.GetProducts(productQuery).Total <= 0)
								{
									nextProductId = productService.GetNextProductId();
									num = 0;
									num1 = decimal.Parse((strArrays[7] == string.Empty ? "0" : strArrays[7]));
									ProductInfo productInfo1 = new ProductInfo()
									{
										Id = nextProductId,
										TypeId = category.TypeId,
										AddedDate = DateTime.Now,
										BrandId = paraBrand,
										CategoryId = category.Id,
										CategoryPath = category.Path,
										MarketPrice = num1,
										ShortDescription = string.Empty,
										ProductCode = strArrays[33].Replace("\"", ""),
										ImagePath = "",
										DisplaySequence = 1,
										ProductName = strArrays[0].Replace("\"", ""),
										MinSalePrice = num1,
										ShopId = _shopid,
										HasSKU = true,
										ProductAttributeInfo = new List<ProductAttributeInfo>()
									};
									List<ProductShopCategoryInfo> productShopCategoryInfos = new List<ProductShopCategoryInfo>();
									ProductShopCategoryInfo productShopCategoryInfo = new ProductShopCategoryInfo()
									{
										ProductId = nextProductId,
										ShopCategoryId = paraShopCategory
									};
									productShopCategoryInfos.Add(productShopCategoryInfo);
									productInfo1.Himall_ProductShopCategories = productShopCategoryInfos;
									ProductDescriptionInfo productDescriptionInfo = new ProductDescriptionInfo()
									{
										AuditReason = "",
										Description = strArrays[20].Replace("\"", ""),
										DescriptiondSuffixId = 0,
										DescriptionPrefixId = 0,
										Meta_Description = string.Empty,
										Meta_Keywords = string.Empty,
										Meta_Title = string.Empty,
										ProductId = nextProductId
									};
									productInfo1.ProductDescriptionInfo = productDescriptionInfo;
									ProductInfo productInfo2 = productInfo1;
									List<SKUInfo> sKUInfos = new List<SKUInfo>();
									List<SKUInfo> sKUInfos1 = sKUInfos;
									SKUInfo sKUInfo = new SKUInfo();
									object[] objArray = new object[] { nextProductId, "0", "0", "0" };
									sKUInfo.Id = string.Format("{0}_{1}_{2}_{3}", objArray);
									sKUInfo.Stock = (long.TryParse(strArrays[9], out num) ? num : 0);
									sKUInfo.SalePrice = num1;
									sKUInfo.CostPrice = num1;
									sKUInfos1.Add(sKUInfo);
									productInfo2.SKUInfo = sKUInfos;
									productInfo1.SaleStatus = (paraSaleStatus == 1 ? ProductInfo.ProductSaleStatus.OnSale : ProductInfo.ProductSaleStatus.InStock);
									productInfo1.AuditStatus = ProductInfo.ProductAuditStatus.WaitForAuditing;
									productInfo = productInfo1;
									long id = productInfo.Id;
									productInfo.ImagePath = string.Concat(imgpath1, "//", id.ToString());
									if (strArrays[28] != string.Empty)
									{
                                        ImportProductImg(productInfo.Id, _shopid, files[i], strArrays[28]);
									}
									productService.AddProduct(productInfo);
									num2++;
									Log.Debug(strArrays[0].Replace("\"", ""));
									Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
								}
								else
								{
									num2++;
									Log.Debug(string.Concat(strArrays[0].Replace("\"", ""), " : 商品不能重复导入"));
									Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
								}
							}
							else if (length == 63)           //淘宝5.7版本
							{
								str = strArrays[0].Replace("\"", "");
								productQuery = new ProductQuery()
								{
									CategoryId = new long?(category.Id),
									ShopId = new long?(_shopid),
									KeyWords = str
								};
								productService = ServiceHelper.Create<IProductService>();
								if (productService.GetProducts(productQuery).Total <= 0)
								{
									nextProductId = productService.GetNextProductId();
									num1 = decimal.Parse((strArrays[7] == string.Empty ? "0" : strArrays[7]));
									ProductInfo productInfo3 = new ProductInfo()
									{
										Id = nextProductId,
										TypeId = category.TypeId,
										AddedDate = DateTime.Now,
										BrandId = paraBrand,
										CategoryId = category.Id,
										CategoryPath = category.Path,
										MarketPrice = num1,
										ShortDescription = string.Empty,
										ProductCode = strArrays[33].Replace("\"", ""),
										ImagePath = "",
										DisplaySequence = 1,
										ProductName = strArrays[0].Replace("\"", ""),
										MinSalePrice = num1,
										ShopId = _shopid,
										HasSKU = true,
										ProductAttributeInfo = new List<ProductAttributeInfo>()
									};
									List<ProductShopCategoryInfo> productShopCategoryInfos1 = new List<ProductShopCategoryInfo>();
									ProductShopCategoryInfo productShopCategoryInfo1 = new ProductShopCategoryInfo()
									{
										ProductId = nextProductId,
										ShopCategoryId = paraShopCategory
									};
									productShopCategoryInfos1.Add(productShopCategoryInfo1);
									productInfo3.Himall_ProductShopCategories = productShopCategoryInfos1;
									ProductDescriptionInfo productDescriptionInfo1 = new ProductDescriptionInfo()
									{
										AuditReason = "",
										Description = strArrays[20].Replace("\"", ""),
										DescriptiondSuffixId = 0,
										DescriptionPrefixId = 0,
										Meta_Description = string.Empty,
										Meta_Keywords = string.Empty,
										Meta_Title = string.Empty,
										ProductId = nextProductId
									};
									productInfo3.ProductDescriptionInfo = productDescriptionInfo1;
									ProductInfo productInfo4 = productInfo3;
									List<SKUInfo> sKUInfos2 = new List<SKUInfo>();
									List<SKUInfo> sKUInfos3 = sKUInfos2;
									SKUInfo sKUInfo1 = new SKUInfo();
									object[] objArray1 = new object[] { nextProductId, "0", "0", "0" };
									sKUInfo1.Id = string.Format("{0}_{1}_{2}_{3}", objArray1);
									sKUInfo1.Stock = (long.TryParse(strArrays[9], out num) ? num : 0);
									sKUInfo1.SalePrice = num1;
									sKUInfo1.CostPrice = num1;
									sKUInfos3.Add(sKUInfo1);
									productInfo4.SKUInfo = sKUInfos2;
									productInfo3.SaleStatus = (paraSaleStatus == 1 ? ProductInfo.ProductSaleStatus.OnSale : ProductInfo.ProductSaleStatus.InStock);
									productInfo3.AuditStatus = ProductInfo.ProductAuditStatus.WaitForAuditing;
									productInfo = productInfo3;
									long id1 = productInfo.Id;
									productInfo.ImagePath = string.Concat(imgpath1, "/", id1.ToString());
									if (strArrays[28] != string.Empty)
									{
                                        ImportProductImg(productInfo.Id, _shopid, files[i], strArrays[28]);
									}
                                  
                                    productInfo.MeasureUnit = "件";
                                   // productInfo.Weight = 0;
                                   // productInfo.Volume = 0;
                                  //  productInfo.EditStatus = 3;
                                    productInfo.FreightTemplateId = 139;
									productService.AddProduct(productInfo);
									num2++;
									Log.Debug(strArrays[0].Replace("\"", ""));
									Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
								}
								else
								{
									num2++;
									Log.Debug(string.Concat(strArrays[0].Replace("\"", ""), " : 商品不能重复导入"));
									Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
								}
							}
                            else if(length == 19) //  
                            {
                                str = strArrays[0].Replace("\"", "");
                                productQuery = new ProductQuery()
                                {
                                    CategoryId = new long?(category.Id),
                                    ShopId = new long?(_shopid),
                                    KeyWords = str
                                };
                                productService = ServiceHelper.Create<IProductService>();
                                if (productService.GetProducts(productQuery).Total <= 0)
                                {
                                    nextProductId = productService.GetNextProductId();
                                    num1 = decimal.Parse((strArrays[1] == string.Empty ? "0" : strArrays[1]));
                                    num1_min = decimal.Parse((strArrays[2] == string.Empty ? "0" : strArrays[2]));
                                    ProductInfo productInfo3 = new ProductInfo()
                                    {
                                        Id = nextProductId,
                                        TypeId = category.TypeId, 
                                        AddedDate = DateTime.Now,
                                        BrandId = paraBrand,
                                        CategoryId = category.Id,
                                        CategoryPath = category.Path,
                                        MarketPrice = num1,
                                        ShortDescription = string.Empty,
                                        ProductCode = strArrays[15].Replace("\"", ""),
                                        ImagePath = "",
                                        DisplaySequence = 1,
                                        ProductName = strArrays[0].Replace("\"", ""),
                                        MinSalePrice = num1_min,
                                        ShopId = _shopid,
                                        HasSKU = true,
                                        ProductAttributeInfo = new List<ProductAttributeInfo>()
                                    };
                                    List<ProductShopCategoryInfo> productShopCategoryInfos1 = new List<ProductShopCategoryInfo>();
                                    ProductShopCategoryInfo productShopCategoryInfo1 = new ProductShopCategoryInfo()
                                    {
                                        ProductId = nextProductId,
                                        ShopCategoryId = paraShopCategory
                                    };
                                    productShopCategoryInfos1.Add(productShopCategoryInfo1);
                                    productInfo3.Himall_ProductShopCategories = productShopCategoryInfos1;
                                    ProductDescriptionInfo productDescriptionInfo1 = new ProductDescriptionInfo()
                                    {
                                        AuditReason = "",
                                        Description = strArrays[11].Replace("\"", ""),
                                        DescriptiondSuffixId = 0,
                                        DescriptionPrefixId = 0,
                                        Meta_Description = string.Empty,
                                        Meta_Keywords = string.Empty,
                                        Meta_Title = string.Empty,
                                        ProductId = nextProductId
                                    };
                                    productInfo3.ProductDescriptionInfo = productDescriptionInfo1;
                                    ProductInfo productInfo4 = productInfo3;
                                    List<SKUInfo> sKUInfos2 = new List<SKUInfo>();
                                    List<SKUInfo> sKUInfos3 = sKUInfos2;
                                    SKUInfo sKUInfo1 = new SKUInfo();
                                    object[] objArray1 = new object[] { nextProductId, "0", "0", "0" };
                                    sKUInfo1.Id = string.Format("{0}_{1}_{2}_{3}", objArray1);
                                    sKUInfo1.Stock = (long.TryParse(strArrays[9], out num) ? num : 0);
                                    sKUInfo1.SalePrice = num1;
                                    sKUInfo1.CostPrice = num1;
                                    sKUInfos3.Add(sKUInfo1);
                                    productInfo4.SKUInfo = sKUInfos2;
                                    productInfo3.SaleStatus = (paraSaleStatus == 1 ? ProductInfo.ProductSaleStatus.OnSale : ProductInfo.ProductSaleStatus.InStock);
                                    productInfo3.AuditStatus = ProductInfo.ProductAuditStatus.WaitForAuditing;
                                    productInfo = productInfo3;
                                    long id1 = productInfo.Id;
                                    productInfo.ImagePath = string.Concat(imgpath1, "/", id1.ToString());
                                    if (strArrays[14] != string.Empty)
                                    {
                                        ImportProductImg(productInfo.Id, _shopid, files[i], strArrays[14]);
                                    }

                                    productInfo.MeasureUnit = strArrays[5].Replace("\"", "");
                                   // productInfo.Volume = null;
                                  //  productInfo.Weight = null;
                                   // productInfo.EditStatus = 3;
                                    productInfo.FreightTemplateId = 139;
                                    productService.AddProduct(productInfo);
                                    num2++;
                                    Log.Debug(strArrays[0].Replace("\"", ""));
                                    Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
                                }
                                else
                                {
                                    num2++;
                                    Log.Debug(string.Concat(strArrays[0].Replace("\"", ""), " : 商品不能重复导入"));
                                    Cache.Insert(CacheKeyCollection.UserImportProductCount(_userid), num2);
                                }
                            }




						}
					}
				}
			}
			return num2;
		}
	}
}