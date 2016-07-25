using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IShippingAddressService : IService, IDisposable
	{
		void AddShippingAddress(ShippingAddressInfo shipinfo);

		void DeleteShippingAddress(long id, long userId);

		ShippingAddressInfo GetDefaultUserShippingAddressByUserId(long userId);

		ShippingAddressInfo GetUserShippingAddress(long shippingAddressId);

		IQueryable<ShippingAddressInfo> GetUserShippingAddressByUserId(long userId);

		void SetDefaultShippingAddress(long id, long userId);

		void SetQuickShippingAddress(long id, long userId);

		void UpdateShippingAddress(ShippingAddressInfo shipinfo);
	}
}