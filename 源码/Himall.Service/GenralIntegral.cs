using Himall.IServices;
using System;

namespace Himall.Service
{
	public class GenralIntegral : ServiceBase, IConversionMemberIntegralBase
	{
		private int _Integral;

		public GenralIntegral(int Integral)
		{
            _Integral = Integral;
		}

		public int ConversionIntegral()
		{
			return _Integral;
		}
	}
}