using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
    public class WithDrawController : BaseMemberController
    {
        public WithDrawController()
        {
        }
        // GET: Web/WithDraw
        public ActionResult Index()
        {
            string membersId = this.CurrentUser.UserName;
            return View(ServiceHelper.Create<IWithDrawService>().GetWithDrawByMembersId(membersId));//因为UserName值唯一，所以没有登录账号ID去获取信息
            //WithDrawInfo w = ServiceHelper.Create<IWithDrawService>().GetWithDrawById(1);

            //return View();
        }
        public ActionResult WithDrawInfomation()
        {
            WithDrawInfo w= ServiceHelper.Create<IWithDrawService>().GetWithDrawById(1);
            return View();
        }
    }
}