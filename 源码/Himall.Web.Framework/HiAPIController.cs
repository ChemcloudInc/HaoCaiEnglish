using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Web.Http;

namespace Himall.Web.Framework
{
	public abstract class HiAPIController : ApiController
	{
		public UserMemberInfo CurrentUser
		{
			get
			{
				long num = UserCookieEncryptHelper.Decrypt(WebHelper.GetCookie("Himall-User"), "Mobile");
				if (num == 0)
				{
					return null;
				}
				return Instance<IMemberService>.Create.GetMember(num);
			}
		}

		protected HiAPIController()
		{
		}
	}
}