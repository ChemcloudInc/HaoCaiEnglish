using Himall.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Runtime.CompilerServices;

namespace Himall.Entity
{
    public class Entities : DbContext
    {
        public virtual DbSet<Himall.Model.AccountDetailInfo> AccountDetailInfo { get; set; }
        public virtual DbSet<Himall.Model.AccountInfo> AccountInfo { get; set; }
        public virtual DbSet<Himall.Model.AccountMetaInfo> AccountMetaInfo { get; set; }
        public virtual DbSet<Himall.Model.AccountPurchaseAgreementInfo> AccountPurchaseAgreementInfo { get; set; }
        public virtual DbSet<Himall.Model.ActiveMarketServiceInfo> ActiveMarketServiceInfo { get; set; }
        public virtual DbSet<Himall.Model.AgreementInfo> AgreementInfo { get; set; }
        public virtual DbSet<Himall.Model.ApplyWithDrawInfo> ApplyWithDrawInfo { get; set; }
        public virtual DbSet<Himall.Model.ArticleCategoryInfo> ArticleCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.ArticleInfo> ArticleInfo { get; set; }
        public virtual DbSet<Himall.Model.AttributeInfo> AttributeInfo { get; set; }
        public virtual DbSet<Himall.Model.AttributeValueInfo> AttributeValueInfo { get; set; }
        public virtual DbSet<Himall.Model.BannerInfo> BannerInfo { get; set; }
        public virtual DbSet<Himall.Model.BonusInfo> BonusInfo { get; set; }
        public virtual DbSet<Himall.Model.BonusReceiveInfo> BonusReceiveInfo { get; set; }
        public virtual DbSet<Himall.Model.BrandInfo> BrandInfo { get; set; }
        public virtual DbSet<Himall.Model.BrowsingHistoryInfo> BrowsingHistoryInfo { get; set; }
        public virtual DbSet<Himall.Model.BusinessCategoryInfo> BusinessCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.CapitalDetailInfo> CapitalDetailInfo { get; set; }
        public virtual DbSet<Himall.Model.CapitalInfo> CapitalInfo { get; set; }
        public virtual DbSet<Himall.Model.CashDepositDetailInfo> CashDepositDetailInfo { get; set; }
        public virtual DbSet<Himall.Model.CashDepositInfo> CashDepositInfo { get; set; }
        public virtual DbSet<Himall.Model.CategoryCashDepositInfo> CategoryCashDepositInfo { get; set; }
        public virtual DbSet<Himall.Model.CategoryInfo> CategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.ChargeDetailInfo> ChargeDetailInfo { get; set; }
        public virtual DbSet<Himall.Model.CollocationInfo> CollocationInfo { get; set; }
        public virtual DbSet<Himall.Model.CollocationPoruductInfo> CollocationPoruductInfo { get; set; }
        public virtual DbSet<Himall.Model.CollocationSkuInfo> CollocationSkuInfo { get; set; }
        public virtual DbSet<Himall.Model.CouponInfo> CouponInfo { get; set; }
        public virtual DbSet<Himall.Model.CouponRecordInfo> CouponRecordInfo { get; set; }
        public virtual DbSet<Himall.Model.CouponSettingInfo> CouponSettingInfo { get; set; }
        public virtual DbSet<Himall.Model.CustomerServiceInfo> CustomerServiceInfo { get; set; }
        public virtual DbSet<Himall.Model.FavoriteInfo> FavoriteInfo { get; set; }
        public virtual DbSet<Himall.Model.FavoriteShopInfo> FavoriteShopInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorBrandInfo> FloorBrandInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorCategoryInfo> FloorCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorProductInfo> FloorProductInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorTablDetailsInfo> FloorTablDetailsInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorTablsInfo> FloorTablsInfo { get; set; }
        public virtual DbSet<Himall.Model.FloorTopicInfo> FloorTopicInfo { get; set; }
        public virtual DbSet<Himall.Model.FreightAreaContentInfo> FreightAreaContentInfo { get; set; }
        public virtual DbSet<Himall.Model.FreightTemplateInfo> FreightTemplateInfo { get; set; }
        public virtual DbSet<Himall.Model.GiftInfo> GiftInfo { get; set; }
        public virtual DbSet<Himall.Model.GiftOrderInfo> GiftOrderInfo { get; set; }
        public virtual DbSet<Himall.Model.GiftOrderItemInfo> GiftOrderItemInfo { get; set; }
        public virtual DbSet<Himall.Model.HandSlideAdInfo> HandSlideAdInfo { get; set; }
        public virtual DbSet<Himall.Model.HomeCategoryInfo> HomeCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.HomeCategoryRowInfo> HomeCategoryRowInfo { get; set; }
        public virtual DbSet<Himall.Model.HomeFloorInfo> HomeFloorInfo { get; set; }
        public virtual DbSet<Himall.Model.ImageAdInfo> ImageAdInfo { get; set; }
        public virtual DbSet<Himall.Model.InviteRecordInfo> InviteRecordInfo { get; set; }
        public virtual DbSet<Himall.Model.InviteRuleInfo> InviteRuleInfo { get; set; }
        public virtual DbSet<Himall.Model.InvoiceContextInfo> InvoiceContextInfo { get; set; }
        public virtual DbSet<Himall.Model.InvoiceTitleInfo> InvoiceTitleInfo { get; set; }
        public virtual DbSet<Himall.Model.LimitTimeMarketInfo> LimitTimeMarketInfo { get; set; }
        public virtual DbSet<Himall.Model.LogInfo> LogInfo { get; set; }
        public virtual DbSet<Himall.Model.ManagerInfo> ManagerInfo { get; set; }
        public virtual DbSet<Himall.Model.MarketServiceRecordInfo> MarketServiceRecordInfo { get; set; }
        public virtual DbSet<Himall.Model.MarketSettingInfo> MarketSettingInfo { get; set; }
        public virtual DbSet<Himall.Model.MarketSettingMetaInfo> MarketSettingMetaInfo { get; set; }
        public virtual DbSet<Himall.Model.MemberContactsInfo> MemberContactsInfo { get; set; }
        public virtual DbSet<Himall.Model.MemberGrade> MemberGrade { get; set; }
        public virtual DbSet<Himall.Model.MemberIntegral> MemberIntegral { get; set; }
        public virtual DbSet<Himall.Model.MemberIntegralExchangeRules> MemberIntegralExchangeRules { get; set; }
        public virtual DbSet<Himall.Model.MemberIntegralRecord> MemberIntegralRecord { get; set; }
        public virtual DbSet<Himall.Model.MemberIntegralRecordAction> MemberIntegralRecordAction { get; set; }
        public virtual DbSet<Himall.Model.MemberIntegralRule> MemberIntegralRule { get; set; }
        public virtual DbSet<Himall.Model.MemberOpenIdInfo> MemberOpenIdInfo { get; set; }
        public virtual DbSet<Himall.Model.MenuInfo> MenuInfo { get; set; }
        public virtual DbSet<Himall.Model.MessageLog> MessageLog { get; set; }
        public virtual DbSet<Himall.Model.MobileHomeProductsInfo> MobileHomeProductsInfo { get; set; }
        public virtual DbSet<Himall.Model.MobileHomeTopicsInfo> MobileHomeTopicsInfo { get; set; }
        public virtual DbSet<Himall.Model.ModuleProductInfo> ModuleProductInfo { get; set; }
        public virtual DbSet<Himall.Model.OpenIdsInfo> OpenIdsInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderCommentInfo> OrderCommentInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderComplaintInfo> OrderComplaintInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderInfo> OrderInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderItemInfo> OrderItemInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderOperationLogInfo> OrderOperationLogInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderPayInfo> OrderPayInfo { get; set; }
        public virtual DbSet<Himall.Model.OrderRefundInfo> OrderRefundInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductAttributeInfo> ProductAttributeInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductCommentInfo> ProductCommentInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductConsultationInfo> ProductConsultationInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductDescriptionInfo> ProductDescriptionInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductDescriptionTemplateInfo> ProductDescriptionTemplateInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductInfo> ProductInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductShopCategoryInfo> ProductShopCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductTypeInfo> ProductTypeInfo { get; set; }
        public virtual DbSet<Himall.Model.ProductVistiInfo> ProductVistiInfo { get; set; }
        public virtual DbSet<Himall.Model.RoleInfo> RoleInfo { get; set; }
        public virtual DbSet<Himall.Model.RolePrivilegeInfo> RolePrivilegeInfo { get; set; }
        public virtual DbSet<Himall.Model.SellerSpecificationValueInfo> SellerSpecificationValueInfo { get; set; }
        public virtual DbSet<Himall.Model.SensitiveWordsInfo> SensitiveWordsInfo { get; set; }
        public virtual DbSet<Himall.Model.ShippingAddressInfo> ShippingAddressInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopBonusGrantInfo> ShopBonusGrantInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopBonusInfo> ShopBonusInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopBonusReceiveInfo> ShopBonusReceiveInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopBrandApplysInfo> ShopBrandApplysInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopBrandsInfo> ShopBrandsInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopCategoryInfo> ShopCategoryInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopGradeInfo> ShopGradeInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopHomeModuleInfo> ShopHomeModuleInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopHomeModuleProductInfo> ShopHomeModuleProductInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopInfo> ShopInfo { get; set; }
        internal virtual DbSet<Himall.Model.ShoppingCartItemInfo> ShoppingCartItemInfo { get; set; }
        public virtual DbSet<Himall.Model.ShopVistiInfo> ShopVistiInfo { get; set; }
        public virtual DbSet<Himall.Model.SiteSettingsInfo> SiteSettingsInfo { get; set; }
        public virtual DbSet<Himall.Model.SKUInfo> SKUInfo { get; set; }
        public virtual DbSet<Himall.Model.SlideAdInfo> SlideAdInfo { get; set; }
        public virtual DbSet<Himall.Model.SpecificationValueInfo> SpecificationValueInfo { get; set; }
        public virtual DbSet<Himall.Model.StatisticOrderCommentsInfo> StatisticOrderCommentsInfo { get; set; }
        public virtual DbSet<Himall.Model.TopicInfo> TopicInfo { get; set; }
        public virtual DbSet<Himall.Model.TopicModuleInfo> TopicModuleInfo { get; set; }
        public virtual DbSet<Himall.Model.TypeBrandInfo> TypeBrandInfo { get; set; }
        public virtual DbSet<Himall.Model.UserMemberInfo> UserMemberInfo { get; set; }
        public virtual DbSet<Himall.Model.VShopExtendInfo> VShopExtendInfo { get; set; }
        public virtual DbSet<Himall.Model.VShopInfo> VShopInfo { get; set; }
        public virtual DbSet<Himall.Model.WeiXinBasicInfo> WeiXinBasicInfo { get; set; }
        public virtual DbSet<Himall.Model.WXAccTokenInfo> WXAccTokenInfo { get; set; }
        public virtual DbSet<Himall.Model.WXCardCodeLogInfo> WXCardCodeLogInfo { get; set; }
        public virtual DbSet<Himall.Model.WXCardLogInfo> WXCardLogInfo { get; set; }
        public virtual DbSet<Himall.Model.WXShopInfo> WXShopInfo { get; set; }
        public virtual DbSet<Himall.Model.WithDrawInfo> WithDrawInfo { get; set; }
        public Entities() : base("name=Entities")
        {
            ShoppingCartItemInfo = base.Set<Himall.Model.ShoppingCartItemInfo>();
        }

        public virtual int Job_Account(DateTime? startDate, DateTime? endDate)
        {
            ObjectParameter objectParameter = (startDate.HasValue ? new ObjectParameter("StartDate", startDate) : new ObjectParameter("StartDate", typeof(DateTime)));
            ObjectParameter objectParameter1 = (endDate.HasValue ? new ObjectParameter("EndDate", endDate) : new ObjectParameter("EndDate", typeof(DateTime)));
            System.Data.Entity.Core.Objects.ObjectContext objectContext = ((IObjectContextAdapter)this).ObjectContext;
            ObjectParameter[] objectParameterArray = new ObjectParameter[] { objectParameter, objectParameter1 };
            return objectContext.ExecuteFunction("Job_Account", objectParameterArray);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    }
}