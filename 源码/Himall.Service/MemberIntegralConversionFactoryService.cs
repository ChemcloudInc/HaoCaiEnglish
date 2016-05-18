using Himall.IServices;
using Himall.Model;
using System;

namespace Himall.Service
{
	public class MemberIntegralConversionFactoryService : ServiceBase, IMemberIntegralConversionFactoryService, IService, IDisposable
	{
		public MemberIntegralConversionFactoryService()
		{
		}

		public IConversionMemberIntegralBase Create(MemberIntegral.IntegralType type, int Integral = 0)
		{
			switch (type)
			{
				case MemberIntegral.IntegralType.Consumption:
				case MemberIntegral.IntegralType.SystemOper:
				case MemberIntegral.IntegralType.Cancel:
				case MemberIntegral.IntegralType.Others:
				{
					return new GenralIntegral(Integral);
				}
				case MemberIntegral.IntegralType.Exchange:
				{
					return new ExchangeGenerateIntegral(Integral);
				}
				case MemberIntegral.IntegralType.InvitationMemberRegiste:
				{
					return new InvitationMemberRegisteGenerateIntegral();
				}
				case (MemberIntegral.IntegralType)4:
				{
					return null;
				}
				case MemberIntegral.IntegralType.Login:
				{
					return new LoginGenerateIntegral();
				}
				case MemberIntegral.IntegralType.BindWX:
				{
					return new BindWXGenerateIntegral();
				}
				case MemberIntegral.IntegralType.Comment:
				{
					return new CommentGenerateIntegral();
				}
				case MemberIntegral.IntegralType.Reg:
				{
					return new RegisteGenerateIntegral();
				}
				default:
				{
					return null;
				}
			}
		}
	}
}