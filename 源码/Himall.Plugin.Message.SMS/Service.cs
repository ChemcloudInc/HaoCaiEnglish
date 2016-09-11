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

namespace Himall.Plugin.Message.SMS
{
    public class Service : ISMSPlugin, IMessagePlugin, IPlugin
    {
        private MessageStatus messageStatus;

        private Dictionary<MessageTypeEnum, StatusEnum> dic = new Dictionary<MessageTypeEnum, StatusEnum>();

        public bool EnableLog
        {
            get
            {
                return true;
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
                if (string.IsNullOrWhiteSpace(SMSCore.WorkDirectory))
                {
                    throw new MissingFieldException("没有设置插件工作目录");
                }
                return string.Concat(SMSCore.WorkDirectory, "/Data/logo.png");
            }
        }

        public string ShortName
        {
            get
            {
                return "Mobile";
            }
        }

        public string WorkDirectory
        {
            set
            {
                SMSCore.WorkDirectory = value;
            }
        }

        public Service()
        {
            if (!string.IsNullOrEmpty(SMSCore.WorkDirectory))
            {
                InitMessageStatus();
            }
        }

        private void BatchSendMessage(string[] destination, string text)
        {
            if (destination.Length > 0)
            {
                MessageSMSConfig config = SMSCore.GetConfig();
                SortedDictionary<string, string> strs = new SortedDictionary<string, string>()
                {
                    { "mobiles", string.Join(",", destination) },
                    { "text", text },
                    { "appkey", config.AppKey },
                    { "sendtime", DateTime.Now.ToString() },
                    { "speed", "1" }
                };
                Dictionary<string, string> strs1 = SMSAPiHelper.Parameterfilter(strs);
                string str = SMSAPiHelper.BuildSign(strs1, config.AppSecret, "MD5", "utf-8");
                strs1.Add("sign", str);
                strs1.Add("sign_type", "MD5");
                SMSAPiHelper.PostData("http://sms.kuaidiantong.cn/SendMsg.aspx", SMSAPiHelper.CreateLinkstring(strs1));
            }
        }

        public void CheckCanEnable()
        {
            MessageSMSConfig config = SMSCore.GetConfig();
            if (string.IsNullOrWhiteSpace(config.AppKey))
            {
                throw new PluginConfigException("未设置AppKey");
            }
            if (string.IsNullOrWhiteSpace(config.AppSecret))
            {
                throw new PluginConfigException("未设置AppSecret");
            }
        }

        public bool CheckDestination(string destination)
        {
            return ValidateHelper.IsMobile(destination);
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
            FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
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
            FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
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

        public string GetBuyLink()
        {
            return "http://sms.kuaidiantong.cn/SMSPackList.aspx";
        }

        public FormData GetFormData()
        {
            MessageSMSConfig config = SMSCore.GetConfig();
            FormData formDatum = new FormData();
            FormData.FormItem[] formItemArray = new FormData.FormItem[2];
            FormData.FormItem formItem = new FormData.FormItem();
            formItem.DisplayName = "AppKey";
            formItem.Name = "AppKey";
            formItem.IsRequired = true;
            formItem.Type = (FormData.FormItemType)1;
            formItem.Value = config.AppKey;
            formItemArray[0] = formItem;
            FormData.FormItem formItem1 = new FormData.FormItem();
            formItem1.DisplayName = "AppSecret";
            formItem1.Name = "AppSecret";
            formItem1.IsRequired = true;
            formItem1.Type = (FormData.FormItemType)1;
            formItem1.Value = config.AppSecret;
            formItemArray[1] = formItem1;
            formDatum.Items = formItemArray;
            return formDatum;
        }

        public string GetLoginLink()
        {
            return "http://sms.kuaidiantong.cn/";
        }

        public string GetSMSAmount()
        {
            MessageSMSConfig config = SMSCore.GetConfig();
            string str = string.Concat("method=getAmount&appkey=", config.AppKey);
            return SMSAPiHelper.PostData("http://sms.kuaidiantong.cn/GetAmount.aspx", str);
        }

        public StatusEnum GetStatus(MessageTypeEnum e)
        {
            InitMessageStatus();
            KeyValuePair<MessageTypeEnum, StatusEnum> keyValuePair = dic.FirstOrDefault((KeyValuePair<MessageTypeEnum, StatusEnum> a) => a.Key == e);
            return keyValuePair.Value;
        }

        private void InitMessageStatus()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SMSCore.WorkDirectory);
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

        private string SendMessage(string destination, string text, string speed = "0")
        {
            string str;
            if (string.IsNullOrWhiteSpace(destination))
            {
                str = "发送目标不能为空！";
            }
            else
            {
                MessageSMSConfig config = SMSCore.GetConfig();
                SortedDictionary<string, string> strs = new SortedDictionary<string, string>()
                {
                    { "mobiles", destination },
                    { "text", text },
                    { "appkey", config.AppKey },
                    { "sendtime", DateTime.Now.ToString() },
                    { "speed", speed }
                };
                Dictionary<string, string> strs1 = SMSAPiHelper.Parameterfilter(strs);
                string str1 = SMSAPiHelper.BuildSign(strs1, config.AppSecret, "MD5", "utf-8");
                strs1.Add("sign", str1);
                strs1.Add("sign_type", "MD5");
                str = SMSAPiHelper.PostData("http://sms.kuaidiantong.cn/SendMsg.aspx", SMSAPiHelper.CreateLinkstring(strs1));
            }
            return str;
        }

        public string SendMessageCode(string destination, MessageUserInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.Bind.Replace("#userName#", info.UserName).Replace("#checkCode#", info.CheckCode).Replace("#siteName#", info.SiteName);
            SendMessage(destination, str, "2");
            return str;
        }

        public string SendMessageOnFindPassWord(string destination, MessageUserInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.FindPassWord.Replace("#userName#", info.UserName).Replace("#checkCode#", info.CheckCode).Replace("#siteName#", info.SiteName);
            SendMessage(destination, str, "2");
            return str;
        }

        public string SendMessageOnOrderCreate(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderCreated.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            SendMessage(destination, str, "0");
            return str;
        }

        public string SendMessageOnOrderPay(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderPay.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            decimal totalMoney = info.TotalMoney;
            string str1 = str.Replace("#Total#", totalMoney.ToString("F2"));
            SendMessage(destination, str1, "0");
            return str1;
        }

        public string SendMessageOnOrderRefund(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderRefund.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName);
            decimal refundMoney = info.RefundMoney;
            string str1 = str.Replace("#RefundMoney#", refundMoney.ToString("F2"));
            SendMessage(destination, str1, "0");
            return str1;
        }

        public string SendMessageOnOrderShipping(string destination, MessageOrderInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.OrderShipping.Replace("#userName#", info.UserName).Replace("#orderId#", info.OrderId).Replace("#siteName#", info.SiteName).Replace("#shippingCompany#", info.ShippingCompany).Replace("#shippingNumber", info.ShippingNumber);
            SendMessage(destination, str, "0");
            return str;
        }

        public string SendMessageOnShopAudited(string destination, MessageShopInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.ShopAudited.Replace("#userName#", info.UserName).Replace("#shopName#", info.ShopName).Replace("#siteName#", info.SiteName);
            SendMessage(destination, str, "0");
            return str;
        }

        public string SendMessageOnShopSuccess(string destination, MessageShopInfo info)
        {
            MessageContent messageContentConfig = SMSCore.GetMessageContentConfig();
            string str = messageContentConfig.ShopSuccess.Replace("#userName#", info.UserName).Replace("#shopName#", info.ShopName).Replace("#siteName#", info.SiteName);
            SendMessage(destination, str, "0");
            return str;
        }

        public void SendMessages(string[] destination, string content, string title)
        {
            BatchSendMessage(destination, content);
        }

        public string SendTestMessage(string destination, string content, string title)
        {
            return SendMessage(destination, content, "0");
        }

        public void SetAllStatus(Dictionary<MessageTypeEnum, StatusEnum> dic)
        {
            foreach (KeyValuePair<MessageTypeEnum, StatusEnum> keyValuePair in dic)
            {
                SetMessageStatus(keyValuePair.Key, keyValuePair.Value);
            }
            FileStream fileStream = new FileStream(string.Concat(SMSCore.WorkDirectory, "/Data/config.xml"), FileMode.Create);
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
            KeyValuePair<string, string> keyValuePair = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppKey");
            if (string.IsNullOrWhiteSpace(keyValuePair.Value))
            {
                throw new PluginConfigException("AppKey不能为空");
            }
            KeyValuePair<string, string> keyValuePair1 = values.FirstOrDefault((KeyValuePair<string, string> item) => item.Key == "AppSecret");
            if (string.IsNullOrWhiteSpace(keyValuePair1.Value))
            {
                throw new PluginConfigException("AppSecret不能为空");
            }
            MessageSMSConfig config = SMSCore.GetConfig();
            config.AppKey = keyValuePair.Value;
            config.AppSecret = keyValuePair1.Value;
            SMSCore.SaveConfig(config);
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