using Himall.Core;
using Himall.Model;
using System;

namespace Himall.Service.Market.Business
{
	public class GenerateDetailContext
	{
		private IGenerateDetail _generate;

		private BonusInfo _bonus;

		public GenerateDetailContext(BonusInfo model)
		{
            _bonus = model;
			BonusInfo.BonusPriceType? priceType = _bonus.PriceType;
			BonusInfo.BonusPriceType valueOrDefault = priceType.GetValueOrDefault();
			if (priceType.HasValue)
			{
				switch (valueOrDefault)
				{
					case BonusInfo.BonusPriceType.Fixed:
					{
                            _generate = new FixedGeneration(model.FixedAmount.Value);
						return;
					}
					case BonusInfo.BonusPriceType.Random:
					{
                            _generate = new RandomlyGeneration(model.RandomAmountStart.Value, model.RandomAmountEnd.Value);
						break;
					}
					default:
					{
						return;
					}
				}
			}
		}

		public void Generate()
		{
			if (_generate == null)
			{
				throw new HimallException("生成红包详情策略构造异常");
			}
            _generate.Generate(_bonus.Id, _bonus.TotalPrice);
		}
	}
}