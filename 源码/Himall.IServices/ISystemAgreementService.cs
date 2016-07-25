using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface ISystemAgreementService : IService, IDisposable
	{
		void AddAgreement(AgreementInfo model);

		AgreementInfo GetAgreement(AgreementInfo.AgreementTypes type);

		bool UpdateAgreement(AgreementInfo model);
	}
}