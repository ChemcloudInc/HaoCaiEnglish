using Himall.Core.Plugins;
using Himall.Core.Plugins.Express;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IExpressService : IService, IDisposable
	{
		IEnumerable<IExpress> GetAllExpress();

		IExpress GetExpress(string name);

		ExpressData GetExpressData(string expressCompanyName, string shipOrderNumber);

		IDictionary<int, string> GetPrintElementIndexAndOrderValue(long shopId, long orderId, IEnumerable<int> printElementIndexes);

		IEnumerable<IExpress> GetRecentExpress(long shopId, int takeNumber);

		void UpdatePrintElement(string name, IEnumerable<ExpressPrintElement> elements);
	}
}