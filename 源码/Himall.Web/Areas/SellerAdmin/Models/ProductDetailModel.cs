using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ProductDetailModel
	{
		public string adWord
		{
			get;
			set;
		}

		public List<AttrSelectData> attrSelectData
		{
			get;
			set;
		}

		public long brandId
		{
			get;
			set;
		}

		public long categoryId
		{
			get;
			set;
		}

		public string cost
		{
			get;
			set;
		}

		public string des
		{
			get;
			set;
		}

		public long FreightTemplateId
		{
			get;
			set;
		}

		public List<long> goodsCategory
		{
			get;
			set;
		}

		public string goodsName
		{
			get;
			set;
		}

		public string mallPrce
		{
			get;
			set;
		}

		public string marketPrice
		{
			get;
			set;
		}

		public string mdes
		{
			get;
			set;
		}

		public string MeasureUnit
		{
			get;
			set;
		}

		public List<string> pic
		{
			get;
			set;
		}

		public string productCode
		{
			get;
			set;
		}

		public long productId
		{
			get;
			set;
		}

		public decimal rebate
		{
			get;
			set;
		}

		public int saleStatus
		{
			get;
			set;
		}

		public string seoDes
		{
			get;
			set;
		}

		public string seoKey
		{
			get;
			set;
		}

		public string seoTitle
		{
			get;
			set;
		}

		public List<Specifications> specifications
		{
			get;
			set;
		}

		public List<SpecificationsValue> specificationsValue
		{
			get;
			set;
		}

		public long stock
		{
			get;
			set;
		}

		public List<long> styleTemplateId
		{
			get;
			set;
		}

		public decimal Volume
		{
			get;
			set;
		}

		public decimal Weight
		{
			get;
			set;
		}

		public ProductDetailModel()
		{
		}

		public static ProductInfo GetProductInfo(ProductDetailModel m, long productId)
		{
			CategoryInfo category = ServiceHelper.Create<ICategoryService>().GetCategory(m.categoryId);
			decimal num = new decimal(0);
			if (m.specificationsValue.Count <= 0)
			{
				num = decimal.Parse(m.mallPrce);
			}
			else
			{
				List<decimal> list = (
					from p in m.specificationsValue
					select decimal.Parse(p.mallPrice)).ToList();
				num = list.Min();
			}
			ProductInfo productInfo = new ProductInfo()
			{
				Id = productId,
				TypeId = category.TypeId,
				AddedDate = DateTime.Now,
				BrandId = m.brandId,
				CategoryId = m.categoryId,
				CategoryPath = category.Path,
				MarketPrice = decimal.Parse(m.marketPrice),
				ShortDescription = m.adWord,
				ProductCode = m.productCode,
				ImagePath = "",
				DisplaySequence = 1,
				ProductName = m.goodsName,
				MinSalePrice = num,
				ShopId = 1,
				HasSKU = true,
				ProductAttributeInfo = new List<ProductAttributeInfo>(),
				Himall_ProductShopCategories = new List<ProductShopCategoryInfo>()
			};
			ProductDescriptionInfo productDescriptionInfo = new ProductDescriptionInfo()
			{
				AuditReason = "",
				Description = m.des,
				MobileDescription = m.mdes,
				DescriptionPrefixId = m.styleTemplateId[0],
				DescriptiondSuffixId = m.styleTemplateId[1],
				Meta_Description = m.seoDes,
				Meta_Keywords = m.seoKey,
				Meta_Title = m.seoTitle,
				ProductId = productId
			};
			productInfo.ProductDescriptionInfo = productDescriptionInfo;
			productInfo.SKUInfo = new List<SKUInfo>();
			productInfo.SaleStatus = (ProductInfo.ProductSaleStatus)m.saleStatus;
			productInfo.AuditStatus = ProductInfo.ProductAuditStatus.WaitForAuditing;
			productInfo.FreightTemplateId = m.FreightTemplateId;
			productInfo.MeasureUnit = m.MeasureUnit;
			productInfo.Volume = new decimal?(m.Volume);
			productInfo.Weight = new decimal?(m.Weight);
			ProductInfo productInfo1 = productInfo;
			foreach (AttrSelectData attrSelectDatum in m.attrSelectData)
			{
				string[] strArrays = attrSelectDatum.valueId.Split(new char[] { ',' });
				for (int i = 0; i < strArrays.Length; i++)
				{
					string str = strArrays[i];
					if (!string.IsNullOrWhiteSpace(str))
					{
						ICollection<ProductAttributeInfo> productAttributeInfo = productInfo1.ProductAttributeInfo;
						ProductAttributeInfo productAttributeInfo1 = new ProductAttributeInfo()
						{
							AttributeId = attrSelectDatum.attrId,
							ProductId = productId,
							ValueId = long.Parse(str)
						};
						productAttributeInfo.Add(productAttributeInfo1);
					}
				}
			}
			foreach (long num1 in m.goodsCategory)
			{
				if (num1.Equals(0))
				{
					continue;
				}
				ICollection<ProductShopCategoryInfo> himallProductShopCategories = productInfo1.Himall_ProductShopCategories;
				ProductShopCategoryInfo productShopCategoryInfo = new ProductShopCategoryInfo()
				{
					ProductId = productId,
					ShopCategoryId = num1
				};
				himallProductShopCategories.Add(productShopCategoryInfo);
			}
			return productInfo1;
		}
	}
}