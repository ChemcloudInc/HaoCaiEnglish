using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserInviteController : BaseMemberController
	{
		public UserInviteController()
		{
		}

		public ActionResult Index()
		{
			string str;
			long id = base.CurrentUser.Id;
			UserInviteModel memberInviteInfo = ServiceHelper.Create<IMemberInviteService>().GetMemberInviteInfo(id);
			InviteRuleInfo inviteRule = ServiceHelper.Create<IMemberInviteService>().GetInviteRule();
			string host = base.Request.Url.Host;
			if (base.Request.Url.Port != 80)
			{
				int port = base.Request.Url.Port;
				str = string.Concat(":", port.ToString());
			}
			else
			{
				str = "";
			}
			string str1 = string.Concat(host, str);
			memberInviteInfo.InviteLink = string.Format("http://{0}/Register/index/{1}", str1, id);
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