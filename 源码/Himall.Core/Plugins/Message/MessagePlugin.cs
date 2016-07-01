using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Himall.Core.Plugins.Message;
using System.Net.Mail; 

namespace Plugins.Message
{
  /*  class MessagePlugin:IMessagePlugin
    {
        public MessagePlugin()
       {
        }
        string SendTestMessage(string destination, string content, string title = "")
        {
            string str = "发送成功";
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(destination);

            

            msg.From = new MailAddress("master@boys90.com", "dulei", System.Text.Encoding.UTF8);
           
            msg.Subject = "这是测试邮件";//邮件标题   
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码   
            msg.Body = "邮件内容";//邮件内容   
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码   
            msg.IsBodyHtml = false;//是否是HTML邮件   
            msg.Priority = MailPriority.High;//邮件优先级   

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("dulei@71info.com", "userpass");
            //在71info.com注册的邮箱和密码   
            client.Host = "smtp.qq.com";
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                //简单一点儿可以client.Send(msg);   
                //  MessageBox.Show("发送成功");   
                str = "发送成功";
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                // MessageBox.Show(ex.Message, "发送邮件出错");  
                str = "发送邮件出错";
            }
            return str;
        }
    }*/
}
