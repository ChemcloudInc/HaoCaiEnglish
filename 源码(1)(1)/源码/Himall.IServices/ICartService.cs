using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himall.IServices
{
	public interface ICartService : IService, IDisposable
	{
		void AddToCart(string skuId, int count, long memberId);

		void AddToCart(IEnumerable<ShoppingCartItem> cartItems, long memberId);

		void ClearCart(long memeberId);

		void DeleteCartItem(string skuId, long memberId);

		void DeleteCartItem(IEnumerable<string> skuIds, long memberId);

		ShoppingCartInfo GetCart(long memeberId);

		IQueryable<ShoppingCartItem> GetCartItems(IEnumerable<long> cartItemIds);

		IQueryable<ShoppingCartItem> GetCartItems(IEnumerable<string> skuIds, long memberId);

		void UpdateCart(string skuId, int count, long memberId);
	}
}