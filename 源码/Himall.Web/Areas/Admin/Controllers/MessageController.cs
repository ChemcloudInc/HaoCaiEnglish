using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Net.Mail;
namespace Himall.Web.Areas.Admin.Controllers
{
<<<<<<< HEAD
    public class MessageController : BaseAdminController
    {
        public MessageController()
=======
	public class MessageController : BaseAdminController
	{
		public MessageController()
		{
		}

		public ActionResult Edit(string pluginId)
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			ViewBag.messagePlugins = objs;
			ViewBag.Id = pluginId;
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			ViewBag.Name = plugin.PluginInfo.DisplayName;
			ViewBag.ShortName = plugin.Biz.ShortName;
			FormData formData = plugin.Biz.GetFormData();
			Plugin<ISMSPlugin> plugin1 = PluginsManagement.GetPlugins<ISMSPlugin>().FirstOrDefault<Plugin<ISMSPlugin>>();
			ViewBag.ShowSMS = false;
			ViewBag.ShowBuy = false;
			if (plugin1 != null && pluginId == plugin1.PluginInfo.PluginId)
			{
				ViewBag.ShowSMS = true;
				ViewBag.LoginLink = plugin1.Biz.GetLoginLink();
				ViewBag.BuyLink = plugin1.Biz.GetBuyLink();
				if (plugin1.Biz.IsSettingsValid)
				{
					ViewBag.Amount = plugin1.Biz.GetSMSAmount();
					ViewBag.ShowBuy = true;
				}
			}
			return View(formData);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Enable(string pluginId, MessageTypeEnum messageType, bool enable)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (!enable)
			{
				plugin.Biz.Disable(messageType);
			}
			else
			{
				plugin.Biz.Enable(messageType);
			}
			return Json(new { success = true });
		}

		public ActionResult Management()
		{
			IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) => {
				dynamic expandoObjects = new ExpandoObject();
				expandoObjects.name = item.PluginInfo.DisplayName;
				expandoObjects.pluginId = item.PluginInfo.PluginId;
				expandoObjects.enable = item.PluginInfo.Enable;
				expandoObjects.status = item.Biz.GetAllStatus();
				return expandoObjects;
			});
			return View(objs);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Save(string pluginId, string values)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			IEnumerable<KeyValuePair<string, string>> keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values);
			plugin.Biz.SetFormValues(keyValuePairs);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Send(string pluginId, string destination)
		{
			Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
			if (string.IsNullOrEmpty(destination))
			{
				Result result = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不能为空！")
				};
				return Json(result);
			}
			if (!plugin.Biz.CheckDestination(destination))
			{
				Result result1 = new Result()
				{
					success = false,
					msg = string.Concat("你填写的", plugin.Biz.ShortName, "不正确")
				};
				return Json(result1);
			}
			string siteName = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().SiteName;
		//	string str = plugin.Biz.SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
			string str= SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
        //    string str = SendMail();
           // string str = SendMails();
            if (str == "发送成功")
			{
				return Json(new { success = true });
			}
			Result result2 = new Result()
			{
				success = false,
				msg = str
			};
			return Json(result2);
		}
        public string SendTestMessage(string destination, string content, string title = "")
>>>>>>> 8b62adbb635f3bb77e4d8915eb0541ae63db9843
        {
        }

<<<<<<< HEAD
        public ActionResult Edit(string pluginId)
        {
            IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) =>
            {
                dynamic expandoObjects = new ExpandoObject();
                expandoObjects.name = item.PluginInfo.DisplayName;
                expandoObjects.pluginId = item.PluginInfo.PluginId;
                expandoObjects.enable = item.PluginInfo.Enable;
                expandoObjects.status = item.Biz.GetAllStatus();
                return expandoObjects;
            });
            ViewBag.messagePlugins = objs;
            ViewBag.Id = pluginId;
            Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
            ViewBag.Name = plugin.PluginInfo.DisplayName;
            ViewBag.ShortName = plugin.Biz.ShortName;
            FormData formData = plugin.Biz.GetFormData();
            Plugin<ISMSPlugin> plugin1 = PluginsManagement.GetPlugins<ISMSPlugin>().FirstOrDefault<Plugin<ISMSPlugin>>();
            ViewBag.ShowSMS = false;
            ViewBag.ShowBuy = false;
            if (plugin1 != null && pluginId == plugin1.PluginInfo.PluginId)
            {
                ViewBag.ShowSMS = true;
                ViewBag.LoginLink = plugin1.Biz.GetLoginLink();
                ViewBag.BuyLink = plugin1.Biz.GetBuyLink();
                if (plugin1.Biz.IsSettingsValid)
                {
                    ViewBag.Amount = plugin1.Biz.GetSMSAmount();
                    ViewBag.ShowBuy = true;
                }
            }
            return View(formData);
        }

        [HttpPost]
        [UnAuthorize]
        [ValidateInput(false)]
        public JsonResult Enable(string pluginId, MessageTypeEnum messageType, bool enable)
        {
            Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
            if (!enable)
=======
       
            msg.From = new MailAddress("15950560518@163.com", "15950560518@163.com", System.Text.Encoding.UTF8);
           
            msg.Subject = "这是测试邮件";//邮件标题   
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码   
            msg.Body = "邮件内容";//邮件内容   
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码   
            msg.IsBodyHtml = false;//是否是HTML邮件   
            msg.Priority = MailPriority.High;//邮件优先级   

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("15950560518@163.com", "10201116yy");
            //在71info.com注册的邮箱和密码   
            client.Host = "smtp.163.com";
            client.Port = 25;
            object userState = msg;
            try
>>>>>>> 8b62adbb635f3bb77e4d8915eb0541ae63db9843
            {
                plugin.Biz.Disable(messageType);
            }
            else
            {
                plugin.Biz.Enable(messageType);
            }
            return Json(new { success = true });
        }

        public ActionResult Management()
        {
            IEnumerable<object> objs = PluginsManagement.GetPlugins<IMessagePlugin>().Select<Plugin<IMessagePlugin>, object>((Plugin<IMessagePlugin> item) =>
            {
                dynamic expandoObjects = new ExpandoObject();
                expandoObjects.name = item.PluginInfo.DisplayName;
                expandoObjects.pluginId = item.PluginInfo.PluginId;
                expandoObjects.enable = item.PluginInfo.Enable;
                expandoObjects.status = item.Biz.GetAllStatus();
                return expandoObjects;
            });
            return View(objs);
        }

        [HttpPost]
        [UnAuthorize]
        [ValidateInput(false)]
        public JsonResult Save(string pluginId, string values)
        {
            Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(values);
            plugin.Biz.SetFormValues(keyValuePairs);
            return Json(new { success = true });
        }

        [HttpPost]
        [UnAuthorize]
        [ValidateInput(false)]
        public JsonResult Send(string pluginId, string destination)
        {
            Plugin<IMessagePlugin> plugin = PluginsManagement.GetPlugin<IMessagePlugin>(pluginId);
            if (string.IsNullOrEmpty(destination))
            {
                Result result = new Result()
                {
                    success = false,
                    msg = string.Concat("你填写的", plugin.Biz.ShortName, "不能为空！")
                };
                return Json(result);
            }
            if (!plugin.Biz.CheckDestination(destination))
            {
                Result result1 = new Result()
                {
                    success = false,
                    msg = string.Concat("你填写的", plugin.Biz.ShortName, "不正确")
                };
                return Json(result1);
            }
<<<<<<< HEAD
            string siteName = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().SiteName;
            string str = plugin.Biz.SendTestMessage(destination, string.Concat("该条为测试信息，请勿回复!【", siteName, "】"), "这是一封测试邮件");
            if (str == "发送成功")
            {
                return Json(new { success = true });
            }
            Result result2 = new Result()
            {
                success = false,
                msg = str
            };
            return Json(result2);
        }

=======
        }*/
        public string SendMail()
        {
            string str;
            MailMessage objMailMessage = new MailMessage();
            string fromAddress = "15950560518@163.com";//你在web.config中配置的发件人地址，就是你的邮箱地址。
            string mailHost = "smtp.163.com";//邮件服务器，如mail.qq.com

            objMailMessage.From = new MailAddress(fromAddress);//发送方地址
            objMailMessage.To.Add(new MailAddress("1446878469@qq.com"));//收信人地址
            objMailMessage.BodyEncoding = System.Text.Encoding.UTF8;//邮件编码
            objMailMessage.Subject = "kobe";//邮件标题
            objMailMessage.Body = "sss";//邮件内容
            objMailMessage.IsBodyHtml = true;//邮件正文是否为html格式

            SmtpClient objSmtpClient = new SmtpClient();
            objSmtpClient.Host = mailHost;//邮件服务器地址
            objSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//通过网络发送到stmp邮件服务器
            objSmtpClient.Credentials = new System.Net.NetworkCredential("15950560518@163.com", "10200610yz");//发送方的邮件地址，密码
            //objSmtpClient.EnableSsl = true;//SMTP 服务器要求安全连接需要设置此属性

            try
            {
                objSmtpClient.Send(objMailMessage);
                str = "发送成功";
            }
            catch (Exception ex)
            {
                str = "发送失败";
            }
            return str;
        }
>>>>>>> 8b62adbb635f3bb77e4d8915eb0541ae63db9843

    }
}