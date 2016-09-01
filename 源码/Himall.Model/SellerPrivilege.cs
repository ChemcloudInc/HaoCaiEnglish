using System;

namespace Himall.Model
{
	public enum SellerPrivilege
	{
		[Privilege("Product", "Product Release", 2001, "product/PublicStepOne", "product", "PublicStepOne")]
		ProductAdd = 2001,
		[Privilege("Product", "Product Management", 2002, "product/management", "product", "")]
		ProductManage = 2002,
		[Privilege("Product", "Category Management", 2003, "category/management", "category", "")]
		ProductCategory = 2003,
        [Privilege("Product", "Brand Applyment", 2004, "brand/management", "brand", "")]
		BrandManage = 2004,
        [Privilege("Product", "Related Format", 2005, "productDescriptiontemplate/management", "productDescriptiontemplate", "")]
		ProductDescriptionTemplate = 2005,
        [Privilege("Product", "Consultment", 2006, "productconsultation/management", "productconsultation", "")]
		ConsultationManage = 2006,
		[Privilege("Product", "Evaluation", 2007, "ProductComment/management", "ProductComment", "")]
		CommentManage = 2007,
		[Privilege("Product", "Product Import", 2008, "ProductImport/ImportManage", "ProductImport", "")]
		ProductImportManage = 2008,
		[Privilege("Sale", "Order Management", 3001, "order/management", "order", "")]
		OrderManage = 3001,

        [Privilege("Sale", "Settlement Applyment", 3011, "Order/ApplySettlement", "order", "")]
        OrderApplySettlement = 3011,
        [Privilege("Sale", "Settlement Status", 3010, "Order/AccountTypeList", "order", "")]
        OrderAccountTypeList = 3010,
		[Privilege("Sale", "Refund Money", 3002, "orderrefund/management?showtype=2", "orderrefund", "")]
		OrderRefund = 3002,
		[Privilege("Sale", "Refund Goods", 3003, "orderrefund/management?showtype=3", "orderrefund", "")]
		OrderGoodsRefund = 3003,
        [Privilege("Sale", "Trade Complaints", 3004, "ordercomplaint/management", "ordercomplaint", "")]
		OrderComplaint = 3004,
        [Privilege("Sale", "Freight Management", 3006, "FreightTemplate/Index", "FreightTemplate", "")]
		FreightTemplate = 3006,
		[Privilege("Shop", "Page Setting", 4001, "PageSettings/management", "PageSettings", "")]
		PageSetting = 4001,
		[Privilege("Shop", "Shop Information", 4002, "shop/ShopDetail", "shop", "ShopDetail")]
		ShopInfo = 4002,
        [Privilege("Shop", "Customer Services", 4004, "CustomerService/management", "CustomerService", "")]
		CustomerService = 4004,
        [Privilege("Shop", "Settlement", 4005, "Account/management", "Account", "")]
		SettlementManage = 4005,
		[Privilege("Shop", "Margin Management", 4006, "CashDeposit/Management", "CashDeposit", "")]
		CashDepositManagement = 4006,
        [Privilege("Statistics", "Flow Statistics", 5001, "statistics/shopflow", "statistics", "shopflow")]
		TrafficStatistics = 5001,
        [Privilege("Statistics", "Shop Statistics", 5002, "statistics/shopsale", "statistics", "shopsale")]
		ShopStatistics = 5002,
        [Privilege("Statistics", "Rate of Bargain", 5003, "statistics/DealConversionRate", "statistics", "DealConversionRate")]
		SalesAnalysis = 5003,
        [Privilege("System", "Administrator", 6001, "Manager/management", "Manager", "")]
		AdminManage = 6001,
        [Privilege("System", " Permission Group", 6002, "Privilege/management", "Privilege", "")]
		PrivilegesManage = 6002,
        [Privilege("System", "Operation Log", 6003, "OperationLog/management", "OperationLog", "")]
		OperationLog = 6003,
        [Privilege("Marketing", "Flash Sale", 7001, "LimitTimeBuy/management", "LimitTimeBuy", "")]
		LimitTimeBuy = 7001,
        [Privilege("Marketing", "Discount Coupon", 7002, "Coupon/management", "Coupon", "")]
		Coupon = 7002,
        [Privilege("Marketing", "Purchase", 7003, "Collocation/management", "Collocation", "")]
		Collocation = 7003,
        [Privilege("Marketing", "Mobile", 7004, "MobileTopic/Management", "MobileTopic", "")]
		MobileTopic = 7004,
        [Privilege("Marketing", "Freight Free", 7005, "shop/FreightSetting", "shop", "FreightSetting")]
		FreightSetting = 7005,
        [Privilege("Marketing", "Cash Coupon", 7006, "ShopBonus/Management", "ShopBonus", "")]
		ShopBonus = 7006
        //[Privilege("微信", "微信配置", 8002, "WeiXin/BasicVShopSettings", "WeiXin", "")]
        //weixinSetting = 8002,
        //[Privilege("微信", "微信菜单", 8003, "WeiXin/MenuManage", "WeiXin", "")]
        //weixinMenu = 8003,
        //[Privilege("微信", "门店管理", 8004, "Poi/Index", "Poi", "")]
        //Poi = 8004,
        //[Privilege("微信", "iBeacon设备", 8005, "ShakeAround/Index", "ShakeAround", "")]
        //ShakeAroundDev = 8005,
        //[Privilege("微信", "摇一摇周边页面", 8006, "ShakeAround/PageIndex", "ShakeAround", "")]
        //ShakeAroundPage = 8006,
        //[Privilege("微店", "我的微店", 9001, "Vshop/management", "Vshop", "")]
        //VShop = 9001,
        //[Privilege("微店", "首页配置", 9002, "Vshop/VshopHomeSite", "Vshop", "")]
        //VshopHomeSite = 9002
	}
}