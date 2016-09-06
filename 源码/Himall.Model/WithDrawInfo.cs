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
       [Required(ErrorMessage = "Please select")]
        public string WithdrawType
        {
            get;
            set;
        }
        /// <summary>
        /// 账号信息
        /// </summary>
        [Required(ErrorMessage = "Please enter account number")]
        public string AccountNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "Please enter Name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Required(ErrorMessage = "Please enter ID Card Number")]
        public string IdNo
        {
            get;
            set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
       [Required(ErrorMessage = "Please enter moblie number")]
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
