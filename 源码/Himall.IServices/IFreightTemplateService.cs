using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IFreightTemplateService : IService, IDisposable
	{
		void DeleteFreightTemplate(long TemplateId);

		IQueryable<FreightAreaContentInfo> GetFreightAreaContent(long TemplateId);

		FreightTemplateInfo GetFreightTemplate(long TemplateId);

		IQueryable<ProductInfo> GetProductUseFreightTemp(long TemplateId);

		IQueryable<FreightTemplateInfo> GetShopFreightTemplate(long ShopID);

		void UpdateFreightTemplate(FreightTemplateInfo templateInfo);
	}
}