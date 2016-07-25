using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Web
{
	public class CartHelper
	{
		public CartHelper()
		{
		}

		public void AddToCart(string skuId, int count, long memberId)
		{
            CheckSkuIdIsValid(skuId);
			if (memberId > 0)
			{
				Instance<ICartService>.Create.AddToCart(skuId, count, memberId);
				return;
			}
			string cookie = WebHelper.GetCookie("HIMALL-CART");
			if (string.IsNullOrWhiteSpace(cookie))
			{
				WebHelper.SetCookie("HIMALL-CART", string.Format("{0}:{1}", skuId, count));
				return;
			}
			string[] strArrays = cookie.Split(new char[] { ',' });
			string empty = string.Empty;
			bool flag = false;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				string[] strArrays2 = str.Split(new char[] { ':' });
				if (strArrays2[0] != skuId)
				{
					empty = string.Concat(empty, ",", str);
				}
				else
				{
					object obj = empty;
					object[] objArray = new object[] { obj, ",", skuId, ":", int.Parse(strArrays2[1]) + count };
					empty = string.Concat(objArray);
					flag = true;
				}
			}
			if (!flag)
			{
				object obj1 = empty;
				object[] objArray1 = new object[] { obj1, ",", skuId, ":", count };
				empty = string.Concat(objArray1);
			}
			if (!string.IsNullOrWhiteSpace(empty))
			{
				empty = empty.Substring(1);
			}
			WebHelper.SetCookie("HIMALL-CART", empty);
		}

		private void CheckSkuIdIsValid(string skuId)
		{
			long num = 0;
			char[] chrArray = new char[] { '\u005F' };
			long.TryParse(skuId.Split(chrArray)[0], out num);
			if (num == 0)
			{
				throw new InvalidPropertyException("SKUId无效");
			}
			if (Instance<IProductService>.Create.GetSku(skuId) == null)
			{
				throw new InvalidPropertyException("SKUId无效");
			}
		}

		public ShoppingCartInfo GetCart(long memberId)
		{
			ShoppingCartInfo shoppingCartInfo;
			if (memberId <= 0)
			{
				shoppingCartInfo = new ShoppingCartInfo();
				string cookie = WebHelper.GetCookie("HIMALL-CART");
				if (!string.IsNullOrWhiteSpace(cookie))
				{
					string[] strArrays = cookie.Split(new char[] { ',' });
					ShoppingCartItem[] shoppingCartItemArray = new ShoppingCartItem[strArrays.Length];
					int num = 0;
					string[] strArrays1 = strArrays;
					for (int i = 0; i < strArrays1.Length; i++)
					{
						string str = strArrays1[i];
						string[] strArrays2 = str.Split(new char[] { ':' });
						int num1 = num;
						num = num1 + 1;
						ShoppingCartItem shoppingCartItem = new ShoppingCartItem();
						string str1 = strArrays2[0];
						char[] chrArray = new char[] { '\u005F' };
						shoppingCartItem.ProductId = long.Parse(str1.Split(chrArray)[0]);
						shoppingCartItem.SkuId = strArrays2[0];
						shoppingCartItem.Quantity = int.Parse(strArrays2[1]);
						shoppingCartItemArray[num1] = shoppingCartItem;
					}
					shoppingCartInfo.Items = shoppingCartItemArray;
				}
			}
			else
			{
				shoppingCartInfo = ServiceHelper.Create<ICartService>().GetCart(memberId);
			}
			return shoppingCartInfo;
		}

		public IEnumerable<long> GetCartProductIds(long memberId)
		{
			long[] array = new long[0];
			if (memberId <= 0)
			{
				string cookie = WebHelper.GetCookie("HIMALL-CART");
				if (!string.IsNullOrWhiteSpace(cookie))
				{
					string[] strArrays = cookie.Split(new char[] { ',' });
					array = new long[strArrays.Length];
					int num = 0;
					string[] strArrays1 = strArrays;
					for (int i = 0; i < strArrays1.Length; i++)
					{
						string str = strArrays1[i];
						string[] strArrays2 = str.Split(new char[] { ':' });
						int num1 = num;
						num = num1 + 1;
						array[num1] = long.Parse(strArrays2[0]);
					}
				}
			}
			else
			{
				array = (
					from item in ServiceHelper.Create<ICartService>().GetCart(memberId).Items
					select item.ProductId).ToArray();
			}
			return array;
		}

		public IEnumerable<string> GetCartProductSKUIds(long memberId)
		{
			string[] array = new string[0];
			if (memberId <= 0)
			{
				string cookie = WebHelper.GetCookie("HIMALL-CART");
				if (!string.IsNullOrWhiteSpace(cookie))
				{
					string[] strArrays = cookie.Split(new char[] { ',' });
					array = new string[strArrays.Length];
					int num = 0;
					string[] strArrays1 = strArrays;
					for (int i = 0; i < strArrays1.Length; i++)
					{
						string str = strArrays1[i];
						string[] strArrays2 = str.Split(new char[] { ':' });
						int num1 = num;
						num = num1 + 1;
						array[num1] = strArrays2[0];
					}
				}
			}
			else
			{
				array = (
					from item in ServiceHelper.Create<ICartService>().GetCart(memberId).Items
					select item.SkuId).ToArray();
			}
			return array;
		}

		public void RemoveFromCart(string skuId, long memberId)
		{
			if (memberId > 0)
			{
				ServiceHelper.Create<ICartService>().DeleteCartItem(skuId, memberId);
				return;
			}
			string cookie = WebHelper.GetCookie("HIMALL-CART");
			if (!string.IsNullOrWhiteSpace(cookie))
			{
				string[] strArrays = cookie.Split(new char[] { ',' });
				string empty = string.Empty;
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					char[] chrArray = new char[] { ':' };
					if (str.Split(chrArray)[0] != skuId)
					{
						empty = string.Concat(empty, ",", str);
					}
				}
				if (!string.IsNullOrWhiteSpace(empty))
				{
					empty = empty.Substring(1);
				}
				WebHelper.SetCookie("HIMALL-CART", empty);
			}
		}

		public void RemoveFromCart(IEnumerable<string> skuIds, long memberId)
		{
			if (memberId > 0)
			{
				ServiceHelper.Create<ICartService>().DeleteCartItem(skuIds, memberId);
				return;
			}
			string cookie = WebHelper.GetCookie("HIMALL-CART");
			if (!string.IsNullOrWhiteSpace(cookie))
			{
				string[] strArrays = cookie.Split(new char[] { ',' });
				string empty = string.Empty;
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					char[] chrArray = new char[] { ':' };
					if (!skuIds.Contains<string>(str.Split(chrArray)[0]))
					{
						empty = string.Concat(empty, ",", str);
					}
				}
				if (!string.IsNullOrWhiteSpace(empty))
				{
					empty = empty.Substring(1);
				}
				WebHelper.SetCookie("HIMALL-CART", empty);
			}
		}

		public void UpdateCartInfoFromCookieToServer(long memberId)
		{
			string cookie = WebHelper.GetCookie("HIMALL-CART");
			if (!string.IsNullOrWhiteSpace(cookie))
			{
				string[] strArrays = cookie.Split(new char[] { ',' });
				ShoppingCartItem[] shoppingCartItemArray = new ShoppingCartItem[strArrays.Length];
				int num = 0;
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					string[] strArrays2 = str.Split(new char[] { ':' });
					int num1 = num;
					num = num1 + 1;
					ShoppingCartItem shoppingCartItem = new ShoppingCartItem()
					{
						SkuId = strArrays2[0],
						Quantity = int.Parse(strArrays2[1])
					};
					shoppingCartItemArray[num1] = shoppingCartItem;
				}
				ServiceHelper.Create<ICartService>().AddToCart(shoppingCartItemArray, memberId);
			}
			WebHelper.DeleteCookie("HIMALL-CART");
		}

		public void UpdateCartItem(string skuId, int count, long memberId)
		{
			if (memberId > 0)
			{
				ServiceHelper.Create<ICartService>().UpdateCart(skuId, count, memberId);
				return;
			}
			string cookie = WebHelper.GetCookie("HIMALL-CART");
			if (string.IsNullOrWhiteSpace(cookie))
			{
				WebHelper.SetCookie("HIMALL-CART", string.Format("{0}:{1}", skuId, count));
				return;
			}
			string[] strArrays = cookie.Split(new char[] { ',' });
			string empty = string.Empty;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				char[] chrArray = new char[] { ':' };
				if (str.Split(chrArray)[0] != skuId)
				{
					empty = string.Concat(empty, ",", str);
				}
				else
				{
					object obj = empty;
					object[] objArray = new object[] { obj, ",", skuId, ":", count };
					empty = string.Concat(objArray);
				}
			}
			if (!string.IsNullOrWhiteSpace(empty))
			{
				empty = empty.Substring(1);
			}
			WebHelper.SetCookie("HIMALL-CART", empty);
		}
	}
}