using Himall.Web;
using System;

namespace Himall.Web.Areas.Web
{
	public static class BizAfterLogin
	{
		public static void Run(long memberId)
		{
			(new CartHelper()).UpdateCartInfoFromCookieToServer(memberId);
		}
	}
}