using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Himall.Web.Tests
{
    [TestClass]
    public class AlipayTest
    {
        [TestMethod]
        public void Alipay()
        {
            Himall.Plugin.Payment.Alipay.Service service = new Plugin.Payment.Alipay.Service();
            service.WorkDirectory = @"D:\Himall2\Himall.Web\Plugins\Payment\Himall.Plugin.Payment.Alipay";
            string AlipayUrl = service.GetRequestUrl("http://#returnUrl", "http://#notyfyUrl", "#0001", 88, "测试商品");

        }
    }
}
