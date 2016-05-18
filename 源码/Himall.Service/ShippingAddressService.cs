using Himall.Core;
using Himall.Entity;
using Himall.IServices;
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
	public class ShippingAddressService : ServiceBase, IShippingAddressService, IService, IDisposable
	{
		public ShippingAddressService()
		{
		}

		public void AddShippingAddress(ShippingAddressInfo shipinfo)
		{
			shipinfo.IsDefault = false;
			shipinfo.IsQuick = false;
			if ((
				from a in context.ShippingAddressInfo
				where a.UserId == shipinfo.UserId
				select a).Count() >= 20)
			{
				throw new HimallException("收货地址最多最能创建20个！");
			}
            context.ShippingAddressInfo.Add(shipinfo);
            context.SaveChanges();
            SetDefaultShippingAddress(shipinfo.Id, shipinfo.UserId);
		}

		public void DeleteShippingAddress(long id, long userId)
		{
			ShippingAddressInfo shippingAddressInfo = (
				from a in context.ShippingAddressInfo
				where a.Id == id && a.UserId == userId
				select a).FirstOrDefault();
			if (shippingAddressInfo == null)
			{
				throw new HimallException("该收货地址不存在或已被删除！");
			}
            context.ShippingAddressInfo.Remove(shippingAddressInfo);
            context.SaveChanges();
		}

		public ShippingAddressInfo GetDefaultUserShippingAddressByUserId(long userId)
		{
			ShippingAddressInfo regionFullName = context.ShippingAddressInfo.FirstOrDefault((ShippingAddressInfo item) => item.UserId == userId && item.IsDefault) ?? (
				from item in context.ShippingAddressInfo
				where item.UserId == userId
				orderby item.Id descending
				select item).FirstOrDefault();
			if (regionFullName != null)
			{
				IRegionService create = Instance<IRegionService>.Create;
				regionFullName.RegionFullName = create.GetRegionFullName(regionFullName.RegionId, " ");
				regionFullName.RegionIdPath = create.GetRegionIdPath(regionFullName.RegionId);
			}
			return regionFullName;
		}

		public ShippingAddressInfo GetUserShippingAddress(long shippingAddressId)
		{
			IRegionService create = Instance<IRegionService>.Create;
			DbSet<ShippingAddressInfo> shippingAddressInfo = context.ShippingAddressInfo;
			object[] objArray = new object[] { shippingAddressId };
			ShippingAddressInfo regionFullName = shippingAddressInfo.Find(objArray);
			regionFullName.RegionFullName = create.GetRegionFullName(regionFullName.RegionId, " ");
			regionFullName.RegionIdPath = create.GetRegionIdPath(regionFullName.RegionId);
			return regionFullName;
		}

		public IQueryable<ShippingAddressInfo> GetUserShippingAddressByUserId(long userId)
		{
			IRegionService create = Instance<IRegionService>.Create;
			IOrderedQueryable<ShippingAddressInfo> shippingAddressInfo = 
				from a in context.ShippingAddressInfo
				where a.UserId == userId
				orderby a.Id descending
				select a;
			foreach (ShippingAddressInfo regionFullName in shippingAddressInfo)
			{
				regionFullName.RegionFullName = create.GetRegionFullName(regionFullName.RegionId, " ");
				regionFullName.RegionIdPath = create.GetRegionIdPath(regionFullName.RegionId);
			}
			return shippingAddressInfo;
		}

		public void SetDefaultShippingAddress(long id, long userId)
		{
			IQueryable<ShippingAddressInfo> shippingAddressInfo = 
				from a in context.ShippingAddressInfo
				where a.UserId == userId
				select a;
			foreach (ShippingAddressInfo list in shippingAddressInfo.ToList())
			{
				if (list.Id != id)
				{
					list.IsDefault = false;
				}
				else
				{
					list.IsDefault = true;
				}
			}
            context.SaveChanges();
		}

		public void SetQuickShippingAddress(long id, long userId)
		{
			IQueryable<ShippingAddressInfo> shippingAddressInfo = 
				from a in context.ShippingAddressInfo
				where a.UserId == userId
				select a;
			foreach (ShippingAddressInfo shippingAddressInfo1 in shippingAddressInfo)
			{
				if (shippingAddressInfo1.Id != id)
				{
					shippingAddressInfo1.IsQuick = false;
				}
				else
				{
					shippingAddressInfo1.IsQuick = true;
				}
			}
            context.SaveChanges();
		}

		public void UpdateShippingAddress(ShippingAddressInfo shipinfo)
		{
			ShippingAddressInfo phone = (
				from a in context.ShippingAddressInfo
				where a.Id == shipinfo.Id && a.UserId == shipinfo.UserId
				select a).FirstOrDefault();
			if (phone == null)
			{
				throw new HimallException("该收货地址不存在或已被删除！");
			}
			phone.Phone = shipinfo.Phone;
			phone.RegionId = shipinfo.RegionId;
			phone.ShipTo = shipinfo.ShipTo;
			phone.Address = shipinfo.Address;
            context.SaveChanges();
		}
	}
}