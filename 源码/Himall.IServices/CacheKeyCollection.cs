using System;

namespace Himall.IServices
{
	public static class CacheKeyCollection
	{
		public const string UserImportOpCount = "Cache-UserImportOpCount";

		public const string Region = "Cache-Regions";

		public const string Category = "Cache-Categories";

		public const string HomeCategory = "Cache-HomeCategories";

		public const string Brand = "Cache-Brands";

		public const string SiteSettings = "Cache-SiteSettings";

		public const string Banners = "Cache-Banners";

		public const string Advertisement = "Cache-Adverts";

		public const string BottomHelpers = "Cache-Helps";

		public const string ExpressTemplate = "Cache-ExperssTemplate";

		public static string ChargeOrderKey(string id)
		{
			return string.Format("Cache-ChargeOrder-{0}", id);
		}

		public static string HotConcernedProduct(long shopId)
		{
			return string.Format("Cache-HotConcernedProduct-{0}", shopId);
		}

		public static string HotSaleProduct(long shopId)
		{
			return string.Format("Cache-HotSaleProduct-{0}", shopId);
		}

		public static string Manager(long managerId)
		{
			return string.Format("Cache-Manager-{0}", managerId);
		}

		public static string ManagerLoginError(string username)
		{
			return string.Format("Cache-Manager-Login-{0}", username);
		}

		public static string Member(long memberId)
		{
			return string.Format("Cache-Member-{0}", memberId);
		}

		public static string MemberFindPassWordCheck(string username, string pluginId)
		{
			return string.Format("Cache-Member-PassWord-{0}-{1}", username, pluginId);
		}

		public static string MemberLoginError(string username)
		{
			return string.Format("Cache-Member-Login-{0}", username);
		}

		public static string MemberPluginCheck(string username, string pluginId)
		{
			return string.Format("Cache-Member-{0}-{1}", username, pluginId);
		}

		public static string MemberPluginCheckTime(string username, string pluginId)
		{
			return string.Format("Cache-CheckTime-{0}-{1}", username, pluginId);
		}

		public static string MemberPluginFindPassWordTime(string username, string pluginId)
		{
			return string.Format("Cache-FindPasswordTime-{0}-{1}", username, pluginId);
		}

		public static string MemberPluginReBindStepTime(string username, string pluginId)
		{
			return string.Format("Cache-ReBindStepTime-{0}-{1}", username, pluginId);
		}

		public static string MemberPluginReBindTime(string username, string pluginId)
		{
			return string.Format("Cache-ReBindTime-{0}-{1}", username, pluginId);
		}

		public static string NewSaleProduct(long shopId)
		{
			return string.Format("Cache-NewSaleProduct-{0}", shopId);
		}

		public static string PaymentState(string orderIds)
		{
			return string.Format("Cache-PaymentState-{0}", orderIds);
		}

		public static string SceneReturn(string sceneid)
		{
			return string.Format("Cache-SceneReturn-{0}", sceneid);
		}

		public static string SceneState(string sceneid)
		{
			return string.Format("Cache-SceneState-{0}", sceneid);
		}

		public static string Seller(long sellerId)
		{
			return string.Format("Cache-Seller-{0}", sellerId);
		}

		public static string ShopConcerned(long shopId)
		{
			return string.Format("Cache-ShopConcerned-{0}", shopId);
		}

		public static string UserImportProductCount(long userid)
		{
			return string.Format("Cache-{0}-ImportProductCount", userid);
		}

		public static string UserImportProductTotal(long userid)
		{
			return string.Format("Cache-{0}-ImportProductTotal", userid);
		}
	}
}