using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class AccountService : ServiceBase, IAccountService, IService, IDisposable
	{
        private OrderService orderService = new OrderService();
		public AccountService()
		{
		}

		public void ConfirmAccount(long id, string managerRemark)
		{
			AccountInfo accountInfo = context.AccountInfo.FindById<AccountInfo>(id);
			accountInfo.Status = AccountInfo.AccountStatus.Accounted;
			accountInfo.Remark = managerRemark;
            accountInfo.FinishDate = DateTime.Now;
            context.SaveChanges();
            foreach (AccountDetailInfo accDetail in GetAccountDetail(accountInfo.Id))
            {
                OrderInfo orderInfo = orderService.GetOrder(accDetail.OrderId);
                if(orderInfo!=null)
                {
                    orderInfo.AccountType = OrderInfo.AccountTypes.FinishAccount;
                    orderService.UpdateOrderInfo(orderInfo);
                }
            }
           
		}

		public AccountInfo GetAccount(long id)
		{
			return context.AccountInfo.FindById<AccountInfo>(id);
		}

        public IQueryable<AccountDetailInfo> GetAccountDetail(long accountid)
        {
            IQueryable<AccountDetailInfo> accountDetailInfos = context.AccountDetailInfo.FindBy((AccountDetailInfo b) => b.AccountId.Equals(accountid));
            return accountDetailInfos;
        }
		public PageModel<AccountDetailInfo> GetAccountDetails(AccountQuery query)
		{
			int num;
			IQueryable<AccountDetailInfo> accountDetailInfo = 
				from item in context.AccountDetailInfo
				where (int)item.OrderType == (int)query.EnumOrderType && item.AccountId == query.AccountId
				select item;
			if (query.StartDate.HasValue)
			{
				accountDetailInfo = 
					from item in accountDetailInfo
					where item.Date >= query.StartDate
                    select item;
			}
			if (query.EndDate.HasValue)
			{
				accountDetailInfo = 
					from item in accountDetailInfo
					where item.Date < query.EndDate
                    select item;
			}
			accountDetailInfo = accountDetailInfo.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<AccountDetailInfo>()
			{
				Models = accountDetailInfo,
				Total = num
			};
		}

		public PageModel<AccountMetaModel> GetAccountMeta(AccountQuery query)
		{
			int num;
			IQueryable<AccountMetaModel> accountInfo = 
				from a in context.AccountInfo
				join b in context.AccountMetaInfo on a.Id equals b.AccountId
				where (!query.StartDate.HasValue || a.StartDate >= query.StartDate) && (!query.EndDate.HasValue || a.EndDate < query.EndDate) && a.Id == query.AccountId
				select new AccountMetaModel()
				{
					AccountId = a.Id,
					Id = b.Id,
					EndDate = b.ServiceEndTime,
					StartDate = b.ServiceStartTime,
					MetaKey = b.MetaKey,
					MetaValue = b.MetaValue
				};
			IQueryable<AccountMetaModel> accountMetaModels = accountInfo.FindBy<AccountMetaModel, long>((AccountMetaModel item) => true, query.PageNo, query.PageSize, out num, (AccountMetaModel item) => item.Id, false);
			return new PageModel<AccountMetaModel>()
			{
				Models = accountMetaModels,
				Total = num
			};
		}

		public PageModel<AccountPurchaseAgreementInfo> GetAccountPurchaseAgreements(AccountQuery query)
		{
			int num;
			IQueryable<AccountPurchaseAgreementInfo> accountPurchaseAgreementInfo = 
				from item in context.AccountPurchaseAgreementInfo
				where item.AccountId == query.AccountId
                select item;
			if (query.StartDate.HasValue)
			{
				accountPurchaseAgreementInfo = 
					from item in accountPurchaseAgreementInfo
					where item.Date >= query.StartDate
                    select item;
			}
			if (query.EndDate.HasValue)
			{
				accountPurchaseAgreementInfo = 
					from item in accountPurchaseAgreementInfo
					where item.Date < query.EndDate
                    select item;
			}
			accountPurchaseAgreementInfo = accountPurchaseAgreementInfo.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<AccountPurchaseAgreementInfo>()
			{
				Models = accountPurchaseAgreementInfo,
				Total = num
			};
		}

		public PageModel<AccountInfo> GetAccounts(AccountQuery query)
		{
			int num;
			IQueryable<AccountInfo> status = context.AccountInfo.AsQueryable<AccountInfo>();
			if (query.Status.HasValue)
			{
				status = 
					from item in status
					where (int?)query.Status == (int?)item.Status
					select item;
			}
			if (query.ShopId.HasValue)
			{
				status = 
					from item in status
					where query.ShopId == item.ShopId
                    select item;
			}
			if (!string.IsNullOrEmpty(query.ShopName))
			{
				status = 
					from item in status
					where item.ShopName.Contains(query.ShopName)
					select item;
			}
			status = status.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<AccountInfo>()
			{
				Models = status,
				Total = num
			};
		}
	}
}