using Himall.IServices;
using Himall.ServiceProvider;
using Quartz;
using System;

namespace Himall.Service.Job
{
	public class OrderJob : IJob
	{
		public OrderJob()
		{
		}

		public void Execute(IJobExecutionContext context)
		{
			IOrderService create = Instance<IOrderService>.Create;
			create.AutoCloseOrder();
			create.AutoConfirmOrder();
			Instance<IGiftsOrderService>.Create.AutoConfirmOrder();
		}
	}
}