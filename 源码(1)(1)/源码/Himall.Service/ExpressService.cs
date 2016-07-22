using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himall.Service
{
	public class ExpressService : ServiceBase, IExpressService, IService, IDisposable
	{
		private const string RELATEIVE_PATH = "/Plugins/Express/";

		public ExpressService()
		{
		}

		public IEnumerable<IExpress> GetAllExpress()
		{
			IEnumerable<PluginInfo> installedPluginInfos = PluginsManagement.GetInstalledPluginInfos(PluginType.Express);
			IExpress[] expressArray = new IExpress[installedPluginInfos.Count()];
			int num = 0;
			foreach (PluginInfo installedPluginInfo in installedPluginInfos)
			{
				string classFullName = installedPluginInfo.ClassFullName;
				char[] chrArray = new char[] { ',' };
				string str = classFullName.Split(chrArray)[1];
				IExpress express = Instance.Get<IExpress>(installedPluginInfo.ClassFullName);
				express.Logo = string.Concat("/Plugins/Express/", str, "/", express.Logo);
				express.BackGroundImage = string.Concat("/Plugins/Express/", str, "/", express.BackGroundImage);
				int num1 = num;
				num = num1 + 1;
				expressArray[num1] = express;
			}
			return expressArray;
		}

		public IExpress GetExpress(string name)
		{
			IEnumerable<IExpress> allExpress = GetAllExpress();
			return allExpress.FirstOrDefault((IExpress item) => item.Name == name);
		}

		public ExpressData GetExpressData(string expressCompanyName, string shipOrderNumber)
		{
			string kuaidi100Code = GetExpress(expressCompanyName).Kuaidi100Code;
			string str = string.Format("http://www.kuaidi100.com/query?type={0}&postid={1}", kuaidi100Code, shipOrderNumber);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str);
			httpWebRequest.Timeout = 8000;
			ExpressData expressDatum = new ExpressData();
			try
			{
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				if (response.StatusCode != HttpStatusCode.OK)
				{
					expressDatum.Message = "网络错误";
				}
				else
				{
					Stream responseStream = response.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
					StringBuilder stringBuilder = new StringBuilder(streamReader.ReadToEnd());
					stringBuilder.Replace("&amp;", "").Replace("&nbsp;", "").Replace("&", "");
					JObject jObjects = JObject.Parse(stringBuilder.ToString());
					string str1 = "200";
					if (jObjects["status"] == null || !(jObjects["status"].ToString() == str1))
					{
						expressDatum.Message = jObjects["message"].ToString();
					}
					else
					{
						JArray item = jObjects["data"] as JArray;
						if (item == null)
						{
							throw new ApplicationException("查询源数据格式错误，没有找到data节点");
						}
						List<ExpressDataItem> expressDataItems = new List<ExpressDataItem>();
						foreach (JToken jTokens in item)
						{
							if (jTokens["time"] == null)
							{
								throw new ApplicationException("查询源数据格式错误，没有找到time节点");
							}
							if (jTokens["context"] == null)
							{
								throw new ApplicationException("查询源数据格式错误，没有找到context节点");
							}
							ExpressDataItem expressDataItem = new ExpressDataItem()
							{
								Time = DateTime.ParseExact(jTokens["time"].ToString(), "yyyy-MM-dd HH:mm:ss", null),
								Content = jTokens["context"].ToString()
							};
							expressDataItems.Add(expressDataItem);
						}
						expressDatum.Success = true;
						expressDatum.ExpressDataItems = expressDataItems;
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(string.Format("快递查询错误:{{kuaidi100Code:{0},shipOrderNumber:{1}}}", kuaidi100Code, shipOrderNumber), exception);
				expressDatum.Message = "未知错误";
			}
			return expressDatum;
		}

		private IExpress GetIExpressByName(string name)
		{
			return PluginsManagement.GetInstalledPlugins<IExpress>(PluginType.Express).FirstOrDefault((IExpress item) => item.Name == name);
		}

		public IDictionary<int, string> GetPrintElementIndexAndOrderValue(long shopId, long orderId, IEnumerable<int> printElementIndexes)
		{
			OrderInfo orderInfo = context.OrderInfo.FirstOrDefault((OrderInfo item) => item.Id == orderId && item.ShopId == shopId);
			if (orderInfo == null)
			{
				throw new HimallException(string.Format("未在本店铺中找到id为{0}的订单", orderId));
			}
			ShopInfo shopInfo = context.ShopInfo.FirstOrDefault((ShopInfo item) => item.Id == shopId);
			return GetPrintElementIndexAndOrderValue(shopInfo, orderInfo, printElementIndexes);
		}

		private IDictionary<int, string> GetPrintElementIndexAndOrderValue(ShopInfo shop, OrderInfo order, IEnumerable<int> printElementIndexes)
		{
			if (order == null)
			{
				throw new NullReferenceException("订单为空");
			}
			Dictionary<int, string> nums = new Dictionary<int, string>();
			RegionService regionService = new RegionService();
			string[] strArrays = order.RegionFullName.Split(new char[] { ' ' });
			string empty = string.Empty;
			string[] strArrays1 = new string[0];
			if (shop.SenderRegionId.HasValue)
			{
				int? senderRegionId = shop.SenderRegionId;
				empty = regionService.GetRegionFullName(senderRegionId.Value, " ");
				strArrays1 = empty.Split(new char[] { ' ' });
			}
			string str = string.Empty;
			foreach (int list in printElementIndexes.ToList())
			{
				string shipTo = string.Empty;
				switch (list)
				{
					case 1:
					{
						shipTo = order.ShipTo;
						break;
					}
					case 2:
					{
						shipTo = order.Address;
						break;
					}
					case 3:
					{
						shipTo = order.CellPhone;
						break;
					}
					case 4:
					{
						shipTo = "";
						break;
					}
					case 5:
					{
						shipTo = strArrays[0];
						break;
					}
					case 6:
					{
						shipTo = strArrays.Length > 1 ? strArrays[1] : "";
						break;
					}
					case 7:
					{
						shipTo = strArrays.Length > 2 ? strArrays[2] : "";
						break;
					}
					case 8:
					{
						shipTo = shop.SenderName;
						break;
					}
					case 9:
					{
						shipTo = strArrays1.Length > 0 ? strArrays1[0] : "";
						break;
					}
					case 10:
					{
						shipTo = strArrays1.Length > 1 ? strArrays1[2] : "";
						break;
					}
					case 11:
					{
						shipTo = strArrays1.Length > 2 ? strArrays1[2] : "";
						break;
					}
					case 12:
					{
						shipTo = shop.SenderAddress;
						break;
					}
					case 13:
					{
						shipTo = "";
						break;
					}
					case 14:
					{
						shipTo = shop.SenderPhone;
						break;
					}
					case 15:
					{
						shipTo = order.Id.ToString();
						break;
					}
					case 16:
					{
						shipTo = order.OrderTotalAmount.ToString("F2");
						break;
					}
					case 17:
					{
						shipTo = string.Empty;
						break;
					}
					case 18:
					{
						shipTo = (string.IsNullOrWhiteSpace(order.UserRemark) ? "" : order.UserRemark.ToString());
						break;
					}
					case 19:
					{
						shipTo = string.Empty;
						break;
					}
					case 20:
					{
						shipTo = string.Empty;
						break;
					}
					case 21:
					{
						shipTo = shop.ShopName;
						break;
					}
					case 22:
					{
						shipTo = "√";
						break;
					}
					default:
					{
						goto case 20;
					}
				}
				nums.Add(list, shipTo);
			}
			return nums;
		}

		public IEnumerable<IExpress> GetRecentExpress(long shopId, int takeNumber)
		{
			IQueryable<string> strs = (
				from item in context.OrderInfo
				where item.ShopId == shopId && !string.IsNullOrEmpty(item.ExpressCompanyName)
				orderby item.Id descending
				select item.ExpressCompanyName).Take(takeNumber);
			IEnumerable<IExpress> allExpress = GetAllExpress();
			List<IExpress> list = (
				from item in allExpress
				where strs.Contains<string>(item.Name)
				select item).ToList();
			if (list.Count < takeNumber)
			{
				Random random = new Random();
				list.AddRange((
					from item in allExpress
					where !strs.Contains<string>(item.Name)
					orderby random.Next()
					select item).Take(takeNumber - list.Count));
			}
			return list;
		}

		public void UpdatePrintElement(string name, IEnumerable<ExpressPrintElement> elements)
		{
            GetIExpressByName(name).UpdatePrintElement(elements);
		}
	}
}