using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface IProductService : IService, IDisposable
	{
		void AddBrowsingProduct(BrowsingHistoryInfo info);

		void AddFavorite(long productId, long userId, out int status);

		void AddProduct(ProductInfo model);

		void AddSKU(ProductInfo pInfo);

		bool ApplyForSale(long id);

		void AuditProduct(long productId, ProductInfo.ProductAuditStatus auditStatus, string message);

		void AuditProducts(IEnumerable<long> productIds, ProductInfo.ProductAuditStatus auditStatus, string message);

		void BindTemplate(long? topTemplateId, long? bottomTemplateId, IEnumerable<long> productIds);

		void CancelConcernProducts(IEnumerable<long> ids, long userId);

		void DeleteFavorite(long productId, long userId);

		void DeleteProduct(long id);

		void DeleteProduct(IEnumerable<long> ids, long shopId);

		AttributeInfo GetAttributeInfo(long attrId);

		IQueryable<BrowsingHistoryInfo> GetBrowsingProducts(long userId);

		ProductInfo.ProductEditStatus GetEditStatus(long id, ProductInfo model);

		decimal GetFreight(IEnumerable<long> productIds, IEnumerable<int> counts, int cityId);

		decimal GetFreight(IEnumerable<string> skuIds, IEnumerable<int> counts, int cityId);

		List<decimal> GetFreights(IEnumerable<string> skuIds, IEnumerable<int> counts, int cityId);

		IQueryable<ProductInfo> GetHotConcernedProduct(long shopId, int count = 10);

		IQueryable<ProductInfo> GetHotSaleProduct(long shopId, int count = 10);

		IQueryable<ProductInfo> GetNewSaleProduct(long shopId, int count = 10);

		long GetNextProductId();

		IQueryable<ProductInfo> GetPlatHotSaleProduct(int count = 3);

		IQueryable<ProductInfo> GetPlatHotSaleProductByNearShop(int count, long userId);

		ProductInfo GetProduct(long id);

		IQueryable<ProductAttributeInfo> GetProductAttribute(long productId);

		IQueryable<ProductInfo> GetProductByIds(IEnumerable<long> ids);

		PageModel<ProductInfo> GetProducts(ProductQuery productQueryModel);

		IQueryable<ProductShopCategoryInfo> GetProductShopCategories(long productId);

		ProductVistiInfo GetProductVistInfo(long pId, ICollection<ProductVistiInfo> pInfo = null);

		IQueryable<SellerSpecificationValueInfo> GetSellerSpecifications(long shopId, long typeId);

		int GetShopAllProducts(long shopId);

		int GetShopOnsaleProducts(long shopId);

		SKUInfo GetSku(string skuId);

		IQueryable<SKUInfo> GetSKUs(long productId);

		IQueryable<SKUInfo> GetSKUs(IEnumerable<long> productIds);

		List<FavoriteInfo> GetUserAllConcern(long userId);

		PageModel<FavoriteInfo> GetUserConcernProducts(long userId, int pageNo, int pageSize);

		bool IsFavorite(long productId, long userId);

		void LogProductVisti(long productId);

		void OnSale(long id, long shopId);

		void OnSale(IEnumerable<long> ids, long shopId);

		void SaleOff(long id, long shopId);

		void SaleOff(IEnumerable<long> ids, long shopId);

		void SaveSellerSpecifications(List<SellerSpecificationValueInfo> info);

		PageModel<ProductInfo> SearchProduct(ProductSearch search);

		void UpdateProduct(ProductInfo model);

		void UpdateProductImagePath(long pId, string path);

		void UpdateSalesCount(string skuId, int addSalesCount);

		void UpdateStock(string skuId, long stockChange);
	}
}