using Himall.Model;
using System;
using System.Web.Mvc;

namespace Himall.Web.Framework
{
	internal interface IMobileOAuth
	{
		MobileOAuthUserInfo GetUserInfo(ActionExecutingContext filterContext, out string redirectUrl);

		MobileOAuthUserInfo GetUserInfo(ActionExecutingContext filterContext, out string redirectUrl, WXShopInfo settings);

		MobileOAuthUserInfo GetUserInfo_bequiet(ActionExecutingContext filterContext, out string redirectUrl, WXShopInfo settings);
	}
}