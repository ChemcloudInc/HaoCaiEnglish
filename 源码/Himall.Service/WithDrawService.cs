using Himall.Core;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Himall.Entity;

namespace Himall.Service
{
    public class WithDrawService : ServiceBase, IWithDrawService, IService, IDisposable
    {
        public WithDrawService()
        {
        }



        public void AddWithDraw(WithDrawInfo model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "添加一个提现信息，Model为空");
            }
            context.WithDrawInfo.Add(model);
            context.SaveChanges();
            Cache.Remove("Cache-WithDraw");
        }

        public void UpdateWithDraw(WithDrawInfo model)
        {
            WithDrawInfo name = context.WithDrawInfo.FindById<WithDrawInfo>(model.Id);
            name.WithdrawType = model.WithdrawType;
            context.SaveChanges();
            Cache.Remove("Cache-WithDraw");
        }

        public void DeleteWithDraw(long id)
        {
            context.WithDrawInfo.Remove(context.WithDrawInfo.FindById<WithDrawInfo>(id));
            context.SaveChanges();
        }

        public IQueryable<WithDrawInfo> GetWithDrawByMembersId(string membersid)
        {
            return context.WithDrawInfo.FindBy((WithDrawInfo w) => w.MembersId == membersid);
        }

        public WithDrawInfo GetWithDrawById(long id)
        {
            return context.WithDrawInfo.FindById<WithDrawInfo>(id);
        }
    }
}
