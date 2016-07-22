using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class PluginsInfo
	{
		public bool Enable
		{
			get;
			set;
		}

		public bool IsBind
		{
			get;
			set;
		}

		public bool IsSettingsValid
		{
			get;
			set;
		}

		public string PluginId
		{
			get;
			set;
		}

		public string ShortName
		{
			get;
			set;
		}

		public PluginsInfo()
		{
		}
	}
}