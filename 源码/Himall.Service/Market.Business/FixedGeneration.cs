using Himall.Entity;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service.Market.Business
{
	internal class FixedGeneration : IGenerateDetail
	{
		private readonly decimal _fixedAmount;

		private Entities _context = new Entities();

		public FixedGeneration(decimal fixedAmount)
		{
            _fixedAmount = fixedAmount;
		}

		public void Generate(long bounsId, decimal totalPrice)
		{
			try
			{
				try
				{
					int num = (int)(totalPrice / _fixedAmount);
					List<BonusReceiveInfo> bonusReceiveInfos = new List<BonusReceiveInfo>();
					for (int i = 0; i < num; i++)
					{
						BonusReceiveInfo bonusReceiveInfo = new BonusReceiveInfo()
						{
							BonusId = bounsId,
							Price = _fixedAmount,
							IsShare = new bool?(false),
							OpenId = null
						};
						bonusReceiveInfos.Add(bonusReceiveInfo);
						if (bonusReceiveInfos.Count >= 500)
						{
                            _context.BonusReceiveInfo.AddRange(bonusReceiveInfos);
                            _context.SaveChanges();
							bonusReceiveInfos.Clear();
						}
					}
                    _context.BonusReceiveInfo.AddRange(bonusReceiveInfos);
                    _context.SaveChanges();
				}
				catch
				{
                    _context.BonusReceiveInfo.Remove((BonusReceiveInfo p) => p.BonusId == bounsId);
                    _context.BonusInfo.Remove((BonusInfo p) => p.Id == bounsId);
                    _context.SaveChanges();
				}
			}
			finally
			{
                _context.Dispose();
			}
		}
	}
}