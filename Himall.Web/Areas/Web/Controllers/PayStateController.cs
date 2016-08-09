using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Himall.Web.Areas.Web.Controllers
{
	public class PayStateController : BaseAsyncController
	{
		public PayStateController()
		{
		}

		public void CheckAsync(string orderIds)
		{
			Func<string, long> func = null;
			Func<OrderInfo, bool> orderStatus = null;
			base.AsyncManager.OutstandingOperations.Increment();
			int num1 = 200;
			int num2 = 10000;
            
                Task.Factory.StartNew(() =>
                {
                    string str = CacheKeyCollection.PaymentState(string.Join(",", new string[] { orderIds }));
                    int num = 0;
                    while (true)
                    {
                        if (Cache.Get(str) == null)
                        {
                            string[] strArrays = orderIds.Split(new char[] { ',' });
                            if (func == null)
                            {
                                func = (string item) => long.Parse(item);
                            }
                            IEnumerable<long> nums = strArrays.Select<string, long>(func);
                            using (IOrderService create = Instance<IOrderService>.Create)
                            {
                                IEnumerable<OrderInfo> orders = create.GetOrders(nums);
                                if (orderStatus == null)
                                {
                                    orderStatus = (OrderInfo item) => item.OrderStatus == OrderInfo.OrderOperateStatus.WaitPay;
                                }
                                Cache.Insert(str, !orders.Any(orderStatus), 15);
                            }
                        }
                        if ((bool)Cache.Get(str))
                        {
                            AsyncManager.Parameters["done"] = true;
                            break;
                        }
                        else if (num <= num2)
                        {
                            num = num + num1;
                            Thread.Sleep(num1);
                        }
                        else
                        {
                            AsyncManager.Parameters["done"] = false;
                            break;
                        }
                    }
                    AsyncManager.OutstandingOperations.Decrement();
                });
            
		}

		public void CheckChargeAsync(string orderIds)
		{
			base.AsyncManager.OutstandingOperations.Increment();
			int num1 = 200;
			int num2 = 10000;
			Task.Factory.StartNew(() => {
				string str = CacheKeyCollection.PaymentState(orderIds);
				int num = 0;
				while (true)
				{
					if (Cache.Get(str) == null)
					{
						using (IMemberCapitalService create = Instance<IMemberCapitalService>.Create)
						{
							ChargeDetailInfo chargeDetail = create.GetChargeDetail(long.Parse(orderIds));
							Cache.Insert(str, (chargeDetail == null ? false : chargeDetail.ChargeStatus == ChargeDetailInfo.ChargeDetailStatus.ChargeSuccess), 15);
						}
					}
					if ((bool)Cache.Get(str))
					{
                        AsyncManager.Parameters["done"] = true;
						break;
					}
					else if (num <= num2)
					{
						num = num + num1;
						Thread.Sleep(num1);
					}
					else
					{
                        AsyncManager.Parameters["done"] = false;
						break;
					}
				}
                AsyncManager.OutstandingOperations.Decrement();
			});
		}

		public JsonResult CheckChargeCompleted(bool done)
		{
			return Json(new { success = done }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult CheckCompleted(bool done)
		{
			return Json(new { success = done }, JsonRequestBehavior.AllowGet);
		}
	}
}