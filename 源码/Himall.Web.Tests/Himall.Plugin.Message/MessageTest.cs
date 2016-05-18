using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Himall.Web.Tests.Message
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void SMS()
        {
            Himall.Plugin.Message.SMS.Service service = new Plugin.Message.SMS.Service();
            service.WorkDirectory = @"D:\Himall2\Himall.Web\Plugins\Message\Himall.Plugin.Message.SMS";
            string ret = service.SendTestMessage("1550769****", "测试", "测试");
        }
    }
}
