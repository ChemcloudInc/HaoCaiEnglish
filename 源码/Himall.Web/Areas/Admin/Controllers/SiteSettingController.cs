using AutoMapper;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class SiteSettingController : BaseAdminController
	{
		public SiteSettingController()
		{
		}

		public ActionResult Edit()
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			Mapper.CreateMap<SiteSettingsInfo, SiteSettingModel>().ForMember((SiteSettingModel a) => a.SiteIsOpen, (IMemberConfigurationExpression<SiteSettingsInfo> b) => b.MapFrom<bool>((SiteSettingsInfo s) => s.SiteIsClose));
			return View(Mapper.Map<SiteSettingsInfo, SiteSettingModel>(siteSettings));
		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult Edit(SiteSettingModel siteSettingModel)
		{
			if (string.IsNullOrWhiteSpace(siteSettingModel.WXLogo))
			{
				Result result = new Result()
				{
					success = false,
					msg = "请上传微信Logo",
					status = 1
				};
				return Json(result);
			}
			string mapPath = IOHelper.GetMapPath(siteSettingModel.Logo);
			string str = IOHelper.GetMapPath(siteSettingModel.MemberLogo);
			string mapPath1 = IOHelper.GetMapPath(siteSettingModel.QRCode);
			string str1 = string.Concat("logo", (new FileInfo(mapPath)).Extension);
			string str2 = string.Concat("memberLogo", (new FileInfo(mapPath)).Extension);
			string str3 = string.Concat("qrCode", (new FileInfo(mapPath)).Extension);
			string str4 = "/Storage/Plat/Site/";
			string mapPath2 = IOHelper.GetMapPath(str4);
			if (!Directory.Exists(mapPath2))
			{
				Directory.CreateDirectory(mapPath2);
			}
			if (!siteSettingModel.Logo.Contains("/Storage"))
			{
				IOHelper.CopyFile(mapPath, mapPath2, false, str1);
			}
			if (!siteSettingModel.MemberLogo.Contains("/Storage"))
			{
				IOHelper.CopyFile(str, mapPath2, false, str2);
			}
			if (!siteSettingModel.QRCode.Contains("/Storage"))
			{
				IOHelper.CopyFile(mapPath1, mapPath2, false, str3);
			}
			if (!siteSettingModel.WXLogo.Contains("/Storage"))
			{
				string str5 = string.Concat(str4, "wxlogo.png");
				string mapPath3 = IOHelper.GetMapPath(siteSettingModel.WXLogo);
				string mapPath4 = IOHelper.GetMapPath(str5);
				using (Image image = Image.FromFile(mapPath3))
				{
					image.Save(string.Concat(mapPath3, ".png"), ImageFormat.Png);
					if (System.IO.File.Exists(mapPath4))
					{
                        System.IO.File.Delete(mapPath4);
					}
					ImageHelper.CreateThumbnail(string.Concat(mapPath3, ".png"), mapPath4, 100, 100);
				}
				siteSettingModel.WXLogo = str5;
			}
			Result result1 = new Result();
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			siteSettings.SiteName = siteSettingModel.SiteName;
			siteSettings.SiteIsClose = siteSettingModel.SiteIsOpen;
			siteSettings.Logo = string.Concat(str4, str1);
			siteSettings.MemberLogo = string.Concat(str4, str2);
			siteSettings.QRCode = string.Concat(str4, str3);
			siteSettings.FlowScript = siteSettingModel.FlowScript;
			siteSettings.Site_SEOTitle = siteSettingModel.Site_SEOTitle;
			siteSettings.Site_SEOKeywords = siteSettingModel.Site_SEOKeywords;
			siteSettings.Site_SEODescription = siteSettingModel.Site_SEODescription;
			siteSettings.MobileVerifOpen = siteSettingModel.MobileVerifOpen;
			siteSettings.WXLogo = siteSettingModel.WXLogo;
			ServiceHelper.Create<ISiteSettingService>().SetSiteSettings(siteSettings);
			result1.success = true;
			return Json(result1);
		}
	}
}