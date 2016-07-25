using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class CustomerService : ServiceBase, ICustomerService, IService, IDisposable
	{
		public CustomerService()
		{
		}

		public void AddCustomerService(CustomerServiceInfo customerService)
		{
            CheckPropertyWhenAdd(customerService);
			customerService.Name = customerService.Name.Trim();
            context.CustomerServiceInfo.Add(customerService);
            context.SaveChanges();
		}

		private void CheckPropertyWhenAdd(CustomerServiceInfo customerService)
		{
			if (string.IsNullOrWhiteSpace(customerService.Name))
			{
				throw new InvalidPropertyException("客服名称不能为空");
			}
			if (customerService.ShopId == 0)
			{
				throw new InvalidPropertyException("店铺id必须大于0");
			}
			if (string.IsNullOrWhiteSpace(customerService.AccountCode))
			{
				throw new InvalidPropertyException("沟通工具账号不能为空");
			}
		}

		private CustomerServiceInfo CheckPropertyWhenUpdate(CustomerServiceInfo customerService)
		{
			if (string.IsNullOrWhiteSpace(customerService.Name))
			{
				throw new InvalidPropertyException("客服名称不能为空");
			}
			if (customerService.Id == 0)
			{
				throw new InvalidPropertyException("客服id必须大于0");
			}
			if (customerService.ShopId == 0)
			{
				throw new InvalidPropertyException("店铺id必须大于0");
			}
			if (string.IsNullOrWhiteSpace(customerService.AccountCode))
			{
				throw new InvalidPropertyException("沟通工具账号不能为空");
			}
			CustomerServiceInfo customerServiceInfo = context.CustomerServiceInfo.FirstOrDefault((CustomerServiceInfo item) => item.Id == customerService.Id && item.ShopId == customerService.ShopId);
			if (customerServiceInfo == null)
			{
				throw new InvalidPropertyException(string.Concat("不存在id为", customerService.Id, "的客服信息"));
			}
			return customerServiceInfo;
		}

		public void DeleteCustomerService(long shopId, params long[] ids)
		{
            context.CustomerServiceInfo.OrderBy((CustomerServiceInfo item) => item.ShopId == shopId && ids.Contains(item.Id));
            context.SaveChanges();
		}

		public IQueryable<CustomerServiceInfo> GetCustomerService(long shopId)
		{
			return 
				from item in context.CustomerServiceInfo
				where item.ShopId == shopId
				select item;
		}

		public CustomerServiceInfo GetCustomerService(long shopId, long id)
		{
			return context.CustomerServiceInfo.FirstOrDefault((CustomerServiceInfo item) => item.Id == id && item.ShopId == shopId);
		}

		public void UpdateCustomerService(CustomerServiceInfo customerService)
		{
			CustomerServiceInfo name = CheckPropertyWhenUpdate(customerService);
			name.Name = customerService.Name;
			name.Type = customerService.Type;
			name.Tool = customerService.Tool;
			name.AccountCode = customerService.AccountCode;
            context.SaveChanges();
		}
	}
}