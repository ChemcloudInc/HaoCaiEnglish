using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.ExpressPlugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.Plugin.Express.EMS
{
	public class Service : ExpressPluginBase, IExpress, IPlugin
	{
		public Service()
		{
		}

		private string getEMSLastNum(string emsno)
		{
			List<char> list = emsno.ToList();
			char item = list[2];
			int num = int.Parse(item.ToString()) * 8;
			item = list[3];
			num = num + int.Parse(item.ToString()) * 6;
			item = list[4];
			num = num + int.Parse(item.ToString()) * 4;
			item = list[5];
			num = num + int.Parse(item.ToString()) * 2;
			item = list[6];
			num = num + int.Parse(item.ToString()) * 3;
			item = list[7];
			num = num + int.Parse(item.ToString()) * 5;
			item = list[8];
			num = num + int.Parse(item.ToString()) * 9;
			item = list[9];
			num = num + int.Parse(item.ToString()) * 7;
			num = 11 - num % 11;
			if (num == 10)
			{
				num = 0;
			}
			else if (num == 11)
			{
				num = 5;
			}
			return num.ToString();
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
			long num = Convert.ToInt64(currentExpressCode.Substring(2, 8));
			if (num < 99999999)
			{
				num = num + 1;
			}
			string str = num.ToString().PadLeft(8, '0');
			string str1 = string.Concat(currentExpressCode.Substring(0, 2), str, currentExpressCode.Substring(10, 1));
			str1 = string.Concat(currentExpressCode.Substring(0, 2), str, getEMSLastNum(str1), currentExpressCode.Substring(11, 2));
			return str1;
		}
	}
}