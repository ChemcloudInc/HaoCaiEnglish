using Himall.Entity;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service.Market.Business
{
	internal class RandomlyGeneration : IGenerateDetail
	{
		private decimal _randomAmountStart;

		private decimal _randomAmountEnd;

		private Entities _context = new Entities();

		private static long tick;

		private Random _random = new Random((int)(RandomlyGeneration.tick & -1) | (int)(RandomlyGeneration.tick >> 32));

		static RandomlyGeneration()
		{
			RandomlyGeneration.tick = DateTime.Now.Ticks;
		}

		public RandomlyGeneration(decimal randomAmountStart, decimal randomAmountEnd)
		{
            _randomAmountStart = randomAmountStart;
            _randomAmountEnd = randomAmountEnd;
		}

		public void Generate(long bounsId, decimal totalPrice)
		{
			try
			{
				try
				{
					List<BonusReceiveInfo> bonusReceiveInfos = new List<BonusReceiveInfo>();
					while (totalPrice != new decimal(0))
					{
						decimal num = new decimal(0);
						if (totalPrice <= _randomAmountEnd && _randomAmountEnd > _randomAmountStart)
						{
                            _randomAmountEnd = totalPrice;
						}
						num = GenerateRandomAmountPrice();
						if ((totalPrice - num) < _randomAmountStart)
						{
							num = totalPrice;
						}
						totalPrice = totalPrice - num;
						BonusReceiveInfo bonusReceiveInfo = new BonusReceiveInfo()
						{
							BonusId = bounsId,
							Price = num,
							IsShare = new bool?(false),
							OpenId = null
						};
						bonusReceiveInfos.Add(bonusReceiveInfo);
						if (bonusReceiveInfos.Count < 500)
						{
							continue;
						}
                        _context.BonusReceiveInfo.AddRange(bonusReceiveInfos);
                        _context.SaveChanges();
						bonusReceiveInfos.Clear();
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

		private decimal GenerateRandomAmountPrice()
		{
			decimal num = _random.Next((int)_randomAmountStart, (int)_randomAmountEnd);
			string str = string.Format("{0:N2} ", _randomAmountStart);
			string str1 = string.Format("{0:N2} ", _randomAmountEnd);
			str = str.Substring(str.IndexOf('.') + 1, 2);
			str1 = str1.Substring(str1.IndexOf('.') + 1, 2);
			if (int.Parse(str) == 0)
			{
				str = "1";
			}
			if (int.Parse(str1) < int.Parse(str))
			{
				str1 = "100";
			}
			decimal num1 = _random.Next(int.Parse(str), int.Parse(str1)) / new decimal(100);
			num = num + num1;
			return num;
		}
	}
}