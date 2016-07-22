using Himall.IServices;
using Himall.Model;
using Himall.Web;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class CartController : BaseMobileMemberController
	{
		public CartController()
		{
		}

		[HttpPost]
		public JsonResult AddProductToCart(string skuId, int count)
		{
			CartHelper cartHelper = new CartHelper();
			cartHelper.AddToCart(skuId, count, (base.CurrentUser != null ? base.CurrentUser.Id : 0));
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult BatchRemoveFromCart(string skuIds)
		{
			long num = (base.CurrentUser != null ? base.CurrentUser.Id : 0);
			string[] strArrays = skuIds.Split(new char[] { ',' });
			(new CartHelper()).RemoveFromCart(strArrays, num);
			return Json(new { success = true });
		}

		public ActionResult Cart()
		{
			return View();
		}

		[HttpPost]
		public JsonResult GetCartProducts()
		{
			ShoppingCartInfo cart = (new CartHelper()).GetCart(base.CurrentUser.Id);
			IProductService productService = ServiceHelper.Create<IProductService>();
			IShopService shopService = ServiceHelper.Create<IShopService>();
			IVShopService vShopService = ServiceHelper.Create<IVShopService>();
			var collection = cart.Items.Select((ShoppingCartItem item) => {
				ProductInfo product = productService.GetProduct(item.ProductId);
				ShopInfo shop = shopService.GetShop(product.ShopId, false);
				if (shop == null)
				{
					return null;
				}
				VShopInfo vShopByShopId = vShopService.GetVShopByShopId(shop.Id);
				SKUInfo sku = productService.GetSku(item.SkuId);
				return new { cartItemId = item.Id, skuId = item.SkuId, id = product.Id, imgUrl = string.Concat(product.ImagePath, "/1_50.png"), name = product.ProductName, price = sku.SalePrice, count = item.Quantity, shopId = shop.Id, vshopId = (vShopByShopId == null ? 0 : vShopByShopId.Id), shopName = shop.ShopName, shopLogo = (vShopByShopId == null ? "" : vShopByShopId.Logo), status = (product.AuditStatus != ProductInfo.ProductAuditStatus.Audited || product.SaleStatus != ProductInfo.ProductSaleStatus.OnSale ? 0 : 1) };
			}).OrderBy((s) => s.vshopId);
			var variable = new { products = collection, amount = collection.Sum((item) => item.price * item.count), totalCount = collection.Sum((item) => item.count) };
			return Json(variable);
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult RemoveFromCart(string skuId)
		{
			long num;
			num = (base.CurrentUser != null ? base.CurrentUser.Id : 0);
			(new CartHelper()).RemoveFromCart(skuId, num);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult UpdateCartItem(string skuId, int count)
		{
			long num;
			num = (base.CurrentUser != null ? base.CurrentUser.Id : 0);
			(new CartHelper()).UpdateCartItem(skuId, count, num);
			return Json(new { success = true });
		}

		public JsonResult UpdateCartItem(Dictionary<string, int> skus, long userId)
		{
			CartHelper cartHelper = new CartHelper();
			foreach (KeyValuePair<string, int> sku in skus)
			{
				cartHelper.UpdateCartItem(sku.Key, sku.Value, userId);
			}
			return Json(new { success = true });
		}
	}
}