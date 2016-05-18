using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class SystemAgreementService : ServiceBase, ISystemAgreementService, IService, IDisposable
	{
		public SystemAgreementService()
		{
		}

		public void AddAgreement(AgreementInfo model)
		{
            context.AgreementInfo.Add(model);
            context.SaveChanges();
		}

		public AgreementInfo GetAgreement(AgreementInfo.AgreementTypes type)
		{
			return (
				from b in context.AgreementInfo
				where b.AgreementType == (int)type
				select b).FirstOrDefault();
		}

		public bool UpdateAgreement(AgreementInfo model)
		{
			AgreementInfo agreement = GetAgreement((AgreementInfo.AgreementTypes)model.AgreementType);
			agreement.AgreementType = model.AgreementType;
			agreement.AgreementContent = model.AgreementContent;
			agreement.LastUpdateTime = DateTime.Now;
			if (context.SaveChanges() > 0)
			{
				return true;
			}
			return false;
		}
	}
}