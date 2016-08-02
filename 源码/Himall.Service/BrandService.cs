using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class BrandService : ServiceBase, IBrandService, IService, IDisposable
	{
		public BrandService()
		{
		}

		public void AddBrand(BrandInfo model)
		{
            context.BrandInfo.Add(model);
            context.SaveChanges();
			model.Logo = MoveImages(model.Id, model.Logo, 0);
            context.SaveChanges();
		}

		public void ApplyBrand(ShopBrandApplysInfo model)
		{
            context.ShopBrandApplysInfo.Add(model);
            context.SaveChanges();
			if (model.ApplyMode == 2)
			{
				model.Logo = MoveImages(model.Id, model.ShopId, model.Logo, "logo", 1);
			}
			string authCertificate = model.AuthCertificate;
			string empty = string.Empty;
			if (!string.IsNullOrEmpty(authCertificate))
			{
				string[] strArrays = authCertificate.Split(new char[] { ',' });
				int num = 0;
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					num++;
					empty = string.Concat(empty, MoveImages(model.Id, model.ShopId, str, "auth", num), ",");
				}
			}
			if (!string.IsNullOrEmpty(empty))
			{
				char[] chrArray = new char[] { ',' };
				model.AuthCertificate = empty.TrimEnd(chrArray);
			}
            context.SaveChanges();
		}

		public void AuditBrand(long id, ShopBrandApplysInfo.BrandAuditStatus status)
		{
			ShopBrandApplysInfo nullable = context.ShopBrandApplysInfo.FindById<ShopBrandApplysInfo>(id);
			nullable.AuditStatus = (int)status;
			if (status == ShopBrandApplysInfo.BrandAuditStatus.Audited)
			{
				if (nullable.ApplyMode != 2)
				{
					ShopBrandsInfo shopBrandsInfo = new ShopBrandsInfo()
					{
						BrandId = nullable.BrandId.Value,
						ShopId = nullable.ShopId
					};
                    context.ShopBrandsInfo.Add(shopBrandsInfo);
                    context.SaveChanges();
				}
				else
				{
					BrandInfo brandInfo = (
						from r in context.BrandInfo
						where r.Name.ToLower() == nullable.BrandName.ToLower()
						select r).FirstOrDefault();
					if (brandInfo != null)
					{
						ShopBrandsInfo shopBrandsInfo1 = new ShopBrandsInfo()
						{
							BrandId = brandInfo.Id,
							ShopId = nullable.ShopId
						};
                        context.ShopBrandsInfo.Add(shopBrandsInfo1);
                        context.SaveChanges();
					}
					else
					{
						BrandInfo brandInfo1 = new BrandInfo()
						{
							Name = nullable.BrandName.Trim(),
							Logo = nullable.Logo,
							Description = nullable.Description
						};
						BrandInfo brandInfo2 = brandInfo1;
                        context.BrandInfo.Add(brandInfo2);
                        context.SaveChanges();
						nullable.BrandId = new long?(brandInfo2.Id);
						BrandInfo brand = GetBrand(brandInfo2.Id);
						brand.Logo = MoveImages(brand.Id, brand.Logo, 1);
						ShopBrandsInfo shopBrandsInfo2 = new ShopBrandsInfo()
						{
							BrandId = brand.Id,
							ShopId = nullable.ShopId
						};
                        context.ShopBrandsInfo.Add(shopBrandsInfo2);
                        context.SaveChanges();
					}
				}
			}
            context.SaveChanges();
		}

		public bool BrandInUse(long id)
		{
			return context.ProductInfo.Any((ProductInfo item) => (int)item.SaleStatus == 1 && item.BrandId.Equals(id));
		}

		public void DeleteApply(int id)
		{
			ShopBrandApplysInfo shopBrandApplysInfo = context.ShopBrandApplysInfo.FindById<ShopBrandApplysInfo>(id);
            context.ShopBrandApplysInfo.Remove(shopBrandApplysInfo);
			if (shopBrandApplysInfo.ApplyMode == 2)
			{
				string mapPath = IOHelper.GetMapPath(shopBrandApplysInfo.Logo);
				if (File.Exists(mapPath))
				{
					File.Delete(mapPath);
				}
			}
			string str = IOHelper.GetMapPath(shopBrandApplysInfo.AuthCertificate);
			if (File.Exists(str))
			{
				File.Delete(str);
			}
            context.SaveChanges();
		}

		public void DeleteBrand(long id)
		{
			BrandInfo brandInfo = context.BrandInfo.FindById<BrandInfo>(id);
			if (brandInfo.Himall_ShopBrandApplys.Any((ShopBrandApplysInfo p) => p.AuditStatus != 2) || brandInfo.Himall_ShopBrands.Count() != 0)
			{
				throw new HimallException("该品牌已有商家使用，或已有商家申请，不能删除！");
			}
			if (brandInfo.TypeBrandInfo.Count() != 0 || brandInfo.FloorBrandInfo.Count() != 0)
			{
				throw new HimallException("该品牌被使用中，不能删除！");
			}
            context.BrandInfo.Remove(brandInfo);
            context.SaveChanges();
			File.Delete(IOHelper.GetMapPath(brandInfo.Logo));
		}

		public BrandInfo GetBrand(long id)
		{
			return context.BrandInfo.FindById<BrandInfo>(id);
		}

		public ShopBrandApplysInfo GetBrandApply(long id)
		{
			ShopBrandApplysInfo description = context.ShopBrandApplysInfo.FindById<ShopBrandApplysInfo>(id);
			if (description.ApplyMode == 1)
			{
				description.Description = description.Himall_Brands.Description;
			}
			return description;
		}

		public PageModel<BrandInfo> GetBrands(string keyWords, int pageNo, int pageSize)
		{
			int num = 0;
			IQueryable<BrandInfo> brandInfos = context.BrandInfo.FindBy<BrandInfo, long>((BrandInfo item) => keyWords == null || (keyWords == "") || item.Name.Contains(keyWords), pageNo, pageSize, out num, (BrandInfo a) => a.Id, false);
			return new PageModel<BrandInfo>()
			{
				Models = brandInfos,
				Total = num
			};
		}

		public IQueryable<BrandInfo> GetBrands(string keyWords)
		{
			return context.BrandInfo.FindBy((BrandInfo item) => keyWords == null || (keyWords == "") || item.Name.Contains(keyWords) || item.RewriteName.Contains(keyWords));
		}

		public IEnumerable<BrandInfo> GetBrandsByCategoryIds(params long[] categoryIds)
		{
			IQueryable<CategoryInfo> categoryInfo = 
				from item in context.CategoryInfo
				where categoryIds.Contains(item.Id)
				select item;
			IQueryable<ProductTypeInfo> productTypeInfo = 
				from item in categoryInfo
				select item.ProductTypeInfo;
			IQueryable<IEnumerable<BrandInfo>> enumerables = 
				from item in productTypeInfo
				select item.TypeBrandInfo.Select<TypeBrandInfo, BrandInfo>((TypeBrandInfo t) => t.BrandInfo);
			List<BrandInfo> brandInfos = new List<BrandInfo>();
			foreach (IEnumerable<BrandInfo> list in enumerables.ToList<IEnumerable<BrandInfo>>())
			{
				brandInfos.AddRange(list);
			}
			return brandInfos.DistinctBy<BrandInfo, long>((BrandInfo item) => item.Id);
		}

		public IEnumerable<BrandInfo> GetBrandsByCategoryIds(long shopId, params long[] categoryIds)
		{
			ShopInfo shopInfo = context.ShopInfo.FirstOrDefault((ShopInfo d) => d.Id == shopId);
			IEnumerable<long> shopBrandsInfo = 
				from item in context.ShopBrandsInfo
				where item.ShopId == shopId
				select item into r
				select r.BrandId;
            
			IQueryable<CategoryInfo> categoryInfo = 
				from item in context.CategoryInfo
				where categoryIds.Contains(item.Id)
				select item;
			IQueryable<ProductTypeInfo> productTypeInfo = 
				from item in categoryInfo
				select item.ProductTypeInfo;
			IQueryable<IEnumerable<BrandInfo>> enumerables = 
				from item in productTypeInfo
				select item.TypeBrandInfo.Select<TypeBrandInfo, BrandInfo>((TypeBrandInfo t) => t.BrandInfo);
            
			List<BrandInfo> brandInfos = new List<BrandInfo>();
			foreach (IEnumerable<BrandInfo> list in enumerables.ToList<IEnumerable<BrandInfo>>())
			{
				foreach (BrandInfo brandInfo in list)
				{
					bool flag = false;
					if (shopBrandsInfo.Contains(brandInfo.Id))
					{
						flag = true;
					}
					if (shopInfo.IsSelf)
					{
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					brandInfos.Add(brandInfo);
				}
			}
			return brandInfos.DistinctBy<BrandInfo, long>((BrandInfo item) => item.Id);
		}

		public PageModel<ShopBrandApplysInfo> GetShopBrandApplys(long? shopId, int? auditStatus, int pageNo, int pageSize, string keyWords)
		{
			int num = 0;
			IQueryable<ShopBrandApplysInfo> shopBrandApplysInfos = context.ShopBrandApplysInfo.FindAll<ShopBrandApplysInfo>();
			if (auditStatus.HasValue)
			{
				shopBrandApplysInfos = 
					from item in shopBrandApplysInfos
					where item.AuditStatus == auditStatus
                    select item;
			}
			if (shopId.HasValue)
			{
				shopBrandApplysInfos = 
					from item in shopBrandApplysInfos
					where item.ShopId == shopId
                    select item;
			}
			if (!string.IsNullOrEmpty(keyWords))
			{
				shopBrandApplysInfos = 
					from item in shopBrandApplysInfos
					where item.Himall_Shops.ShopName.Contains(keyWords)
					select item;
			}
			shopBrandApplysInfos = shopBrandApplysInfos.FindBy<ShopBrandApplysInfo, int>((ShopBrandApplysInfo item) => true, pageNo, pageSize, out num, (ShopBrandApplysInfo a) => a.Id, false);
			return new PageModel<ShopBrandApplysInfo>()
			{
				Models = shopBrandApplysInfos,
				Total = num
			};
		}

		public IQueryable<ShopBrandApplysInfo> GetShopBrandApplys(long shopId)
		{
			return context.ShopBrandApplysInfo.FindBy((ShopBrandApplysInfo item) => item.ShopId == shopId);
		}

		public PageModel<BrandInfo> GetShopBrands(long shopId, int pageNo, int pageSize)
		{
			int num = 0;
			IQueryable<BrandInfo> brandInfos = context.BrandInfo.FindBy<BrandInfo, long>((BrandInfo item) => (
				from r in context.ShopBrandsInfo
				where r.ShopId == shopId
				select r.BrandId).Contains(item.Id), pageNo, pageSize, out num, (BrandInfo a) => a.Id, false);
			return new PageModel<BrandInfo>()
			{
				Models = brandInfos,
				Total = num
			};
		}

		public IQueryable<BrandInfo> GetShopBrands(long shopId)
		{
			return 
				from item in context.BrandInfo
				where context.ShopBrandsInfo.Where((ShopBrandsInfo r) => r.ShopId == shopId).Select<ShopBrandsInfo, long>((ShopBrandsInfo r) => r.BrandId).Contains(item.Id)
				select item;
		}

		public bool IsExistApply(long shopId, string brandName)
		{
			if ((
				from item in context.ShopBrandApplysInfo
				where item.ShopId == shopId && (item.BrandName.ToLower().Trim() == brandName.ToLower().Trim()) && item.AuditStatus != 2
				select item).FirstOrDefault() != null)
			{
				return true;
			}
			return false;
		}

		public bool IsExistBrand(string brandName)
		{
			if ((
				from item in context.BrandInfo
				where item.Name.ToLower().Trim() == brandName.ToLower().Trim()
				select item).FirstOrDefault() != null)
			{
				return true;
			}
			return false;
		}

		private string MoveImages(long brandId, string image, int type = 0)
		{
			string mapPath = IOHelper.GetMapPath(image);
			string extension = (new FileInfo(mapPath)).Extension;
			string empty = string.Empty;
			empty = IOHelper.GetMapPath("/Storage/Plat/Brand");
			string str = "/Storage/Plat/Brand/";
			string str1 = string.Concat("logo_", brandId, extension);
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (!image.Replace("\\", "/").Contains("/temp/") && type != 1)
			{
				return image;
			}
			IOHelper.CopyFile(mapPath, empty, false, str1);
			return string.Concat(str, str1);
		}

		private string MoveImages(int id, long shopId, string image, string name, int index = 1)
		{
			if (string.IsNullOrEmpty(image))
			{
				return "";
			}
			string mapPath = IOHelper.GetMapPath(image);
			string str = ".png";
			string empty = string.Empty;
			string str1 = string.Concat("/Storage/Shop/", shopId, "/Brand");
			empty = IOHelper.GetMapPath(str1);
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			object[] objArray = new object[] { name, "_", id, "_", index, str };
			string str2 = string.Concat(objArray);
			if (image.Replace("\\", "/").Contains("/temp/"))
			{
				IOHelper.CopyFile(mapPath, empty, false, str2);
			}
			return string.Concat(str1, "/", str2);
		}

		public void UpdateBrand(BrandInfo model)
		{
			model.Logo = MoveImages(model.Id, model.Logo, 0);
			BrandInfo brand = GetBrand(model.Id);
			brand.Name = model.Name.Trim();
			brand.Description = model.Description;
			brand.Logo = model.Logo;
			brand.Meta_Description = model.Meta_Description;
			brand.Meta_Keywords = model.Meta_Keywords;
			brand.Meta_Title = model.Meta_Title;
			brand.RewriteName = model.RewriteName;
			brand.IsRecommend = model.IsRecommend;
			ShopBrandApplysInfo name = context.ShopBrandApplysInfo.FirstOrDefault((ShopBrandApplysInfo p) => p.BrandId == model.Id);
			name.BrandName = model.Name;
			name.Description = model.Description;
			name.Logo = model.Logo;
            context.SaveChanges();
		}

		public void UpdateSellerBrand(ShopBrandApplysInfo model)
		{
			ShopBrandApplysInfo brandName = context.ShopBrandApplysInfo.FindBy((ShopBrandApplysInfo a) => a.Id == model.Id && a.ShopId != 0 && a.AuditStatus == 0).FirstOrDefault();
			if (brandName == null)
			{
				throw new HimallException("该品牌已被审核或删除，不能修改！");
			}
			brandName.Logo = MoveImages(model.Id, model.Logo, 0);
			brandName.BrandName = model.BrandName;
			brandName.Description = model.Description;
            context.SaveChanges();
		}
	}
}