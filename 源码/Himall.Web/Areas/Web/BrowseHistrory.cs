using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web
{
	public class BrowseHistrory
	{
		public BrowseHistrory()
		{
		}

		public static void AddBrowsingProduct(long productId, long userId = 0L)
		{
			List<ProductBrowsedHistoryModel> productBrowsedHistoryModels = new List<ProductBrowsedHistoryModel>();
			string cookie = WebHelper.GetCookie("Himall_ProductBrowsingHistory");
			if (!string.IsNullOrEmpty(cookie))
			{
				string[] strArrays = cookie.Split(new char[] { ',' });
				for (int i = 0; i < strArrays.Length; i++)
				{
					string str = strArrays[i];
					string[] strArrays1 = str.Split(new char[] { ';' });
					if (strArrays1.Length <= 1)
					{
						ProductBrowsedHistoryModel productBrowsedHistoryModel = new ProductBrowsedHistoryModel()
						{
							ProductId = long.Parse(strArrays1[0]),
							BrowseTime = DateTime.Now
						};
						productBrowsedHistoryModels.Add(productBrowsedHistoryModel);
					}
					else
					{
						ProductBrowsedHistoryModel productBrowsedHistoryModel1 = new ProductBrowsedHistoryModel()
						{
							ProductId = long.Parse(strArrays1[0]),
							BrowseTime = DateTime.Parse(strArrays1[1])
						};
						productBrowsedHistoryModels.Add(productBrowsedHistoryModel1);
					}
				}
			}
			if (productBrowsedHistoryModels.Count < 20 && !productBrowsedHistoryModels.Any((ProductBrowsedHistoryModel a) => a.ProductId == productId))
			{
				ProductBrowsedHistoryModel productBrowsedHistoryModel2 = new ProductBrowsedHistoryModel()
				{
					ProductId = productId,
					BrowseTime = DateTime.Now
				};
				productBrowsedHistoryModels.Add(productBrowsedHistoryModel2);
			}
			else if (productBrowsedHistoryModels.Count < 20 || productBrowsedHistoryModels.Any((ProductBrowsedHistoryModel a) => a.ProductId == productId))
			{
				ProductBrowsedHistoryModel productBrowsedHistoryModel3 = (
					from a in productBrowsedHistoryModels
					where a.ProductId == productId
					select a).FirstOrDefault();
				productBrowsedHistoryModels.Remove(productBrowsedHistoryModel3);
				ProductBrowsedHistoryModel productBrowsedHistoryModel4 = new ProductBrowsedHistoryModel()
				{
					ProductId = productId,
					BrowseTime = DateTime.Now
				};
				productBrowsedHistoryModels.Add(productBrowsedHistoryModel4);
			}
			else
			{
				productBrowsedHistoryModels.RemoveAt(productBrowsedHistoryModels.Count - 1);
				ProductBrowsedHistoryModel productBrowsedHistoryModel5 = new ProductBrowsedHistoryModel()
				{
					ProductId = productId,
					BrowseTime = DateTime.Now
				};
				productBrowsedHistoryModels.Add(productBrowsedHistoryModel5);
			}
			if (userId != 0)
			{
				foreach (ProductBrowsedHistoryModel productBrowsedHistoryModel6 in productBrowsedHistoryModels)
				{
					try
					{
						IProductService create = Instance<IProductService>.Create;
						BrowsingHistoryInfo browsingHistoryInfo = new BrowsingHistoryInfo()
						{
							MemberId = userId,
							BrowseTime = productBrowsedHistoryModel6.BrowseTime,
							ProductId = productBrowsedHistoryModel6.ProductId
						};
						create.AddBrowsingProduct(browsingHistoryInfo);
					}
					catch
					{
					}
				}
				WebHelper.DeleteCookie("Himall_ProductBrowsingHistory");
				return;
			}
			string str1 = "";
			foreach (ProductBrowsedHistoryModel productBrowsedHistoryModel7 in productBrowsedHistoryModels)
			{
				object obj1 = str1;
				object[] objArray = new object[] { obj1, productBrowsedHistoryModel7.ProductId, ";", null, null };
				objArray[3] = productBrowsedHistoryModel7.BrowseTime.ToString();
				objArray[4] = ",";
				str1 = string.Concat(objArray);
			}
			string str2 = str1.TrimEnd(new char[] { ',' });
			DateTime now = DateTime.Now;
			WebHelper.SetCookie("Himall_ProductBrowsingHistory", str2, now.AddDays(7));
		}

		public static List<ProductBrowsedHistoryModel> GetBrowsingProducts(int num, long userId = 0L)
		{
			List<ProductBrowsedHistoryModel> productBrowsedHistoryModels = new List<ProductBrowsedHistoryModel>();
			string cookie = WebHelper.GetCookie("Himall_ProductBrowsingHistory");
			if (!string.IsNullOrEmpty(cookie))
			{
				string[] strArrays = cookie.Split(new char[] { ',' });
				for (int i = 0; i < strArrays.Length; i++)
				{
					string str = strArrays[i];
					string[] strArrays1 = str.Split(new char[] { ';' });
					if (strArrays1.Length <= 1)
					{
						ProductBrowsedHistoryModel productBrowsedHistoryModel = new ProductBrowsedHistoryModel()
						{
							ProductId = long.Parse(strArrays1[0]),
							BrowseTime = DateTime.Now
						};
						productBrowsedHistoryModels.Add(productBrowsedHistoryModel);
					}
					else
					{
						ProductBrowsedHistoryModel productBrowsedHistoryModel1 = new ProductBrowsedHistoryModel()
						{
							ProductId = long.Parse(strArrays1[0]),
							BrowseTime = DateTime.Parse(strArrays1[1])
						};
						productBrowsedHistoryModels.Add(productBrowsedHistoryModel1);
					}
				}
			}
			List<ProductBrowsedHistoryModel> productBrowsedHistoryModels1 = new List<ProductBrowsedHistoryModel>();
			if (userId == 0)
			{
				List<ProductBrowsedHistoryModel> list = (
					from a in (
                        from d in Instance<IProductService>.Create.GetProductByIds(
                            from a in productBrowsedHistoryModels
                            select a.ProductId)
                        where (int)d.SaleStatus == 1 && (int)d.AuditStatus == 2
                        select d).ToArray()
                    select new ProductBrowsedHistoryModel()
					{
						ImagePath = a.ImagePath,
						ProductId = a.Id,
						ProductName = a.ProductName,
						ProductPrice = a.MinSalePrice
					}).ToList();
				foreach (ProductBrowsedHistoryModel productBrowsedHistoryModel2 in list)
				{
					ProductBrowsedHistoryModel imagePath = (
						from item in productBrowsedHistoryModels
						where item.ProductId == productBrowsedHistoryModel2.ProductId
						select item).FirstOrDefault();
					imagePath.ImagePath = productBrowsedHistoryModel2.ImagePath;
					imagePath.ProductName = productBrowsedHistoryModel2.ProductName;
					imagePath.ProductPrice = productBrowsedHistoryModel2.ProductPrice;
				}
				return (
					from a in productBrowsedHistoryModels
					orderby a.BrowseTime descending
					select a).ToList();
			}
			foreach (ProductBrowsedHistoryModel productBrowsedHistoryModel3 in productBrowsedHistoryModels)
			{
				BrowseHistrory.AddBrowsingProduct(productBrowsedHistoryModel3.ProductId, userId);
			}
			return (
				from a in (
					from d in Instance<IProductService>.Create.GetBrowsingProducts(userId)
					where (int)d.Himall_Products.SaleStatus == 1 && (int)d.Himall_Products.AuditStatus == 2
					select d into a
					orderby a.BrowseTime descending
					select a).Take(num).ToArray().AsEnumerable<BrowsingHistoryInfo>()
				select new ProductBrowsedHistoryModel()
				{
					ImagePath = a.Himall_Products.ImagePath,
					ProductId = a.ProductId,
					ProductName = a.Himall_Products.ProductName,
					ProductPrice = a.Himall_Products.MinSalePrice,
					BrowseTime = a.BrowseTime
				}).ToList();
		}
	}
}