using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class AjaxReturnData
	{
		public string msg
		{
			get;
			set;
		}

		public bool success
		{
			get;
			set;
		}

		public AjaxReturnData()
		{
		}
	}
}