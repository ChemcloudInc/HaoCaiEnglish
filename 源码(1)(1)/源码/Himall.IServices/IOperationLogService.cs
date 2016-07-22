using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IOperationLogService : IService, IDisposable
	{
		void AddPlatformOperationLog(LogInfo info);

		void AddSellerOperationLog(LogInfo info);

		void DeletePlatformOperationLog(long id);

		PageModel<LogInfo> GetPlatformOperationLogs(OperationLogQuery query);
	}
}