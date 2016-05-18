using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Core.Plugins.Express
{
	public class ExpressPrintElement
	{
		private static Dictionary<int, string> _allPrintElements;

		public static Dictionary<int, string> AllPrintElements
		{
			get
			{
				if (ExpressPrintElement._allPrintElements == null)
				{
					ExpressPrintElement.initExpressAllElements();
				}
				return ExpressPrintElement._allPrintElements;
			}
		}

		public ExpressPrintElement.Point LeftTopPoint
		{
			get;
			set;
		}

		public int PrintElementIndex
		{
			get;
			set;
		}

		public ExpressPrintElement.Point RightBottomPoint
		{
			get;
			set;
		}

		public ExpressPrintElement()
		{
		}

		private static void initExpressAllElements()
		{
			ExpressPrintElement._allPrintElements = new Dictionary<int, string>()
			{
				{ 1, "收货人-姓名" },
				{ 2, "收货人-地址" },
				{ 3, "收货人-电话" },
				{ 5, "收货人-地区1级" },
				{ 6, "收货人-地区2级" },
				{ 7, "收货人-地区3级" },
				{ 8, "发货人-姓名" },
				{ 9, "发货人-地区1级" },
				{ 10, "发货人-地区2级" },
				{ 11, "发货人-地区3级" },
				{ 12, "发货人-地址" },
				{ 14, "发货人-电话" },
				{ 15, "订单-订单号" },
				{ 16, "订单-总金额" },
				{ 17, "订单-物品总重量" },
				{ 18, "订单-备注" },
				{ 19, "订单-详情" },
				{ 21, "网店名称" },
				{ 22, "对号-√" }
			};
		}

		public class Point
		{
			public int X
			{
				get;
				set;
			}

			public int Y
			{
				get;
				set;
			}

			public Point()
			{
			}
		}
	}
}