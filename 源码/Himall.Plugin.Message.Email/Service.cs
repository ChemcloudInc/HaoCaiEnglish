using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.MessagePlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Himall.Plugin.Message.Email
{
    public class Service : IEmailPlugin, IMessagePlugin, IPlugin
    {
        private MessageStatus messageStatus;

        private Dictionary<MessageTypeEnum, StatusEnum> dic = new Dictionary<MessageTypeEnum, StatusEnum>();

        public bool EnableLog
        {
            get
            {
                return false;
            }
        }

        public bool IsSettingsValid
        {
            get
            {
                bool flag;
                try
                {
                    CheckCanEnable();
                    flag = true;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
        }

        public string Logo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EmailCore.WorkDirectory))
                {
                    throw new MissingFieldException("没有设置插件工作目录");
                }
                return string.Concat(EmailCore.WorkDirectory, "/Data/logo.png");
            }
        }

        public string ShortName
        {
            get
            {
                return "邮箱";
            }
        }

        public string WorkDirectory
        {
            set
            {
                EmailCore.WorkDirectory = value;
            }
        }

        public Service()
        {
            if (!string.IsNullOrEmpty(EmailCore.WorkDirectory))
            {
                InitMessageStatus();
            }
        }

        public void CheckCanEnable()
        {
            MessageEmailConfig config = EmailCore.GetConfig();
            if (string.IsNullOrWhiteSpace(config.SendAddress))
            {
                throw new PluginConfigException("未设置SMTP邮件地址");
            }
            if (!ValidateHelper.IsEmail(config.SendAddress))
            {
                throw new PluginConfigException("SMTP用户名填写错误");
            }
            if (string.IsNullOrWhiteSpace(config.EmailName))
            {
                throw new PluginConfigException("未设置SMTP邮箱用户名");
            }
            if (string.IsNullOrWhiteSpace(config.Password))
            {
                throw new PluginConfigException("未设置SMTP邮箱密码");
            }
            if (!ValidateHelper.IsNumeric(config.SmtpPort))
            {
                throw new PluginConfigException("SMTP端口错误");
            }
            if (string.IsNullOrWhiteSpace(config.SmtpServer))
            {
                throw new PluginConfigException("未设置SMTP邮箱服务器");
            }
            if (string.IsNullOrWhiteSpace(config.DisplayName))
            {
                throw new PluginConfigException("未设置SMTP邮箱显示名称");
            }
        }

        public bool CheckDestination(string destination)
        {
            return ValidateHelper.IsEmail(destination);
        }

        public void Disable(MessageTypeEnum e)
        {
            CheckCanEnable();
            if ((
                from a in dic
                where a.Key == e
                select a).FirstOrDefault().Value == (StatusEnum)3)
            {
                throw new HimallException("该功能已被禁止，不能进行设置");
            }
            SetMessageStatus(e, (StatusEnum)2);
            FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
            try
            {
                (new XmlSerializer(typeof(MessageStatus))).Serialize(fileStream, messageStatus);
            }
            finally
            {
                if (fileStream != null)
                {
                    ((IDisposable)fileStream).Dispose();
                }
            }
        }

        public void Enable(MessageTypeEnum e)
        {
            CheckCanEnable();
            if ((
                from a in dic
                where a.Key == e
                select a).FirstOrDefault().Value == (StatusEnum)3)
            {
                throw new HimallException("该功能已被禁止，不能进行设置");
            }
            SetMessageStatus(e, (StatusEnum)1);
            FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
            try
            {
                (new XmlSerializer(typeof(MessageStatus))).Serialize(fileStream, messageStatus);
            }
            finally
            {
                if (fileStream != null)
                {
                    ((IDisposable)fileStream).Dispose();
                }
            }
        }

        public Dictionary<MessageTypeEnum, StatusEnum> GetAllStatus()
        {
            InitMessageStatus();
            return dic;
        }

        public FormData GetFormData()
        {
            MessageEmailConfig config = EmailCore.GetConfig();
            FormData formDatum = new FormData();
            FormData.FormItem[] formItemArray = new FormData.FormItem[6];
            FormData.FormItem formItem = new FormData.FormItem();
            formItem.DisplayName = "SMTP服务器";
            formItem.Name = "SmtpServer";
            formItem.IsRequired = true;
            formItem.Type = FormData.FormItemType.text;
            formItem.Value = config.SmtpServer;
            formItemArray[0] = formItem;
            FormData.FormItem formItem1 = new FormData.FormItem();
            formItem1.DisplayName = "SMTP服务器端口";
            formItem1.Name = "SmtpPort";
            formItem1.IsRequired = true;
            formItem1.Type = FormData.FormItemType.text;
            formItem1.Value = config.SmtpPort;
            formItemArray[1] = formItem1;
            FormData.FormItem formItem2 = new FormData.FormItem();
            formItem2.DisplayName = "SMTP用户名";
            formItem2.Name = "EmailName";
            formItem2.IsRequired = true;
            formItem2.Type = FormData.FormItemType.text;
            formItem2.Value = config.EmailName;
            formItemArray[2] = formItem2;
            FormData.FormItem formItem3 = new FormData.FormItem();
            formItem3.DisplayName = "SMTP用户密码";
            formItem3.Name = "Password";
            formItem3.IsRequired = true;
            formItem3.Type = FormData.FormItemType.password;
            formItem3.Value = config.Password;
            formItemArray[3] = formItem3;
            FormData.FormItem formItem4 = new FormData.FormItem();
            formItem4.DisplayName = "SMTP邮箱";
            formItem4.Name = "SendAddress";
            formItem4.IsRequired = true;
            formItem4.Type = FormData.FormItemType.text;
            formItem4.Value = config.SendAddress;
            formItemArray[4] = formItem4;
            FormData.FormItem formItem5 = new FormData.FormItem();
            formItem5.DisplayName = "显示名称";
            formItem5.Name = "DisplayName";
            formItem5.IsRequired = true;
            formItem5.Type = FormData.FormItemType.text;
            formItem5.Value = config.DisplayName;
            formItemArray[5] = formItem5;
            formDatum.Items = formItemArray;
            return formDatum;
        }

        public StatusEnum GetStatus(MessageTypeEnum e)
        {
            InitMessageStatus();
            KeyValuePair<MessageTypeEnum, StatusEnum> keyValuePair = dic.FirstOrDefault((KeyValuePair<MessageTypeEnum, StatusEnum> a) => a.Key == e);
            return keyValuePair.Value;
        }

        private void InitMessageStatus()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(EmailCore.WorkDirectory);
            FileInfo fileInfo = directoryInfo.GetFiles("Data/config.xml").FirstOrDefault();
            if (fileInfo != null)
            {
                FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open);
                try
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(MessageStatus));
                    messageStatus = (MessageStatus)xmlSerializer.Deserialize(fileStream);
                    dic.Clear();
                    dic.Add((MessageTypeEnum)5, (StatusEnum)messageStatus.FindPassWord);
                    dic.Add((MessageTypeEnum)1, (StatusEnum)messageStatus.OrderCreated);
                    dic.Add((MessageTypeEnum)2, (StatusEnum)messageStatus.OrderPay);
                    dic.Add((MessageTypeEnum)4, (StatusEnum)messageStatus.OrderRefund);
                    dic.Add((MessageTypeEnum)3, (StatusEnum)messageStatus.OrderShipping);
                    dic.Add((MessageTypeEnum)6, (StatusEnum)messageStatus.ShopAudited);
                    dic.Add((MessageTypeEnum)7, (StatusEnum)messageStatus.ShopSuccess);
                }
                finally
                {
                    if (fileStream != null)
                    {
                        ((IDisposable)fileStream).Dispose();
                    }
                }
            }
        }

        private void SendMessage(string[] destination, string title, string body)
        {
            if (destination.Length > 0)
            {
                (new SendMail()).SendEmail(title, destination, body, true);
            }
        }

        private void SendMessage(string destination, string title, string body, bool async = false)
        {
            if (!string.IsNullOrWhiteSpace(destination))
            {
                SendMail sendMail = new SendMail();
                string[] strArrays = new string[] { destination };
                sendMail.SendEmail(title, strArrays, body, async);
            }
        }

        public string SendMessageCode(string destination, MessageUserInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.Bind.Replace("#userName#", info.UserName).Replace("#checkCode#", info.CheckCode).Replace("#siteName#", info.SiteName);
            SendMessage(destination, string.Concat(info.SiteName, "邮箱验证码"), str, false);
            return str;
        }

        public string SendMessageOnFindPassWord(string destination, MessageUserInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.FindPassWord.Replace("#userName#", info.UserName).Replace("#checkCode#", info.CheckCode).Replace("#siteName#", info.SiteName);
            SendMessage(destination, string.Concat(info.SiteName, "找回密码验证"), str, false);
            return str;
        }

        public string SendMessageOnOrderCreate(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderCreated.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            SendMessage(destination, string.Concat(info.SiteName, "订单创建成功"), str, false);
            return str;
        }

        public string SendMessageOnOrderPay(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderPay.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            decimal totalMoney = info.TotalMoney;
            string str1 = str.Replace("#Total#", totalMoney.ToString("F2"));
            SendMessage(destination, string.Concat(info.SiteName, "订单支付成功"), str1, false);
            return str1;
        }

        public string SendMessageOnOrderRefund(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderRefund.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            decimal refundMoney = info.RefundMoney;
            string str1 = str.Replace("#RefundMoney#", refundMoney.ToString("F2"));
            SendMessage(destination, string.Concat(info.SiteName, "订单(", info.OrderId, ")退款已处理"), str1, false);
            return str1;
        }

        public string SendMessageOnOrderShipping(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderShipping.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName).Replace("#shippingCompany#", info.ShippingCompany).Replace("#shippingNumber", info.ShippingNumber);
            SendMessage(destination, string.Concat(info.SiteName, "订单(", info.OrderId, ")已发货"), str, false);
            return str;
        }

        public string SendMessageOnShopAudited(string destination, MessageShopInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.ShopAudited.Replace("#userName#", info.UserName).Replace("#shopName#", info.ShopName).Replace("#siteName#", info.SiteName);
            SendMessage(destination, string.Concat(info.SiteName, "店铺(", info.ShopName, ")审核已审核"), str, false);
            return str;
        }

        public string SendMessageOnShopSuccess(string destination, MessageShopInfo info)
        {
            MessageContent messageContentConfig = EmailCore.GetMessageContentConfig();
            string str = messageContentConfig.ShopSuccess.Replace("#userName#", info.UserName).Replace("#shopName#", info.ShopName).Replace("#siteName#", info.SiteName);
            SendMessage(destination, string.Concat(info.SiteName, "店铺(", info.ShopName, ")已开通"), str, false);
            return str;
        }

        public void SendMessages(string[] destination, string content, string title)
        {
            SendMessage(destination, title, content);
        }

        public string SendTestMessage(string destination, string content, string title)
        {
            string message = "发送成功";
            string str = content;
            try
            {
                SendMessage(destination, title, str, false);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            return message;
        }

        public void SetAllStatus(Dictionary<MessageTypeEnum, StatusEnum> dic)
        {
            foreach (KeyValuePair<MessageTypeEnum, StatusEnum> keyValuePair in dic)
            {
                SetMessageStatus(keyValuePair.Key, keyValuePair.Value);
            }
            FileStream fileStream = new FileStream(string.Concat(EmailCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
            try
            {
                (new XmlSerializer(typeof(MessageStatus))).Serialize(fileStream, messageStatus);
            }
            finally
            {
                if (fileStream != null)
                {
                    ((IDisposable)fileStream).Dispose();
                }
            }
        }

        public void SetFormValues(IEnumerable<KeyValuePair<string, string>> values)
        {
            KeyValuePair<string, string> keyValuePair = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "SmtpServer");
            if (string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                throw new PluginConfigException("SMTP服务器不能为空");
            }
            KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "SmtpPort");
            if (!ValidateHelper.IsNumeric(keyValuePair1.Value))
            {
                throw new PluginConfigException("SMTP端口错误");
            }
            KeyValuePair<string, string> keyValuePair2 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "EmailName");
            if (string.IsNullOrWhiteSpace(keyValuePair2.Value))
            {
                throw new PluginConfigException("SMTP用户名不能为空");
            }
            KeyValuePair<string, string> keyValuePair3 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "Password");
            if (string.IsNullOrWhiteSpace(keyValuePair3.Value))
            {
                throw new PluginConfigException("SMTP密码不能为空");
            }
            KeyValuePair<string, string> keyValuePair4 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "SendAddress");
            if (string.IsNullOrWhiteSpace(keyValuePair4.Value))
            {
                throw new PluginConfigException("SMTP邮箱不能为空");
            }
            if (!ValidateHelper.IsEmail(keyValuePair4.Value))
            {
                throw new PluginConfigException("SMTP邮箱错误");
            }
            KeyValuePair<string, string> keyValuePair5 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "DisplayName");
            if (string.IsNullOrWhiteSpace(keyValuePair5.Value))
            {
                throw new PluginConfigException("SMTP显示名称不能为空");
            }
            MessageEmailConfig config = EmailCore.GetConfig();
            config.SmtpPort = keyValuePair1.Value;
            config.SmtpServer = keyValuePair.Value;
            config.EmailName = keyValuePair2.Value;
            config.Password = keyValuePair3.Value;
            config.SendAddress = keyValuePair4.Value;
            config.DisplayName = keyValuePair5.Value;
            EmailCore.SaveConfig(config);
        }

        private void SetMessageStatus(MessageTypeEnum e, StatusEnum s)
        {
            switch (e)
            {
                case (MessageTypeEnum)1:
                    {
                        messageStatus.OrderCreated = (int)s;
                        break;
                    }
                case (MessageTypeEnum)2:
                    {
                        messageStatus.OrderPay = (int)s;
                        break;
                    }
                case (MessageTypeEnum)3:
                    {
                        messageStatus.OrderShipping = (int)s;
                        break;
                    }
                case (MessageTypeEnum)4:
                    {
                        messageStatus.OrderRefund = (int)s;
                        break;
                    }
                case (MessageTypeEnum)5:
                    {
                        messageStatus.FindPassWord = (int)s;
                        break;
                    }
                case (MessageTypeEnum)6:
                    {
                        messageStatus.ShopAudited = (int)s;
                        break;
                    }
                case (MessageTypeEnum)7:
                    {
                        messageStatus.ShopSuccess = (int)s;
                        break;
                    }
            }
        }
    }
}