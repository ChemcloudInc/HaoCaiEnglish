using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class MemberGradeService : ServiceBase, IMemberGradeService, IService, IDisposable
	{
		public MemberGradeService()
		{
		}

		public void AddMemberGrade(MemberGrade model)
		{
			if (model.Integral < 0)
			{
				throw new HimallException("积分不能为负数");
			}
			if (context.MemberGrade.Any((MemberGrade a) => a.Integral == model.Integral))
			{
				throw new HimallException("等级之间的积分不能相同");
			}
            context.MemberGrade.Add(model);
            context.SaveChanges();
		}

		public void DeleteMemberGrade(long id)
		{
			if (context.GiftInfo.Any((GiftInfo d) => d.NeedGrade == id))
			{
				throw new HimallException("有礼品兑换与等级关联，不可删除！");
			}
            context.MemberGrade.OrderBy((MemberGrade a) => a.Id == id);
            context.SaveChanges();
		}

		public MemberGrade GetMemberGrade(long id)
		{
			return context.MemberGrade.Find(new object[] { id });
		}

		public IEnumerable<MemberGrade> GetMemberGradeList()
		{
			List<MemberGrade> list = context.MemberGrade.ToList();
			var collection = (
				from d in context.GiftInfo
				group d by d.NeedGrade into d
				select new { cnt = d.Count(), grid = d.Key }).ToList();
			foreach (MemberGrade memberGrade in list)
			{
				if (!collection.Any((d) => {
					if (d.grid != memberGrade.Id)
					{
						return false;
					}
					return d.cnt > 0;
				}))
				{
					continue;
				}
				memberGrade.IsNoDelete = true;
			}
			return list;
		}

		public void UpdateMemberGrade(MemberGrade model)
		{
			if (context.MemberGrade.Any((MemberGrade a) => a.Integral == model.Integral && a.Id != model.Id))
			{
				throw new HimallException("等级之间的积分不能相同");
			}
            context.Set<MemberGrade>().Attach(model);
            context.Entry<MemberGrade>(model).State = EntityState.Modified;
            context.SaveChanges();
		}
	}
}