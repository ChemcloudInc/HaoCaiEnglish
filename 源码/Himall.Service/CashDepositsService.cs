using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class CashDepositsService : ServiceBase, ICashDepositsService, IService, IDisposable
	{
		public CashDepositsService()
		{
		}

		public void AddCashDeposit(CashDepositInfo cashDeposit)
		{
            context.CashDepositInfo.Add(cashDeposit);
            context.SaveChanges();
		}

		public void AddCashDepositDetails(CashDepositDetailInfo cashDepositDetail)
		{
            context.CashDepositDetailInfo.Add(cashDepositDetail);
			CashDepositInfo currentBalance = context.CashDepositInfo.FindById<CashDepositInfo>(cashDepositDetail.CashDepositId);
			if (cashDepositDetail.Balance < new decimal(0) && (currentBalance.CurrentBalance + cashDepositDetail.Balance) < new decimal(0))
			{
				HimallException himallException = new HimallException("扣除金额不能多余店铺可用余额");
			}
			currentBalance.CurrentBalance = currentBalance.CurrentBalance + cashDepositDetail.Balance;
			if (cashDepositDetail.Balance > new decimal(0))
			{
				currentBalance.EnableLabels = true;
			}
			if (cashDepositDetail.Balance > new decimal(0))
			{
				currentBalance.TotalBalance = currentBalance.TotalBalance + cashDepositDetail.Balance;
				currentBalance.Date = DateTime.Now;
			}
            context.SaveChanges();
		}

		public void AddCategoryCashDeposits(CategoryCashDepositInfo model)
		{
            context.CategoryCashDepositInfo.Add(model);
            context.SaveChanges();
		}

		public void CloseNoReasonReturn(long categoryId)
		{
			CategoryCashDepositInfo categoryCashDepositInfo = (
				from item in context.CategoryCashDepositInfo
				where item.CategoryId == categoryId
				select item).FirstOrDefault();
			categoryCashDepositInfo.EnableNoReasonReturn = false;
            context.SaveChanges();
		}

		public void DeleteCategoryCashDeposits(long categoryId)
		{
			CategoryCashDepositInfo categoryCashDepositInfo = (
				from item in context.CategoryCashDepositInfo
				where item.CategoryId == categoryId
				select item).FirstOrDefault();
			if (categoryCashDepositInfo != null)
			{
                context.CategoryCashDepositInfo.Remove(categoryCashDepositInfo);
                context.SaveChanges();
			}
		}

		public CashDepositInfo GetCashDeposit(long id)
		{
			return context.CashDepositInfo.FindById<CashDepositInfo>(id);
		}

		public CashDepositInfo GetCashDepositByShopId(long shopId)
		{
			return (
				from item in context.CashDepositInfo
				where item.ShopId == shopId
				select item).FirstOrDefault();
		}

		public PageModel<CashDepositDetailInfo> GetCashDepositDetails(CashDepositDetailQuery query)
		{
			int num;
			IQueryable<CashDepositDetailInfo> startDate = context.CashDepositDetailInfo.AsQueryable<CashDepositDetailInfo>();
			if (query.StartDate.HasValue)
			{
				startDate = 
					from item in startDate
					where query.StartDate <= item.AddDate
                    select item;
			}
			if (query.EndDate.HasValue)
			{
				startDate = 
					from item in startDate
					where query.EndDate >= item.AddDate
                    select item;
			}
			if (!string.IsNullOrWhiteSpace(query.Operator))
			{
				startDate = 
					from item in startDate
					where item.Operator.Contains(query.Operator)
					select item;
			}
			startDate = startDate.FindBy<CashDepositDetailInfo, DateTime>((CashDepositDetailInfo item) => item.CashDepositId == query.CashDepositId, query.PageNo, query.PageSize, out num, (CashDepositDetailInfo item) => item.AddDate, false);
			return new PageModel<CashDepositDetailInfo>()
			{
				Models = startDate,
				Total = num
			};
		}

		public PageModel<CashDepositInfo> GetCashDeposits(CashDepositQuery query)
		{
			int num;
			IQueryable<CashDepositInfo> page = context.CashDepositInfo.AsQueryable<CashDepositInfo>();
			if (!string.IsNullOrWhiteSpace(query.ShopName))
			{
				page = 
					from item in page
					where item.Himall_Shops.ShopName.Contains(query.ShopName)
					select item;
			}
			List<CashDepositInfo> list = page.ToList();
			List<long> nums = new List<long>();
			foreach (CashDepositInfo cashDepositInfo in list)
			{
				if (GetNeedPayCashDepositByShopId(cashDepositInfo.ShopId) <= new decimal(0))
				{
					continue;
				}
				nums.Add(cashDepositInfo.ShopId);
			}
			if (query.Type.HasValue)
			{
				page = (query.Type.Value ? 
					from item in page
					where !nums.Contains(item.ShopId)
					select item : 
					from item in page
					where nums.Contains(item.ShopId)
					select item);
			}
			page = page.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<CashDepositInfo> d) => 
				from o in d
				orderby o.Date descending
				select o);
			return new PageModel<CashDepositInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public CashDepositsObligation GetCashDepositsObligation(long productId)
		{
			CashDepositsObligation cashDepositsObligation = new CashDepositsObligation()
			{
				IsCustomerSecurity = false,
				IsSevenDayNoReasonReturn = false,
				IsTimelyShip = false
			};
			CashDepositsObligation enableNoReasonReturn = cashDepositsObligation;
			IProductService create = Instance<IProductService>.Create;
			IShopService shopService = Instance<IShopService>.Create;
			IShopCategoryService shopCategoryService = Instance<IShopCategoryService>.Create;
			ICategoryService categoryService = Instance<ICategoryService>.Create;
			ProductInfo product = create.GetProduct(productId);
			ShopInfo shop = shopService.GetShop(product.ShopId, false);
			CashDepositInfo cashDepositInfo = (
				from item in context.CashDepositInfo
				where item.ShopId == shop.Id
				select item).FirstOrDefault();
			List<CategoryInfo> list = shopCategoryService.GetBusinessCategory(shop.Id).ToList();
			IEnumerable<long> parentCategoryId = 
				from item in list
				where item.ParentCategoryId == 0
                select item.Id;
			decimal num = context.CategoryCashDepositInfo.FindBy((CategoryCashDepositInfo item) => parentCategoryId.Contains(item.CategoryId)).Max<CategoryCashDepositInfo, decimal>((CategoryCashDepositInfo item) => item.NeedPayCashDeposit);
			if (shop.IsSelf || cashDepositInfo != null && cashDepositInfo.CurrentBalance >= num || cashDepositInfo != null && cashDepositInfo.CurrentBalance < num && cashDepositInfo.EnableLabels)
			{
				List<long> nums = new List<long>()
				{
					product.CategoryId
				};
				CategoryInfo categoryInfo = categoryService.GetTopLevelCategories(nums).FirstOrDefault();
				CategoryCashDepositInfo categoryCashDepositInfo = (
					from item in context.CategoryCashDepositInfo
					where item.CategoryId == categoryInfo.Id
					select item).FirstOrDefault();
				enableNoReasonReturn.IsSevenDayNoReasonReturn = categoryCashDepositInfo.EnableNoReasonReturn;
				enableNoReasonReturn.IsCustomerSecurity = true;
				if (!string.IsNullOrEmpty(product.Himall_FreightTemplate.SendTime))
				{
					enableNoReasonReturn.IsTimelyShip = true;
				}
			}
			return enableNoReasonReturn;
		}

		public IEnumerable<CategoryCashDepositInfo> GetCategoryCashDeposits()
		{
			return context.CategoryCashDepositInfo;
		}

		public decimal GetNeedPayCashDepositByShopId(long shopId)
		{
			decimal num = new decimal(0, 0, 0, false, 2);
			IShopService create = Instance<IShopService>.Create;
			IShopCategoryService shopCategoryService = Instance<IShopCategoryService>.Create;
			List<CategoryInfo> list = shopCategoryService.GetBusinessCategory(shopId).ToList();
			IEnumerable<long> parentCategoryId = 
				from item in list
				where item.ParentCategoryId == 0
                select item.Id;
			decimal num1 = context.CategoryCashDepositInfo.FindBy((CategoryCashDepositInfo item) => parentCategoryId.Contains(item.CategoryId)).Max<CategoryCashDepositInfo, decimal>((CategoryCashDepositInfo item) => item.NeedPayCashDeposit);
			CashDepositInfo cashDepositInfo = (
				from item in context.CashDepositInfo
				where item.ShopId == shopId
				select item).FirstOrDefault();
			if (cashDepositInfo != null && cashDepositInfo.CurrentBalance < num1)
			{
				num = num1 - cashDepositInfo.CurrentBalance;
			}
			if (cashDepositInfo == null)
			{
				num = num1;
			}
			return num;
		}

		public void OpenNoReasonReturn(long categoryId)
		{
			CategoryCashDepositInfo categoryCashDepositInfo = (
				from item in context.CategoryCashDepositInfo
				where item.CategoryId == categoryId
				select item).FirstOrDefault();
			categoryCashDepositInfo.EnableNoReasonReturn = true;
            context.SaveChanges();
		}

		public void UpdateEnableLabels(long id, bool enableLabels)
		{
			CashDepositInfo cashDepositInfo = context.CashDepositInfo.FindById<CashDepositInfo>(id);
			cashDepositInfo.EnableLabels = enableLabels;
            context.SaveChanges();
		}

		public void UpdateNeedPayCashDeposit(long categoryId, decimal cashDeposit)
		{
			CategoryCashDepositInfo categoryCashDepositInfo = (
				from item in context.CategoryCashDepositInfo
				where item.CategoryId == categoryId
				select item).FirstOrDefault();
			categoryCashDepositInfo.NeedPayCashDeposit = cashDeposit;
            context.SaveChanges();
		}
	}
}