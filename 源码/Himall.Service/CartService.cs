using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class CartService : ServiceBase, ICartService, IService, IDisposable
	{
		public CartService()
		{
		}

		public void AddToCart(string skuId, int count, long memberId)
		{
			if (count != 0)
			{
                CheckCartItem(skuId, count, memberId);
				ShoppingCartItemInfo shoppingCartItemInfo = context.ShoppingCartItemInfo.FirstOrDefault((ShoppingCartItemInfo item) => item.UserId == memberId && (item.SkuId == skuId));
				if (shoppingCartItemInfo != null)
				{
					ShoppingCartItemInfo quantity = shoppingCartItemInfo;
					quantity.Quantity = quantity.Quantity + count;
				}
				else if (count > 0)
				{
					string str = skuId;
					char[] chrArray = new char[] { '\u005F' };
					long num = long.Parse(str.Split(chrArray)[0]);
					DbSet<ShoppingCartItemInfo> shoppingCartItemInfos = context.ShoppingCartItemInfo;
					ShoppingCartItemInfo shoppingCartItemInfo1 = new ShoppingCartItemInfo()
					{
						UserId = memberId,
						Quantity = count,
						SkuId = skuId,
						ProductId = num,
						AddTime = DateTime.Now
					};
					shoppingCartItemInfos.Add(shoppingCartItemInfo1);
				}
                context.SaveChanges();
			}
		}

		public void AddToCart(IEnumerable<ShoppingCartItem> cartItems, long memberId)
		{
			foreach (ShoppingCartItem list in cartItems.ToList())
			{
                CheckCartItem(list.SkuId, list.Quantity, memberId);
				ShoppingCartItemInfo shoppingCartItemInfo = context.ShoppingCartItemInfo.FirstOrDefault((ShoppingCartItemInfo item) => item.UserId == memberId && (item.SkuId == list.SkuId));
				if (shoppingCartItemInfo == null)
				{
					string skuId = list.SkuId;
					char[] chrArray = new char[] { '\u005F' };
					long num = long.Parse(skuId.Split(chrArray)[0]);
					DbSet<ShoppingCartItemInfo> shoppingCartItemInfos = context.ShoppingCartItemInfo;
					ShoppingCartItemInfo shoppingCartItemInfo1 = new ShoppingCartItemInfo()
					{
						UserId = memberId,
						Quantity = list.Quantity,
						SkuId = list.SkuId,
						ProductId = num,
						AddTime = DateTime.Now
					};
					shoppingCartItemInfos.Add(shoppingCartItemInfo1);
				}
				else
				{
					ShoppingCartItemInfo quantity = shoppingCartItemInfo;
					quantity.Quantity = quantity.Quantity + list.Quantity;
				}
			}
            context.SaveChanges();
		}

		private void CheckCartItem(string skuId, int count, long memberId)
		{
			if (string.IsNullOrWhiteSpace(skuId))
			{
				throw new InvalidPropertyException("SKUId不能为空");
			}
			if (count < 0)
			{
				throw new InvalidPropertyException("商品数量不能小于0");
			}
			if (context.UserMemberInfo.FirstOrDefault((UserMemberInfo item) => item.Id == memberId) == null)
			{
				throw new InvalidPropertyException(string.Concat("会员Id", memberId, "不存在"));
			}
		}

		public void ClearCart(long memeberId)
		{
            context.ShoppingCartItemInfo.OrderBy((ShoppingCartItemInfo item) => item.UserId == memeberId);
            context.SaveChanges();
		}

		public void DeleteCartItem(string skuId, long memberId)
		{
            context.ShoppingCartItemInfo.OrderBy((ShoppingCartItemInfo item) => (item.SkuId == skuId) && item.UserId == memberId);
            context.SaveChanges();
		}

		public void DeleteCartItem(IEnumerable<string> skuIds, long memberId)
		{
            context.ShoppingCartItemInfo.OrderBy((ShoppingCartItemInfo item) => skuIds.Contains<string>(item.SkuId) && item.UserId == memberId);
            context.SaveChanges();
		}

		public ShoppingCartInfo GetCart(long memeberId)
		{
			ShoppingCartInfo shoppingCartInfo = new ShoppingCartInfo()
			{
				MemberId = memeberId
			};
			IQueryable<ShoppingCartItemInfo> shoppingCartItemInfo = 
				from item in context.ShoppingCartItemInfo
				where item.UserId == memeberId
				select item;
			shoppingCartInfo.Items = 
				from item in shoppingCartItemInfo
				select new ShoppingCartItem()
				{
					Id = item.Id,
					SkuId = item.SkuId,
					Quantity = item.Quantity,
					AddTime = item.AddTime,
					ProductId = item.ProductId
				};
			return shoppingCartInfo;
		}

		public IQueryable<ShoppingCartItem> GetCartItems(IEnumerable<long> cartItemIds)
		{
			return 
				from item in context.ShoppingCartItemInfo.FindBy((ShoppingCartItemInfo item) => cartItemIds.Contains(item.Id))
				select new ShoppingCartItem()
				{
					Id = item.Id,
					SkuId = item.SkuId,
					Quantity = item.Quantity,
					ProductId = item.ProductId,
					AddTime = item.AddTime
				};
		}

		public IQueryable<ShoppingCartItem> GetCartItems(IEnumerable<string> skuIds, long memberId)
		{
			return 
				from item in context.ShoppingCartItemInfo
				where item.UserId == memberId && skuIds.Contains<string>(item.SkuId)
				select new ShoppingCartItem()
				{
					Id = item.Id,
					SkuId = item.SkuId,
					Quantity = item.Quantity,
					ProductId = item.ProductId,
					AddTime = item.AddTime
				};
		}

		public void UpdateCart(string skuId, int count, long memberId)
		{
            CheckCartItem(skuId, count, memberId);
			ShoppingCartItemInfo shoppingCartItemInfo = context.ShoppingCartItemInfo.FirstOrDefault((ShoppingCartItemInfo item) => item.UserId == memberId && (item.SkuId == skuId));
			if (shoppingCartItemInfo != null)
			{
				if (count != 0)
				{
					shoppingCartItemInfo.Quantity = count;
				}
				else
				{
					DbSet<ShoppingCartItemInfo> shoppingCartItemInfos = context.ShoppingCartItemInfo;
					object[] id = new object[] { shoppingCartItemInfo.Id };
					shoppingCartItemInfos.Remove(id);
				}
			}
			else if (count > 0)
			{
				string str = skuId;
				char[] chrArray = new char[] { '\u005F' };
				long num = long.Parse(str.Split(chrArray)[0]);
				DbSet<ShoppingCartItemInfo> shoppingCartItemInfo1 = context.ShoppingCartItemInfo;
				ShoppingCartItemInfo shoppingCartItemInfo2 = new ShoppingCartItemInfo()
				{
					UserId = memberId,
					Quantity = count,
					SkuId = skuId,
					ProductId = num,
					AddTime = DateTime.Now
				};
				shoppingCartItemInfo1.Add(shoppingCartItemInfo2);
			}
            context.SaveChanges();
		}
	}
}