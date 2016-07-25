using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface ICollocationService : IService, IDisposable
	{
		void AddCollocation(CollocationInfo info);

		void CancelCollocation(long CollocationId, long shopId);

		void EditCollocation(CollocationInfo info);

		CollocationInfo GetCollocation(long Id);

		CollocationInfo GetCollocationByProductId(long productId);

		PageModel<CollocationInfo> GetCollocationList(CollocationQuery query);

		CollocationSkuInfo GetColloSku(long colloPid, string skuid);

		List<CollocationSkuInfo> GetProductColloSKU(long productid, long colloPid);
	}
}