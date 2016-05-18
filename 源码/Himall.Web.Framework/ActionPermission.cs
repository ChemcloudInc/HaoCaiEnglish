using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Framework
{
	public class ActionPermission
	{
		public string ActionName
		{
			get;
			set;
		}

		public string ControllerName
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public ActionPermission()
		{
		}
	}
}