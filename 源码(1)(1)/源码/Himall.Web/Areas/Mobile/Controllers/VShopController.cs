using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Mobile;
using Himall.Web.Areas.Mobile.Models;
using Himall.Web.Areas.Web.Helper;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
    public class VShopController : BaseMobileTemplatesController
    {
        private SiteSettingsInfo _siteSetting;

        private string wxlogo = "/images/defaultwxlogo.png";

        private WXCardLogInfo.CouponTypeEnum ThisCouponType = WXCardLogInfo.CouponTypeEnum.Coupon;

        private IWXCardService ser_wxcard;

        public VShopController()
        {
            _siteSetting = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            ser_wxcard = ServiceHelper.Create<IWXCardService>();
        }

        [HttpPost]
        public JsonResult AcceptCoupon(long vshopid, long couponid)
        {
            if (base.CurrentUser == null)
            {
                return Json(new { status = 1, success = false, msg = "未登录." });
            }
            ICouponService service = ServiceHelper.Create<ICouponService>();
            CouponInfo couponInfo = service.GetCouponInfo(couponid);
            if (couponInfo.EndTime < DateTime.Now)
            {
                return Json(new { status = 2, success = false, msg = "优惠券已经过期." });
            }
            CouponRecordQuery query = new CouponRecordQuery
            {
                CouponId = new long?(couponid),
                UserId = new long?(base.CurrentUser.Id)
            };
            PageModel<CouponRecordInfo> couponRecordList = service.GetCouponRecordList(query);
            if ((couponInfo.PerMax != 0) && (couponRecordList.Total >= couponInfo.PerMax))
            {
                return Json(new { status = 3, success = false, msg = "达到个人领取最大张数，不能再领取." });
            }
            query = new CouponRecordQuery
            {
                CouponId = new long?(couponid)
            };
            if (service.GetCouponRecordList(query).Total >= couponInfo.Num)
            {
                return Json(new { status = 4, success = false, msg = "此优惠券已经领完了." });
            }
            if ((couponInfo.ReceiveType == (CouponInfo.CouponReceiveType)1) && (ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id).AvailableIntegrals < couponInfo.NeedIntegral))
            {
                return Json(new { status = 5, success = false, msg = "积分不足 " + couponInfo.NeedIntegral.ToString() });
            }
            CouponRecordInfo info = new CouponRecordInfo
            {
                CouponId = couponid,
                UserId = base.CurrentUser.Id,
                UserName = base.CurrentUser.UserName,
                ShopId = couponInfo.ShopId
            };
            service.AddCouponRecord(info);
            return Json(new { status = 0, success = true, msg = "领取成功", crid = info.Id });
        }

        public JsonResult AddFavorite(long shopId)
        {
            ServiceHelper.Create<IShopService>().AddFavoriteShop(base.CurrentUser.Id, shopId);
            Result result = new Result()
            {
                success = true,
                msg = "成功关注该微店."
            };
            return Json(result);
        }

        public ActionResult Category(long vShopId)
        {
            VShopInfo vShop = ServiceHelper.Create<IVShopService>().GetVShop(vShopId);
            List<ShopCategoryInfo> list = ServiceHelper.Create<IShopCategoryService>().GetShopCategory(vShop.ShopId).ToList();
            IEnumerable<CategoryModel> subCategories = GetSubCategories(list, 0, 0);
            ViewBag.VShopId = vShopId;
            return View(subCategories);
        }

        public ActionResult CouponInfo(long id, int? accept)
        {
            VshopCouponInfoModel model = new VshopCouponInfoModel();
            ICouponService service = ServiceHelper.Create<ICouponService>();
            CouponInfo info = service.GetCouponInfo(id) ?? new CouponInfo();
            if (info.EndTime < DateTime.Now)
            {
                model.CouponStatus = (CouponInfo.CouponReceiveStatus)2;
            }
            if (base.CurrentUser != null)
            {
                CouponRecordQuery query = new CouponRecordQuery
                {
                    CouponId = new long?(id),
                    UserId = new long?(base.CurrentUser.Id)
                };
                PageModel<CouponRecordInfo> couponRecordList = service.GetCouponRecordList(query);
                if ((info.PerMax != 0) && (couponRecordList.Total >= info.PerMax))
                {
                    model.CouponStatus = (CouponInfo.CouponReceiveStatus)3;
                }
                query = new CouponRecordQuery
                {
                    CouponId = new long?(id),
                    PageNo = 1,
                    PageSize = 0x270f
                };
                if (service.GetCouponRecordList(query).Total >= info.Num)
                {
                    model.CouponStatus = (CouponInfo.CouponReceiveStatus)4;
                }
                if (((int)info.ReceiveType == 1) && (ServiceHelper.Create<IMemberService>().GetMember(base.CurrentUser.Id).AvailableIntegrals < info.NeedIntegral))
                {
                    model.CouponStatus = (CouponInfo.CouponReceiveStatus)5;
                }
                if (ServiceHelper.Create<IShopService>().IsFavoriteShop(base.CurrentUser.Id, info.ShopId))
                {
                    model.IsFavoriteShop = true;
                }
            }
            model.CouponId = id;
            if (accept.HasValue)
            {
                model.AcceptId = new int?(accept.Value);
            }
            VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(info.ShopId);
            string wxlogo = this.wxlogo;
            if (vShopByShopId != null)
            {
                model.VShopid = vShopByShopId.Id;
                if (!string.IsNullOrWhiteSpace(vShopByShopId.WXLogo))
                {
                    wxlogo = vShopByShopId.WXLogo;
                }
                if (string.IsNullOrWhiteSpace(this.wxlogo) && !string.IsNullOrWhiteSpace(_siteSetting.WXLogo))
                {
                    wxlogo = _siteSetting.WXLogo;
                }
            }
        ((dynamic)base.ViewBag).ShopLogo = wxlogo;
            WXShopInfo vShopSetting = ServiceHelper.Create<IVShopService>().GetVShopSetting(info.ShopId);
            if (vShopSetting != null)
            {
                model.FollowUrl = vShopSetting.FollowUrl;
            }
            model.ShopId = info.ShopId;
            model.CouponData = info;
            ((dynamic)base.ViewBag).ShopId = model.ShopId;
            ((dynamic)base.ViewBag).FollowUrl = model.FollowUrl;
            ((dynamic)base.ViewBag).FavText = model.IsFavoriteShop ? "己收藏" : "收藏店铺";
            ((dynamic)base.ViewBag).VShopid = model.VShopid;
            return View(model);
        }

        public JsonResult DeleteFavorite(long shopId)
        {
            ServiceHelper.Create<IShopService>().CancelConcernShops(shopId, base.CurrentUser.Id);
            Result result = new Result()
            {
                success = true,
                msg = "成功取消关注该微店."
            };
            return Json(result);
        }

        public ActionResult Detail(long id, int? couponid, int? shop, bool sv = false)
        {
            IVShopService vShopService = ServiceHelper.Create<IVShopService>();
            VShopInfo vShop = vShopService.GetVShop(id);
            List<SlideAdInfo> list = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(vShop.ShopId, SlideAdInfo.SlideAdType.VShopHome).ToList();
            IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = (
                from item in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(vShop.ShopId, Himall.Core.PlatformType.WeiXin)
                orderby item.Sequence, item.Id descending
                select item).Take(8);
            IEnumerable<ProductItem> array =
                from item in mobileHomeProductsInfos.ToArray()
                select new ProductItem()
                {
                    Id = item.ProductId,
                    ImageUrl = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_350, 1),
                    Name = item.Himall_Products.ProductName,
                    MarketPrice = item.Himall_Products.MarketPrice,
                    SalePrice = item.Himall_Products.MinSalePrice
                };
            List<BannerInfo> bannerInfos = ServiceHelper.Create<INavigationService>().GetSellerNavigations(vShop.ShopId, Himall.Core.PlatformType.WeiXin).ToList();
            IEnumerable<CouponInfo> couponList = GetCouponList(vShop.ShopId);
            if (base.Request.UserAgent.Contains("AppWebview(iOS)"))
            {
                foreach (SlideAdInfo slideAdInfo in list)
                {
                    slideAdInfo.Url = slideAdInfo.Url.ToLower().Replace("/m-weixin", "/m-IOS").Replace("/m-wap", "/m-IOS");
                }
                foreach (BannerInfo bannerInfo in bannerInfos)
                {
                    bannerInfo.Url = bannerInfo.Url.ToLower().Replace("/m-weixin", "/m-IOS").Replace("/m-wap", "/m-IOS");
                }
            }
            dynamic viewBag = base.ViewBag;
            SlideAdInfo[] slideAdInfoArray = list.ToArray();
            viewBag.SlideAds =
                from item in slideAdInfoArray
                select new HomeSlideAdsModel()
                {
                    ImageUrl = item.ImageUrl,
                    Url = item.Url
                };
            ViewBag.Banner = bannerInfos;
            ViewBag.Products = array;
            if (base.CurrentUser != null)
            {
                ViewBag.IsFavorite = ServiceHelper.Create<IShopService>().IsFavoriteShop(base.CurrentUser.Id, vShop.ShopId);
            }
            else
            {
                ViewBag.IsFavorite = false;
            }
            WXShopInfo vShopSetting = ServiceHelper.Create<IVShopService>().GetVShopSetting(vShop.ShopId);
            if (vShopSetting != null)
            {
                ViewBag.FollowUrl = vShopSetting.FollowUrl;
            }
            ViewBag.Coupon = couponList;
            ViewBag.VshopId = vShop.Id;
            if (couponid.HasValue)
            {
                ViewBag.AcceptId = couponid.Value;
            }
            if (!sv)
            {
                vShopService.LogVisit(id);
            }
            if (shop.HasValue)
            {
                ViewBag.IsPlatform = 1;
            }
            return View(vShop);
        }

        private IEnumerable<CouponInfo> GetCouponList(long shopid)
        {
            IQueryable<CouponInfo> couponList = ServiceHelper.Create<ICouponService>().GetCouponList(shopid);
            IQueryable<long> vShopCouponSetting =
                from item in ServiceHelper.Create<IVShopService>().GetVShopCouponSetting(shopid)
                select item.CouponID;
            if (couponList.Count() <= 0 || vShopCouponSetting.Count() <= 0)
            {
                return null;
            }
            IEnumerable<CouponInfo> array =
                from item in couponList.ToArray()
                where vShopCouponSetting.Contains(item.Id)
                select item;
            return array;
        }

        public ActionResult GetCouponSuccess(long id)
        {
            VshopCouponInfoModel vshopCouponInfoModel = new VshopCouponInfoModel();
            ICouponService couponService = ServiceHelper.Create<ICouponService>();
            CouponRecordInfo couponRecordById = couponService.GetCouponRecordById(id);
            if (couponRecordById == null)
            {
                throw new HimallException("错误的优惠券编号");
            }
            CouponInfo couponInfo = couponService.GetCouponInfo(couponRecordById.ShopId, couponRecordById.CouponId);
            if (couponInfo == null)
            {
                throw new HimallException("错误的优惠券编号");
            }
            vshopCouponInfoModel.CouponData = couponInfo;
            vshopCouponInfoModel.CouponId = couponInfo.Id;
            vshopCouponInfoModel.CouponRecordId = couponRecordById.Id;
            vshopCouponInfoModel.ShopId = couponInfo.ShopId;
            vshopCouponInfoModel.IsShowSyncWeiXin = false;
            if (base.CurrentUser != null && ServiceHelper.Create<IShopService>().IsFavoriteShop(base.CurrentUser.Id, couponInfo.ShopId))
            {
                vshopCouponInfoModel.IsFavoriteShop = true;
            }
            vshopCouponInfoModel.CouponId = id;
            if (couponInfo.IsSyncWeiXin == 1 && base.PlatformType == Himall.Core.PlatformType.WeiXin)
            {
                vshopCouponInfoModel.WXJSInfo = ser_wxcard.GetSyncWeiXin(couponInfo.Id, couponRecordById.Id, ThisCouponType, base.Request.Url.AbsoluteUri);
                if (vshopCouponInfoModel.WXJSInfo != null)
                {
                    vshopCouponInfoModel.IsShowSyncWeiXin = true;
                }
            }
            string wXLogo = wxlogo;
            VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(couponInfo.ShopId);
            if (vShopByShopId != null)
            {
                vshopCouponInfoModel.VShopid = vShopByShopId.Id;
                if (!string.IsNullOrWhiteSpace(vShopByShopId.WXLogo))
                {
                    wXLogo = vShopByShopId.WXLogo;
                }
                if (string.IsNullOrWhiteSpace(wxlogo) && !string.IsNullOrWhiteSpace(_siteSetting.WXLogo))
                {
                    wXLogo = _siteSetting.WXLogo;
                }
            }
            ViewBag.ShopLogo = wXLogo;
            ViewBag.ShopId = vshopCouponInfoModel.ShopId;
            ViewBag.FollowUrl = vshopCouponInfoModel.FollowUrl;
            base.ViewBag.FavText = (vshopCouponInfoModel.IsFavoriteShop ? "己收藏" : "收藏店铺");
            ViewBag.VShopid = vshopCouponInfoModel.VShopid;
            return View(vshopCouponInfoModel);
        }

        [HttpPost]
        public JsonResult GetHotShops(int page, int pageSize)
        {
            int num;
            VShopInfo[] infoArray = ServiceHelper.Create<IVShopService>().GetHotShops(page, pageSize, out num).ToArray();
            IMobileHomeProductsService homeProductService = ServiceHelper.Create<IMobileHomeProductsService>();
            long[] favoriteShopIds = new long[0];
            if (base.CurrentUser != null)
            {
                favoriteShopIds = (from item in ServiceHelper.Create<IShopService>().GetFavoriteShopInfos(base.CurrentUser.Id) select item.ShopId).ToArray();
            }
            var data = from item in infoArray
                       select new
                       {
                           id = item.Id,
                           name = item.Name,
                           logo = item.Logo,
                           products = from t in (from t in (from t in homeProductService.GetMobileHomePageProducts(item.ShopId, PlatformType.WeiXin)
                                                            orderby t.Sequence, t.Id descending
                                                            select t).Take(2)
                                                 select t.Himall_Products).ToArray()
                                      select new { id = t.Id, name = t.ProductName, image = t.GetImage(ProductInfo.ImageSize.Size_220, 1), salePrice = t.MinSalePrice },
                           favorite = favoriteShopIds.Contains(item.ShopId),
                           shopId = item.ShopId
                       };
            return Json(data);
        }


        private IEnumerable<CategoryModel> GetSubCategories(IEnumerable<ShopCategoryInfo> allCategoies, long categoryId, int depth)
        {
            IEnumerable<CategoryModel> categoryModels = (
                from item in allCategoies
                where item.ParentCategoryId == categoryId
                select item).Select<ShopCategoryInfo, CategoryModel>((ShopCategoryInfo item) =>
                {
                    string empty = string.Empty;
                    return new CategoryModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        SubCategories = GetSubCategories(allCategoies, item.Id, depth + 1),
                        Depth = 1
                    };
                });
            return categoryModels;
        }

        [HttpPost]
        public JsonResult GetVShopIdByShopId(long shopId)
        {
            VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(shopId);
            Result result = new Result()
            {
                success = true,
                msg = vShopByShopId.Id.ToString()
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetWXCardData(long id)
        {
            WXJSCardModel wXJSCardModel = new WXJSCardModel();
            bool flag = true;
            ICouponService couponService = ServiceHelper.Create<ICouponService>();
            CouponRecordInfo couponRecordById = null;
            if (flag)
            {
                couponRecordById = couponService.GetCouponRecordById(id);
                if (couponRecordById == null)
                {
                    flag = false;
                }
            }
            CouponInfo couponInfo = null;
            if (flag)
            {
                couponInfo = couponService.GetCouponInfo(couponRecordById.ShopId, couponRecordById.CouponId);
                if (couponInfo == null)
                {
                    flag = false;
                }
            }
            if (flag && couponInfo.IsSyncWeiXin == 1 && base.PlatformType == Himall.Core.PlatformType.WeiXin)
            {
                wXJSCardModel = ser_wxcard.GetJSWeiXinCard(couponRecordById.CouponId, couponRecordById.Id, ThisCouponType);
            }
            return Json(wXJSCardModel);
        }

        public ActionResult Introduce(long id)
        {
            Image image;
            VShopInfo vShop = ServiceHelper.Create<IVShopService>().GetVShop(id);
            string empty = string.Empty;
            if (vShop != null)
            {
                object[] host = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", Himall.Core.PlatformType.WeiXin.ToString(), "/vshop/detail/", id };
                string str = string.Concat(host);
                string str1 = Server.MapPath(vShop.Logo);
                image = (string.IsNullOrWhiteSpace(vShop.Logo) || !System.IO.File.Exists(str1) ? QRCodeHelper.Create(str) : QRCodeHelper.Create(str, str1));
                DateTime now = DateTime.Now;
                string str2 = string.Concat(now.ToString("yyMMddHHmmssffffff"), ".jpg");
                empty = string.Concat("/temp/", str2);
                image.Save(string.Concat(Server.MapPath("~/temp/"), str2));
            }
            ViewBag.QRCode = empty;
            if (base.CurrentUser != null)
            {
                ViewBag.IsFavorite = ServiceHelper.Create<IShopService>().IsFavoriteShop(base.CurrentUser.Id, vShop.ShopId);
            }
            else
            {
                ViewBag.IsFavorite = false;
            }
            ShopServiceMarkModel shopComprehensiveMark = ShopServiceMark.GetShopComprehensiveMark(vShop.ShopId);
            dynamic viewBag = base.ViewBag;
            decimal comprehensiveMark = shopComprehensiveMark.ComprehensiveMark;
            viewBag.shopMark = comprehensiveMark.ToString();
            IQueryable<StatisticOrderCommentsInfo> shopStatisticOrderComments = ServiceHelper.Create<IShopService>().GetShopStatisticOrderComments(vShop.ShopId);
            StatisticOrderCommentsInfo statisticOrderCommentsInfo = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 1
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo1 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 9
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo2 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 5
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo3 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 2
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo4 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 10
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo5 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 6
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo6 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 3
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo7 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 4
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo8 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 11
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo9 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 12
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo10 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 7
                select c).FirstOrDefault();
            StatisticOrderCommentsInfo statisticOrderCommentsInfo11 = (
                from c in shopStatisticOrderComments
                where (int)c.CommentKey == 8
                select c).FirstOrDefault();
            int num = 5;
            if (statisticOrderCommentsInfo == null || statisticOrderCommentsInfo3 == null)
            {
                ViewBag.ProductAndDescription = num;
                ViewBag.ProductAndDescriptionPeer = num;
                ViewBag.ProductAndDescriptionMin = num;
                ViewBag.ProductAndDescriptionMax = num;
            }
            else
            {
                ViewBag.ProductAndDescription = statisticOrderCommentsInfo.CommentValue;
                ViewBag.ProductAndDescriptionPeer = statisticOrderCommentsInfo3.CommentValue;
                ViewBag.ProductAndDescriptionMin = statisticOrderCommentsInfo7.CommentValue;
                ViewBag.ProductAndDescriptionMax = statisticOrderCommentsInfo6.CommentValue;
            }
            if (statisticOrderCommentsInfo1 == null || statisticOrderCommentsInfo4 == null)
            {
                ViewBag.SellerServiceAttitude = num;
                ViewBag.SellerServiceAttitudePeer = num;
                ViewBag.SellerServiceAttitudeMax = num;
                ViewBag.SellerServiceAttitudeMin = num;
            }
            else
            {
                ViewBag.SellerServiceAttitude = statisticOrderCommentsInfo1.CommentValue;
                ViewBag.SellerServiceAttitudePeer = statisticOrderCommentsInfo4.CommentValue;
                ViewBag.SellerServiceAttitudeMax = statisticOrderCommentsInfo8.CommentValue;
                ViewBag.SellerServiceAttitudeMin = statisticOrderCommentsInfo9.CommentValue;
            }
            if (statisticOrderCommentsInfo5 == null || statisticOrderCommentsInfo2 == null)
            {
                ViewBag.SellerDeliverySpeed = num;
                ViewBag.SellerDeliverySpeedPeer = num;
                ViewBag.SellerDeliverySpeedMax = num;
                ViewBag.sellerDeliverySpeedMin = num;
            }
            else
            {
                ViewBag.SellerDeliverySpeed = statisticOrderCommentsInfo2.CommentValue;
                ViewBag.SellerDeliverySpeedPeer = statisticOrderCommentsInfo5.CommentValue;
                ViewBag.SellerDeliverySpeedMax = statisticOrderCommentsInfo10.CommentValue;
                ViewBag.sellerDeliverySpeedMin = statisticOrderCommentsInfo11.CommentValue;
            }
            return View(vShop);
        }

        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(int page, int pageSize)
        {
            int num;
            VShopInfo[] array = ServiceHelper.Create<IVShopService>().GetVShops(page, pageSize, out num, VShopInfo.VshopStates.Normal).ToArray();
            long[] numArray = new long[0];
            if (base.CurrentUser != null)
            {
                numArray = (
                    from item in ServiceHelper.Create<IShopService>().GetFavoriteShopInfos(base.CurrentUser.Id)
                    select item.ShopId).ToArray();
            }
            var variable =
                from item in array
                select new { id = item.Id, image = item.BackgroundImage, tags = item.Tags, name = item.Name, shopId = item.ShopId, favorite = numArray.Contains(item.ShopId) };
            return Json(variable);
        }

        [ActionName("Index")]
        public ActionResult Main()
        {
            VShopInfo topShop = ServiceHelper.Create<IVShopService>().GetTopShop();
            ViewBag.IsFavorite = false;
            if (topShop != null)
            {
                IQueryable<ProductInfo> productInfos =
                    from item in (
                        from t in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(topShop.ShopId, Himall.Core.PlatformType.WeiXin)
                        orderby t.Sequence, t.Id descending
                        select t).Take(2)
                    select item.Himall_Products;
                IEnumerable<ProductItem> array =
                    from item in productInfos.ToArray()
                    select new ProductItem()
                    {
                        Id = item.Id,
                        ImageUrl = item.GetImage(ProductInfo.ImageSize.Size_350, 1),
                        MarketPrice = item.MarketPrice,
                        Name = item.ProductName,
                        SalePrice = item.MinSalePrice
                    };
                ViewBag.TopShopProducts = array;
                if (base.CurrentUser != null)
                {
                    long[] numArray = (
                        from item in ServiceHelper.Create<IShopService>().GetFavoriteShopInfos(base.CurrentUser.Id)
                        select item.ShopId).ToArray();
                    ViewBag.IsFavorite = numArray.Contains(topShop.ShopId);
                }
            }
            return View(topShop);
        }

        [HttpPost]
        public JsonResult ProductList(long shopId, int pageNo, int pageSize)
        {
            IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = (
                from item in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(shopId, Himall.Core.PlatformType.WeiXin)
                orderby item.Sequence, item.Id descending
                select item).Skip((pageNo - 1) * pageSize).Take(pageSize);
            var array =
                from item in mobileHomeProductsInfos.ToArray()
                select new { Id = item.ProductId, ImageUrl = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_350, 1), Name = item.Himall_Products.ProductName, MarketPrice = item.Himall_Products.MarketPrice, SalePrice = item.Himall_Products.MinSalePrice.ToString("F2") };
            return Json(array);
        }

        public ActionResult Search(string keywords = "", string exp_keywords = "", long cid = 0L, long b_id = 0L, string a_id = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 6, long vshopId = 0L)
        {
            long shopId = -1;
            if (vshopId > 0)
            {
                VShopInfo vShop = ServiceHelper.Create<IVShopService>().GetVShop(vshopId);
                if (vShop != null)
                {
                    shopId = vShop.ShopId;
                }
            }
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                keywords = keywords.Trim();
            }
            ProductSearch productSearch = new ProductSearch()
            {
                shopId = shopId,
                BrandId = b_id,
                CategoryId = cid,
                Ex_Keyword = exp_keywords,
                Keyword = keywords,
                OrderKey = orderKey,
                OrderType = orderType == 1,
                AttrIds = new List<string>(),
                PageNumber = pageNo,
                PageSize = pageSize
            };
            PageModel<ProductInfo> pageModel = ServiceHelper.Create<IProductService>().SearchProduct(productSearch);
            int total = pageModel.Total;
            ProductInfo[] array = pageModel.Models.ToArray();
            ICommentService commentService = ServiceHelper.Create<ICommentService>();
            IEnumerable<ProductItem> productItem =
                from item in array
                select new ProductItem()
                {
                    Id = item.Id,
                    ImageUrl = item.GetImage(ProductInfo.ImageSize.Size_350, 1),
                    SalePrice = item.MinSalePrice,
                    Name = item.ProductName,
                    CommentsCount = commentService.GetCommentsByProductId(item.Id).Count()
                };
            IQueryable<ShopCategoryInfo> shopCategory = ServiceHelper.Create<IShopCategoryService>().GetShopCategory(shopId);
            IEnumerable<CategoryModel> subCategories = GetSubCategories(shopCategory, 0, 0);
            ViewBag.ShopCategories = subCategories;
            ViewBag.Total = total;
            ViewBag.Keywords = keywords;
            ViewBag.VShopId = vshopId;
            return View(productItem);
        }

        [HttpPost]
        public JsonResult Search(string keywords = "", string exp_keywords = "", long cid = 0L, long b_id = 0L, string a_id = "", int orderKey = 1, int orderType = 1, int pageNo = 1, int pageSize = 6, long vshopId = 0L, string t = "")
        {
            long shopId = -1;
            if (vshopId > 0)
            {
                VShopInfo vShop = ServiceHelper.Create<IVShopService>().GetVShop(vshopId);
                if (vShop != null)
                {
                    shopId = vShop.ShopId;
                }
            }
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                keywords = keywords.Trim();
            }
            ProductSearch productSearch = new ProductSearch()
            {
                shopId = shopId,
                BrandId = b_id,
                CategoryId = cid,
                Ex_Keyword = exp_keywords,
                Keyword = keywords,
                OrderKey = orderKey,
                OrderType = orderType == 1,
                AttrIds = new List<string>(),
                PageNumber = pageNo,
                PageSize = pageSize
            };
            PageModel<ProductInfo> pageModel = ServiceHelper.Create<IProductService>().SearchProduct(productSearch);
            int total = pageModel.Total;
            ProductInfo[] array = pageModel.Models.ToArray();
            ICommentService commentService = ServiceHelper.Create<ICommentService>();
            var variable =
                from item in array
                select new { id = item.Id, name = item.ProductName, price = item.MinSalePrice.ToString("F2"), commentsCount = commentService.GetCommentsByProductId(item.Id).Count(), img = item.GetImage(ProductInfo.ImageSize.Size_350, 1) };
            return Json(variable);
        }
    }
}