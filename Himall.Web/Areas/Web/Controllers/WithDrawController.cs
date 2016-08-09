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
        public ActionResult AddWithDraw(long? id)
        {
            
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "支付宝", Value = "支付宝" });
            items.Add(new SelectListItem { Text = "微信", Value = "微信" });
            items.Add(new SelectListItem { Text = "银联", Value = "银联" });
            this.ViewData["list"] = items;

            WithDrawInfo withDrawInfo;
            WithDrawInfo withDrawModel;
            IWithDrawService withDrawService = ServiceHelper.Create<IWithDrawService>();
            if (id.HasValue)
            {
                long? nullable = id;
                if ((nullable.GetValueOrDefault() <= 0 ? true : !nullable.HasValue))
                {
                    withDrawInfo = new WithDrawInfo();
                    withDrawModel = new WithDrawInfo()
                    {
                        Id = withDrawInfo.Id,
                        WithdrawType = withDrawInfo.WithdrawType,
                        AccountNumber = withDrawInfo.AccountNumber,
                        Name = withDrawInfo.Name,
                        IdNo = withDrawInfo.IdNo,
                        Mobile = withDrawInfo.Mobile


                    };
                    return View(withDrawModel);
                }
                withDrawInfo = withDrawService.GetWithDrawById(id.Value);
                withDrawModel = new WithDrawInfo()
                {
                    Id = withDrawInfo.Id,
                    WithdrawType = withDrawInfo.WithdrawType,
                    AccountNumber = withDrawInfo.AccountNumber,
                    Name = withDrawInfo.Name,
                    IdNo = withDrawInfo.IdNo,
                    Mobile = withDrawInfo.Mobile
                };
                return View(withDrawModel);
            }
            withDrawInfo = new WithDrawInfo();
            withDrawModel = new WithDrawInfo()
            {
                Id = withDrawInfo.Id,
                WithdrawType = withDrawInfo.WithdrawType,
                AccountNumber = withDrawInfo.AccountNumber,
                Name = withDrawInfo.Name,
                IdNo = withDrawInfo.IdNo,
                Mobile = withDrawInfo.Mobile
            };
            return View(withDrawModel);
        }
        [HttpPost]
        [UnAuthorize]
        public JsonResult AddWithDraw(WithDrawInfo infoModel)
        {
            //info.MembersId = base.CurrentUser.UserName;
            //info.Id = ServiceHelper.Create<IWithDrawService>().GetNextWithDrawId();
            //ServiceHelper.Create<IWithDrawService>().AddWithDraw(info);
            //return Json(new { success = true });

            IWithDrawService withDrawService = ServiceHelper.Create<IWithDrawService>();
            WithDrawInfo withDrawInfo = new WithDrawInfo()
            {
                Id = infoModel.Id,
                WithdrawType=infoModel.WithdrawType,
                AccountNumber =infoModel.AccountNumber,
                Name = infoModel.Name,
                IdNo = infoModel.IdNo,
                Mobile=infoModel.Mobile,
                MembersId = base.CurrentSellerManager.UserName
            };
            WithDrawInfo withDrawInfo1 = withDrawInfo;
            if (withDrawInfo1.Id <= 0)
            {
                withDrawInfo1.Id = ServiceHelper.Create<IWithDrawService>().GetNextWithDrawId();
                withDrawService.AddWithDraw(withDrawInfo1);
            }
            else
            {
                withDrawService.UpdateWithDraw(withDrawInfo1);
            }
            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult EditWithDraw(WithDrawInfo info)
        {
            info.MembersId = this.CurrentUser.UserName;
            ServiceHelper.Create<IWithDrawService>().UpdateWithDraw(info);
            return Json(new { success = true, msg = "修改成功", id = info.Id });
        }

        [HttpPost]
        public JsonResult DeleteWithDraw(long id)
        {
            
            ServiceHelper.Create<IWithDrawService>().DeleteWithDraw(id);
            Result result = new Result()
            {
                success = true,
                msg = "删除成功"
            };
            return Json(result);
        }
        public ActionResult Index()
        {
            string membersId = this.CurrentUser.UserName;
            return View(ServiceHelper.Create<IWithDrawService>().GetWithDrawByMembersId(membersId));//因为UserName值唯一，所以没有登录账号ID去获取信息
            //WithDrawInfo w = ServiceHelper.Create<IWithDrawService>().GetWithDrawById(1);

            //return View();
        }
        
    }
}