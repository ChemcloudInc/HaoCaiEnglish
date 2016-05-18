using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.ExpressPlugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.Plugin.Express.Shunfeng
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
			int i;
			long num;
			char item;
			int[] numArray = new int[12];
			int[] numArray1 = new int[12];
			List<char> list = currentExpressCode.ToList();
			string str = currentExpressCode.Substring(0, 11);
			string empty = string.Empty;
			if (!(currentExpressCode.Substring(0, 1) == "0"))
			{
				num = Convert.ToInt64(str) + 1;
				empty = num.ToString();
			}
			else
			{
				num = Convert.ToInt64(str) + 1;
				empty = string.Concat("0", num.ToString());
			}
			for (i = 0; i < 12; i++)
			{
				item = list[i];
				numArray[i] = int.Parse(item.ToString());
			}
			empty.ToList();
			for (i = 0; i < 11; i++)
			{
				item = empty[i];
				numArray1[i] = int.Parse(item.ToString());
			}
			if (!(numArray1[8] - numArray[8] != 1 ? true : numArray[8] % 2 != 1))
			{
				if (numArray[11] - 8 < 0)
				{
					numArray1[11] = numArray[11] - 8 + 10;
				}
				else
				{
					numArray1[11] = numArray[11] - 8;
				}
			}
			else if (!(numArray1[8] - numArray[8] != 1 ? true : numArray[8] % 2 != 0))
			{
				if (numArray[11] - 7 < 0)
				{
					numArray1[11] = numArray[11] - 7 + 10;
				}
				else
				{
					numArray1[11] = numArray[11] - 7;
				}
			}
			else if (!(numArray[9] == 3 || numArray[9] == 6 ? numArray[10] != 9 : true))
			{
				if (numArray[11] - 5 < 0)
				{
					numArray1[11] = numArray[11] - 5 + 10;
				}
				else
				{
					numArray1[11] = numArray[11] - 5;
				}
			}
			else if (numArray[10] == 9)
			{
				if (numArray[11] - 4 < 0)
				{
					numArray1[11] = numArray[11] - 4 + 10;
				}
				else
				{
					numArray1[11] = numArray[11] - 4;
				}
			}
			else if (numArray[11] - 1 < 0)
			{
				numArray1[11] = numArray[11] - 1 + 10;
			}
			else
			{
				numArray1[11] = numArray[11] - 1;
			}
			string str1 = string.Concat(empty, numArray1[11].ToString());
			return str1;
		}
	}
}