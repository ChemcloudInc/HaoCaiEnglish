using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Linq;

namespace Himall.IServices
{
	public interface IConsultationService : IService, IDisposable
	{
		void AddConsultation(ProductConsultationInfo model);

		void DeleteConsultation(long id);

		ProductConsultationInfo GetConsultation(long id);

		PageModel<ProductConsultationInfo> GetConsultations(ConsultationQuery query);

		IQueryable<ProductConsultationInfo> GetConsultations(long pid);

		void ReplyConsultation(long id, string ReplyContent, long shopId);
	}
}