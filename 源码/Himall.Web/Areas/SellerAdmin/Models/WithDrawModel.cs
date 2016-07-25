using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Himall.Web.Areas.SellerAdmin.Models
{
    public class WithDrawModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id
        {
            get;
            set;
        }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string MembersId
        {
            get;
            set;
        }
        /// <summary>
        /// 提现类型：支付宝，微信，银联
        /// </summary>
        public string WithdrawType
        {
            get;
            set;
        }
        /// <summary>
        /// 账号信息
        /// </summary>
        public string AccountNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdNo
        {
            get;
            set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }
        public WithDrawModel()
        {
        }
    }
}