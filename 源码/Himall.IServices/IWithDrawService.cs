using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Himall.Model;

namespace Himall.IServices
{
    public interface IWithDrawService : IService, IDisposable
    {
        /// <summary>
        /// 添加提现方式信息
        /// </summary>
        /// <param name="model"></param>
        void AddWithDraw(WithDrawInfo model);

        /// <summary>
        /// 修改提现方式信息
        /// </summary>
        /// <param name="model"></param>
        void UpdateWithDraw(WithDrawInfo model);

        /// <summary>
        /// 删除提现方式信息
        /// </summary>
        /// <param name="id"></param>
        void DeleteWithDraw(long id);

        /// <summary>
        /// 通过卖家登录名获取提现方式列表
        /// </summary>
        /// <param name="membersid"></param>
        /// <returns></returns>
        IQueryable<WithDrawInfo> GetWithDrawByMembersId(string membersid);

        /// <summary>
        /// 通过编号获取提现方式信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WithDrawInfo GetWithDrawById(long id);



    }
}
