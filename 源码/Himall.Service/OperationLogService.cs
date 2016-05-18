using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class OperationLogService : ServiceBase, IOperationLogService, IService, IDisposable
	{
		public OperationLogService()
		{
		}

		public void AddPlatformOperationLog(LogInfo model)
		{
			model.ShopId = 0;
            context.LogInfo.Add(model);
            context.SaveChanges();
		}

		public void AddSellerOperationLog(LogInfo model)
		{
			if (model.ShopId == 0)
			{
				throw new HimallException("日志获取店铺ID错误");
			}
			model.ShopId = model.ShopId;
            context.LogInfo.Add(model);
            context.SaveChanges();
		}

		public void DeletePlatformOperationLog(long id)
		{
			throw new NotImplementedException();
		}

		public PageModel<LogInfo> GetPlatformOperationLogs(OperationLogQuery query)
		{
			int num = 0;
			IQueryable<LogInfo> userName = context.LogInfo.FindBy((LogInfo item) => item.ShopId == query.ShopId);
			if (!string.IsNullOrWhiteSpace(query.UserName))
			{
				userName = 
					from item in userName
					where query.UserName == item.UserName
					select item;
			}
			if (query.StartDate.HasValue)
			{
				userName = 
					from item in userName
					where item.Date >= query.StartDate.Value
					select item;
			}
			if (query.EndDate.HasValue)
			{
				userName = 
					from item in userName
					where item.Date <= query.EndDate.Value
					select item;
			}
			userName = userName.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<LogInfo>()
			{
				Models = userName,
				Total = num
			};
		}
	}
}