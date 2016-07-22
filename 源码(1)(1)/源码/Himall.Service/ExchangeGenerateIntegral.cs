using Himall.IServices;
using System;

namespace Himall.Service
{
	public class ExchangeGenerateIntegral : ServiceBase, IConversionMemberIntegralBase
	{
		private int _Integral;

		public ExchangeGenerateIntegral(int Integral)
		{
			if (Integral > 0)
			{
                _Integral = -Integral;
			}
		}

		public int ConversionIntegral()
		{
			return _Integral;
		}
	}
}