using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.ExpressPlugin;
using System;
using System.Collections.Generic;

namespace Himall.Plugin.Express.Zhaijisong
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


		public override string NextExpressCode(string currentExpressCode)
		{
			long num = Convert.ToInt64(currentExpressCode) + 11;
			if (num % 10 > 6)
			{
				num = num - 7;
			}
			return num.ToString().PadLeft(currentExpressCode.Length, '0');
		}
	}
}