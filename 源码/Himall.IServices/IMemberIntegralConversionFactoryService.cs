using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IMemberIntegralConversionFactoryService : IService, IDisposable
	{
		IConversionMemberIntegralBase Create(MemberIntegral.IntegralType type, int use = 0);
	}
}