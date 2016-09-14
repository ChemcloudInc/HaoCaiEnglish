using com.paypal.sdk.profiles;
using com.paypal.sdk.services;
using com.paypal.sdk.util;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Controllers
{
    public class PaypalController : BaseWebController
    {
        /// <summary>
        /// 初始化APIProfile
        /// </summary>
        /// <returns></returns>
        private  IAPIProfile CreateProfile()
        {

            IAPIProfile profile = ProfileFactory.createSignatureAPIProfile();

            profile.APIUsername = base.CurrentSiteSetting.PayPalAPIAccountName;// Himall.Web.Framework.ServiceHelper.Create<Himall.IServices.IHimall_DictionariesService>().GetValueBYKey("PayPalAPIAccountName");
            profile.APIPassword = base.CurrentSiteSetting.PayPalAPIAccountPassword;//Himall.Web.Framework.ServiceHelper.Create<Himall.IServices.IHimall_DictionariesService>().GetValueBYKey("PayPalAPIAccountPassword");
            profile.APISignature = base.CurrentSiteSetting.PayPalAPISignature;//Himall.Web.Framework.ServiceHelper.Create<Himall.IServices.IHimall_DictionariesService>().GetValueBYKey("PayPalAPISignature");
            profile.Environment = base.CurrentSiteSetting.PayPalEnvenment;//Himall.Web.Framework.ServiceHelper.Create<Himall.IServices.IHimall_DictionariesService>().GetValueBYKey("PayPalEnvenment");
            profile.Subject = "";
            return profile;
        }

        /// <summary>
        /// 初始化支付服务器调用
        /// </summary>
        /// <returns></returns>
        private  NVPCallerServices InitializeServices()
        {

            NVPCallerServices service = new NVPCallerServices();
            IAPIProfile profile = CreateProfile();
            service.APIProfile = profile;
            return service;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="nvp"></param>
        /// <returns></returns>
        private  NVPCodec SendExpressCheckoutCommand(NVPCodec nvp)
        {
            NVPCallerServices service = InitializeServices();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            string response = service.Call(nvp.Encode());
            NVPCodec responsenvp = new NVPCodec();
            responsenvp.Decode(response);
            return responsenvp;

        }

        /// <summary>
        /// 发送支付请求
        /// </summary>
        /// <param name="nvp"></param>
        /// <returns></returns>
        public  NVPCodec SetExpressCheckout(NVPCodec nvp)
        {
            nvp.Add("METHOD", "SetExpressCheckout");
            return SendExpressCheckoutCommand(nvp);
        }

        /// <summary>
        /// 得到用户在paypay中填写的详细信息
        /// </summary>
        /// <param name="nvp"></param>
        /// <returns></returns>
        public  NVPCodec GetExpressCheckoutDetails(NVPCodec nvp)
        {
            nvp.Add("METHOD", "GetExpressCheckoutDetails");
            return SendExpressCheckoutCommand(nvp);
        }

        /// <summary>
        /// 付款操作
        /// </summary>
        /// <param name="nvp"></param>
        /// <returns></returns>
        public  NVPCodec DoExpressCheckoutPayment(NVPCodec nvp)
        {
            nvp.Add("METHOD", "DoExpressCheckoutPayment");
            return SendExpressCheckoutCommand(nvp);
        }


        public  bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }
    }
}
