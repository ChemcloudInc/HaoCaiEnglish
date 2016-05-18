using Himall.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductInfo : BaseModel
	{
		private long _id;

		public DateTime AddedDate
		{
			get;
			set;
		}

		[NotMapped]
		public string Address
		{
			get;
			set;
		}

		public ProductInfo.ProductAuditStatus AuditStatus
		{
			get;
			set;
		}

		public long BrandId
		{
			get;
			set;
		}

		[NotMapped]
		public string BrandName
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public string CategoryPath
		{
			get;
			set;
		}

		public int ConcernedCount
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public int EditStatus
		{
			get;
			set;
		}

		public long FreightTemplateId
		{
			get;
			set;
		}

		public bool HasSKU
		{
			get;
			set;
		}

		public virtual ICollection<BrowsingHistoryInfo> Himall_BrowsingHistory
		{
			get;
			set;
		}

		public virtual CategoryInfo Himall_Categories
		{
			get;
			set;
		}

		public virtual ICollection<CollocationPoruductInfo> Himall_CollocationPoruducts
		{
			get;
			set;
		}

		public virtual ICollection<CollocationSkuInfo> Himall_CollocationSkus
		{
			get;
			set;
		}

		public virtual ICollection<FavoriteInfo> Himall_Favorites
		{
			get;
			set;
		}

		public virtual ICollection<FloorProductInfo> Himall_FloorProducts
		{
			get;
			set;
		}

		public virtual ICollection<FloorTablDetailsInfo> Himall_FloorTablDetails
		{
			get;
			set;
		}

		public virtual FreightTemplateInfo Himall_FreightTemplate
		{
			get;
			set;
		}

		public virtual ICollection<MobileHomeProductsInfo> Himall_MobileHomeProducts
		{
			get;
			set;
		}

		public virtual ICollection<ModuleProductInfo> Himall_ModuleProducts
		{
			get;
			set;
		}

		public virtual ICollection<ProductCommentInfo> Himall_ProductComments
		{
			get;
			set;
		}

		public virtual ICollection<ProductShopCategoryInfo> Himall_ProductShopCategories
		{
			get;
			set;
		}

		public virtual ICollection<ProductVistiInfo> Himall_ProductVistis
		{
			get;
			set;
		}

		public virtual ICollection<ShopHomeModuleProductInfo> Himall_ShopHomeModuleProducts
		{
			get;
			set;
		}

		internal virtual ICollection<ShoppingCartItemInfo> Himall_ShoppingCarts
		{
			get;
			set;
		}

		public virtual ShopInfo Himall_Shops
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		private string imagePath
		{
			get;
			set;
		}

		[NotMapped]
		public string ImagePath
		{
			get
			{
				return string.Concat(ImageServerUrl, imagePath);
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(ImageServerUrl))
				{
                    imagePath = value;
					return;
				}
                imagePath = value.Replace(ImageServerUrl, "");
			}
		}

		public decimal MarketPrice
		{
			get;
			set;
		}

		public string MeasureUnit
		{
			get;
			set;
		}

		public decimal MinSalePrice
		{
			get;
			set;
		}

		[NotMapped]
		public long OrderCounts
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.ProductAttributeInfo> ProductAttributeInfo
		{
			get;
			set;
		}

		public string ProductCode
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.ProductConsultationInfo> ProductConsultationInfo
		{
			get;
			set;
		}

		public virtual Himall.Model.ProductDescriptionInfo ProductDescriptionInfo
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public int? Quantity
		{
			get;
			set;
		}

		public long SaleCounts
		{
			get;
			set;
		}

		public ProductInfo.ProductSaleStatus SaleStatus
		{
			get;
			set;
		}

		[NotMapped]
		public IEnumerable<ShopCategoryInfo> ShopCateogryInfos
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		[NotMapped]
		public string ShopName
		{
			get;
			set;
		}

		public string ShortDescription
		{
			get;
			set;
		}

		[NotMapped]
		public string ShowProductState
		{
			get
			{
				string description = "错误数据";
				if (this != null)
				{
					if (AuditStatus != ProductInfo.ProductAuditStatus.WaitForAuditing)
					{
						description = AuditStatus.ToDescription();
					}
					else
					{
						description = (SaleStatus == ProductInfo.ProductSaleStatus.OnSale ? ProductInfo.ProductAuditStatus.WaitForAuditing.ToDescription() : ProductInfo.ProductSaleStatus.InStock.ToDescription());
					}
				}
				return description;
			}
		}

		public virtual ICollection<Himall.Model.SKUInfo> SKUInfo
		{
			get;
			set;
		}

		public long TypeId
		{
			get;
			set;
		}

		public long VistiCounts
		{
			get;
			set;
		}

		public decimal? Volume
		{
			get;
			set;
		}

		public decimal? Weight
		{
			get;
			set;
		}

		public ProductInfo()
		{
            ProductConsultationInfo = new HashSet<Himall.Model.ProductConsultationInfo>();
            ProductAttributeInfo = new HashSet<Himall.Model.ProductAttributeInfo>();
            SKUInfo = new HashSet<Himall.Model.SKUInfo>();
            Himall_Favorites = new HashSet<FavoriteInfo>();
            Himall_FloorProducts = new HashSet<FloorProductInfo>();
            Himall_ProductShopCategories = new HashSet<ProductShopCategoryInfo>();
            Himall_ShoppingCarts = new HashSet<ShoppingCartItemInfo>();
            Himall_ProductComments = new HashSet<ProductCommentInfo>();
            Himall_ModuleProducts = new HashSet<ModuleProductInfo>();
            Himall_ShopHomeModuleProducts = new HashSet<ShopHomeModuleProductInfo>();
            Himall_ProductVistis = new HashSet<ProductVistiInfo>();
            Himall_BrowsingHistory = new HashSet<BrowsingHistoryInfo>();
            Himall_MobileHomeProducts = new HashSet<MobileHomeProductsInfo>();
            Himall_FloorTablDetails = new HashSet<FloorTablDetailsInfo>();
            Himall_CollocationPoruducts = new HashSet<CollocationPoruductInfo>();
            Himall_CollocationSkus = new HashSet<CollocationSkuInfo>();
		}

		public string GetImage(ProductInfo.ImageSize imageSize, int imageIndex = 1)
		{
			return string.Format(string.Concat(ImagePath, "/{0}_{1}.png"), imageIndex, (int)imageSize);
		}

		public enum ImageSize
		{
			[Description("50×50")]
			Size_50 = 50,
			[Description("100×100")]
			Size_100 = 100,
			[Description("150×150")]
			Size_150 = 150,
			[Description("220×220")]
			Size_220 = 220,
			[Description("350×350")]
			Size_350 = 350
		}

		public enum ProductAuditStatus
		{
			[Description("待审核")]
			WaitForAuditing = 1,
			[Description("销售中")]
			Audited = 2,
			[Description("未通过")]
			AuditFailed = 3,
			[Description("违规下架")]
			InfractionSaleOff = 4,
			[Description("未审核")]
			UnAudit = 5
		}

		public enum ProductEditStatus
		{
			[Description("正常")]
			Normal,
			[Description("己修改")]
			Edited,
			[Description("待审核")]
			PendingAudit,
			[Description("己修改待审核")]
			EditedAndPending,
			[Description("强制待审核")]
			CompelPendingAudit,
			[Description("强制待审己修改")]
			CompelPendingHasEdited
		}

		public enum ProductSaleStatus
		{
			[Description("原始状态")]
			RawState,
			[Description("出售中")]
			OnSale,
			[Description("仓库中")]
			InStock,
			[Description("草稿箱")]
			InDraft,
			[Description("已删除")]
			InDelete
		}
	}
}