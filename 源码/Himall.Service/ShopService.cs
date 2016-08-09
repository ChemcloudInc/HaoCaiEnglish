using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;

namespace Himall.Service
{
	public class ShopService : ServiceBase, IShopService, IService, IDisposable
	{
		public ShopService()
		{
		}

		public void AddFavoriteShop(long memberId, long shopId)
		{
			if (context.FavoriteShopInfo.FirstOrDefault((FavoriteShopInfo item) => item.UserId == memberId && item.ShopId == shopId) != null)
			{
				throw new HimallException("您已经关注过该店铺");
			}
			FavoriteShopInfo favoriteShopInfo = new FavoriteShopInfo()
			{
				ShopId = shopId,
				UserId = memberId,
				Date = DateTime.Now,
				Tags = ""
			};
            context.FavoriteShopInfo.Add(favoriteShopInfo);
            context.SaveChanges();
		}

		public long AddShop(ShopInfo shop)
		{
            context.ShopInfo.Add(shop);
            context.SaveChanges();
			return shop.Id;
		}

		public void AddShopGrade(ShopGradeInfo shopGrade)
		{
            context.ShopGradeInfo.Add(shopGrade);
            context.SaveChanges();
		}

		public void CancelConcernShops(IEnumerable<long> ids, long userId)
		{
            context.FavoriteShopInfo.OrderBy((FavoriteShopInfo a) => a.UserId == userId && ids.Contains(a.Id));
            context.SaveChanges();
		}

		public void CancelConcernShops(long shopId, long userId)
		{
            context.FavoriteShopInfo.OrderBy((FavoriteShopInfo a) => a.UserId == userId && a.ShopId == shopId);
            context.SaveChanges();
		}

		public ShopInfo CreateEmptyShop()
		{
			ShopInfo shopInfo = new ShopInfo()
			{
				ShopName = "",
				GradeId = 1,
				IsSelf = false,
				ShopStatus = ShopInfo.ShopAuditStatus.Unusable,
				CreateDate = DateTime.Now,
				CompanyRegionId = 0,
				CompanyEmployeeCount = CompanyEmployeeCount.LessThanFive,
				CompanyRegisteredCapital = new decimal(0),
				BusinessLicenceNumberPhoto = "",
				BusinessLicenceRegionId = 0,
				BankRegionId = 0,
				FreeFreight = new decimal(0),
				Freight = new decimal(0),
                Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.CompanyInfo),
			};
			ShopInfo shopInfo1 = shopInfo;
            context.ShopInfo.Add(shopInfo1);
          
            context.SaveChanges();
            
      
			return shopInfo1;
		}

		public void DeleteShop(long id)
		{
            context.ShopInfo.Remove(context.ShopInfo.FindById<ShopInfo>(id));
            context.SaveChanges();
		}

		public void DeleteShopGrade(long id, out string msg)
		{
			msg = "";
			if (context.ShopInfo.Any((ShopInfo s) => s.GradeId == id))
			{
				msg = "删除失败，因为该套餐和店铺有关联，所以不能删除.";
				return;
			}
            context.ShopGradeInfo.Remove(context.ShopGradeInfo.FindById<ShopGradeInfo>(id));
            context.SaveChanges();
		}

		public bool ExistCompanyName(string companyName, long shopId = 0L)
		{
			return context.ShopInfo.Any((ShopInfo s) => s.CompanyName.Equals(companyName) && s.Id != shopId);
		}

		public bool ExistShop(string shopName, long shopId = 0L)
		{
			ShopInfo shopInfo = (
				from p in context.ShopInfo
				where p.ShopName.Equals(shopName) && p.Id != shopId
				select p).FirstOrDefault();
			if (shopInfo != null && shopInfo.ShopStatus != ShopInfo.ShopAuditStatus.Refuse)
			{
				return true;
			}
			return false;
		}

		public PageModel<ShopInfo> GetAuditingShops(ShopQuery shopQueryModel)
		{
			return null;
		}

		public IQueryable<BusinessCategoryInfo> GetBusinessCategory(long id)
		{
			IQueryable<BusinessCategoryInfo> businessCategoryInfos = context.BusinessCategoryInfo.FindBy((BusinessCategoryInfo b) => b.ShopId.Equals(id));
			foreach (BusinessCategoryInfo list in businessCategoryInfos.ToList())
			{
				list.CategoryName = GetCategoryNameByPath(list.CategoryId);
			}
			return businessCategoryInfos;
		}

		private string GetCategoryNameByPath(long id)
		{
			CategoryInfo categoryInfo = context.CategoryInfo.FindById<CategoryInfo>(id);
			if (categoryInfo.Depth == 1 && categoryInfo.ParentCategoryId == 0)
			{
				return categoryInfo.Name;
			}
			return string.Concat(GetCategoryNameByPath(categoryInfo.ParentCategoryId), " > ", categoryInfo.Name);
		}

		public IQueryable<FavoriteShopInfo> GetFavoriteShopInfos(long memberId)
		{
			return context.FavoriteShopInfo.FindBy((FavoriteShopInfo item) => item.UserId == memberId);
		}

		public PlatConsoleModel GetPlatConsoleMode()
		{
			DbSet<ShopInfo> shopInfo = context.ShopInfo;
			DbSet<OrderInfo> orderInfo = context.OrderInfo;
			IQueryable<ProductInfo> productInfo = 
				from item in context.ProductInfo
				where (int)item.SaleStatus != 4
				select item;
			DbSet<OrderRefundInfo> orderRefundInfo = context.OrderRefundInfo;
			DbSet<ProductCommentInfo> productCommentInfo = context.ProductCommentInfo;
			DbSet<ProductConsultationInfo> productConsultationInfo = context.ProductConsultationInfo;
			DbSet<OrderComplaintInfo> orderComplaintInfo = context.OrderComplaintInfo;
			DbSet<UserMemberInfo> userMemberInfo = context.UserMemberInfo;
			PlatConsoleModel platConsoleModel = new PlatConsoleModel();
			DateTime date = DateTime.Now.Date;
			DateTime dateTime = DateTime.Now.Date.AddDays(-1);
			int num = 0;
			decimal? nullable = (
				from a in orderInfo
				where (int)a.OrderStatus != 4 && (int)a.OrderStatus != 1 && (a.PayDate >= date)
				select a).Sum<OrderInfo>((OrderInfo a) => (decimal?)(a.ProductTotalAmount + a.Freight + a.Tax - a.DiscountAmount));
			platConsoleModel.TodaySaleAmount = new decimal?(nullable.GetValueOrDefault());
			platConsoleModel.TodayMemberIncrease = (
                from a in userMemberInfo
                where a.CreateDate >= date
                select a).Count();
			platConsoleModel.TodayShopIncrease = (
                from a in shopInfo
                where (a.CreateDate >= date) && (int)a.ShopStatus == 7 && (int?)a.Stage == (int?)ShopInfo.ShopStage.Finish
                select a).Count();
			platConsoleModel.YesterdayShopIncrease = (
                from a in shopInfo
                where (a.CreateDate >= dateTime) && (a.CreateDate < date) && (int)a.ShopStatus == 7 && (int?)a.Stage == (int?)ShopInfo.ShopStage.Finish
                select a).Count();
			platConsoleModel.WaitAuditShops = (
                from a in shopInfo
                where (int)a.ShopStatus == 2 
                select a).Count();
            platConsoleModel.WaitPayShops = (
               from a in shopInfo
               where  (int)a.ShopStatus == 5
               select a).Count();
			platConsoleModel.ExpiredShops = (
                from a in shopInfo
                where a.EndDate < (DateTime?)date
                select a).Count();
			platConsoleModel.ShopNum = shopInfo.Count((ShopInfo a) => (int)a.ShopStatus == 7 && (int?)a.Stage == (int?)ShopInfo.ShopStage.Finish);
			platConsoleModel.WaitForAuditingBrands = context.ShopBrandApplysInfo.Count((ShopBrandApplysInfo a) => a.AuditStatus == num);
			platConsoleModel.ProductNum = productInfo.Count();
			platConsoleModel.OnSaleProducts = (
				from a in productInfo
				where (int)a.SaleStatus == 1 && (int)a.AuditStatus == 2
				select a).Count();
			platConsoleModel.WaitForAuditingProducts = (
				from a in productInfo
				where (int)a.AuditStatus == 1 && (int)a.SaleStatus == 1
				select a).Count();
			platConsoleModel.ProductComments = productCommentInfo.Count();
			platConsoleModel.ProductConsultations = productConsultationInfo.Count();
			platConsoleModel.WaitPayTrades = (
                from a in orderInfo
                where (int)a.OrderStatus == 1
                select a).Count();
			platConsoleModel.RefundTrades = (
                from a in orderRefundInfo
                where (int)a.RefundMode != 3 && (int)a.SellerAuditStatus == 5 && (int)a.ManagerConfirmStatus == 6
                select a).Count();
			platConsoleModel.OrderWithRefundAndRGoods = (
                from a in orderRefundInfo
                where (int)a.RefundMode == 3 && (int)a.SellerAuditStatus == 5 && (int)a.ManagerConfirmStatus == 6
                select a).Count();
			platConsoleModel.WaitDeliveryTrades = (
                from a in orderInfo
                where (int)a.OrderStatus == 2
                select a).Count();

			platConsoleModel.Complaints = orderComplaintInfo.Count((OrderComplaintInfo a) => (int)a.Status == 3);
			platConsoleModel.OrderCounts = (
				from a in orderInfo
				where (int)a.OrderStatus == 5
				select a).Count();
			platConsoleModel.Cash = (
                from a in context.ApplyWithDrawInfo
                where (int)a.ApplyStatus == 1
                select a).Count();
			platConsoleModel.GiftSend = (
                from a in context.GiftOrderInfo
                where (int)a.OrderStatus == 2
                select a).Count();
			return platConsoleModel;
		}

		public int GetSales(long id)
		{
			return (
				from p in context.OrderInfo
				where p.ShopId == id && (int)p.OrderStatus == 5
				select p).Count();
		}

		public SellerConsoleModel GetSellerConsoleModel(long shopId)
		{
			ShopInfo shopInfo = (
				from a in context.ShopInfo
				where a.Id == shopId
				select a).FirstOrDefault();
			ShopGradeInfo shopGradeInfo = context.ShopGradeInfo.FindById<ShopGradeInfo>(shopInfo.GradeId);
			IQueryable<OrderInfo> orderInfo = 
				from a in context.OrderInfo
				where a.ShopId == shopId
				select a;
			IQueryable<ProductInfo> productInfo = 
				from a in context.ProductInfo
				where a.ShopId == shopId && (int)a.SaleStatus != 4
				select a;
			IQueryable<ShopBrandApplysInfo> shopBrandApplysInfo = 
				from a in context.ShopBrandApplysInfo
				where a.ShopId == shopId
				select a;
			IQueryable<OrderRefundInfo> orderRefundInfo = 
				from a in context.OrderRefundInfo
				where a.ShopId == shopId && (int)a.RefundMode != 3 && ((int)a.SellerAuditStatus == 1 || (int)a.SellerAuditStatus == 3)
				select a;
			IQueryable<OrderRefundInfo> orderRefundInfos = 
				from a in context.OrderRefundInfo
				where a.ShopId == shopId && (int)a.RefundMode == 3 && ((int)a.SellerAuditStatus == 1 || (int)a.SellerAuditStatus == 3)
				select a;
			IQueryable<ProductCommentInfo> productCommentInfo = 
				from a in context.ProductCommentInfo
				where a.ShopId == shopId
				select a;
			IQueryable<ProductConsultationInfo> productConsultationInfo = 
				from a in context.ProductConsultationInfo
				where a.ShopId == shopId
				select a;
			IQueryable<OrderComplaintInfo> orderComplaintInfo = 
				from a in context.OrderComplaintInfo
				where a.ShopId == shopId && (int)a.Status == 1
				select a;
			SellerConsoleModel sellerConsoleModel = new SellerConsoleModel()
			{
				ShopName = shopInfo.ShopName,
				ShopGrade = shopGradeInfo.Name,
				ShopEndDate = shopInfo.EndDate.Value,
				ShopFreight = shopInfo.Freight,
				ProductsCount = productInfo.Count(),
				OnSaleProducts = (
					from a in productInfo
					where (int)a.SaleStatus == 1 && (int)a.AuditStatus == 2
					select a).Count(),
				AuditFailureProducts = (
					from a in productInfo
					where (int)a.AuditStatus == 3
					select a).Count(),
				InfractionSaleOffProducts = (
					from a in productInfo
					where (int)a.AuditStatus == 4
					select a).Count(),
				InStockProducts = (
					from a in productInfo
					where (int)a.SaleStatus == 2
					select a).Count(),
				WaitForAuditingProducts = (
					from a in productInfo
					where (int)a.AuditStatus == 1 && (int)a.SaleStatus == 1
					select a).Count(),
				BrandApply = (
					from a in shopBrandApplysInfo
					where a.AuditStatus == 0
					select a).Count(),
				WaitPayTrades = (
					from a in orderInfo
					where (int)a.OrderStatus == 1
					select a).Count(),
				ProductComments = productCommentInfo.Count(),
				ProductConsultations = productConsultationInfo.Count(),
				ProductLimit = shopGradeInfo.ProductLimit
			};
			long shopSpaceUsage2 = GetShopSpaceUsage2(shopId);
			sellerConsoleModel.ProductsCount = productInfo.Count();
			sellerConsoleModel.ImageLimit = shopGradeInfo.ImageLimit;
			sellerConsoleModel.ProductImages = shopSpaceUsage2;
			sellerConsoleModel.WaitDeliveryTrades = (
				from a in orderInfo
				where (int)a.OrderStatus == 2
				select a).Count();
            #region 账户动态
            if ((from a in orderInfo
                 where (int)a.AccountType == 1
                 select a).Count() > 0)
            {
               
                sellerConsoleModel.NoAccount = (
                   from a in orderInfo
                   where (int)a.AccountType == 1
                   select a).Sum(a => a.ProductTotalAmount + a.Freight + a.RefundCommisAmount - a.RefundTotalAmount - a.CommisTotalAmount - a.DiscountAmount);
            }
            else
            {
                sellerConsoleModel.NoAccount = 0;
            }
                
            #endregion
            sellerConsoleModel.RefundTrades = orderRefundInfo.Count();
			sellerConsoleModel.RefundAndRGoodsTrades = orderRefundInfos.Count();
			sellerConsoleModel.Complaints = orderComplaintInfo.Count();
			sellerConsoleModel.OrderCounts = orderInfo.Count();
			return sellerConsoleModel;
		}

		public PageModel<ShopInfo> GetSellers(SellerQuery sellerQueryModel)
		{
			IQueryable<ShopInfo> shopInfo = context.ShopInfo;
			if (!string.IsNullOrEmpty(sellerQueryModel.ShopName))
			{
				shopInfo = 
					from item in shopInfo
					where item.ShopName.Contains(sellerQueryModel.ShopName)
					select item;
			}
			if (sellerQueryModel.RegionId.HasValue)
			{
				shopInfo = 
					from item in shopInfo
					where item.CompanyRegionId >= sellerQueryModel.RegionId.Value
					select item;
			}
			if (sellerQueryModel.NextRegionId.HasValue)
			{
				shopInfo = 
					from item in shopInfo
					where item.CompanyRegionId < sellerQueryModel.NextRegionId.Value
					select item;
			}
			if (sellerQueryModel.Ids != null && sellerQueryModel.Ids.Count() > 0)
			{
				shopInfo = 
					from item in shopInfo
					where sellerQueryModel.Ids.Contains(item.Id)
					select item;
			}
			if (sellerQueryModel.ShopId.HasValue)
			{
				shopInfo = 
					from item in shopInfo
					where item.Id != sellerQueryModel.ShopId.Value
					select item;
			}
			int num = 0;
			num = shopInfo.Count();
			shopInfo = shopInfo.FindBy<ShopInfo, long>((ShopInfo item) => (int)item.ShopStatus == 7 && (int?)item.Stage == (int?)ShopInfo.ShopStage.Finish, sellerQueryModel.PageNo, sellerQueryModel.PageSize, out num, (ShopInfo item) => item.Id, false);
			return new PageModel<ShopInfo>()
			{
				Models = shopInfo,
				Total = num
			};
		}

		public ShopInfo GetShop(long id, bool businessCategoryOn = false)
		{
			ShopInfo nums = context.ShopInfo.FindById<ShopInfo>(id);
			if (nums == null)
			{
				return null;
			}
			ManagerInfo managerInfo = context.ManagerInfo.FirstOrDefault((ManagerInfo m) => m.ShopId.Equals(nums.Id));
			nums.ShopAccount = (managerInfo == null ? "" : managerInfo.UserName);
			if (businessCategoryOn)
			{
				nums.BusinessCategory = new Dictionary<long, decimal>();
				foreach (BusinessCategoryInfo list in GetBusinessCategory(id).ToList())
				{
					if (nums.BusinessCategory.ContainsKey(list.CategoryId))
					{
						continue;
					}
					nums.BusinessCategory.Add(list.CategoryId, list.CommisRate);
				}
			}
			return nums;
		}

		public ShopInfo GetShopBasicInfo(long id)
		{
			return context.ShopInfo.FindById<ShopInfo>(id);
		}

		public long GetShopConcernedCount(long shopId)
		{
			long num = 0;
			string str = CacheKeyCollection.ShopConcerned(shopId);
			if (Cache.Get(str) == null)
			{
				num = (
                    from a in context.FavoriteShopInfo
                    where a.ShopId == shopId
                    select a).Count();
				object obj = num;
				DateTime now = DateTime.Now;
				Cache.Insert(str, obj, now.AddMinutes(5));
			}
			else
			{
				num = (long)Cache.Get(str);
			}
			return num;
		}

		public ShopGradeInfo GetShopGrade(long id)
		{
			return context.ShopGradeInfo.FindById<ShopGradeInfo>(id);
		}

		public IQueryable<ShopGradeInfo> GetShopGrades()
		{
			return context.ShopGradeInfo.FindAll<ShopGradeInfo>();
		}

		public string GetShopName(long id)
		{
			return (
				from p in context.ShopInfo
				where p.Id == id
				select p.ShopName).FirstOrDefault();
		}

		public PageModel<ShopInfo> GetShops(ShopQuery shopQueryModel)
		{
			IQueryable<ShopInfo> gradeId = context.ShopInfo.AsQueryable<ShopInfo>();
			long? shopGradeId = shopQueryModel.ShopGradeId;
			if ((shopGradeId.GetValueOrDefault() <= 0 ? false : shopGradeId.HasValue))
			{
				gradeId = 
					from item in gradeId
					where item.GradeId == shopQueryModel.ShopGradeId.Value
					select item;
			}
			if (shopQueryModel.BrandId > 0)
			{
				gradeId = 
					from item in gradeId
					where item.Himall_ShopBrands.Any((ShopBrandsInfo a) => a.BrandId == shopQueryModel.BrandId)
					select item;
			}
			if (shopQueryModel.CategoryId > 0)
			{
				gradeId = 
					from item in gradeId
					where item.Himall_Products.Any((ProductInfo a) => a.CategoryId == shopQueryModel.CategoryId)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(shopQueryModel.ShopName))
			{
				gradeId = 
					from item in gradeId
					where item.ShopName.Contains(shopQueryModel.ShopName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(shopQueryModel.ShopAccount))
			{
				gradeId = 
					from item in gradeId
					where context.ManagerInfo.Any((ManagerInfo m) => m.UserName.Equals(shopQueryModel.ShopAccount) && m.ShopId != 0 && m.RoleId == 0)
					select item;
			}
			if (shopQueryModel.Status.HasValue && shopQueryModel.Status.Value != 0)
			{
				ShopInfo.ShopAuditStatus value = shopQueryModel.Status.Value;
				DateTime dateTime = DateTime.Now.Date.AddSeconds(-1);
				ShopInfo.ShopAuditStatus shopAuditStatu = value;
				if (shopAuditStatu == ShopInfo.ShopAuditStatus.HasExpired)
				{
					gradeId = 
						from d in gradeId
						where (int)d.ShopStatus == 7 && (d.EndDate < dateTime)
						select d;
				}
				else
				{
					gradeId = (shopAuditStatu != ShopInfo.ShopAuditStatus.Open ? 
						from d in gradeId
						where (int)d.ShopStatus == (int)value
						select d : 
						from d in gradeId
						where (int)d.ShopStatus == 7 && (d.EndDate > dateTime)
						select d);
				}
			}
			Func<IQueryable<ShopInfo>, IOrderedQueryable<ShopInfo>> func = null;
			if (shopQueryModel.OrderBy == 1)
			{
				DbSet<OrderInfo> orderInfo = context.OrderInfo;
				func = (IQueryable<ShopInfo> d) => 
					from o in d
					orderby orderInfo.Where((OrderInfo p) => p.ShopId == o.Id && (int)p.OrderStatus == 5).Count() descending
					select o;
			}
			else
			{
				func = (IQueryable<ShopInfo> d) => 
					from o in d
					orderby o.ShopStatus descending
					select o;
			}
			int num = gradeId.Count();
			gradeId = gradeId.GetPage(out num, shopQueryModel.PageNo, shopQueryModel.PageSize, func);
			foreach (ShopInfo list in gradeId.ToList())
			{
				ManagerInfo managerInfo = context.ManagerInfo.FirstOrDefault((ManagerInfo m) => m.ShopId.Equals(list.Id));
				list.ShopAccount = (managerInfo == null ? "" : managerInfo.UserName);
			}
			return new PageModel<ShopInfo>()
			{
				Models = gradeId,
				Total = num
			};
		}

		public long GetShopSpaceUsage(long shopId)
		{
			string str = string.Format("/Storage/Shop/{0}/Products/", shopId);
			string mapPath = IOHelper.GetMapPath(str);
			long directoryLength = IOHelper.GetDirectoryLength(mapPath) / 1024 / 1024;
			ShopInfo shopInfo = (
				from a in context.ShopInfo
				where a.Id == shopId
				select a).FirstOrDefault();
			if (directoryLength <= context.ShopGradeInfo.FindById<ShopGradeInfo>(shopInfo.GradeId).ImageLimit)
			{
				return directoryLength;
			}
			return -1;
		}

		public long GetShopSpaceUsage2(long shopId)
		{
			string str = string.Format("/Storage/Shop/{0}/Products/", shopId);
			string mapPath = IOHelper.GetMapPath(str);
			long directoryLength = IOHelper.GetDirectoryLength(mapPath) / 1024 / 1024;
			return directoryLength;
		}

		public IQueryable<StatisticOrderCommentsInfo> GetShopStatisticOrderComments(long shopId)
		{
			return 
				from c in context.StatisticOrderCommentsInfo
				where c.ShopId == shopId
				select c;
		}

		public ShopInfo.ShopVistis GetShopVistiInfo(DateTime startDate, DateTime endDate, long shopId)
		{
			ShopInfo.ShopVistis shopVisti = (
				from p in context.ShopVistiInfo
				where p.ShopId == shopId && (p.Date >= startDate) && (p.Date <= endDate)
				group p by p.ShopId into g
				select new ShopInfo.ShopVistis()
				{
					SaleAmounts = g.Sum<ShopVistiInfo>((ShopVistiInfo c) => c.SaleAmounts),
					SaleCounts = g.Sum<ShopVistiInfo>((ShopVistiInfo c) => c.SaleCounts),
					VistiCounts = g.Sum<ShopVistiInfo>((ShopVistiInfo c) => c.VistiCounts),
					OrderCounts = new decimal(0)
				}).FirstOrDefault<ShopInfo.ShopVistis>();
			if (shopVisti != null)
			{
				shopVisti.OrderCounts = (
					from c in context.OrderInfo
					where (c.OrderDate >= startDate) && (c.OrderDate <= endDate) && c.ShopId == shopId
					select c).Count();
			}
			return shopVisti;
		}

		private IEnumerable<long> GetThirdLevelCategories(IEnumerable<long> categoryIds)
		{
			CategoryInfo[] array = Instance<ICategoryService>.Create.GetCategories().ToArray();
			List<long> nums = new List<long>();
			foreach (long list in categoryIds.ToList())
			{
				CategoryInfo categoryInfo = ((IEnumerable<CategoryInfo>)array).FirstOrDefault((CategoryInfo item) => item.Id == list);
				if (categoryInfo.Depth == 1)
				{
					IEnumerable<long> parentCategoryId = 
						from item in array
                        where item.ParentCategoryId == categoryInfo.Id
						select item.Id;
					nums.AddRange(
						from item in array
						where parentCategoryId.Contains(item.ParentCategoryId)
						select item.Id);
				}
				else if (categoryInfo.Depth != 2)
				{
					nums.Add(list);
				}
				else
				{
					nums.AddRange(
						from item in array
						where item.ParentCategoryId == categoryInfo.Id
						select item.Id);
				}
			}
			return nums;
		}

		public PageModel<FavoriteShopInfo> GetUserConcernShops(long userId, int pageNo, int pageSize)
		{
			int num = 0;
			IQueryable<FavoriteShopInfo> favoriteShopInfos = context.FavoriteShopInfo.FindBy<FavoriteShopInfo, long>((FavoriteShopInfo a) => a.UserId == userId, pageNo, pageSize, out num, (FavoriteShopInfo a) => a.Id, false);
			return new PageModel<FavoriteShopInfo>()
			{
				Models = favoriteShopInfos,
				Total = num
			};
		}

		public bool IsExpiredShop(long shopId)
		{
			DateTime date = DateTime.Now.Date;
			ShopInfo shopInfo = context.ShopInfo.FindById<ShopInfo>(shopId);
			DateTime? endDate = shopInfo.EndDate;
			DateTime dateTime = date;
			if ((endDate.HasValue ? endDate.GetValueOrDefault() <= dateTime : false))
			{
				return true;
			}
			return false;
		}

		public bool IsFavoriteShop(long memberId, long shopId)
		{
			bool flag = false;
			if (memberId <= 0)
			{
				throw new HimallException("用户ID不存在！");
			}
			if (context.FavoriteShopInfo.FindBy((FavoriteShopInfo f) => f.ShopId.Equals(shopId) && f.UserId.Equals(memberId)).Count() >= 1)
			{
				flag = true;
			}
			return flag;
		}

		public void LogShopVisti(long shopId)
		{
			DateTime now = DateTime.Now;
			ShopVistiInfo shopVistiInfo = context.ShopVistiInfo.FirstOrDefault((ShopVistiInfo s) => s.ShopId.Equals(shopId) && s.Date.Year.Equals(now.Year) && s.Date.Month.Equals(now.Month) && s.Date.Day.Equals(now.Day));
			if (shopVistiInfo == null || !shopVistiInfo.ShopId.Equals(shopId))
			{
				DbSet<ShopVistiInfo> shopVistiInfos = context.ShopVistiInfo;
				ShopVistiInfo shopVistiInfo1 = new ShopVistiInfo()
				{
					ShopId = shopId,
					Date = DateTime.Now,
					VistiCounts = 1,
					SaleAmounts = new decimal(0),
					SaleCounts = 0
                };
				shopVistiInfos.Add(shopVistiInfo1);
			}
			else
			{
				ShopVistiInfo vistiCounts = shopVistiInfo;
				vistiCounts.VistiCounts = vistiCounts.VistiCounts + 1;
			}
            context.SaveChanges();
		}

		private string MoveImages(long shopId, string image, string name, int index = 1)
		{
			if (string.IsNullOrEmpty(image))
			{
				return "";
			}
			string mapPath = IOHelper.GetMapPath(image);
			string str = ".png";
			string empty = string.Empty;
			string str1 = string.Concat("/Storage/Shop/", shopId, "/Cert");
			empty = IOHelper.GetMapPath(str1);
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			string str2 = string.Concat(name, index, str);
			if (image.Replace("\\", "/").Contains("/temp/"))
			{
				IOHelper.CopyFile(mapPath, empty, true, str2);
			}
			return string.Concat(str1, "/", str2);
		}

		public void SaveBusinessCategory(long id, Dictionary<long, decimal> bCategoryList)
		{
			IQueryable<BusinessCategoryInfo> businessCategoryInfos = context.BusinessCategoryInfo.FindBy((BusinessCategoryInfo b) => b.ShopId.Equals(id));
			foreach (BusinessCategoryInfo list in businessCategoryInfos.ToList())
			{
                context.BusinessCategoryInfo.Remove(list);
			}
			foreach (KeyValuePair<long, decimal> keyValuePair in bCategoryList)
			{
				DbSet<BusinessCategoryInfo> businessCategoryInfo = context.BusinessCategoryInfo;
				BusinessCategoryInfo businessCategoryInfo1 = new BusinessCategoryInfo()
				{
					CategoryId = keyValuePair.Key,
					CommisRate = keyValuePair.Value,
					ShopId = id
				};
				businessCategoryInfo.Add(businessCategoryInfo1);
			}
            context.SaveChanges();
		}

		public void SaveBusinessCategory(long id, decimal commisRate)
		{
			if (commisRate > new decimal(100))
			{
				throw new InvalidPropertyException("分佣比例不能大于100");
			}
			if (commisRate < new decimal(0))
			{
				throw new InvalidPropertyException("分佣比例不能小于0");
			}
			BusinessCategoryInfo businessCategoryInfo = context.BusinessCategoryInfo.FirstOrDefault((BusinessCategoryInfo item) => item.Id == id);
			if (businessCategoryInfo == null)
			{
				throw new HimallException(string.Concat("未找到", id, "对应的经营类目"));
			}
			businessCategoryInfo.CommisRate = commisRate;
            context.SaveChanges();
		}

		public void UpdateBusinessCategory(long shopId, IEnumerable<long> categoryIds)
		{
			IEnumerable<long> nums = categoryIds;
			IQueryable<BusinessCategoryInfo> businessCategoryInfos = context.BusinessCategoryInfo.FindBy((BusinessCategoryInfo b) => b.ShopId.Equals(shopId));
            context.BusinessCategoryInfo.RemoveRange(businessCategoryInfos);
            context.SaveChanges();
			nums = nums.Distinct<long>();
			CategoryInfo[] array = context.CategoryInfo.FindBy((CategoryInfo item) => nums.Contains(item.Id)).ToArray();
			IEnumerable<BusinessCategoryInfo> businessCategoryInfos1 = nums.Select<long, BusinessCategoryInfo>((long item) => {
				CategoryInfo categoryInfo = array.FirstOrDefault((CategoryInfo t) => t.Id == item);
				return new BusinessCategoryInfo()
				{
					CategoryId = item,
					ShopId = shopId,
					CommisRate = categoryInfo.CommisRate
				};
			});
            context.BusinessCategoryInfo.AddRange(businessCategoryInfos1);
            context.SaveChanges();
		}

		public void UpdateLogo(long shopId, string img)
		{
            context.ShopInfo.FindById<ShopInfo>(shopId).Logo = img;
            context.SaveChanges();
		}

		public void UpdateShop(ShopInfo shop)
		{
			ShopInfo shopName = context.ShopInfo.FindById<ShopInfo>(shop.Id);
			shopName.ShopName = shop.ShopName ?? shopName.ShopName;
			shopName.GradeId = (shop.GradeId == 0 ? shopName.GradeId : shop.GradeId);
			ShopInfo shopInfo = shopName;
			DateTime? endDate = shop.EndDate;
			shopInfo.EndDate = (endDate.HasValue ? new DateTime?(endDate.GetValueOrDefault()) : shopName.EndDate);
			shopName.ShopStatus = shop.ShopStatus == 0 ? shopName.ShopStatus : shop.ShopStatus;
			ShopInfo shopInfo1 = shopName;
			ShopInfo.ShopStage? stage = shop.Stage;
			shopInfo1.Stage = (stage.HasValue ? new ShopInfo.ShopStage?(stage.GetValueOrDefault()) : shopName.Stage);
			shopName.BankAccountName = shop.BankAccountName ?? shopName.BankAccountName;
			shopName.BankAccountNumber = shop.BankAccountNumber ?? shopName.BankAccountNumber;
			shopName.BankCode = shop.BankCode ?? shopName.BankCode;
			shopName.BankName = shop.BankName ?? shopName.BankName;
			shopName.BankRegionId = (shop.BankRegionId == 0 ? shopName.BankRegionId : shop.BankRegionId);
			shopName.CompanyRegionId = (shop.CompanyRegionId == 0 ? shopName.CompanyRegionId : shop.CompanyRegionId);
			shopName.CompanyRegisteredCapital = (shop.CompanyRegisteredCapital == new decimal(0) ? shopName.CompanyRegisteredCapital : shop.CompanyRegisteredCapital);
			ShopInfo shopInfo2 = shopName;
			DateTime? businessLicenceEnd = shop.BusinessLicenceEnd;
			shopInfo2.BusinessLicenceEnd = (businessLicenceEnd.HasValue ? new DateTime?(businessLicenceEnd.GetValueOrDefault()) : shopName.BusinessLicenceEnd);
			ShopInfo shopInfo3 = shopName;
			DateTime? businessLicenceStart = shop.BusinessLicenceStart;
			shopInfo3.BusinessLicenceStart = (businessLicenceStart.HasValue ? new DateTime?(businessLicenceStart.GetValueOrDefault()) : shopName.BusinessLicenceStart);
			shopName.legalPerson = shop.legalPerson ?? shopName.legalPerson;
			if (shop.CompanyFoundingDate.HasValue)
			{
				shopName.CompanyFoundingDate = new DateTime?((shop.CompanyFoundingDate.HasValue ? shop.CompanyFoundingDate.Value : shopName.CompanyFoundingDate.Value));
			}
			shopName.Logo = shop.Logo ?? shopName.Logo;
			shopName.SubDomains = shop.SubDomains ?? shopName.SubDomains;
			shopName.Theme = shop.Theme ?? shopName.Theme;
			shopName.BusinessLicenceRegionId = (shop.BusinessLicenceRegionId == 0 ? shopName.BusinessLicenceRegionId : shop.BusinessLicenceRegionId);
			shopName.CompanyEmployeeCount = shop.CompanyEmployeeCount == 0 ? shopName.CompanyEmployeeCount : shop.CompanyEmployeeCount;
			shopName.BusinessLicenceNumber = shop.BusinessLicenceNumber ?? shopName.BusinessLicenceNumber;
			shopName.BusinessSphere = shop.BusinessSphere ?? shopName.BusinessSphere;
			shopName.CompanyAddress = shop.CompanyAddress ?? shopName.CompanyAddress;
			shopName.CompanyName = shop.CompanyName ?? shopName.CompanyName;
			shopName.CompanyPhone = shop.CompanyPhone ?? shopName.CompanyPhone;
			shopName.CompanyRegionAddress = shop.CompanyRegionAddress ?? shopName.CompanyRegionAddress;
			shopName.ContactsEmail = shop.ContactsEmail ?? shopName.ContactsEmail;
			shopName.ContactsName = shop.ContactsName ?? shopName.ContactsName;
			shopName.ContactsPhone = shop.ContactsPhone ?? shopName.ContactsPhone;
			shopName.OrganizationCode = shop.OrganizationCode ?? shopName.OrganizationCode;
			shopName.TaxpayerId = shop.TaxpayerId ?? shopName.TaxpayerId;
			shopName.TaxRegistrationCertificate = shop.TaxRegistrationCertificate ?? shopName.TaxRegistrationCertificate;
			shopName.PayRemark = shop.PayRemark ?? shopName.PayRemark;
			shopName.SenderAddress = shop.SenderAddress ?? shopName.SenderAddress;
			shopName.SenderName = shop.SenderName ?? shopName.SenderName;
			shopName.SenderPhone = shop.SenderPhone ?? shopName.SenderPhone;
			if (!string.IsNullOrEmpty(shop.BusinessLicenseCert))
			{
				shopName.BusinessLicenseCert = string.Concat("/Storage/Shop/", shop.Id, "/Cert/BusinessLicenseCert");
			}
			if (!string.IsNullOrEmpty(shop.ProductCert))
			{
				shopName.ProductCert = string.Concat("/Storage/Shop/", shop.Id, "/Cert/ProductCert");
			}
			if (!string.IsNullOrEmpty(shop.OtherCert))
			{
				shopName.OtherCert = string.Concat("/Storage/Shop/", shop.Id, "/Cert/OtherCert");
			}
			if (!string.IsNullOrEmpty(shop.TaxRegistrationCertificatePhoto))
			{
				shopName.TaxRegistrationCertificatePhoto = string.Concat("/Storage/Shop/", shop.Id, "/Cert/TaxRegistrationCertificatePhoto1.png");
			}
			if (!string.IsNullOrEmpty(shop.BusinessLicenceNumberPhoto))
			{
				shopName.BusinessLicenceNumberPhoto = string.Concat("/Storage/Shop/", shop.Id, "/Cert/BusinessLicenceNumberPhoto1.png");
			}
			if (!string.IsNullOrEmpty(shop.OrganizationCodePhoto))
			{
				shopName.OrganizationCodePhoto = string.Concat("/Storage/Shop/", shop.Id, "/Cert/OrganizationCodePhoto1.png");
			}
			if (!string.IsNullOrEmpty(shop.GeneralTaxpayerPhot))
			{
				shopName.GeneralTaxpayerPhot = string.Concat("/Storage/Shop/", shop.Id, "/Cert/GeneralTaxpayerPhoto1.png");
			}
			if (!string.IsNullOrEmpty(shop.BankPhoto))
			{
				shopName.BankPhoto = string.Concat("/Storage/Shop/", shop.Id, "/Cert/BankPhoto1.png");
			}
			if (!string.IsNullOrEmpty(shop.PayPhoto))
			{
				shopName.PayPhoto = string.Concat("/Storage/Shop/", shop.Id, "/Cert/PayPhoto1.png");
			}
            context.SaveChanges();
			string businessLicenseCert = shop.BusinessLicenseCert;
			string productCert = shop.ProductCert;
			string otherCert = shop.OtherCert;
			string businessLicenceNumberPhoto = shop.BusinessLicenceNumberPhoto;
			string taxRegistrationCertificatePhoto = shop.TaxRegistrationCertificatePhoto;
			string organizationCodePhoto = shop.OrganizationCodePhoto;
			string generalTaxpayerPhot = shop.GeneralTaxpayerPhot;
			string bankPhoto = shop.BankPhoto;
			string payPhoto = shop.PayPhoto;
			if (!string.IsNullOrEmpty(businessLicenceNumberPhoto))
			{
                MoveImages(shop.Id, businessLicenceNumberPhoto, "BusinessLicenceNumberPhoto", 1);
			}
			if (!string.IsNullOrEmpty(taxRegistrationCertificatePhoto))
			{
                MoveImages(shop.Id, taxRegistrationCertificatePhoto, "TaxRegistrationCertificatePhoto", 1);
			}
			if (!string.IsNullOrEmpty(organizationCodePhoto))
			{
                MoveImages(shop.Id, organizationCodePhoto, "OrganizationCodePhoto", 1);
			}
			if (!string.IsNullOrEmpty(generalTaxpayerPhot))
			{
                MoveImages(shop.Id, generalTaxpayerPhot, "GeneralTaxpayerPhoto", 1);
			}
			if (!string.IsNullOrEmpty(bankPhoto))
			{
                MoveImages(shop.Id, bankPhoto, "BankPhoto", 1);
			}
			if (!string.IsNullOrEmpty(payPhoto))
			{
                MoveImages(shop.Id, payPhoto, "PayPhoto", 1);
			}
			if (!string.IsNullOrEmpty(businessLicenseCert))
			{
				string[] strArrays = businessLicenseCert.Split(new char[] { ',' });
				int num = 0;
				string[] strArrays1 = strArrays;
				for (int i = 0; i < strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					num++;
                    MoveImages(shop.Id, str, "BusinessLicenseCert", num);
				}
			}
			if (!string.IsNullOrEmpty(productCert))
			{
				string[] strArrays2 = productCert.Split(new char[] { ',' });
				int num1 = 0;
				string[] strArrays3 = strArrays2;
				for (int j = 0; j < strArrays3.Length; j++)
				{
					string str1 = strArrays3[j];
					num1++;
                    MoveImages(shop.Id, str1, "ProductCert", num1);
				}
			}
			if (!string.IsNullOrEmpty(otherCert))
			{
				string[] strArrays4 = otherCert.Split(new char[] { ',' });
				int num2 = 0;
				string[] strArrays5 = strArrays4;
				for (int k = 0; k < strArrays5.Length; k++)
				{
					string str2 = strArrays5[k];
					num2++;
                    MoveImages(shop.Id, str2, "OtherCert", num2);
				}
			}
			IMessageService create = Instance<IMessageService>.Create;
			Plugin<IEmailPlugin> plugin = PluginsManagement.GetPlugins<IEmailPlugin>().FirstOrDefault<Plugin<IEmailPlugin>>();
			if (!string.IsNullOrWhiteSpace(shopName.ContactsEmail))
			{
				MemberContactsInfo memberContactsInfo = new MemberContactsInfo()
				{
					Contact = shopName.ContactsEmail,
					ServiceProvider = plugin.PluginInfo.PluginId,
					UserId = shopName.Id,
					UserType = MemberContactsInfo.UserTypes.ShopManager
				};
				create.UpdateMemberContacts(memberContactsInfo);
			}
			Plugin<ISMSPlugin> plugin1 = PluginsManagement.GetPlugins<ISMSPlugin>().FirstOrDefault<Plugin<ISMSPlugin>>();
			if (plugin1 != null && !string.IsNullOrWhiteSpace(shopName.ContactsPhone))
			{
				MemberContactsInfo memberContactsInfo1 = new MemberContactsInfo()
				{
					Contact = shopName.ContactsPhone,
					ServiceProvider = plugin1.PluginInfo.PluginId,
					UserId = shopName.Id,
					UserType = MemberContactsInfo.UserTypes.ShopManager
				};
				create.UpdateMemberContacts(memberContactsInfo1);
			}
		}

		public void UpdateShop(ShopInfo shop, IEnumerable<long> categoryIds)
		{
			categoryIds = categoryIds.Distinct<long>();
			categoryIds = GetThirdLevelCategories(categoryIds);
			using (TransactionScope transactionScope = new TransactionScope())
			{
				shop.ShopStatus = ShopInfo.ShopAuditStatus.WaitAudit;
                UpdateShop(shop);
                UpdateBusinessCategory(shop.Id, categoryIds);
				transactionScope.Complete();
			}
		}

		public void UpdateShopFreight(long shopId, decimal freight, decimal freeFreight)
		{
			ShopInfo shopInfo = context.ShopInfo.FindById<ShopInfo>(shopId);
			shopInfo.Freight = freight;
			shopInfo.FreeFreight = freeFreight;
            context.SaveChanges();
		}

		public void UpdateShopGrade(ShopGradeInfo shopGrade)
		{
			ShopGradeInfo name = context.ShopGradeInfo.FindById<ShopGradeInfo>(shopGrade.Id);
			name.Name = shopGrade.Name;
			name.ProductLimit = shopGrade.ProductLimit;
			name.ImageLimit = shopGrade.ImageLimit;
			name.ChargeStandard = shopGrade.ChargeStandard;
            context.SaveChanges();
		}

		public void UpdateShopSenderInfo(long shopId, int regionId, string address, string senderName, string senderPhone)
		{
			ShopInfo nullable = context.ShopInfo.FirstOrDefault((ShopInfo item) => item.Id == shopId);
			if (nullable == null)
			{
				throw new InvalidPropertyException("未找到对应的商铺");
			}
			nullable.SenderRegionId = new int?(regionId);
			nullable.SenderAddress = address;
			nullable.SenderName = senderName;
			nullable.SenderPhone = senderPhone;
            context.SaveChanges();
		}

		public void UpdateShopStatus(long shopId, ShopInfo.ShopAuditStatus status, string comments = "")
		{
			ShopInfo nullable = context.ShopInfo.FindById<ShopInfo>(shopId);
			nullable.ShopStatus = status;
			if (!string.IsNullOrWhiteSpace(comments))
			{
				nullable.RefuseReason = comments;
			}
			if (nullable.IsSelf)
			{
				status = ShopInfo.ShopAuditStatus.Open;
			}
			if (status == ShopInfo.ShopAuditStatus.Open)
			{
				if (!nullable.IsSelf)
				{
					DateTime now = DateTime.Now;
					nullable.EndDate = new DateTime?(now.AddYears(1));
				}
				else
				{
					nullable.ShopStatus = ShopInfo.ShopAuditStatus.Open;
					DateTime dateTime = DateTime.Now;
					nullable.EndDate = new DateTime?(dateTime.AddYears(10));
				}
				MessageShopInfo messageShopInfo = new MessageShopInfo()
				{
					ShopId = shopId,
					ShopName = nullable.ShopName,
					SiteName = Instance<ISiteSettingService>.Create.GetSiteSettings().SiteName
				};
				Task.Factory.StartNew(() => Instance<IMessageService>.Create.SendMessageOnShopAudited(shopId, messageShopInfo));
			}
			if (status == ShopInfo.ShopAuditStatus.WaitPay)
			{
				MessageShopInfo messageShopInfo1 = new MessageShopInfo()
				{
					ShopId = shopId,
					ShopName = nullable.ShopName,
					SiteName = Instance<ISiteSettingService>.Create.GetSiteSettings().SiteName
				};
				Task.Factory.StartNew(() => Instance<IMessageService>.Create.SendMessageOnShopSuccess(shopId, messageShopInfo1));
			}
			if (status == ShopInfo.ShopAuditStatus.Refuse)
			{
				nullable.Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.CompanyInfo);
			}
			nullable.CreateDate = DateTime.Now;
            context.SaveChanges();
		}
	}
}