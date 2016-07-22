using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.ExpressPlugin;
using System;
using System.Collections.Generic;

namespace Himall.Plugin.Express.Zhongtong
{
	public class Service : ExpressPluginBase, IExpress, IPlugin
	{
		public Service()
		{
		}

		void Himall.Core.Plugins.IExpress.UpdatePrintElement(IEnumerable<ExpressPrintElement> expressPrintElements)
		{
			base.UpdatePrintElement(expressPrintElements);
		}

		void Himall.Core.Plugins.IPlugin.CheckCanEnable()
		{
			base.CheckCanEnable();
		}

	}
}