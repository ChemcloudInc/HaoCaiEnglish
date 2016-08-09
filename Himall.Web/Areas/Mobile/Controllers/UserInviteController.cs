using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class UserInviteController : BaseMobileMemberController
	{
		private SiteSettingsInfo _siteSetting;

		public UserInviteController()
		{
            _siteSetting = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
		}

		public ActionResult Index()
		{
			string str;
			ViewBag.WeiXin = false;
			if (!string.IsNullOrWhiteSpace(_siteSetting.WeixinAppId) && !string.IsNullOrWhiteSpace(_siteSetting.WeixinAppSecret) && base.PlatformType == Himall.Core.PlatformType.WeiXin)
			{
				ViewBag.WeiXin = true;
				string empty = string.Empty;
				IWXApiService wXApiService = ServiceHelper.Create<IWXApiService>();
				empty = wXApiService.GetTicket(_siteSetting.WeixinAppId, _siteSetting.WeixinAppSecret);
				JSSDKHelper jSSDKHelper = new JSSDKHelper();
				string timestamp = JSSDKHelper.GetTimestamp();
				string noncestr = JSSDKHelper.GetNoncestr();
				string signature = jSSDKHelper.GetSignature(empty, noncestr, timestamp, base.Request.Url.AbsoluteUri);
				ViewBag.Timestamp = timestamp;
				ViewBag.NonceStr = noncestr;
				ViewBag.Signature = signature;
				ViewBag.AppId = _siteSetting.WeixinAppId;
			}
			long id = base.CurrentUser.Id;
			UserInviteModel memberInviteInfo = ServiceHelper.Create<IMemberInviteService>().GetMemberInviteInfo(id);
			InviteRuleInfo inviteRule = ServiceHelper.Create<IMemberInviteService>().GetInviteRule();
			MemberIntegralExchangeRules integralChangeRule = ServiceHelper.Create<IMemberIntegralService>().GetIntegralChangeRule();
			if (integralChangeRule != null)
			{
				dynamic viewBag = base.ViewBag;
				int value = inviteRule.InviteIntegral.Value / integralChangeRule.IntegralPerMoney;
				viewBag.IntergralMoney = value.ToString("f2");
			}
			string host = base.Request.Url.Host;
			string str1 = host;
			if (base.Request.Url.Port != 80)
			{
				int port = base.Request.Url.Port;
				str = string.Concat(":", port.ToString());
			}
			else
			{
				str = "";
			}
			host = string.Concat(str1, str);
			memberInviteInfo.InviteLink = string.Format("http://{0}/Register/index/{1}", host, id);
			inviteRule.ShareIcon = string.Format("http://{0}{1}", host, inviteRule.ShareIcon);
			Bitmap bitmap = QRCodeHelper.Create(memberInviteInfo.InviteLink);
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Gif);
			string str2 = string.Concat("data:image/gif;base64,", Convert.ToBase64String(memoryStream.ToArray()));
			memoryStream.Dispose();
			memberInviteInfo.QR = str2;
			Tuple<UserInviteModel, InviteRuleInfo, UserMemberInfo> tuple = new Tuple<UserInviteModel, InviteRuleInfo, UserMemberInfo>(memberInviteInfo, inviteRule, base.CurrentUser);
			return View(tuple);
		}
	}
}