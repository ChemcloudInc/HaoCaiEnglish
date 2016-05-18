using System;

namespace Himall.Service.Market.Business
{
	internal interface IGenerateDetail
	{
		void Generate(long bounsId, decimal totalPrice);
	}
}