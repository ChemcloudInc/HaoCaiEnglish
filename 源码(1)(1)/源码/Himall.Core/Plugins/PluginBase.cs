using System;
using System.Runtime.CompilerServices;

namespace Himall.Core.Plugins
{
	public abstract class PluginBase
	{
		public Himall.Core.Plugins.PluginInfo PluginInfo
		{
			get;
			set;
		}

		protected PluginBase()
		{
		}
	}
}