using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himall.Model
{
    public class WithDrawInfo : BaseModel
    {
        private long _id;
        public new long Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                base.Id = value;
            }
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
       [Required(ErrorMessage = "请选择提现类型")]
        public string WithdrawType
        {
            get;
            set;
        }
        /// <summary>
        /// 账号信息
        /// </summary>
        [Required(ErrorMessage = "请输入账号信息")]
        public string AccountNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "请输入姓名")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Required(ErrorMessage = "请输入身份证号")]
        public string IdNo
        {
            get;
            set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
       [Required(ErrorMessage = "请输入手机号")]
        public string Mobile
        {
            get;
            set;
        }
        public WithDrawInfo()
        {
        }
    }
}
