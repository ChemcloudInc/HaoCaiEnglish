using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface ICustomerService : IService, IDisposable
	{
		void AddCustomerService(CustomerServiceInfo customerService);

		void DeleteCustomerService(long shopId, params long[] ids);

		IQueryable<CustomerServiceInfo> GetCustomerService(long shopId);

		CustomerServiceInfo GetCustomerService(long shopId, long id);

		void UpdateCustomerService(CustomerServiceInfo customerService);
	}
}